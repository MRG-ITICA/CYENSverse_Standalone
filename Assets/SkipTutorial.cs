using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.XR.Interaction.Toolkit;

public class SkipTutorial : MonoBehaviour
{
    [Title("Word")]
    [SerializeField, ReadOnly]
    public TextMeshPro text;

    [Title("Interaction")]
    [SerializeField, ReadOnly]
    private XRSimpleInteractable interactable;

    [SerializeField, ReadOnly]
    private SpinnerController loadingEffect;


    private Color initialColor;
    private float initialFade = 0.6f;

    private Vector3 initialScale;
    private float scaleFactor = 1.2f;

    [SerializeField]
    private GameObject tutorial;

    [SerializeField]
    private Menu menu;

    [SerializeField]
    private bool originSkip;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        initialScale = gameObject.transform.localScale;
        initialColor = text.color;
        initialColor.a = initialFade;
        interactable = GetComponent<XRSimpleInteractable>();
        loadingEffect = GetComponentInChildren<SpinnerController>(true);
    }

    public void OnHoverEntered()
    {
        ScaleUp();
        HoverColour();
        StartCoroutine(Hovering());
    }

    private void HoverColour()
    {
        text.color = Color.cyan;
    }

    private void ScaleUp()
    {
        transform.localScale = initialScale * scaleFactor;
    }

    private IEnumerator Hovering()
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        loadingEffect.Show();
        loadingEffect.Load();

        yield return StartCoroutine(FadeTextAlpha(1, loadingEffect.duration));

        loadingEffect.Hide();
        interactable.hoverExited.RemoveAllListeners();
        tutorial.SetActive(false);
        if (!originSkip)
        {
            XrReferences.XrCameraOffset.Rotate(0, 180, 0);
        }
        menu.Show();
    }

    private void ScaleDown()
    {
        gameObject.transform.localScale = initialScale;
    }

    public void OnHoverExit()
    {
        StopAllCoroutines();
        loadingEffect.Hide();
        ScaleDown();
        text.color = initialColor;
    }

    private IEnumerator FadeTextAlpha(float to, float duration)
    {
        // Color variables
        var color = text.color;

        // Time variables
        var timeStep = (color.a - to) / duration * Time.fixedDeltaTime;

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
