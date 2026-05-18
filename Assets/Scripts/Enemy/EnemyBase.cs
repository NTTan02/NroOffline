using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Data")]
    public EnemyData data;

    [Header("Health Bar")]
    public EnemyHealthBar healthBar;

    [Header("Respawn")]
    public bool canRespawn = true;
    public float respawnTime = 3f;

    [HideInInspector] public int currentHP;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Collider2D col;
    private EnemyAI ai;

    private bool isDead = false;
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        ai = GetComponent<EnemyAI>();

        spawnPos = transform.position;
        spawnRot = transform.rotation;

        InitEnemy();

    }

    void InitEnemy()
    {
        if (data == null)
        {
            Debug.LogError(name + " chưa gán EnemyData!");
            return;
        }

        isDead = false;
        currentHP = data.hp;

        if (sr != null)
        {
            sr.enabled = true;
            sr.color = Color.white;
        }

        if (col != null)
            col.enabled = true;

        if (ai != null)
            ai.enabled = true;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.Init(data.hp);
            healthBar.UpdateBar(currentHP);
        }
        else
        {
            Debug.LogWarning(name + " chưa gán EnemyHealthBar!");
        }
    }

    public int TakeDamage(int rawDmg)
    {
        if (isDead) return 0;

        int dmg = Mathf.Max(1, rawDmg - data.def);
        currentHP = Mathf.Max(0, currentHP - dmg);

        if (healthBar != null)
            healthBar.UpdateBar(currentHP);

        StartCoroutine(DamageFlash());

        if (currentHP <= 0)
            Die();

        return dmg;
    }

    System.Collections.IEnumerator DamageFlash()
    {
        if (sr != null) sr.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (sr != null && !isDead)
            sr.color = Color.white;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.Instance != null)
        {
            bool levelUp = GameManager.Instance.AddEXP(data.expReward);
            GameManager.Instance.playerGold += data.goldReward;

            if (levelUp)
                FindObjectOfType<HUDManager>()?.ShowLevelUp();
        }
        GetComponent<ItemDropSpawner>()?.TryDrop(transform.position);
        QuestSystem.Instance?.OnEnemyKilled(data.enemyName);
        // if (anim != null)
        //     anim.SetTrigger("die");

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        if (col != null)
            col.enabled = false;

        if (ai != null)
            ai.enabled = false;

        if (canRespawn)
            StartCoroutine(Respawn());
        else
            Destroy(gameObject, 1.5f);
    }

    System.Collections.IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        // Reset physics trước
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.simulated = false;
        }

        transform.position = spawnPos;
        transform.rotation = spawnRot;

        yield return null;

        if (rb != null)
        {
            rb.simulated = true;
        }

        InitEnemy();
    }

    public bool IsDead => isDead;
}