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
    Attack1,
    Attack1Cast
}


public class BossEnemyAi : MonoBehaviour
{
    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField]
    private Transform firePrefab;

    [SerializeField]
    private Transform leftHand;
    [SerializeField]

    private Transform rightHand;

    private BossState state = BossState.Init;
    private BossAction action = BossAction.Attack1;
    private Transform player;
    private float fightRange = 60.0f;

    private float castTime = 3f;
    private float castTimeLeft;
    private Transform fire;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, fightRange);
    }

    private void Update()
    {
        if(state == BossState.Init)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 500.0f, Ai.playerLayerMask);
            Debug.Assert(colliders.Length == 1, "Collider count doesn't match. Either there is more than one player or there is none.");
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

            UpdateFight();
        }
    }

    private void UpdateFight()
    {
        transform.rotation = Quaternion.LookRotation(player.position - transform.position) * Quaternion.Euler(0f, -80f, 0f);
        
        if(action == BossAction.Attack1)
        {
            castTimeLeft = 3f;
            //FireProjectile(leftHand.position, player.position, 1f, 7f);
            fire = Instantiate(firePrefab);
            fire.position = leftHand.position;
            Attack1(fire);

            action = BossAction.Attack1Cast;
        }
        else if(action == BossAction.Attack1Cast)
        {
            if (Attack1(fire))
            {
                action = BossAction.Attack1;
            }
        }
        //else if (action == BossAction.Attack2)
        //{
        //    action = BossAction.Attack1;
        //}
    }

    private bool Attack1(Transform fire)
    {
        float size = 7f;

        if (castTimeLeft <= 0f)
        {
            Projectile p = fire.gameObject.AddComponent<Projectile>();
            Vector3 pp = player.position;
            pp.y -= 4f;
            p.Setup(fire.position, pp, 0.5f);
            return true;
        }

        fire.localScale = (new Vector3(20f, 20f, 20f) * 7f * (castTime - castTimeLeft)/castTime);

        Vector3 pos = leftHand.position;
        pos.y -= 5 *(castTime - castTimeLeft) / castTime;
        fire.position = pos;

        castTimeLeft -= Time.deltaTime;
        return false;
    }

    private void FireProjectile(Vector3 startingPos, Vector3 destination, float speed, float scale )
    {
        Projectile p = Instantiate(projectilePrefab);

        p.Setup(startingPos, destination, speed);
        p.transform.localScale = p.transform.localScale * scale;
    }


}
