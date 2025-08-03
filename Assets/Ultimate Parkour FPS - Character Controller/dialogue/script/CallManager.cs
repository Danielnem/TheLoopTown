using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallManager : MonoBehaviour
{
    public CallUIManager callUI;
    public List<NPCState> npcStates = new();

    private bool isCallRunning = false;

    void Start()
    {
        // Set all meters to initial happiness values
        foreach (var npc in npcStates)
        {
            npc.happiness = Mathf.Clamp(npc.happiness, 0, 100);
            npc.happinessMeter.maxValue = 100;
            npc.happinessMeter.minValue = 0;
            npc.happinessMeter.value = npc.happiness;
        }

        StartCoroutine(CallLoop());
    }


    IEnumerator CallLoop()
    {
        while (true)
        {
            if (!isCallRunning)
            {
                float waitTime = Random.Range(5f, 15f);
                yield return new WaitForSeconds(waitTime);

                TriggerRandomCall();
            }

            yield return null;
        }
    }

    void TriggerRandomCall()
    {
        if (npcStates.Count == 0) return;

        var npc = npcStates[Random.Range(0, npcStates.Count)];
        isCallRunning = true;

        callUI.StartCall(npc.profile, npc.profile.introLine, this);
    }

    public void ResolveCall(NPCProfile profile, CallUIManager.ResponseType type)
    {
        var npc = npcStates.Find(n => n.profile == profile);
        if (npc == null) return;

        switch (type)
        {
            case CallUIManager.ResponseType.Positive:
                npc.happiness += 10;
                break;
            case CallUIManager.ResponseType.Neutral:
                break;
            case CallUIManager.ResponseType.Negative:
                npc.happiness -= 10;
                break;
        }

        npc.happiness = Mathf.Clamp(npc.happiness, 0, 100);
        npc.happinessMeter.value = npc.happiness;

        if (npc.happiness <= 0)
        {
            Debug.Log($"{npc.name} is heartbroken. GAME OVER.");
            // TODO: Show game over UI
        }

        isCallRunning = false;
    }
}
