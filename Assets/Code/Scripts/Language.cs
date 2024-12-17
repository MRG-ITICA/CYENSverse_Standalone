using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.UIElements;
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

    private ContentController contentController;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        initialScale = gameObject.transform.localScale;
        initialColor = text.color;
        initialColor.a = initialFade;
        interactable = GetComponent<XRSimpleInteractable>();
        loadingEffect = GetComponentInChildren<SpinnerController>(true);

        contentController = FindObjectOfType<ContentController>();
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

        Menu[] menus = contentController.menus;
        foreach (var menu in menus)
        {
            menu.Show();
        }
        contentController.ShowMenuSkybox();
        FindObjectOfType<ContentController>().SetFloor360Mode(true);
        PopUpController popUpController = FindObjectOfType<PopUpController>();
        popUpController.ShowInstructionWithRayAnimation(popUpController.introductionInstructions[2], 2, 5);
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
