using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UltimateParkourFPS
{
    public class PlayerHealthSystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float startingHealth = 100f;
        [SerializeField] private Text healthText;
        [SerializeField] private GameObject deathUI;
        [SerializeField] private Text deathMessageText;

        private float currentHealth;
        private bool isDead = false;

        void Start()
        {
            currentHealth = startingHealth;
            UpdateHealthUI();

            if (deathUI != null)
                deathUI.SetActive(false);
        }

        public void TakeDamage(float damageAmount, Vector3? knockbackDirection = null, float knockbackForce = 1f)
        {
            if (isDead) return;

            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);
            UpdateHealthUI();

            if (knockbackDirection != null)
                StartCoroutine(ApplyKnockback((Vector3)knockbackDirection, knockbackForce));

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void UpdateHealthUI()
        {
            if (healthText != null)
                healthText.text = Mathf.CeilToInt(currentHealth).ToString();
        }

        private IEnumerator ApplyKnockback(Vector3 direction, float force)
        {
            Vector3 targetPosition = transform.position + direction.normalized * force;
            float knockTime = 0.2f;
            float t = 0f;

            while (t < knockTime)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, t / knockTime);
                t += Time.deltaTime;
                yield return null;
            }
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log("Player Died");

            // Disable movement script
            // Disable PlayerController movement script
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = false;

            // Show "You Died" UI
            if (deathUI != null)
            {
                deathUI.SetActive(true);

                if (deathMessageText != null)
                    deathMessageText.text = "You Died";
            }

            // End the game in 10 seconds
            StartCoroutine(EndGame());
        }

        private IEnumerator EndGame()
        {
            yield return new WaitForSeconds(10f);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
