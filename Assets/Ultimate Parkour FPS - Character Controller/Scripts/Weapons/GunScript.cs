using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
    Script to handle gun functions and animations.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    public class GunScript : WeaponScript
    {
        [SerializeField] private GunSO gunSO;
        public override WeaponScriptableObject scriptableObject() => gunSO;
        public GunSO gunScriptableObject() => gunSO;

        private bool reloading; // if currently reloading

        private int currAmmo; // current ammo left
        private int currMagaines; // current magazines left

        private float lastShotTime;
        private float shotDelay; // time between shots in seconds
        
        private Vector3 startPos; // start local position of the gun
        private Vector3 maxRecoilPos; // position after applying max recoil amount
        private bool recoilApplied;
        
        #region components
        private Animation animationPlayer;
        private SoundPlayer soundPlayer;

        private Text ammoText;
        private Text magazineText;
        #endregion

        private void Awake()
        {
            #region initialize components
            animationPlayer = GetComponent<Animation>();
            soundPlayer = GetComponent<SoundPlayer>();

            if (transform.root.GetComponent<WeaponManager>() != null)
            {
                ammoText = transform.root.GetComponent<WeaponManager>().ammoText;
                magazineText = transform.root.GetComponent<WeaponManager>().magazineText;
            }
            #endregion
        }

        // Start is called before the first frame update
        private void Start()
        {
            // initialize ammo and magazines
            currAmmo = gunSO.ammo;
            currMagaines = gunSO.magazines;

            // calculate time betwwen shots based on fire rate
            shotDelay = 1f / gunSO.fireRate;

            // set start position
            startPos = transform.localPosition;

            // set max recoil position
            maxRecoilPos = startPos - new Vector3(0, -0.001f, 0.01f) * gunSO.recoilAmount * 4f;
        }

        // Update is called once per frame
        public void Update()
        {
            // if recoil was applied to the gun
            if (transform.localPosition != startPos && recoilApplied)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 0.1f); // move it back to starting position
                
                if (transform.localPosition == startPos)
                    recoilApplied = false;
            }
        }

        // called when player pressed the shoot button
        public override void Shoot()
        {
            // if not currently reloading and has remaining ammo
            if (!reloading && currAmmo > 0)
            {
                if (Time.time >= lastShotTime + shotDelay)
                {
                    // play shoot sound effect
                    if (gunSO.shootSound != null)
                        soundPlayer.PlaySound(gunSO.shootSound, pitch:1 - Random.Range(0, 0.2f));
                    else
                        Debug.Log("Gun shoot sound effect not set");

                    // apply recoil
                    StartCoroutine(ApplyRecoil());

                    currAmmo--; // decrease ammo
                    UpdateAmmoUI();

                    // if ammo is empty try to reload
                    if (currAmmo <= 0 && currMagaines > 0)
                        Invoke(nameof(Reload), 0.5f);

                    DamageEnemy();

                    lastShotTime = Time.time; // set last shot time
                }
            }
        }

        private IEnumerator ApplyRecoil()
        {
            recoilApplied = true;

            // target position after applying recoil
            Vector3 targetPos = transform.localPosition - new Vector3(0, -0.001f, 0.01f) * gunSO.recoilAmount;
            
            if (targetPos.z > maxRecoilPos.z) // if past max recoil
                targetPos = maxRecoilPos;
            
            float recoilTime = 0.2f;
            float elapsedTime = 0;
            while (elapsedTime < recoilTime)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, (elapsedTime / recoilTime));
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
        }

        // called when player pressed the reload button
        public void Reload()
        {
            // if not currently reloading and has remaining magazine and needs ammo
            if (!reloading && currMagaines > 0 && currAmmo < gunSO.ammo) 
            {
                // play reload animation
                if (gunSO.reloadAnimation != null)
                {
                    animationPlayer.clip = gunSO.reloadAnimation;
                    animationPlayer.Play();
                }
                else
                    Debug.Log("Gun reload animation not set");

                // play reload sound effect
                if (gunSO.reloadSound != null)
                    soundPlayer.PlaySound(gunSO.reloadSound);
                else
                    Debug.Log("Gun reload sound effect not set");

                StartCoroutine(CheckEndReload(gunSO.reloadAnimation.length));
            }
        }

        // check when reloading animation ends
        private IEnumerator CheckEndReload(float reloadTime)
        {
            reloading = true;

            yield return new WaitForSeconds(reloadTime);

            if (reloading) // check if reload not cancelled
            {
                currMagaines--; // decrease magazines
                currAmmo = gunSO.ammo; // restore ammo

                UpdateAmmoUI();

                reloading = false;
            }
        }

        // switch to this gun
        public override void Activate()
        {
            Invoke(nameof(UpdateAmmoUI), 0.1f);

            reloading = false; // cancel reloading

            // play switch animation
            if (gunSO.switchAnimation != null)
            {
                animationPlayer.clip = gunSO.switchAnimation;
                animationPlayer.Play();
            }
            else
                Debug.Log("Weapon switch animation not set");

            // play switch sound effect
            if (gunSO.switchSound != null)
                soundPlayer.PlaySound(gunSO.switchSound);
            else
                Debug.Log("Weapon switch sound effect not set");
        }

        // update ammo and magazines ui elements text
        private void UpdateAmmoUI()
        {
            if (ammoText != null)
                ammoText.text = $"{currAmmo} / {gunSO.ammo}";

            if (magazineText != null)
                magazineText.text = $"{currMagaines} / {gunSO.magazines}";
        }
    }
}