using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NPCState
{
    public string name;
    public NPCProfile profile;
    public int happiness = 50; // default
    public Slider happinessMeter;
}
