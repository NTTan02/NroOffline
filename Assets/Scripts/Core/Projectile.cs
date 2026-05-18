using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private SkillData skill;
    private int atk;
    private float speed;

    public void Init(Vector2 dir, SkillData skillData, int playerATK)
    {
        direction = dir.normalized;
        skill     = skillData;
        atk       = playerATK;
        speed     = skillData.projectileSpeed;

        // Xoay projectile theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))     return;
        if (other.CompareTag("Projectile")) return;

        var enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            int dmg = Mathf.RoundToInt(atk * skill.damageMultiplier);
            enemy.TakeDamage(dmg);
            DamageNumber.Show(other.transform.position, dmg, Color.yellow);

            if (!skill.piercing)
                DestroyProjectile();
        }
        else
        {
            if (!other.isTrigger)
                DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        if (skill.effectPrefab != null)
            Instantiate(skill.effectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}