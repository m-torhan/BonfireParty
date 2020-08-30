using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public enum RangedEnemyState
{
    Patrolling,
    Chasing,
    Casting,
    Firing,
    Recharging
}

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemyAi : MonoBehaviour
{
    [SerializeField]
    private Transform[] partolPoints;
    private int patrolDestination = 0;

    [SerializeField, Range(1.0f, 100.0f)]
    private float hearingRadius = 30.0f;  // from how far away enemy starts chasing player

    [SerializeField, Range(5.0f, 35.0f)]
    private float castDistance = 20.0f;  // how close enemy needs to be to start casting 

    [SerializeField, Range(0.0f, 5.0f)]
    private float castTime = 1.0f;  // how long is casting

    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField, Range(0.0f, 10.0f)]
    private float rechargeTime = 3.0f;  // how long is rechargning



    private bool playerInLineOfSight;
    private Vector3 eyePos;  // TODO: zołatwić to w jakiś lepszy sposób?


    private RangedEnemyState state;
    private const int playerLayerMask = 1 << 9;
    private NavMeshAgent agent;
    private Transform player;
    private bool isCasting = false;
    private bool isRecharging = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnDrawGizmos()
    {
        if (state == RangedEnemyState.Patrolling)
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
        if (state == RangedEnemyState.Chasing)
        {

            if (Vector3.Distance(eyePos, player.position) < castDistance)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, castDistance);


            if (playerInLineOfSight)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePos, player.position);
        }
        else if (state == RangedEnemyState.Casting)
        {
        }

    }

    private void Update()
    {
        if(state == RangedEnemyState.Patrolling)
        {
            UpdatePatrolling();
        }
        else if(state == RangedEnemyState.Chasing)
        {
            UpdateChasing();
        }
        else if(state == RangedEnemyState.Casting)
        {
            UpdateCasting();
        }
        else if(state == RangedEnemyState.Firing)
        {
            Fire();
        }
        else if(state == RangedEnemyState.Recharging)
        {
            UpdateRecharging();
        }
    }

    private void UpdatePatrolling()
    {
        if (Physics.CheckSphere(transform.position, hearingRadius, playerLayerMask))
        {
            state = RangedEnemyState.Chasing;
            Collider[] colliders = Physics.OverlapSphere(transform.position, hearingRadius, playerLayerMask);
            Debug.Assert(colliders.Length == 1, "Collider count doesn't match. Either there is more than one player or there is none.");
            player = colliders[0].gameObject.transform;

        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            PatrolNextPoint();
    }
    private void PatrolNextPoint()
    {
        if (partolPoints.Length == 0)
        {
            Debug.LogError("Patrol points are not set up for " + gameObject.name);
            return;
        }

        agent.destination = partolPoints[patrolDestination].position;
        patrolDestination = (patrolDestination + 1) % partolPoints.Length;
    }

    private void UpdateChasing()
    {
        agent.destination = player.position;

        playerInLineOfSight = false;
        RaycastHit hitInfo;

        eyePos = transform.position;
        eyePos.y += 2.0f;

        Ray ray = new Ray(eyePos, player.position - eyePos);
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.transform.position == player.position)
                playerInLineOfSight = true;
        }
        if (playerInLineOfSight && Vector3.Distance(eyePos, player.position) < castDistance)
        {
            state = RangedEnemyState.Casting;
        }
    }

    private void Fire()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                FireProjectile(eyePos, player.position, 0.25f);
                break;
            case 1:
                FireProjectile(eyePos, player.position, 0.18f);
                FireProjectile(eyePos, player.position + transform.right * 2f, 0.15f);
                FireProjectile(eyePos, player.position - transform.right * 2f, 0.15f);
                break;
            case 2:
                FireProjectile(eyePos, player.position, 0.1f);
                FireProjectile(eyePos, player.position + transform.right * 1.5f, 0.1f);
                FireProjectile(eyePos, player.position - transform.right * 1.5f, 0.1f);
                FireProjectile(eyePos, player.position + transform.right * 3f, 0.1f);
                FireProjectile(eyePos, player.position - transform.right * 3f, 0.1f);
                break;
        }

        state = RangedEnemyState.Recharging;
    }

    private void FireProjectile(Vector3 startingPos, Vector3 destination, float speed)
    {
        Projectile p = Instantiate(projectilePrefab);
        p.Setup(startingPos, destination, speed);
    }

    private void UpdateCasting()
    {
        // TODO: jakas animacja?
        agent.isStopped = true;
        if (!isCasting)
        {
            StartCoroutine(StartCasting());
            isCasting = true;
        }

    }
    private IEnumerator StartCasting()
    {
        yield return new WaitForSeconds(castTime);
        state = RangedEnemyState.Firing;
        isCasting = false;
    }

    private void UpdateRecharging()
    {
        if (!isRecharging)
        {
            StartCoroutine(StartRecharging());
            isRecharging = true;
        }
    }

    private IEnumerator StartRecharging()
    {
        // jakas animacja w tym czasie?
        yield return new WaitForSeconds(rechargeTime);
        state = RangedEnemyState.Chasing;
        agent.isStopped = false;
        isRecharging = false;
    }
}
