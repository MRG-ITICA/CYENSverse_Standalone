// *********************************************
// *********************************************
// <info>
//   File: VideoController.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/11 10:31 AM
// </info>
// <copyright file="VideoController.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections;
using System.Collections.Generic;

using TriInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class VideoController : MonoBehaviour
{
    private Vector3 initialImageScale;
    private Vector3 initialPosition;
    private Vector3 initialLocalPosition;

    [SerializeField]
    private XRSimpleInteractable videoPokeInteractable;

    [SerializeField]
    private VideoPlayer videoPlayer;

    private List<Transform> allVideos;
    private GameObject videoContent;

    #region Video references

    public int category { get; private set; }
    private Texture videoPreviewCap;
    private AssetReference assetReference;
    private AsyncOperationHandle<VideoClip> assetHandle;
    private bool isVideoReady = false;
    private bool isQueuedToPlayOrPlaying = false;

    #endregion Video references

    private VideoGenerator videoGenerator;

    [SerializeField]
    private SpinnerController loadingEffect;

    #region Close button
    [SerializeField]
    private XRSimpleInteractable closeButton;
    [SerializeField]
    private SpinnerController closeButtonLoadingEffect;
    #endregion

    #region Frame

    [Title("Frame")]
    [SerializeField]
    private Image frame;

    [SerializeField]
    private Sprite disabledFrame;

    [SerializeField]
    private Sprite enabledFrame;

    [SerializeField]
    private Sprite hoveredFrame;

    [SerializeField]
    private Sprite selectedFrame;

    [SerializeField]
    private Sprite viewedFrame;

    #endregion Frame

    public float scaleFactor;

    [SerializeField]
    private RawImage videoScreen;

    public Action<VideoController> OnVideoSelected;

    private bool videoViewed = false;

    void Reset()
    {
        videoScreen = GetComponentInChildren<RawImage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        allVideos = new List<Transform>();

        initialImageScale = transform.localScale;
        initialLocalPosition = transform.localPosition;
        initialPosition = transform.position;

        videoPlayer.Pause();
        videoContent = transform.parent.gameObject;

        foreach (Transform v in videoContent.transform)
        {
            allVideos.Add(v);
        }
    }

    void OnEnable()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
        UnloadAsset();
    }

    public void Initialize(VideoGenerator generator, int categoryValue, Texture previewImageValue,
        RenderTexture targetTexture, AssetReference assetReferenceValue)
    {
        videoGenerator = generator;
        category = categoryValue;
        SetPreview(previewImageValue);
        SetVideoClip(targetTexture);
        assetReference = assetReferenceValue;
    }

    public void SetPreview(Texture previewImageValue)
    {
        videoPreviewCap = previewImageValue;
        SetToPreviewScreen();
    }

    private void SetToPreviewScreen()
    {
        videoPlayer.Stop();
        videoScreen.texture = videoPreviewCap;
    }

    private void SetToVideoRenderingScreen()
    {
        videoScreen.texture = videoPlayer.targetTexture;
    }

    public void SetVideoClip(RenderTexture targetTexture)
    {
        videoPlayer.Stop();
        videoPlayer.targetTexture = targetTexture;
    }

    public void Activate()
    {
        videoPokeInteractable.enabled = true;
    }

    public void Deactivate()
    {
        videoPokeInteractable.enabled = false;
    }

    public void FadeOutVideo()
    {
        FadeOutFrame();
        FadeOutScreen();
    }

    private void FadeOutFrame()
    {
        frame.sprite = disabledFrame;
        frame.CrossFadeAlpha(0.2f, 0.5f, false);
    }

    private void FadeOutScreen()
    {
        SetToPreviewScreen();
        videoScreen.CrossFadeAlpha(0.2f, 0.5f, false);
    }

    public void FadeInVideo()
    {
        FadeInFrame();
        FadeInScreen();
    }

    private void FadeInFrame()
    {
        if (videoViewed)
        {
            frame.sprite = viewedFrame;
        } else
        {
            frame.sprite = enabledFrame;
        }
        frame.CrossFadeAlpha(1f, 0.5f, false);
    }

    private void FadeInScreen()
    {
        videoScreen.CrossFadeAlpha(1f, 0.5f, false);
    }

    public void FadeFrame(float toValue, float duration)
    {
        frame.CrossFadeAlpha(toValue, duration, false);
    }


    public void DequeueVideo()
    {
        isQueuedToPlayOrPlaying = false;
    }

    public void QueueToPlay()
    {
        isQueuedToPlayOrPlaying = true;
        if (!isVideoReady) return;
        Play();
    }

    protected void Play()
    {
        videoPlayer.Play();
        videoViewed = true;
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        SetToPreviewScreen();
        DequeueVideo();
    }

    protected bool LoadAsset()
    {
        if (isVideoReady && assetHandle.IsValid()) return true;

        StartCoroutine(LoadAssetAsync());
        return false;
    }

    protected IEnumerator LoadAssetAsync()
    {
        if (assetHandle.IsValid() || videoPlayer.clip != null) yield break;

        assetHandle = assetReference.LoadAssetAsync<VideoClip>();
        isVideoReady = false;

        var loadWaitTime = new WaitForFixedUpdate();
        float elapsedTime = 0;
        while (!assetHandle.IsDone)
        {
            Debug.Log($"Loading video asset async, percentage: {assetHandle.PercentComplete}, asset status: {assetHandle.Status}");
            elapsedTime += Time.fixedDeltaTime;
            if (elapsedTime > 1)
            {
                assetHandle.Release();
                yield return new WaitForEndOfFrame();
                elapsedTime = 0;
                assetHandle = assetReference.LoadAssetAsync<VideoClip>();
            }
            yield return loadWaitTime;
        }

        if (assetHandle.Status != AsyncOperationStatus.Succeeded) yield break;

        videoPlayer.clip = assetHandle.Result;
        Debug.Log("Video is ready");
        isVideoReady = true;

        if (!isQueuedToPlayOrPlaying) yield break;

        Play();
    }

    public void UnloadAsset()
    {
        if (!assetHandle.IsValid()) return;
        StopVideo();

        videoPlayer.clip = null;

        Addressables.Release(assetHandle);
        isVideoReady = false;
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

    public void HoverExited()
    {
        DequeueVideo();
        StopAllCoroutines();
        ScaleDown();
        loadingEffect.Hide();
    }

    public void CloseButtonHoverEntered()
    {
        StartCoroutine(CloseButtonHovering());
    }

    IEnumerator CloseButtonHovering()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CloseButtonLoading());
    }

    public void CloseButtonHoverExited()
    {
        StopAllCoroutines();
        closeButtonLoadingEffect.Hide();
    }

    IEnumerator CloseButtonLoading()
    {
        closeButtonLoadingEffect.Show();
        closeButtonLoadingEffect.Load();
        yield return new WaitUntil(() => !closeButtonLoadingEffect.IsLoading());
        closeButtonLoadingEffect.Hide();
        CloseVideo();
    }

    private void ScaleUp()
    {
        gameObject.transform.localScale *= scaleFactor;
        if (videoViewed)
        {
            frame.sprite = viewedFrame;
            Color color = new Color(0.8f, 1, 0.9f, 1);
            frame.color = color;
        } else
        {
            frame.sprite = hoveredFrame;
        }
    }

    private void ScaleDown()
    {
        gameObject.transform.localScale = initialImageScale;
        if (videoViewed)
        {
            frame.sprite = viewedFrame;
            Color color = new Color(1, 1, 1, 1);
            frame.color = color;
        } else
        {
            frame.sprite = enabledFrame;
        }
    }

    public void Activate(float duration)
    {
        SetToVideoRenderingScreen();
        ToggleVideoState(1f, duration, true);
    }

    public void Deactivate(float duration, bool releaseResources = false)
    {
        ToggleVideoState(0.2f, duration, false);
        SetToPreviewScreen();

        if (!releaseResources) return;
        UnloadAsset();
    }

    public void ToggleVideoState(float visibility, float transitionDuration, bool isEnabled)
    {
        videoScreen.CrossFadeAlpha(visibility, transitionDuration, false);

        frame.CrossFadeAlpha(visibility, transitionDuration, false);
        if (isEnabled)
        {
            if (videoViewed)
            {
                frame.sprite = viewedFrame;
            } else
            {
                frame.sprite = enabledFrame;
            }
        } else
        {
            frame.sprite = disabledFrame;
        }

        videoPokeInteractable.enabled = isEnabled;
    }

    IEnumerator Loading()
    {
        Debug.Log("Loading video");
        if (LoadAsset())
        {
            videoPlayer.time = 0;
        }

        loadingEffect.Show();
        loadingEffect.Load();
        yield return new WaitUntil(() => !loadingEffect.IsLoading());
        loadingEffect.Hide();
        FocusPlayVideo();
    }

    private IEnumerator ShowCloseContentInstruction()
    {
        yield return new WaitForSeconds(7);
        ContentController contentController = FindObjectOfType<ContentController>();
        if (videoPlayer.isPlaying && !contentController.in360)
        {
            PopUpController popUpController = FindObjectOfType<PopUpController>();
            popUpController.ShowInstructionWithRayAnimation(popUpController.closeContentInstruction, 1, 4);
        }
    }

    // Called when the video is selected through the interactor
    public void FocusPlayVideo()
    {
        if (videoPlayer.isPlaying)
        {
            return;
        }

        Deactivate();

        // Move playing video in front of the other videos (last sibling in canvas hierarchy)
        videoScreen.rectTransform.SetAsLastSibling();
        frame.sprite = selectedFrame;

        QueueToPlay();

        // Double the video's size and play
        SetToVideoRenderingScreen();

        videoScreen.CrossFadeAlpha(1, 0.5f, false);

        initialPosition = transform.position;
        var xrCameraTransform = XrReferences.XrCameraTransform;
        var direction = (initialPosition - xrCameraTransform.position).normalized;
        Vector3 newPosition = new Vector3(initialPosition.x - 7f * direction.x,
            xrCameraTransform.position.y + 1f, initialPosition.z - 7f * direction.z);
        LeanTween.move(gameObject, newPosition, 2f);
        Vector3 newScale = new Vector3(3f, 3f, initialImageScale.z);
        LeanTween.scale(gameObject, newScale, 2f).setOnComplete(EnableVideo);

        OnVideoSelected?.Invoke(this);

        PopUpController popUpController = FindObjectOfType<PopUpController>();
        if (!popUpController.selectedVideo)
        {
            StartCoroutine(ShowCloseContentInstruction());
            popUpController.selectedVideo = true;
        }
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        CloseVideo();
        //videoGenerator.ResetVideos();
    }

    private void EnableVideo()
    {
        EnableCloseButton();
    }

    private void EnableCloseButton()
    {
        closeButton.gameObject.SetActive(true);
        closeButton.enabled = true;
    }

    private IEnumerator LoadCloseVideo()
    {
        yield return new WaitForSeconds(0.6f);
        CloseVideo();
    }

    public void CloseVideo()
    {
        LeanTween.move(gameObject, initialPosition, 1f);
        LeanTween.scale(gameObject, initialImageScale, 1f).setOnComplete(videoGenerator.ResetVideos);

        frame.sprite = viewedFrame;
        SetToPreviewScreen();

        DisableCloseButton();
    }

    public void DisableCloseButton()
    {
        // Disable video close button
        closeButton.enabled = false;
        closeButton.gameObject.SetActive(false);
    }

    /// <summary>
    ///     Check if the video belong to the category
    ///     <param name="targetCategory"></param>
    /// </summary>
    /// <param name="targetCategory"></param>
    /// <returns></returns>
    public bool IsOfCategory(int targetCategory)
    {
        return category == targetCategory;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        transform.localPosition = initialLocalPosition;
        transform.localScale = initialImageScale;
    }
}