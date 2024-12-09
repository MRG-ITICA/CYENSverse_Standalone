using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class PopUpController : MonoBehaviour
{
    #region Instruction Texts
    [SerializeField, TextArea]
    public List<string> introductionInstructions;

    [SerializeField]
    public string turnAroundInstruction;

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

    [SerializeField]
    public string futureWordInstruction;
    #endregion

    [SerializeField]
    private GameObject handAnimation;

    [SerializeField]
    private GameObject instructionVisual;

    [SerializeField]
    public TextMeshProUGUI instructionText;

    [SerializeField]
    public Sprite turnAroundImage;

    private LocalizeStringEvent localizedString;

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
        contentController = FindObjectOfType<ContentController>();
        xrReferences = FindObjectOfType<XrReferences>();
        localizedString = GetComponent<LocalizeStringEvent>();
        StartCoroutine(Introduction());
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

        // Show turn around pop up
        if (time > 20 && !turnAroundShown && !contentController.in360)
        {
            bool facingPins = xrReferences.FacingPins();
            if (!selectedVideo && facingPins)
            {
                ShowInstructionWithImage(turnAroundInstruction, turnAroundImage, 0, 6);
                turnAroundShown = true;
                time = 0;
            }
        }

        // Show open video instruction
        if (timeVideos > 20 && !contentController.in360)
        {
            bool facingVideos = xrReferences.FacingVideos();
            if (!selectedVideo && facingVideos)
            {
                ShowInstructionWithRayAnimation(videoInstruction, 1, 6);
                timeVideos = 0;
            }
        }

        // Show open pin instruction
        if (timePins > 20 && !contentController.in360) {
            bool facingPins = xrReferences.FacingPins();
            if (!selectedPin && facingPins)
            {
                ShowInstructionWithRayAnimation(pinInstruction, 0, 6);
                timePins = 0;
            }
        }
    }

    private IEnumerator Introduction()
    {
        ShowInstructionWithRayAnimation(introductionInstructions[0], 3, 4);
        yield return new WaitForSeconds(7);
        ShowInstructionWithRayAnimation(introductionInstructions[1], 2, 3);
        yield return new WaitForSeconds(5);
        FindObjectOfType<LanguageSelection>().FadeIn();
        ShowInstructionWithRayAnimation(introductionInstructions[2], 1, 5);
    }

    // Called after user selects ring to enter main environment
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
        StartCoroutine(FadeInPopUp(delay, text));
        instructionVisual.SetActive(true);
        handAnimation.SetActive(false);
        instructionVisual.GetComponent<Image>().sprite = sprite;
    }

    public void ShowInstructionWithRayAnimation(string text, float delay, float duration)
    {
        StartCoroutine(FadeOutPopUp());
        SetPopUpDuration(duration);
        StartCoroutine(FadeInPopUp(delay, text));
        instructionVisual.SetActive(false);
        StartCoroutine(ActivateHandAnimation(delay));
    }

    public IEnumerator PopUps360()
    {
        ShowInstructionWithImage(turnAroundInstruction, turnAroundImage, 2, 3);
        yield return new WaitForSeconds(6);
        ShowInstructionWithRayAnimation(openPolaroidInstruction, 1, 6);
    }
    
    private IEnumerator ActivateHandAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        handAnimation.SetActive(true);
    }

    private IEnumerator FadeInPopUp(float delay, string text)
    {
        yield return new WaitForSeconds(delay);

        localizedString.StringReference.TableEntryReference = text;
        localizedString.StringReference.RefreshString();

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
