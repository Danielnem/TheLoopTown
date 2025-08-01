using UnityEngine;

/*
    Scriptable Object class to hold gun info and stats.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    [CreateAssetMenu(fileName = "NewMelee", menuName = "Weapons/Melee")]
    public class MeleeSO : WeaponScriptableObject
    {
        [Header("Animations")]

        [Tooltip("attack animation clip")]
        public AnimationClip attackAnimation;
    }
}