using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Target")]
    public LayerMask enemyLayer;
    public EnemyBase currentTarget;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;

    private PlayerStats stats;
    private PlayerAnimator animator;
    private float attackTimer = 0f;
    private HUDManager hud;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        animator = GetComponent<PlayerAnimator>();
        hud = FindObjectOfType<HUDManager>();
    }

    void Update()
    {
        if (stats == null || !stats.IsAlive) return;

        attackTimer += Time.deltaTime;

        HandleSelectTarget();
        HandleAttackInput();
    }

    void HandleSelectTarget()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, enemyLayer);

        if (hit == null)
        {
            currentTarget = null;
            Debug.Log("Bỏ chọn enemy");
            hud?.HideEnemyInfo();
            return;
        }

        EnemyBase enemy = hit.GetComponent<EnemyBase>();

        if (enemy != null && !enemy.IsDead)
        {
            currentTarget = enemy;
            Debug.Log("Đã chọn: " + enemy.name);
            hud?.ShowEnemyInfo(currentTarget);
        }
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NormalAttack();
        }
    }

    public void NormalAttack()
    {
        if (currentTarget == null)
        {
            Debug.Log("Chưa chọn enemy!");
            return;
        }

        if (currentTarget.IsDead)
        {
            Debug.Log("Enemy đã chết!");
            currentTarget = null;
            return;
        }

        if (!IsTargetInRange())
        {
            Debug.Log("Enemy quá xa!");
            return;
        }

        if (attackTimer < attackCooldown)
        {
            Debug.Log("Đang hồi đánh thường!");
            return;
        }

        attackTimer = 0f;

        animator?.PlayAttack();

        int dmg = currentTarget.TakeDamage(stats.ATK);
        DamageNumber.Show(currentTarget.transform.position, dmg, Color.red);

        Debug.Log("Đánh thường: " + currentTarget.name + " -" + dmg);
        hud?.UpdateEnemyInfo(currentTarget);
    }

    bool IsTargetInRange()
    {
        if (currentTarget == null) return false;

        float distance = Vector2.Distance(
            transform.position,
            currentTarget.transform.position
        );

        return distance <= attackRange;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}