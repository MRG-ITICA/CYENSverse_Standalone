using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialInstructions : MonoBehaviour
{

    /*[SerializeField]
    private TextMeshProUGUI[] instructionLabels;

    [SerializeField]
    private TextMeshProUGUI instructionLabel;

    [SerializeField, TextArea]
    private string[] instructions;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private CanvasGroup videoComponent;*/

    [SerializeField]
    private GameObject wordsComponent;

    /*[SerializeField]
    private TMP_Text skipVideo;

    private int instructionCount = 0;*/

    void OnEnable()
    {
        PlayerPrefs.SetString("ageRange", "Prefer not to say");
        //XrReferences.Instance.PositionTowardsCamera(wordsComponent.transform);

        /*var instructionTransform = GetComponent<Transform>();

        instructionLabel = instructionLabels[0];
        instructionLabel.text = instructions[instructionCount];*/

        FadeInAge();
    }

    private void FadeInAge()
    {
        wordsComponent.SetActive(true);
        TMP_Text[] texts = wordsComponent.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text t in texts)
        {
            FadeInWord(t, 2);
        }
    }

    /*private IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(2f);
        FadeInInstruction();
    }

    public void SetInstructionLabel(int label)
    {
        instructionLabel = instructionLabels[label];   
    }

    public IEnumerator NextInstruction(float duration, int label)
    {
        FadeOutInstruction();
        yield return new WaitForSeconds(duration);
        instructionCount++;
        if (instructionCount < instructions.Length)
        {
            instructionLabels[label].text = instructions[instructionCount];

            FadeInInstruction();
        } else
        {
            videoComponent.gameObject.SetActive(false);
            wordsComponent.SetActive(true);
            TMP_Text[] texts = wordsComponent.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text t in texts)
            {
                FadeInWord(t, 2);
            }
        }
    }

    public void FadeInInstruction()
    {
        canvasGroup.LeanAlpha(1, 1f);
    }

    public void FadeOutInstruction()
    {
        canvasGroup.LeanAlpha(0, 1f);
        foreach(TextMeshProUGUI t in instructionLabels)
        {
            t.text = "";
        }
    }

    public void FadeInVideo()
    {
        videoComponent.gameObject.SetActive(true);
        videoComponent.LeanAlpha(1, 1);
        FadeInWord(skipVideo, 2);
    }

    public void FadeOutVideo()
    {
        videoComponent.LeanAlpha(0, 1.5f);
    }*/

    public void FadeInWord(TMP_Text text, float duration)
    {
        StartCoroutine(FadeTextAlpha(text, 1, duration));
    }

    private IEnumerator FadeTextAlpha(TMP_Text text, float to, float duration)
    {
        // Color variables
        var color = text.color;

        // Time variables
        var timeStep = (to - color.a) / duration * Time.fixedDeltaTime;

        var elapsedTime = 0f;
        var waitTime = new WaitForFixedUpdate();
        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;

            color.a += timeStep;

            text.color = color;

            yield return waitTime;
        }

        color.a = to;
        text.color = color;
    }
}
