using UnityEngine;
using UnityEngine.AI;

public enum MeleeEnemyState
{
    Patrolling,
    Chasing,
    Charge,
    Dead
}

public class MeleeEnemyAi : MonoBehaviour
{
    [SerializeField, Range(1.0f, 10.0f)]
    private float health = 3.0f;

    public Transform[] partolPoints;
    private int patrolDestination = 0;

    [SerializeField, Range(1.0f, 100.0f)]
    private float hearingRadius = 30.0f;  // from how far away enemy starts chasing player

    [SerializeField, Range(1.0f, 5f)]
    private float speedWhileChasingMultiplayer = 2.0f;  // how fast enemy starts running while chasing player
    private bool speedChanged = false;

    [SerializeField, Range(1.0f, 35.0f)]
    private float chargingDistance = 10.0f;  // how close enemy needs to be to start charging at player, certenly this number should be smaller than hearing Radius

    [SerializeField, Range(0.0f, 3.0f)]
    private float explostionTriggerRange = 2f;  // how close will enemy charge toi player before explosion

    [SerializeField, Range(1.0f, 5.0f)]
    private float explostionRange = 3.0f;  

    [SerializeField, Range(1.0f, 20.0f)]
    private float explosionDamage = 10.0f;  

    [SerializeField]
    private ParticleSystem explosion;

    [SerializeField]
    private GameObject visuals;


    bool playerInLineOfSight = false; // this is only updated in chasing state, it is member variable only to use is to draw gizmos for visualization
    private Vector3? explostionPosition = null;
    private MeleeEnemyState state = MeleeEnemyState.Patrolling;
    private NavMeshAgent agent;
    private Transform player;

    private Vector3 eyePos;  // TODO: zołatwić to w jakiś lepszy sposób?

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false; // disable slowing as aproaching target
    }

    private void OnDrawGizmos()
    {
        if(state == MeleeEnemyState.Patrolling)
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
        if (state == MeleeEnemyState.Chasing)
        {

            if (agent.remainingDistance < chargingDistance)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
            }
            
            if (playerInLineOfSight)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePos, player.position);
        }
        else if(state == MeleeEnemyState.Charge)
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = transform.position;
            pos.y += 2f;
            Gizmos.DrawWireSphere(pos, explostionTriggerRange);
        }
            
    }

    private void Update()
    {
        if (state == MeleeEnemyState.Patrolling)
            UpdatePatrolling();
        else if (state == MeleeEnemyState.Chasing)
            UpdateChasing();
        else if (state == MeleeEnemyState.Charge)
            UpdateCharging();
        else if(state == MeleeEnemyState.Dead)
        {
            if (!explosion.isEmitting)
                Destroy(gameObject);
        }

    }


    private void UpdatePatrolling()
    {
        if (Physics.CheckSphere(transform.position, hearingRadius, Ai.playerLayerMask))
        {
            state = MeleeEnemyState.Chasing;
            Collider[] colliders = Physics.OverlapSphere(transform.position, hearingRadius, Ai.playerLayerMask);
            player = colliders[0].gameObject.transform;

        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            PatrolNextPoint();
    }
    private void PatrolNextPoint()
    {
        if (partolPoints.Length == 0)
        {
            Debug.Log("Patrol points are not set up for " + gameObject.name);
            return;
        }

        agent.destination = partolPoints[patrolDestination].position;
        patrolDestination = (patrolDestination + 1) % partolPoints.Length;
    }

    private void UpdateChasing()
    {
        if (!speedChanged)
        {
            agent.speed *= speedWhileChasingMultiplayer;
            speedChanged = true;
        }

        agent.destination = player.position;

        eyePos = transform.position;
        eyePos.y += 2.0f;
        playerInLineOfSight = Ai.IsInLineOfSight(eyePos, player.position);

        if (playerInLineOfSight && agent.remainingDistance < chargingDistance)
        {
            state = MeleeEnemyState.Charge;
        }
    }

    private void UpdateCharging()
    {
        if (explostionPosition == null)
        {
            explostionPosition = player.position;
            agent.destination = (Vector3)explostionPosition;
        }

        if (agent.remainingDistance < explostionTriggerRange)
        {
            explosion.gameObject.SetActive(true);
            explosion.Play();

            Collider[] colliders = Physics.OverlapSphere(transform.position, explostionRange, Ai.playerLayerMask);
            if(colliders.Length > 0)
            {
                colliders[0].GetComponent<PlayerMovement>().ReceiveDamage(explosionDamage);
            }

            state = MeleeEnemyState.Dead;
            Destroy(gameObject.GetComponent<CapsuleCollider>());
            Destroy(gameObject.GetComponent<NavMeshAgent>());
            Destroy(visuals);
        }
        
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
