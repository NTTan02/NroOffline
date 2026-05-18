using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Aggro,
        Attack
    }

    [Header("Settings")]
    public float patrolDistance = 3f;
    public LayerMask playerLayer;

    private State state = State.Patrol;

    private EnemyBase enemyBase;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;

    private Transform player;
    private Vector3 startPos;
    private int patrolDir = 1;
    private float attackTimer = 0f;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        startPos = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (anim == null)
        {
            Debug.LogWarning(gameObject.name + " chưa có Animator. Enemy vẫn chạy AI nhưng không chạy animation.");
        }
    }

    void Update()
    {
        if (enemyBase == null) return;
        if (enemyBase.IsDead) return;
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        attackTimer += Time.deltaTime;

        switch (state)
        {
            case State.Patrol:
                DoPatrol();

                if (distToPlayer <= enemyBase.data.aggroRange)
                    state = State.Aggro;

                break;

            case State.Aggro:
                DoAggro();

                if (distToPlayer <= enemyBase.data.attackRange)
                    state = State.Attack;
                else if (distToPlayer > enemyBase.data.aggroRange * 1.5f)
                    state = State.Patrol;

                break;

            case State.Attack:
                DoAttack();

                if (distToPlayer > enemyBase.data.attackRange)
                    state = State.Aggro;

                break;
        }

        UpdateAnimator();
    }

    void DoPatrol()
    {
        if (enemyBase.data == null) return;

        transform.Translate(
            Vector2.right * patrolDir * enemyBase.data.moveSpeed * 0.5f * Time.deltaTime
        );

        if (sr != null)
            sr.flipX = patrolDir < 0;

        if (Mathf.Abs(transform.position.x - startPos.x) > patrolDistance)
            patrolDir *= -1;
    }

    void DoAggro()
    {
        if (enemyBase.data == null) return;

        float distFromSpawn = Vector2.Distance(transform.position, startPos);

        if (distFromSpawn >= patrolDistance * 2f)
        {
            state = State.Patrol;
            return;
        }

        Vector2 dir = new Vector2(
            player.position.x - transform.position.x,
            0
        ).normalized;

        transform.Translate(
            dir * enemyBase.data.moveSpeed * Time.deltaTime
        );

        if (sr != null)
            sr.flipX = dir.x < 0;
    }

    void DoAttack()
    {
        if (enemyBase.data == null) return;

        if (sr != null)
            sr.flipX = player.position.x < transform.position.x;

        if (attackTimer >= 1f / enemyBase.data.attackSpeed)
        {
            attackTimer = 0f;

            if (anim != null)
                anim.SetTrigger("attack");

            PlayerStats pStats = player.GetComponent<PlayerStats>();

            if (pStats != null)
            {
                int dmg = pStats.TakeDamage(enemyBase.data.atk);
                DamageNumber.Show(player.position, dmg, Color.red);

                HUDManager hud = FindObjectOfType<HUDManager>();
                if (hud != null)
                    hud.UpdateHUD();
            }
        }
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetInteger("state", (int)state);
    }

    //Test Tọa độ tìm Player
    // void OnDrawGizmosSelected()
    // {
    //     EnemyBase eb = GetComponent<EnemyBase>();

    //     if (eb == null || eb.data == null) return;

    //     Gizmos.color = Color.red;

    //     Gizmos.DrawWireSphere(
    //         transform.position,
    //         eb.data.aggroRange
    //     );
    // }
}