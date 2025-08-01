using UnityEngine;

/*
    Scriptable Object class to hold gun info and stats.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    [CreateAssetMenu(fileName = "NewGun", menuName = "Weapons/Gun")]
    public class GunSO : WeaponScriptableObject
    {
        [Header("Info")]
        
        [Tooltip("if the gun is automatic")]
        public bool automaticGun;

        [Header("Stats")]

        [Tooltip("max bullets in magazine")]
        public int ammo;
        [Tooltip("number of magazines")]
        public int magazines;
        [Tooltip("recoil amount per bullet")]
        public float recoilAmount;

        [Header("Sounds")]
        
        [Tooltip("reload sound audio clip")]
        public AudioClip reloadSound;

        [Header("Animations")]
        
        [Tooltip("reload animation clip")]
        public AnimationClip reloadAnimation;
    }
}