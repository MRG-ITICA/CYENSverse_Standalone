using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialPin : MonoBehaviour
{
    [SerializeField]
    private SpinnerController loadingEffect;

    public float imageScaleFactor;

    private Vector3 initialScale;

    [SerializeField]
    private TutorialInstructions instructionsManager;

    [SerializeField]
    private XRSimpleInteractable interactable;

    [SerializeField]
    private TutorialVideo videoTutorial;

    [SerializeField]
    private GameObject hoverPinAnim;

    [SerializeField]
    private GameObject outline;

    [SerializeField]
    private Material disabledPin;

    private void Awake()
    {
        initialScale = gameObject.transform.localScale;
    }

    public void HoverEntered()
    {
        ScaleUp();
        StartCoroutine(Hovering());
    }

    IEnumerator Hovering()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Loading());
    }

    private void ScaleUp()
    {
        gameObject.transform.localScale *= imageScaleFactor;
    }

    private void ScaleDown()
    {
        gameObject.transform.localScale = initialScale;
    }

    IEnumerator Loading()
    {
        loadingEffect.Show();
        loadingEffect.Load();
        yield return new WaitUntil(() => !loadingEffect.IsLoading());
        DisablePin();
        loadingEffect.Hide();
        hoverPinAnim.SetActive(false);
        instructionsManager.FadeOutInstruction();
        // Turn around instruction
        StartCoroutine(instructionsManager.NextInstruction(1f, 0));
        instructionsManager.FadeInVideo();
        StartCoroutine(instructionsManager.NextInstruction(2, 1));
    }

    private void DisablePin()
    {
        interactable.enabled = false;
        gameObject.GetComponent<MeshRenderer>().material = disabledPin;
        outline.SetActive(false);
    }

    public void HoverExited()
    {
        StopAllCoroutines();
        ScaleDown();
        loadingEffect.Hide();
    }
}
