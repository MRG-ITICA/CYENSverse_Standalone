// *********************************************
// *********************************************
// <info>
//   File: Exit360.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/09 4:32 PM
// </info>
// <copyright file="Exit360.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using UnityEngine;

public class Exit360 : MonoBehaviour
{
    public GameObject loadingEffect;

    private Vector3 initialScale;
    public float imageScaleFactor;

    private FadeController fadeController;

    void Awake()
    {
        fadeController = XrReferences.FadeToBlack;
    }

    // Start is called before the first frame update
    void Start()
    {
        initialScale = gameObject.transform.localScale;
    }

    void OnEnable()
    {
        loadingEffect.SetActive(false);
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
        loadingEffect.SetActive(true);
        var spinner = loadingEffect.GetComponent<SpinnerController>();
        spinner.Load();
        yield return new WaitUntil(() => !spinner.IsLoading());
        loadingEffect.SetActive(false);
        fadeController.LoadMainScene(false);
    }

    public void HoverExited()
    {
        StopAllCoroutines();
        ScaleDown();
        loadingEffect.SetActive(false);
    }
}