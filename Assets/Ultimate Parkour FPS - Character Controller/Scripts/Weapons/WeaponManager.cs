using UnityEngine;
using UnityEngine.UI;

/*
    Script to handle weapon input and call weapon functions.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("Guns")]
        [Tooltip("transform of the gun holder component under the camera")]
        [SerializeField] private Transform weaponHolder;
        [Tooltip("currently equipped primary weapon")]
        [SerializeField] private GameObject primaryWeapon;
        [Tooltip("currently equipped secondary weapon")]
        [SerializeField] private GameObject secondaryWeapon;
        [Tooltip("currently equipped melee weapon")]
        [SerializeField] private GameObject meleeWeapon;

        private WeaponScript[] equippedWeapons = new WeaponScript[3]; // currently equipped weapons
        private int currWeaponInd = 0; // current active weapon index in array

        private static int primaryWeaponInd = 0;
        private static int secondaryWeaponInd = 1;
        private static int meleeWeaponInd = 2;

        private GameObject[] weaponPickups = new GameObject[2]; // array to store weapon pickups for dropping

        [Header("Parameters")]
        [Tooltip("delay between switching weapons")]
        [SerializeField] private float weaponSwitchDelay = 0.5f;
        [Tooltip("the maximum range for picking up weapons")]
        [SerializeField] private float weaponPickupRange = 5f;

        private float weaponSwitchTime = 0; // the last weapon switch time

        [Header("Input")]
        [SerializeField] private KeyCode shootButton = KeyCode.Mouse0;
        [SerializeField] private KeyCode reloadButton = KeyCode.R;
        [SerializeField] private KeyCode pickupButton = KeyCode.E;
        [SerializeField] private KeyCode dropButton = KeyCode.F;

        private KeyCode primaryWeaponButton = KeyCode.Alpha1;
        private KeyCode secondaryWeaponButton = KeyCode.Alpha2;
        private KeyCode meleeWeaponButton = KeyCode.Alpha3;

        [Header("UI")]
        [Tooltip("UI weapon pickup text")]
        [SerializeField] private Text pickupText;
        public Text ammoText;
        public Text magazineText;

        // Start is called before the first frame update
        private void Start()
        {
            /* initialize equipped weapons array */
            if (primaryWeapon != null)
                equippedWeapons[primaryWeaponInd] = primaryWeapon.GetComponent<WeaponScript>();
            if (secondaryWeapon != null)
                equippedWeapons[secondaryWeaponInd] = secondaryWeapon.GetComponent<WeaponScript>();
            if (meleeWeapon != null)
                equippedWeapons[meleeWeaponInd] = meleeWeapon.GetComponent<WeaponScript>();

            // switch to first available weapon
            for (int i = 0; i < equippedWeapons.Length; i++)
                if (equippedWeapons[i] != null)
                {
                    SwitchWeapon(i, checkDelay:false);
                    break;
                }
        }

        // Update is called once per frame
        private void Update()
        {
            GameObject newPickup = CheckPickupAvailable(); // get currently looked at pickup

            /* Weapon switching input */

            if (Input.GetKeyDown(primaryWeaponButton) && primaryWeapon != null) // if pressed primary weapon button
                SwitchWeapon(primaryWeaponInd);
            else if (Input.GetKeyDown(secondaryWeaponButton) && secondaryWeapon != null) // if pressed secondary weapon button
                SwitchWeapon(secondaryWeaponInd);
            else if (Input.GetKeyDown(meleeWeaponButton) && meleeWeapon != null) // if pressed melee weapon button
                SwitchWeapon(meleeWeaponInd);
            else if (Input.mouseScrollDelta.y > 0) // if scrolling up
                SwitchWeapon(currWeaponInd - 1);
            else if (Input.mouseScrollDelta.y < 0) // if scrolling down
                SwitchWeapon(currWeaponInd + 1);

            /* Weapon pickup and drop input */

            if (Input.GetKeyDown(pickupButton) && newPickup != null) // if pressed pickup button and looking at pickup
            {
                EquipWeapon(newPickup.name.Split('_')[0]); // equip weapon by pickup name

                // add pickup to weapon pickups array
                WeaponScriptableObject.WeaponType pickupType = newPickup.GetComponent<WeaponScript>().scriptableObject().weaponType;
                weaponPickups[(int)pickupType] = newPickup;
                newPickup.SetActive(false);
            }
            else if (Input.GetKeyDown(dropButton)) // if pressed drop weapon button
                DropWeapon();

            /* Weapon reload input */

            if (Input.GetKeyDown(reloadButton))
                if (equippedWeapons[currWeaponInd] != null && equippedWeapons[currWeaponInd] is GunScript) // if current weapon is a gun
                    equippedWeapons[currWeaponInd].GetComponent<GunScript>().Reload();

            /* Weapon shooting input */

            // if equipped gun is automatic/melee and holding shoot button or manual and pressing shoot button
            if (equippedWeapons[currWeaponInd] != null)
                if ((((equippedWeapons[currWeaponInd] is GunScript && equippedWeapons[currWeaponInd].GetComponent<GunScript>().gunScriptableObject().automaticGun)
                    || equippedWeapons[currWeaponInd] is MeleeScript) && Input.GetKey(shootButton))
                    || Input.GetKeyDown(shootButton))
                {
                    equippedWeapons[currWeaponInd].Shoot();
                }
        }
        
        // Check if the player is looking at a weapon pickup, return it if true
        private GameObject CheckPickupAvailable()
        {
            RaycastHit hit;
            if (Physics.Raycast(weaponHolder.position, weaponHolder.forward, out hit, weaponPickupRange))
            {
                if (hit.transform.tag.Equals("WeaponPickup")) // if player is looking at a weapon pickup
                {
                    // set weapon pickup text according to selected button and weapon
                    pickupText.text = $"press '{pickupButton.ToString()}' to pickup {hit.transform.GetComponent<WeaponScript>().scriptableObject().weaponName}";

                    pickupText.gameObject.SetActive(true); // enable pickup ui prompt

                    return hit.transform.gameObject;
                }
            }

            pickupText.gameObject.SetActive(false); // disable pickup ui prompt
            return null;
        }

        // Switch between equipped weapons
        private void SwitchWeapon(int newWeapon, bool checkDelay = true)
        {
            // if passed weapon switch delay time since last switch
            if (Time.time >= (weaponSwitchTime + weaponSwitchDelay) || !checkDelay)
            {
                // check if new weapon is valid in array
                if (newWeapon < 0)
                    newWeapon = equippedWeapons.Length - 1;
                else if (newWeapon >= equippedWeapons.Length)
                    newWeapon = 0;

                // check if new weapon is equipped and not current weapon
                while (equippedWeapons[newWeapon] == null)
                {
                    // choose next available weapon
                    if (currWeaponInd > newWeapon)
                        newWeapon++;
                    else
                        newWeapon--;

                    // check if new weapon is valid in array
                    if (newWeapon < 0)
                        newWeapon = equippedWeapons.Length - 1;
                    else if (newWeapon >= equippedWeapons.Length)
                        newWeapon = 0;
                }

                // switch to new weapon
                if (primaryWeapon != null)
                    primaryWeapon.SetActive(newWeapon == primaryWeaponInd);
                if (secondaryWeapon != null)
                    secondaryWeapon.SetActive(newWeapon == secondaryWeaponInd);
                if (meleeWeapon != null)
                    meleeWeapon.SetActive(newWeapon == meleeWeaponInd);

                equippedWeapons[newWeapon].Activate(); // activate chosen weapon

                currWeaponInd = newWeapon; // update current weapon index

                weaponSwitchTime = Time.time; // set last weapon switch time
            }
        }

        // Equip a new weapon
        private void EquipWeapon(string weaponName)
        {
            // find new equipped weapon game object
            WeaponScript newWeapon = weaponHolder.Find(weaponName).GetComponent<WeaponScript>();
            WeaponScriptableObject.WeaponType newWeaponType = newWeapon.scriptableObject().weaponType;

            // disable current weapons
            for (int i = 0; i < equippedWeapons.Length; i++)
                if (equippedWeapons[i] != null)
                    equippedWeapons[i].gameObject.SetActive(false);

            // set new weapon
            equippedWeapons[(int)newWeaponType] = newWeapon;
            switch (newWeaponType)
            {
                case WeaponScriptableObject.WeaponType.primary:
                    primaryWeapon = newWeapon.gameObject;
                    break;
                case WeaponScriptableObject.WeaponType.secondary:
                    secondaryWeapon = newWeapon.gameObject;
                    break;
                case WeaponScriptableObject.WeaponType.melee:
                    meleeWeapon = newWeapon.gameObject;
                    break;
            }
            
            SwitchWeapon((int)newWeaponType, checkDelay:false); // switch to new weapon
        }

        // Drop currently equipped weapon
        private void DropWeapon()
        {
            // if there is an active non-melee weapon
            if (equippedWeapons[currWeaponInd] != null && currWeaponInd != meleeWeaponInd)
            {
                // drop gun pickup at player location
                weaponPickups[currWeaponInd].transform.position = weaponHolder.position + weaponHolder.forward;
                weaponPickups[currWeaponInd].SetActive(true);

                // remove equipped weapon
                equippedWeapons[currWeaponInd].gameObject.SetActive(false);
                equippedWeapons[currWeaponInd] = null;
                if (currWeaponInd == primaryWeaponInd)
                    primaryWeapon = null;
                else if (currWeaponInd == secondaryWeaponInd)
                    secondaryWeapon = null;

                weaponPickups[currWeaponInd] = null; // remove pickup from wepaon pickups array

                // try to switch to other available weapon
                for (int i = 0; i < equippedWeapons.Length; i++)
                    if (equippedWeapons[i] != null)
                        SwitchWeapon(i);
            }
        }
    }
}