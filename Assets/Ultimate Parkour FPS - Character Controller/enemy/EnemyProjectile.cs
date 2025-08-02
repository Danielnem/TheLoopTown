using UnityEngine;
using UltimateParkourFPS;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
            if (health != null)
            {
                Vector3 knockbackDir = (collision.transform.position - transform.position).normalized;
                health.TakeDamage(damage, knockbackDir); // You can tweak knockback amount
            }
        }

        Destroy(gameObject);
    }
}
