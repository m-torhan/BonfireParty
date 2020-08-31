using System.Collections;
using UnityEngine;

public enum BossState
{
    Init,
    Idle,
    Fight
}

public enum BossAction
{
    Spawn,
    AutoAttack,
    AutoAttackCast,
    None
}


public class BossEnemyAi : MonoBehaviour
{
    [SerializeField, Range(10f, 1000f)]
    private float health = 150f;

    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField]
    private Transform firePrefab;

    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Transform rightHand;

    private BossState state = BossState.Init;
    private BossAction action1 = BossAction.AutoAttack;
    private BossAction action2 = BossAction.None;

    private Transform player;
    private float fightRange = 60.0f;

    [SerializeField, Range(0.5f, 3f)]
    private float autoAttackCastTime = 2f;

    private float autoAttakCastTimeLeft;
    private Transform fire;

    private float spawnMeleeTimeMin = 14f;
    private float spawnMeleeTimeMax = 22f;
    private float spawnMeleeTimeLeft = 3f;

    private float spawnRangedTimeMin = 18f;
    private float spawnRangedTimeMax = 28f;
    private float spawnRangedTimeLeft = 10f;

    [SerializeField]
    private MeleeEnemyAi meleeEnemyPrefab;

    [SerializeField]
    private Transform[] meleeEnemyPath1;

    [SerializeField]
    private Transform[] meleeEnemyPath2;


    [SerializeField]
    private RangedEnemyAi rangedEnemyPrefab;

    [SerializeField]
    private Transform[] rangedEnemyPath1;

    [SerializeField]
    private Transform[] rangedEnemyPath2;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, fightRange);
    }

    private void Update()
    {
        if(state == BossState.Init)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 500.0f, Ai.playerLayerMask);
            player = colliders[0].gameObject.transform;

            state = BossState.Idle;
        }
        if(state == BossState.Idle)
        {
            if (Vector3.Distance(player.position, transform.position) <= fightRange)
                state = BossState.Fight;
        }
        else if(state == BossState.Fight)
        {
            if (Vector3.Distance(player.position, transform.position) > fightRange)
                state = BossState.Idle;

            UpdateBossAction1();
            UpdateSpawnMeleeEnemies();
            UpdateSpawnRangedEnemies();
        }
    }

    private void UpdateBossAction1()
    {
        transform.rotation = Quaternion.LookRotation(player.position - transform.position) * Quaternion.Euler(0f, -80f, 0f);
        
        if(action1 == BossAction.AutoAttack)
        {
            autoAttakCastTimeLeft = 3f;
            //FireProjectile(leftHand.position, player.position, 1f, 7f);
            fire = Instantiate(firePrefab);
            fire.position = leftHand.position;
            AutoAttack(fire);

            action1 = BossAction.AutoAttackCast;
        }
        else if(action1 == BossAction.AutoAttackCast)
        {
            if (AutoAttack(fire))
            {
                action1 = BossAction.AutoAttack;
            }
        }
        //else if (action == BossAction.Attack2)
        //{
        //    action = BossAction.Attack1;
        //}
    }


    private void UpdateSpawnMeleeEnemies()
    {
        spawnMeleeTimeLeft -= Time.deltaTime;
        if (spawnMeleeTimeLeft <= 0f)
        {
            SpawnMeleeEnemy(meleeEnemyPath1);
            SpawnMeleeEnemy(meleeEnemyPath2);
            spawnMeleeTimeLeft = Random.Range(spawnMeleeTimeMin, spawnMeleeTimeMax);
        }
    }

    private void UpdateSpawnRangedEnemies()
    {
        spawnRangedTimeLeft -= Time.deltaTime;
        if (spawnRangedTimeLeft <= 0f)
        {
            SpawnRangedEnemy(rangedEnemyPath1);
            SpawnRangedEnemy(rangedEnemyPath2);
            spawnRangedTimeLeft = Random.Range(spawnRangedTimeMin, spawnRangedTimeMax);
        }
    }

    private void SpawnMeleeEnemy(Transform[] patrolPoints)
    {
        MeleeEnemyAi m = Instantiate(meleeEnemyPrefab).GetComponent<MeleeEnemyAi>();
        m.gameObject.SetActive(false);
        m.partolPoints = patrolPoints;
        m.transform.position = patrolPoints[0].position;
        m.gameObject.SetActive(true);
    }

    private void SpawnRangedEnemy(Transform[] patrolPoints)
    {
        RangedEnemyAi m = Instantiate(rangedEnemyPrefab).GetComponent<RangedEnemyAi>();
        m.gameObject.SetActive(false);
        m.partolPoints = patrolPoints;
        m.transform.position = patrolPoints[0].position;
        m.gameObject.SetActive(true);
    }

    private bool AutoAttack(Transform fire)
    {
        if (autoAttakCastTimeLeft <= 0f)
        {
            Projectile p = fire.gameObject.AddComponent<Projectile>();
            Vector3 pp = player.position;
            pp.y -= 4f;
            p.Setup(fire.position, pp, 0.5f, damage: 15f);
            return true;
        }

        fire.localScale = (new Vector3(20f, 20f, 20f) * 5f * (autoAttackCastTime - autoAttakCastTimeLeft)/autoAttackCastTime);

        Vector3 pos = leftHand.position;
        pos.y -= 3 *(autoAttackCastTime - autoAttakCastTimeLeft) / autoAttackCastTime;
        fire.position = pos;

        autoAttakCastTimeLeft -= Time.deltaTime;
        return false;
    }

    private void FireProjectile(Vector3 startingPos, Vector3 destination, float speed, float scale )
    {
        Projectile p = Instantiate(projectilePrefab);

        p.Setup(startingPos, destination, speed);
        p.transform.localScale = p.transform.localScale * scale;
    }


    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        Debug.Log("boss otrzymal obrazenia. Pozostałe życie: " + health);
    }

}
