using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CallUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject callCanvas;
    public Image npcPortrait;
    public TextMeshProUGUI dialogueText;
    public GameObject responseButtons;
    public List<Button> responseButtonList;
    public Slider responseTimerSlider;

    private bool isWaitingForResponse = false;
    private float responseTimeLeft = 10f;
    private List<ResponseOption> currentOptions;

    [Header("Active Call Data")]
    private NPCProfile currentNPC;
    private CallManager callManager;

    [System.Serializable]
    public class ResponseOption
    {
        public string text;
        public ResponseType type;
        public string resultText;
    }

    public enum ResponseType { Positive, Neutral, Negative }

    void Start()
    {
        callCanvas.SetActive(false);
    }

    public void StartCall(NPCProfile npc, string message, CallManager manager)
    {
        currentNPC = npc;
        callManager = manager;

        callCanvas.SetActive(true);
        npcPortrait.sprite = currentNPC.neutralSprite;
        dialogueText.text = message;

        responseButtons.SetActive(false);

        // Phase 1: Show and fill the slider from 0 → 3
        responseTimerSlider.gameObject.SetActive(true);
        responseTimerSlider.maxValue = 3f;
        responseTimerSlider.value = 0f;

        isWaitingForResponse = false;

        StartCoroutine(IntroSliderFill());
    }

   IEnumerator IntroSliderFill()
    {
        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            responseTimerSlider.value = elapsed;
            yield return null;
        }

        responseTimerSlider.gameObject.SetActive(false);
        Invoke(nameof(ShowResponses), 0.1f);
    }


    void ShowResponses()
    {
        currentOptions = new List<ResponseOption>();
        foreach (var opt in currentNPC.options)
        {
            currentOptions.Add(new ResponseOption
            {
                text = opt.text,
                type = opt.type,
                resultText = opt.resultText
            });
        }

        Shuffle(currentOptions);

        for (int i = 0; i < responseButtonList.Count; i++)
        {
            if (i >= currentOptions.Count) break;
            var btn = responseButtonList[i];
            var option = currentOptions[i];
            btn.GetComponentInChildren<TextMeshProUGUI>().text = $"[{i + 1}] {option.text}";
        }

        responseButtons.SetActive(true);

        // Phase 2: Prepare slider for countdown
        responseTimeLeft = 10f;
        responseTimerSlider.maxValue = 10f;
        responseTimerSlider.value = 10f;
        responseTimerSlider.gameObject.SetActive(true);

        StartCoroutine(DelayedStartResponsePhase());
    }

    IEnumerator DelayedStartResponsePhase()
    {
        yield return new WaitForSeconds(0.2f);
        isWaitingForResponse = true;
    }

    void Update()
    {
        if (!isWaitingForResponse) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ChooseResponse(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChooseResponse(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChooseResponse(2);

        responseTimeLeft -= Time.deltaTime;
        responseTimerSlider.value = responseTimeLeft;

        if (responseTimeLeft <= 0f)
        {
            AutoFailChoice();
        }
    }

    void ChooseResponse(int index)
    {
        if (index >= currentOptions.Count) return;

        isWaitingForResponse = false;

        var selected = currentOptions[index];
        dialogueText.text = selected.resultText;

        switch (selected.type)
        {
            case ResponseType.Positive:
                npcPortrait.sprite = currentNPC.happySprite;
                break;
            case ResponseType.Neutral:
                npcPortrait.sprite = currentNPC.neutralSprite;
                break;
            case ResponseType.Negative:
                npcPortrait.sprite = currentNPC.negativeSprite;
                break;
        }

        responseTimerSlider.gameObject.SetActive(false);
        callManager.ResolveCall(currentNPC, selected.type);
        Invoke(nameof(EndCall), 3f);
    }

    void AutoFailChoice()
    {
        if (!isWaitingForResponse) return;

        isWaitingForResponse = false;
        responseTimerSlider.gameObject.SetActive(false);

        npcPortrait.sprite = currentNPC.negativeSprite;
        dialogueText.text = "You didn’t reply in time...";
        callManager.ResolveCall(currentNPC, ResponseType.Negative);

        Invoke(nameof(EndCall), 3f);
    }

    void EndCall()
    {
        callCanvas.SetActive(false);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}

