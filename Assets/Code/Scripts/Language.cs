using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.XR.Interaction.Toolkit;

public class Language : MonoBehaviour
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

        yield return new WaitUntil(() => !loadingEffect.IsLoading());

        loadingEffect.Hide();
        interactable.hoverExited.RemoveAllListeners();
        SaveLanguage();
        tutorial.SetActive(false);
        menu.Show();
        FindObjectOfType<ContentController>().SetFloor360Mode(true);
        PopUpController popUpController = FindObjectOfType<PopUpController>();
        popUpController.ShowInstructionWithRayAnimation(popUpController.introductionInstructions[3], 2, 5);
    }

    private void SaveLanguage()
    {

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
}
