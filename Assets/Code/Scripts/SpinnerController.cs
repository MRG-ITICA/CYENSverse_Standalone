// *********************************************
// *********************************************
// <info>
//   File: SpinnerController.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/06 6:33 PM
// </info>
// <copyright file="SpinnerController.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class SpinnerController : MonoBehaviour
{
    Image[] spinnerElements;
    public float duration;
    private int elementCount;
    private float singleDuration;
    private int counter = 0;
    private readonly Color fadeOutColor = new Color(.3f, .3f, .3f);
    private bool loadingStatus = false;

    public AudioSource audioSource;

    void Awake()
    {
        spinnerElements = GetComponentsInChildren<Image>(true);
        elementCount = spinnerElements.Length;
        singleDuration = duration / elementCount;
        audioSource = GameObject.FindGameObjectWithTag("UIAudio").GetComponent<AudioSource>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsLoading()
    {
        return loadingStatus;
    }

    private void OnDisable()
    {
        FadeOut();
    }

    public void Load()
    {
        loadingStatus = true;
        StartCoroutine(LoadAsync());
    }

    void FadeOut()
    {
        foreach (Image img in spinnerElements)
        {
            img.color = fadeOutColor;
        }
    }

    private IEnumerator LoadAsync()
    {
        FadeOut();

        var waitTime = new WaitForSeconds(singleDuration);
        counter = 0;
        while (counter < elementCount)
        {
            spinnerElements[counter].color = Color.white;
            counter++;
            yield return waitTime;
        }

        audioSource.Play();
        StopAllCoroutines();
        loadingStatus = false;
    }
}