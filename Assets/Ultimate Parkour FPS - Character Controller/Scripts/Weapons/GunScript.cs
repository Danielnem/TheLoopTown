using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateParkourFPS
{
    public class GunScript : WeaponScript
    {
        [SerializeField] private GunSO gunSO;
        public override WeaponScriptableObject scriptableObject() => gunSO;
        public GunSO gunScriptableObject() => gunSO;

        private bool reloading;
        private int currAmmo;
        private int currMagaines;

        private float lastShotTime;
        private float shotDelay;

        private Vector3 startPos;
        private Vector3 maxRecoilPos;
        private bool recoilApplied;

        [Header("Muzzle Flash")]
        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private float flashDuration = 0.05f;

        #region components
        private Animation animationPlayer;
        private SoundPlayer soundPlayer;

        private Text ammoText;
        private Text magazineText;
        #endregion

        private void Awake()
        {
            animationPlayer = GetComponent<Animation>();
            soundPlayer = GetComponent<SoundPlayer>();

            if (transform.root.GetComponent<WeaponManager>() != null)
            {
                ammoText = transform.root.GetComponent<WeaponManager>().ammoText;
                magazineText = transform.root.GetComponent<WeaponManager>().magazineText;
            }
        }

        private void Start()
        {
            currAmmo = gunSO.ammo;
            currMagaines = gunSO.magazines;

            shotDelay = 1f / gunSO.fireRate;

            startPos = transform.localPosition;
            maxRecoilPos = startPos - new Vector3(0, -0.001f, 0.01f) * gunSO.recoilAmount * 4f;
        }

        public void Update()
        {
            if (transform.localPosition != startPos && recoilApplied)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 0.1f);

                if (transform.localPosition == startPos)
                    recoilApplied = false;
            }
        }

        public override void Shoot()
        {
            if (!reloading && currAmmo > 0)
            {
                if (Time.time >= lastShotTime + shotDelay)
                {
                    if (gunSO.shootSound != null)
                        soundPlayer.PlaySound(gunSO.shootSound, pitch: 1 - Random.Range(0, 0.2f));
                    else
                        Debug.Log("Gun shoot sound effect not set");

                    StartCoroutine(ApplyRecoil());

                    currAmmo--;
                    UpdateAmmoUI();

                    if (currAmmo <= 0 && currMagaines > 0)
                        Invoke(nameof(Reload), 0.5f);

                    DamageEnemy();

                    if (muzzleFlash != null)
                        StartCoroutine(FlashMuzzle());

                    lastShotTime = Time.time;
                }
            }
        }

        private IEnumerator ApplyRecoil()
        {
            recoilApplied = true;
            Vector3 targetPos = transform.localPosition - new Vector3(0, -0.001f, 0.01f) * gunSO.recoilAmount;

            if (targetPos.z > maxRecoilPos.z)
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

        private IEnumerator FlashMuzzle()
        {
            muzzleFlash.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            muzzleFlash.SetActive(false);
        }

        public void Reload()
        {
            if (!reloading && currMagaines > 0 && currAmmo < gunSO.ammo)
            {
                if (gunSO.reloadAnimation != null)
                {
                    animationPlayer.clip = gunSO.reloadAnimation;
                    animationPlayer.Play();
                }
                else
                    Debug.Log("Gun reload animation not set");

                if (gunSO.reloadSound != null)
                    soundPlayer.PlaySound(gunSO.reloadSound);
                else
                    Debug.Log("Gun reload sound effect not set");

                StartCoroutine(CheckEndReload(gunSO.reloadAnimation.length));
            }
        }

        private IEnumerator CheckEndReload(float reloadTime)
        {
            reloading = true;
            yield return new WaitForSeconds(reloadTime);

            if (reloading)
            {
                currMagaines--;
                currAmmo = gunSO.ammo;
                UpdateAmmoUI();
                reloading = false;
            }
        }

        public override void Activate()
        {
            Invoke(nameof(UpdateAmmoUI), 0.1f);

            reloading = false;

            if (gunSO.switchAnimation != null)
            {
                animationPlayer.clip = gunSO.switchAnimation;
                animationPlayer.Play();
            }
            else
                Debug.Log("Weapon switch animation not set");

            if (gunSO.switchSound != null)
                soundPlayer.PlaySound(gunSO.switchSound);
            else
                Debug.Log("Weapon switch sound effect not set");
        }

        private void UpdateAmmoUI()
        {
            if (ammoText != null)
                ammoText.text = $"{currAmmo} / {gunSO.ammo}";

            if (magazineText != null)
                magazineText.text = $"{currMagaines} / {gunSO.magazines}";
        }
    }
}
