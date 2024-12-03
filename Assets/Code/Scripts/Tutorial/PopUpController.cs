using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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


    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ShowInstructionWithRayAnimation(introductionInstructions[0]);
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void ShowInstructionWithImage(string text, Sprite sprite)
    {
        StartCoroutine(FadeInPopUp());
        instructionText.text = text;
        instructionVisual.GetComponent<Image>().sprite = sprite;
    }

    public void ShowInstructionWithRayAnimation(string text)
    {
        StartCoroutine(FadeInPopUp());
        instructionText.text = text;
        handAnimation.SetActive(true);
    }

    private IEnumerator FadeInPopUp()
    {
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
    }

    private IEnumerator FadeOutPopUp()
    {
        float elapsedTime = 0;
        float fadeDuration = 0.5f;
        float inverseFadeDuration = 1f / fadeDuration;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime * inverseFadeDuration);
            yield return null;
        }
        handAnimation.SetActive(false);
    }
}
