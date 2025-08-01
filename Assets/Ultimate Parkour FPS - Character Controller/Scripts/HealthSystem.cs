using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
    Script to handle health and damage systems.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private Text healthText; // health UI text component
        [SerializeField] private float health = 100f; // current health

        private float maxHealth; // maximum health amount

        // Start is called before the first frame update
        void Start()
        {
            maxHealth = health; // set max health

            // update health UI text
            if (healthText != null)
                healthText.text = health.ToString();
        }

        // take specified amount of damage
        public void TakeDamage(float damageAmount, Vector3? knockbackDirection = null, float knockbackAmount = 1f)
        {
            health -= damageAmount; // decrease health

            if (knockbackDirection != null) // if knockback enabled
                StartCoroutine(Knockback((Vector3)knockbackDirection, knockbackAmount));

            // if health is less than zero
            if (health <= 0)
                Destroy(gameObject); // destroy game object

            // update health UI text
            if (healthText != null)
                healthText.text = health.ToString();
        }

        // lerp object position backwards
        private IEnumerator Knockback(Vector3 direction, float knockbackAmount)
        {
            Vector3 targetPos = transform.position + direction * knockbackAmount;

            float knockbackTime = 0.2f;
            float elapsedTime = 0;
            while (elapsedTime < knockbackTime)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, (elapsedTime / knockbackTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }
    }
}