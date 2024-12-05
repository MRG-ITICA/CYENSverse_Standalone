// *********************************************
// *********************************************
// <info>
//   File: WordController.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/10 12:42 PM
// </info>
// <copyright file="WordController.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using TMPro;

using TriInspector;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class WordController : MonoBehaviour
{
    [Title("Word")]
    [SerializeField, ReadOnly]
    public TextMeshPro text;

    private WordData data;

    private Vector3 initialScale;
    private float scaleFactor = 1.2f;

    private bool selected = false;

    private Color initialColor;
    private float initialFade = 1f;

    [Title("Interaction")]
    [SerializeField, ReadOnly]
    private XRSimpleInteractable interactable;

    [SerializeField, ReadOnly]
    private SpinnerController loadingEffect;

    [SerializeField]
    private BoxCollider textCollider;

    [SerializeField]
    private float textColliderPadding = 0.05f;

    private UsersFile user;

    [SerializeField]
    private Color hoverColor;

    [SerializeField]
    private Color selectedColor;

    [Button(ButtonSizes.Medium, "Set collider size")]
    private void SetColliderSizeButton()
    {
        SetColliderSize();
    }

    void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        text = GetComponentInChildren<TextMeshPro>();
        loadingEffect = GetComponentInChildren<SpinnerController>(true);
        textCollider = GetComponentInChildren<BoxCollider>();
    }

    void Start()
    {
        initialScale = gameObject.transform.localScale;
        initialColor = text.color;
        initialColor.a = initialFade;
        user = GetComponentInParent<UsersFile>();
    }

    private void Update()
    {
        // TODO: check if this is necessary
        if (!selected) return;
        transform.localScale = initialScale * scaleFactor;
    }

    public bool IsSelected()
    {
        return selected;
    }

    private void SetColliderSize()
    {
        var textRectTransform = text.GetComponent<RectTransform>();
        if (textRectTransform == null) return;
        var rect = textRectTransform.rect;
        textCollider.size = new Vector3(rect.width + textColliderPadding, rect.height + textColliderPadding, 0.01f);
    }

    public void Hide()
    {
        StopAllCoroutines();
        Deactivate();
        FadeOutWord();
    }

    public void Activate()
    {
        interactable.enabled = true;
    }

    public void Deactivate()
    {
        interactable.enabled = false;
    }

    public void SetData(WordData dataValue)
    {
        data = dataValue;
        data.SetController(this);

        text.text = data.word;

        StartCoroutine(SetTextColliderSize());
    }

    private IEnumerator SetTextColliderSize()
    {
        yield return new WaitForEndOfFrame();
        SetColliderSize();
    }

    public void ShowWordBasedOnUserSelections(int sumOfWordCounts, int numberOfWords)
    {
        var scaleValue = 0f;
        if (sumOfWordCounts > 0)
        {
            scaleValue = (float)data.count * numberOfWords / sumOfWordCounts;
        }
        transform.localScale *= 1 + scaleValue * 0.4f;
        FadeInWord(0.2f + scaleValue, 3);
    }

    public void FadeInWord(float duration = 1)
    {
        StartCoroutine(FadeTextAlpha(initialFade, duration));
    }

    public void FadeInWord(float? to, float duration = 1)
    {
        StartCoroutine(FadeTextAlpha(to ?? initialFade, duration));
    }

    private void FadeOutWord(float toAlpha = 0, float duration = 2)
    {
        StartCoroutine(FadeTextAlpha(toAlpha, duration));
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

    public void OnHoverEntered()
    {
        ScaleUp();
        HoverColour();
        StartCoroutine(Hovering());
    }

    private void HoverColour()
    {
        text.color = hoverColor;
    }

    private void ScaleUp()
    {
        transform.localScale = initialScale * scaleFactor;
    }

    private IEnumerator Hovering()
    {
        yield return new WaitForSeconds(0.3f);
        text.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0f);
        StartCoroutine(selected ? UnloadWord() : LoadWord());
    }

    IEnumerator UnloadWord()
    {
        loadingEffect.Show();
        loadingEffect.Load();
        var elapsedTime = 0f;
        var duration = loadingEffect.duration;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, initialFade, elapsedTime / duration);
            Color newColor = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
            text.color = newColor;
            yield return null;
        }

        loadingEffect.Hide();
        interactable.hoverEntered.AddListener(delegate { OnHoverEntered(); });
        interactable.hoverExited.AddListener(delegate { OnHoverExit(); });
        WordUnselected();
    }

    IEnumerator LoadWord()
    {
        loadingEffect.Show();
        loadingEffect.Load();

        yield return StartCoroutine(FadeTextAlpha(1, loadingEffect.duration));

        loadingEffect.Hide();
        interactable.hoverExited.RemoveAllListeners();
        WordSelected();
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
        text.color = selected ? selectedColor : initialColor;
        if (selected) text.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.8f);
    }

    private void WordSelected()
    {
        FindObjectOfType<WordCloud>().wordSelected = true;
        selected = true;
        text.color = selectedColor;
        // Make selected words glow
        text.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.8f);
        text.UpdateMeshPadding();

        // Add count for word in dictionary
        data.IncreaseCount();
    }

    private void WordUnselected()
    {
        selected = false;
        text.color = initialColor;
        text.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0f);
        text.UpdateMeshPadding();

        data.DecreaseCount();
    }
}