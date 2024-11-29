// *********************************************
// *********************************************
// <info>
//   File: FadeController.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/10 4:05 PM
// </info>
// <copyright file="FadeController.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeController : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public GameObject menu;

    public LayerMask startCullingMask;
    public ContentController contentController;

    public Categories categoryManager;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        MoveUnderCamera();
    }

    // Start is called before the first frame update
    void Start()
    {
        XrReferences.Instance.ChangeCameraCullingMask(startCullingMask.value);
    }

    /// <summary>
    ///     Move the fade controller in front of the camera and reparent it to it
    /// </summary>
    private void MoveUnderCamera()
    {
        var cameraTransform = XrReferences.XrCameraTransform;
        var fadeTransform = transform;
        fadeTransform.SetParent(cameraTransform);
        fadeTransform.SetLocalPositionAndRotation(Vector3.forward, Quaternion.identity);
    }

    public void Load360Image(IImageController pinImageController)
    {
        StartCoroutine(Fade360Image(pinImageController));
    }

    public IEnumerator Fade360Image(IImageController pinImageController)
    {
        Debug.Log("fade image");
        float elapsedTime = 0;
        float fadeDuration = 0.5f;
        float inverseFadeDuration = 1f / fadeDuration;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime * inverseFadeDuration);
            yield return null;
        }

        contentController.EnteringImage(pinImageController);
        yield return new WaitForSeconds(0.25f);
        FadeToTransparency(1f);
    }

    public void LoadMainScene(bool fromMenu, float fadeOutDuration = 1.5f, float fadeInDelay = 0.25f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeMainScene(fromMenu, fadeOutDuration, fadeInDelay));
    }

    public IEnumerator FadeMainScene(bool fromMenu, float fadeOutDuration = 1.5f, float fadeInDelay = 0.25f)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeOutDuration);
            yield return null;
        }

        var fadeBackDuration = 0.5f;
        if (fromMenu)
        {
            menu.SetActive(false);
            categoryManager.StartCategories(fadeInDelay + fadeBackDuration);
        }
        else
        {
            contentController.ExitingImageToHome();
        }

        //fadedOut = true;

        // If not future
        if (categoryManager.currentCategoryIndex <= categoryManager.categories.Count - 1)
        {
            XrReferences.Instance.ChangeCameraCullingMask();
        }

        yield return new WaitForSeconds(fadeInDelay);

        FadeToTransparency(fadeBackDuration);
    }

    public void FadeToTransparency(float fadeDuration)
    {
        StartCoroutine(FadeAsync(1, 0, fadeDuration));
    }

    public void FadeFromTransparency(float fadeDuration)
    {
        StartCoroutine(FadeAsync(0, 1, fadeDuration));
    }

    private IEnumerator FadeAsync(float from, float to, float fadeDuration)
    {
        float elapsedTime = 0;
        var waitTime = new WaitForFixedUpdate();
        canvasGroup.alpha = from;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / fadeDuration);
            yield return waitTime;
            elapsedTime += Time.fixedDeltaTime;
        }

        canvasGroup.alpha = to;
    }
}