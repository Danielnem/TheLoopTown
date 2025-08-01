using UnityEngine;

/*
    Script to handle melee functions and animations.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    public class MeleeScript : WeaponScript
    {
        [SerializeField] private MeleeSO weaponSO;
        public override WeaponScriptableObject scriptableObject() => weaponSO;

        private float lastShotTime;
        private float shotDelay; // time between shots in seconds

        #region components
        private Animation animationPlayer;
        private SoundPlayer soundPlayer;
        #endregion

        private void Awake()
        {
            #region initialize components
            animationPlayer = GetComponent<Animation>();
            soundPlayer = GetComponent<SoundPlayer>();
            #endregion
        }

        // Start is called before the first frame update
        private void Start()
        {
            // calculate time betwwen shots based on fire rate
            shotDelay = 1f / weaponSO.fireRate;
        }

        // called when player pressed the shoot button
        public override void Shoot()
        {
            if (Time.time >= lastShotTime + shotDelay)
            {
                // play shoot sound effect
                if (weaponSO.shootSound != null)
                    soundPlayer.PlaySound(weaponSO.shootSound, pitch:1 - Random.Range(0, 0.2f));
                else
                    Debug.Log("Weapon shoot sound effect not set");

                // play shoot animation
                if (weaponSO.attackAnimation != null)
                {
                    animationPlayer.clip = weaponSO.attackAnimation;
                    animationPlayer.Play();
                }
                else
                    Debug.Log("Weapon shoot animation not set");

                DamageEnemy();

                lastShotTime = Time.time; // set last shot time
            }
        }

        // switch to this weapon
        public override void Activate()
        {
            // play switch animation
            if (weaponSO.switchAnimation != null)
            {
                animationPlayer.clip = weaponSO.switchAnimation;
                animationPlayer.Play();
            }
            else
                Debug.Log("Weapon switch animation not set");

            // play switch sound effect
            if (weaponSO.switchSound != null)
                soundPlayer.PlaySound(weaponSO.switchSound);
            else
                Debug.Log("Weapon switch sound effect not set");
        }
    }
}