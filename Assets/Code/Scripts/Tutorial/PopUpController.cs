using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class PopUpController : MonoBehaviour
{
    #region Instruction Texts
    [SerializeField]
    public List<string> introductionInstructions;

    [SerializeField]
    private string turnAroundInstruction;

    [SerializeField]
    private string pinInstruction;

    [SerializeField]
    private string videoInstruction;

    [SerializeField]
    private string exit360Instruction;

    [SerializeField]
    private string openPolaroidInstruction;

    [SerializeField]
    private string closeContentInstruction;
    #endregion

    [SerializeField]
    private GameObject handAnimation;

    [SerializeField]
    private GameObject instructionVisual;

    [SerializeField]
    private TextMeshProUGUI instructionText;

    [SerializeField]
    private Sprite turnAroundImage;

    [SerializeField]
    private float popUpTotalDuration;

    private float currentDuration = 0;

    private CanvasGroup canvasGroup;

    private float time = 0;
    private float timeVideos = 0;
    private float timePins = 0;

    public bool selectedVideo = false;
    public bool selectedPin = false;

    private bool turnAroundShown = false;

    private bool entered = false;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ShowInstructionWithRayAnimation(introductionInstructions[0]);
    }

    // Update is called once per frame
    void Update()
    {
        // Fade out pop-up after set total duration
        if (canvasGroup.alpha == 1)
        {
            if (currentDuration < popUpTotalDuration)
            {
                currentDuration += Time.deltaTime;
            }
            else
            {
                currentDuration = 0;
                StartCoroutine(FadeOutPopUp());
            }
        }
        if (entered)
        {
            time += Time.deltaTime;
            timeVideos += Time.deltaTime;
            timePins += Time.deltaTime;
        }

        if (time > 20 && !turnAroundShown)
        {
            bool facingPins = FindObjectOfType<XrReferences>().FacingPins();
            // Show turn around pop up
            if (!selectedVideo && facingPins || !selectedPin && !facingPins)
            {
                ShowInstructionWithImage(turnAroundInstruction, turnAroundImage);
                turnAroundShown = true;
                time = 0;
            }
        }
        if (timeVideos > 30)
        {
            bool facingPins = FindObjectOfType<XrReferences>().FacingPins();
            if (!selectedVideo && !facingPins)
            {
                ShowInstructionWithRayAnimation(videoInstruction);
                timeVideos = 0;
            }
        }
        if (timePins > 30) {
            bool facingPins = FindObjectOfType<XrReferences>().FacingPins();
            if (!selectedPin && facingPins)
            {
                ShowInstructionWithRayAnimation(pinInstruction);
                timePins = 0;
            }
        }
    }

    public void EnteredMainEnvironment()
    {
        entered = true;
        bool facingPins = FindObjectOfType<XrReferences>().FacingPins();
        if (facingPins)
        {
            ShowInstructionWithRayAnimation(pinInstruction);
        } else
        {
            ShowInstructionWithRayAnimation(videoInstruction);
        }
    }

    public void ShowInstructionWithImage(string text, Sprite sprite)
    {
        StartCoroutine(FadeInPopUp());
        instructionText.text = text;
        instructionVisual.SetActive(true);
        handAnimation.SetActive(false);
        instructionVisual.GetComponent<Image>().sprite = sprite;
    }

    public void ShowInstructionWithRayAnimation(string text)
    {
        StartCoroutine(FadeInPopUp());
        instructionText.text = text;
        instructionVisual.SetActive(false);
        handAnimation.SetActive(true);
    }

    private IEnumerator FadeInPopUp()
    {
        canvasGroup.alpha = 0;
        GetComponent<ParentConstraint>().constraintActive = true;
        currentDuration = 0;
        float elapsedTime = 0;
        float fadeDuration = 0.5f;
        float inverseFadeDuration = 1f / fadeDuration;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime * inverseFadeDuration);
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        GetComponent<ParentConstraint>().constraintActive = false;
    }

    public IEnumerator FadeOutPopUp()
    {
        handAnimation.SetActive(false);
        float elapsedTime = 0;
        float fadeDuration = 0.5f;
        float inverseFadeDuration = 1f / fadeDuration;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime * inverseFadeDuration);
            yield return null;
        }
    }
}
