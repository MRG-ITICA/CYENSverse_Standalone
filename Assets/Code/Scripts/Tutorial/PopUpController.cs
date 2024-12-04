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
    public string exit360Instruction;

    [SerializeField]
    public string openPolaroidInstruction;

    [SerializeField]
    public string closeContentInstruction;
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

    private ContentController contentController;

    private XrReferences xrReferences;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ShowInstructionWithRayAnimation(introductionInstructions[0], 2f, 6);
        contentController = FindObjectOfType<ContentController>();
        xrReferences = FindObjectOfType<XrReferences>();
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

        if (time > 20 && !turnAroundShown && !contentController.in360)
        {
            bool facingPins = xrReferences.FacingPins();
            // Show turn around pop up
            if (!selectedVideo && facingPins)
            {
                ShowInstructionWithImage(turnAroundInstruction, turnAroundImage, 0, 6);
                turnAroundShown = true;
                time = 0;
            }
        }
        if (timeVideos > 20 && !contentController.in360)
        {
            bool facingVideos = xrReferences.FacingVideos();
            if (!selectedVideo && facingVideos)
            {
                ShowInstructionWithRayAnimation(videoInstruction, 0, 6);
                timeVideos = 0;
            }
        }
        if (timePins > 20 && !contentController.in360) {
            bool facingPins = xrReferences.FacingPins();
            if (!selectedPin && facingPins)
            {
                ShowInstructionWithRayAnimation(pinInstruction, 0, 6);
                timePins = 0;
            }
        }
    }

    public void EnteredMainEnvironment()
    {
        entered = true;
        bool facingPins = xrReferences.FacingPins();
        bool facingVideos = xrReferences.FacingVideos();
        if (facingPins)
        {
            ShowInstructionWithRayAnimation(pinInstruction, 2, 6);
        } else if (facingVideos)
        {
            ShowInstructionWithRayAnimation(videoInstruction, 2, 6);
        }
    }

    public void ShowInstructionWithImage(string text, Sprite sprite, float delay, float duration)
    {
        StartCoroutine(FadeOutPopUp());
        SetPopUpDuration(duration);
        StartCoroutine(FadeInPopUp(delay));
        instructionText.text = text;
        instructionVisual.SetActive(true);
        handAnimation.SetActive(false);
        instructionVisual.GetComponent<Image>().sprite = sprite;
    }

    public void ShowInstructionWithRayAnimation(string text, float delay, float duration)
    {
        StartCoroutine(FadeOutPopUp());
        SetPopUpDuration(duration);
        StartCoroutine(FadeInPopUp(delay));
        instructionText.text = text;
        instructionVisual.SetActive(false);
        StartCoroutine(ActivateHandAnimation(delay));
    }
    
    private IEnumerator ActivateHandAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        handAnimation.SetActive(true);
    }

    private IEnumerator FadeInPopUp(float delay)
    {
        yield return new WaitForSeconds(delay);
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
        float fadeDuration = 0.2f;
        float inverseFadeDuration = 1f / fadeDuration;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime * inverseFadeDuration);
            yield return null;
        }
    }

    private void SetPopUpDuration(float duration)
    {
        popUpTotalDuration = duration;
    }
}
