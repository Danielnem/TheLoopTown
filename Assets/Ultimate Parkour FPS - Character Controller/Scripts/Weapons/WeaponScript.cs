using UnityEngine;

/*
    Base class for weapons.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(SoundPlayer))]
    public abstract class WeaponScript : MonoBehaviour
    {
        public abstract WeaponScriptableObject scriptableObject();

        public abstract void Shoot();

        // try to damage enemy in front of the player if in range
        public void DamageEnemy()
        {
            if (transform.parent != null)
            {
                RaycastHit hit;

                // if raycast hit a target in range
                if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, scriptableObject().range))
                {
                    if (hit.transform.tag.Equals("Target")) // if hit target object
                        hit.transform.GetComponent<HealthSystem>().TakeDamage(scriptableObject().damage, transform.parent.forward); // make object take damage
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (transform.parent != null)
            {
                // draw weapon range raycast gizmo
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.parent.position, transform.parent.forward * scriptableObject().range);
            }
        }

        public abstract void Activate();
    }
}