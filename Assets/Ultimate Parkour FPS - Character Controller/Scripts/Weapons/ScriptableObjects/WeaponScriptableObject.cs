using UnityEngine;

/*
    Scriptable Object class to hold weapon info and stats.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
    public class WeaponScriptableObject : ScriptableObject
    {
        public enum WeaponType { primary, secondary, melee }

        [Header("Info")]

        [Tooltip("name of the weapon")]
        public string weaponName;
        [Tooltip("the type of the weapon")]
        public WeaponType weaponType;

        [Header("Stats")]

        [Tooltip("damage per bullet")]
        public float damage;
        [Tooltip("bullets per second")]
        public float fireRate;
        [Tooltip("damage range for the weapon")]
        public float range;

        [Header("Sounds")]

        [Tooltip("shoot sound audio clip")]
        public AudioClip shootSound;
        [Tooltip("weapon switch sound audio clip")]
        public AudioClip switchSound;

        [Header("Animations")]
        
        [Tooltip("switch animation clip")]
        public AnimationClip switchAnimation;
    }
}