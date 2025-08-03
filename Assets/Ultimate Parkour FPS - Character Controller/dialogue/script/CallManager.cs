using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallManager : MonoBehaviour
{
    [Header("UI References")]
    public CallUIManager callUI;

    [Header("NPC Data")]
    public List<NPCState> npcStates = new();

    [Header("Ending Canvases")]
    public GameObject heartbreakUI;
    public GameObject loveEndingUI;

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
        // Hide ending canvases at start
        if (heartbreakUI != null)
            heartbreakUI.SetActive(false);
        if (loveEndingUI != null)
            loveEndingUI.SetActive(false);

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
                npc.happiness += 30;
                break;
            case CallUIManager.ResponseType.Neutral:
                break;
            case CallUIManager.ResponseType.Negative:
                npc.happiness -= 30;
                break;
        }

        npc.happiness = Mathf.Clamp(npc.happiness, 0, 100);
        npc.happinessMeter.value = npc.happiness;

        if (npc.happiness <= 0)
        {
            Debug.Log($"{npc.name} is heartbroken. GAME OVER.");
            TriggerHeartbreakEnding();
            return;
        }

        if (AllNPCsMaxed())
        {
            Debug.Log("All NPCs at 100! YOU FOUND LOVE!");
            TriggerLoveEnding();
            return;
        }

        isCallRunning = false;
    }

    private bool AllNPCsMaxed()
    {
        foreach (var npc in npcStates)
        {
            if (npc.happiness < 100)
                return false;
        }
        return true;
    }

    private void TriggerHeartbreakEnding()
    {
        if (heartbreakUI != null)
            heartbreakUI.SetActive(true);

        StartCoroutine(EndGame());
    }

    private void TriggerLoveEnding()
    {
        if (loveEndingUI != null)
            loveEndingUI.SetActive(true);

        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(10f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
