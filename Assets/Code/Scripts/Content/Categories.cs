// *********************************************
// *********************************************
// <info>
//   File: Categories.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/11 10:44 AM
// </info>
// <copyright file="Categories.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections;
using System.Collections.Generic;

using SourceBase.Utilities.Helpers;

using TMPro;

using TriInspector;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

public class Categories : Singleton<Categories>
{
    #region Variables

#if UNITY_EDITOR
    public float customDuration = -1f;
#endif

    [SerializeField, Range(0, 1)]
    [Tooltip("The percentage of the category audio that will be played.")]
    private float timeMultiplier = 1f;

    /// <summary>
    ///     The percentage of the category audio that will be played. In range 0-1.
    /// </summary>
    public float TimeMultiplier => timeMultiplier;

    private AudioSource audioSource;

    #region Categories

    public List<CategoryData> categories;
    private int numberOfCategories = 0;
    public int currentCategoryIndex = 0;

    #endregion Categories

    #region Time Management

    private DateTime startingTime;
    private DateTime currentTime;

    private TimeSpan timeElapsed;

    [ShowInInspector]
    private float ElapsedTime => (float)timeElapsed.TotalSeconds;

    [ShowInInspector]
    private float RemainingTime => ElapsedTime -
                                   (currentCategoryIndex < numberOfCategories
                                       ? categories[currentCategoryIndex].duration
                                       : 0);

    #endregion Time Management

    [SerializeField]
    private float videosTransitionDuration = 2;

    bool videosLoaded = false;

    int totalVideos = 0;

    public List<VideoPlayer> surfaces;
    public List<Material> surfacesMaterials;

    [SerializeField]
    private FadeController fadeController;

    public Action onTransition;
    public VideoGenerator videoGenerator;

    private int audioCount = 0;

    public GameObject homeContent;

    private bool trackCategoriesProgress = false;

    public LayerMask startCullingMask;
    private LayerMask cyensVerseCullingMask;


    //public GameObject menu;

    [Title("Future")]
    public WordCloud futureWords;

    public AudioClip futureAudio;
    public VideoClip futureClip;
    public Material futureSkyboxMaterial;

    public List<GameObject> futureAnimators;

    [Title("Transitions")]
    public AudioSource transitionSource;

    [SerializeField]
    private CanvasGroup transitionCategoryCanvasGroup;

    [SerializeField]
    private TMP_Text transitionCategoryLabel;

    [SerializeField]
    private TMP_Text quotesCategoryLabel;

    private bool transitioning = false;

    [SerializeField]
    private CoordinatesMapper coordinatesMapper;

    private ImageController currentImage;

    #endregion Variables

    #region Unity Messages

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        //videoGenerator.videosLoaded += ChangeCategory;

        numberOfCategories = categories.Count;
        for (int i = 0; i < numberOfCategories; i++)
        {
            totalVideos += categories[i].videoCaps.Count;
            categories[i].ComputeDuration((i == 0) ? 2f : 0f, timeMultiplier);
        }

        if (fadeController == null)
            fadeController = XrReferences.FadeToBlack;

        cyensVerseCullingMask = XrReferences.XrCamera.cullingMask;
        XrReferences.Instance.ChangeRayCastCullingMask(startCullingMask.value);
    }

    private void OnDisable()
    {
        var exposurePropertyId = Shader.PropertyToID("_Exposure");
        foreach (var material in surfacesMaterials)
        {
            if (!material.HasFloat(exposurePropertyId)) continue;
            material.SetFloat(exposurePropertyId, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!trackCategoriesProgress || !videosLoaded || currentCategoryIndex >= numberOfCategories) return;

        /*if (!audioSource.isPlaying)
            PlayNextAudio();*/

        currentTime = DateTime.Now;
        timeElapsed = currentTime - startingTime;
#if UNITY_EDITOR
        var categoryDuration = (customDuration > 0) ? customDuration : categories[currentCategoryIndex].duration;
#else
        var categoryDuration = categories[currentCategoryIndex].duration;
#endif

        if (timeElapsed.TotalSeconds < categoryDuration || transitioning) return; // || in360Image)
        currentCategoryIndex++;

        StartCoroutine(Transition());
    }

    #endregion Unity Messages

    #region Categories

    // Start playing categories after entering through menu
    public void StartCategories(float loadDuration)
    {
        trackCategoriesProgress = true;
        currentCategoryIndex = 0;
        audioSource.Stop();

        StartCoroutine(StartFirstCategory(loadDuration));
    }

    private IEnumerator StartFirstCategory(float loadDuration)
    {
        transitionCategoryLabel.text = (currentCategoryIndex < numberOfCategories)
            ? categories[currentCategoryIndex].name
            : "future";

        //StartCoroutine(FadeTransitionCategory(0, 1, 0.5f));

        yield return new WaitForSeconds(loadDuration - 1);

        ChangeCategory();

        //StartCoroutine(FadeTransitionCategory(1, 0, 0.5f, false));
        XrReferences.Instance.ChangeRayCastCullingMask();

        FindObjectOfType<PopUpController>().EnteredMainEnvironment();
    }

    // Make actual transition to next category
    private void ChangeCategory()
    {
        videosLoaded = true;

        //LoadAll360Assets();

        quotesCategoryLabel.text = (currentCategoryIndex < numberOfCategories)
            ? categories[currentCategoryIndex].name
            : string.Empty;

        ChangeSurfaceVideos();

        onTransition?.Invoke();
        startingTime = DateTime.Now;

        audioCount = 0;
        PlayNextAudio();
    }

    #endregion Categories

    #region Audio Playback

    private void PlayNextAudio()
    {
        if (currentCategoryIndex >= numberOfCategories) return;

        if (audioCount == categories[currentCategoryIndex].audioClips.Count)
        {
            audioCount = 0;
        }

        audioSource.clip = categories[currentCategoryIndex].audioClips[audioCount];
        if (audioCount == 0)
            audioSource.PlayDelayed(2);
        else
            audioSource.Play();

        audioCount++;
    }

    private IEnumerator FadeAudio(float fromVolume, float toVolume, float duration)
    {
        audioSource.volume = fromVolume;

        var timeStep = 1 / duration * Time.fixedDeltaTime;
        timeStep *= ((fromVolume < toVolume) ? 1 : -1);

        var waitTime = new WaitForFixedUpdate();
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            yield return waitTime;
            audioSource.volume += timeStep;
            elapsedTime += Time.fixedDeltaTime;
        }

        audioSource.volume = toVolume;
    }

    #endregion Audio Playback

    #region Surface Videos

    private void ChangeSurfaceVideos()
    {
        StartCoroutine(PlaySurfaceVideosChangeTransition());
    }

    IEnumerator PlaySurfaceVideosChangeTransition()
    {
        yield return StartCoroutine(FadeSurfaceVideos(1, 1));

        SetSurfaceVideos();

        yield return StartCoroutine(FadeSurfaceVideos(8, -1));
    }

    private IEnumerator FadeSurfaceVideos(float startValue, float signMultiplier)
    {
        // Exposure values
        var exposureStep = 7 / videosTransitionDuration * Time.fixedDeltaTime * signMultiplier;
        var exposure = startValue;
        var exposurePropertyId = Shader.PropertyToID("_Exposure");

        var timer = 0f;
        var waitTime = new WaitForFixedUpdate();
        while (timer < videosTransitionDuration)
        {
            foreach (var material in surfacesMaterials)
            {
                if (!material.HasFloat(exposurePropertyId)) continue;
                material.SetFloat(exposurePropertyId, exposure);
            }

            yield return waitTime;

            exposure += exposureStep;
            timer += Time.fixedDeltaTime;
        }
    }

    private void SetSurfaceVideos()
    {
        if (currentCategoryIndex >= numberOfCategories) return;

        // Set the new videos
        int i = 0;
        foreach (VideoPlayer v in surfaces)
        {
            v.clip = categories[currentCategoryIndex].graphicVideos[i];
            i++;
        }
    }

    #endregion Surface Videos

    #region Transition

    // Trigger transition audios and videos
    private IEnumerator Transition()
    {
        //XrReferences.RightRayInteractor.enabled = false;
        //XrReferences.LeftRayInteractor.enabled = false;

        transitioning = true;
        transitionSource.Play();

        /*if (currentCategoryIndex < numberOfCategories)
            fadeController.LoadMainScene(false, 1f, 3.5f);
        else
            XrReferences.FadeToWhite.FadeFromTransparency(1f);*/
        if (currentCategoryIndex == numberOfCategories)
        {
            XrReferences.FadeToWhite.FadeFromTransparency(1f);
        }

        StartCoroutine(FadeAudio(1, 0, 1f));

        yield return new WaitForSeconds(1);

        audioSource.Stop();

        if (currentCategoryIndex < numberOfCategories)
        {
            //transitionCategoryLabel.text = categories[currentCategoryIndex].name;
        }
        else
        {
            transitionCategoryLabel.text = "future";
            //transitionCategoryLabel.color = Color.blue * .15f;
            Color color = new Color(0, 0, .15f, 1);
            transitionCategoryLabel.color = color;
        }

        //StartCoroutine(FadeTransitionCategory(0, 1, 0.5f));

        if (currentCategoryIndex < numberOfCategories)
        {
            int i = 0;
            foreach (VideoClip v in categories[currentCategoryIndex - 1].graphicTransitionVideos)
            {
                surfaces[i].clip = v;
                i++;
            }
        }

        yield return new WaitForSeconds(2.5f);

        if (currentCategoryIndex < numberOfCategories)
        {
            ChangeCategory();
        }
        else
        {
            TransitionToFuture();
            XrReferences.FadeToWhite.FadeToTransparency(1f);
        }
        StartCoroutine(FadeAudio(0, 1, 1));

        //StartCoroutine(FadeTransitionCategory(1, 0, 0.5f, false));

        yield return new WaitForSeconds(1);
        //videoGenerator.ResetVideos();

        transitionCategoryLabel.text = string.Empty;
        transitioning = false;

        //XrReferences.RightRayInteractor.enabled = true;
        //XrReferences.LeftRayInteractor.enabled = true;
    }

    private IEnumerator FadeTransitionCategory(float fromAlpha, float toAlpha, float duration,
        bool repositionCanvas = true)
    {
        if (repositionCanvas)
            XrReferences.Instance.PositionTowardsCamera(transitionCategoryCanvasGroup.transform, 6);

        transitionCategoryCanvasGroup.alpha = fromAlpha;

        var timeStep = 1 / duration * Time.fixedDeltaTime;
        timeStep *= ((fromAlpha < toAlpha) ? 1 : -1);

        var waitTime = new WaitForFixedUpdate();
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            yield return waitTime;
            transitionCategoryCanvasGroup.alpha += timeStep;
            elapsedTime += Time.fixedDeltaTime;
        }

        transitionCategoryCanvasGroup.alpha = toAlpha;
    }

    #endregion Transition

    #region Future

    private void TransitionToFuture()
    {
        Clear360Data();
        trackCategoriesProgress = false;

        // Hide cylinder and canvases
        XrReferences.Instance.ChangeRayCastCullingMask(startCullingMask.value);
        XrReferences.Instance.ChangeCameraCullingMask(startCullingMask.value);

        // Play future skybox
        surfaces[2].clip = futureClip;
        surfaces[2].playbackSpeed = 0.25f;

        RenderSettings.skybox = futureSkyboxMaterial;

        // Disable animators
        foreach (GameObject go in futureAnimators)
        {
            go.GetComponent<Animator>().enabled = false;
        }

        // Play future audio clips
        audioSource.clip = futureAudio;
        audioSource.Play();
        XrReferences.FadeToWhite.FadeToTransparency(1.5f);

        futureWords.Show();
    }

    private void Clear360Data()
    {
        coordinatesMapper.ClearAndDestroyAnnotations();
        coordinatesMapper.HideHomeButton();
    }

    #endregion Future
}