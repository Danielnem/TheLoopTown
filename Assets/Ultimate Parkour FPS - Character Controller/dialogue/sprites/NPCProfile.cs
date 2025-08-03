using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NPC/New NPC Profile")]
public class NPCProfile : ScriptableObject
{
    public string npcName;

    public Sprite neutralSprite;
    public Sprite happySprite;
    public Sprite negativeSprite;

    [TextArea]
    public string introLine;

    [System.Serializable]
    public class DialogueOption
    {
        [TextArea]
        public string text; // What the player sees
        public CallUIManager.ResponseType type;
        [TextArea]
        public string resultText; // What NPC says/reacts with
    }

    public List<DialogueOption> options;
}
