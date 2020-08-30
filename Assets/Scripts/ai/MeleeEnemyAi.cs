﻿using UnityEngine;
using UnityEngine.AI;

public enum MeleeEnemyState
{
    Patroling,
    Chasing,
    Charging
}

public class MeleeEnemyAi : MonoBehaviour
{
    [SerializeField]
    private Transform[] partolPoints;
    private int patrolDestination = 0;

    [SerializeField, Range(1.0f, 100.0f)]
    private float hearingRadius = 10.0f;  // from how far away enemy starts chasing player

    [SerializeField, Range(1.0f, 5f)]
    private float speedWhileChasingMultiplayer = 2.0f;  // how fast enemy starts running while chasing player
    private bool speedChanged = false;

    [SerializeField, Range(1.0f, 35.0f)]
    private float chargingDistance = 5.0f;  // how close enemy needs to be to start charging at player, certenly this number should be smaller than hearing Radius

    [SerializeField, Range(0.0f, 3.0f)]
    private float explostionTriggerRange = 0.5f;  // how close will enemy charge toi player before explosion

    //[SerializeField, Range(1.0f, 5.0f)]
    //private float explostionRange = 3.0f;  // explosion radius, should be bigger than explosionTriggerRange

    private int playerLayerMask = 1 << 9;

    bool enemyInLineOfSight = false; // this is only updated in chasing state, it is member variable only to use is to draw gizmos for visualization
    private Vector3? explostionPosition = null;
    private MeleeEnemyState state = MeleeEnemyState.Patroling;
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
        if(state == MeleeEnemyState.Patroling)
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
            
            if (enemyInLineOfSight)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePos, player.position);
        }
        else if(state == MeleeEnemyState.Charging)
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = transform.position;
            pos.y += 2f;
            Gizmos.DrawWireSphere(pos, explostionTriggerRange);
        }
            
    }

    private void Update()
    {
        if (state == MeleeEnemyState.Patroling)
            UpdatePatrolling();
        else if (state == MeleeEnemyState.Chasing)
            UpdateChasing();
        else if (state == MeleeEnemyState.Charging)
            UpdateCharging();
    }


    private void UpdatePatrolling()
    {
        if (Physics.CheckSphere(transform.position, hearingRadius, playerLayerMask))
        {
            state = MeleeEnemyState.Chasing;
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

        enemyInLineOfSight = false;
        RaycastHit hitInfo;

        eyePos = transform.position;
        eyePos.y += 2.0f;

        Ray ray = new Ray(eyePos, player.position - eyePos);
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.transform.position == player.position)
                enemyInLineOfSight = true;
        }


        if (enemyInLineOfSight && agent.remainingDistance < chargingDistance)
        {
            // TODO: skomplikować movement troche bardziej
            // jeżeli jesteśmy odpowiednio blisko, ale nie w line of sight to mozna sprawdzic dodatkowo
            //  czy jeżeli przesuniemy się trochę w prawo / lewo to czy będzie line of sight 
            state = MeleeEnemyState.Charging;
        }
    }

    private void UpdateCharging()
    {
        // TODO: zostawia trail za soba podczas szarży?
        if (explostionPosition == null)
        {
            explostionPosition = player.position;
            agent.destination = (Vector3)explostionPosition;
        }

        if (agent.remainingDistance < explostionTriggerRange)
        {
            // Animacja eksplozji trwajaca chwile, po tym czasie zadaj obrazenia wszystkim trafionym
            Debug.Log("BUM");
            Destroy(gameObject);
        }
        
    }
}