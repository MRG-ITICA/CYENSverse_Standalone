// *********************************************
// *********************************************
// <info>
//   File: VideoGenerator.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/11 8:22 AM
// </info>
// <copyright file="VideoGenerator.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoGenerator : MonoBehaviour
{
    private List<VideoClip> allVideoClips = new List<VideoClip>();
    private List<VideoClip> perceptionsVideoClips = new List<VideoClip>();
    private List<VideoClip> heritageVideoClips = new List<VideoClip>();
    private List<VideoClip> biodiversityVideoClips = new List<VideoClip>();

    public GameObject videoObjectPrefab;
    public GameObject target;

    private float videoWidth;
    private float videoHeight;
    private float videoPadding = -.85f;
    private float canvasWidth;
    private float canvasHeight;
    private int videoCount;

    private int columns;
    private int rows;

    private Vector3 canvasTopLeft;
    private float videoAreaHorizontal;
    private float videoAreaVertical;

    private List<VideoController> allVideos;

    public Action videosLoaded;

    public Categories categoryManager;

    private GameObject rotator;

    // Start is called before the first frame update
    void Start()
    {
        rotator = GameObject.FindGameObjectWithTag("rotator");
        if (categoryManager == null)
            categoryManager = Categories.Instance;
        //categoryManager.onTransition += ResetVideos;

        //videoWidth = (int)videoObjectPrefab.GetComponent<RectTransform>().rect.width;
        //videoHeight = (int)videoObjectPrefab.GetComponent<RectTransform>().rect.height;
        if (TryGetComponent<RectTransform>(out var canvasRectTransform))
        {
            var rect = canvasRectTransform.rect;
            canvasWidth = rect.width;
            canvasHeight = rect.height;
        }

        if (videoObjectPrefab != null)
        {
            var videoImage = videoObjectPrefab.GetComponentInChildren<Image>();
            if (videoImage != null)
            {
                var videoRectTransform = videoImage.GetComponent<RectTransform>();
                var videoRect = videoRectTransform.rect;
                videoWidth = videoRect.width;
                videoHeight = videoRect.height;
            }
        }

        videoAreaHorizontal = videoWidth + videoPadding;
        videoAreaVertical = videoHeight + videoPadding;

        rows = (int)canvasHeight / (int)(videoAreaVertical);
        columns = videoCount / rows + videoCount % rows;

        allVideos = new();

        for (int i = 0; i < categoryManager.categories.Count; i++)
        {
            InstantiateVideos(i);
        }

        ShuffleVideos();
        PositionVideos();

        videosLoaded?.Invoke();
    }

    private void InstantiateVideos(int category)
    {
        var categoryData = categoryManager.categories[category];

        var videoCaps = categoryData.videoCaps;
        var width = videoCaps[0].width;
        var height = videoCaps[0].height;

        var rt = CreateRenderTexture(width, height);

        var videoReferences = categoryData.videoAssetReferences;
        int videoCount = videoCaps.Count;
        for (int i = 0; i < videoCount; i++)
        {
            var videoObjectInstance = Instantiate(videoObjectPrefab, transform, false);

            var videoController = videoObjectInstance.GetComponent<VideoController>();
            videoController.Initialize(this, category, videoCaps[i], rt, videoReferences[i]);
            videoController.OnVideoSelected += OnVideoSelected;

            allVideos.Add(videoController);
        }
    }

    private static RenderTexture CreateRenderTexture(int width, int height)
    {
        // Set render texture of video          
        var rt = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32)
        {
            useMipMap = true,
            autoGenerateMips = true,
            anisoLevel = 4,
            depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.None
        };
        rt.Create();
        return rt;
    }

    private void OnVideoSelected(VideoController selectedVideo)
    {
        // Stop all other videos, scale them down and fade out
        foreach (var videoController in allVideos)
        {
            if (videoController == selectedVideo) continue;

            videoController.Deactivate();
            videoController.FadeOutVideo();
        }

        // Disable cylinder rotation
        //rotator.SetActive(false);
    }

    private void ShuffleVideos()
    {
        UnityEngine.Random.InitState(3);
        allVideos = allVideos.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    private void PositionVideos()
    {
        videoCount = allVideos.Count;
        rows = (int)canvasHeight / (int)(videoAreaVertical);
        columns = videoCount / rows + videoCount % rows;
        videoAreaHorizontal = videoWidth + videoPadding;
        videoAreaVertical = videoHeight + videoPadding;
        var angleIncrement = 170f / columns * Mathf.Deg2Rad; // Angle increment between videos
        var initialAngle = (transform.eulerAngles.y + 11) * Mathf.Deg2Rad;
        var radius = 14.85f; // Radius of the cylindrical surface
        var worldCenter = target.transform.position;

        for (int i = 0, videoIndex = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++, videoIndex++)
            {
                if (videoIndex >= videoCount) break;

                // Calculate position on the cylinder using polar coordinates
                float angle = initialAngle + angleIncrement * i;
                float x = worldCenter.x + radius * Mathf.Cos(angle);

                // Calculate the y position based on the row index
                var y = -j * videoAreaVertical;

                // Calculate the z position based on the cylindrical surface
                float z = worldCenter.z + radius * Mathf.Sin(angle);

                var videoObjectInstance = allVideos[videoIndex];
                Transform videoTransform = videoObjectInstance.transform;
                videoTransform.position = new Vector3(x, y, z);
                videoTransform.localPosition = new Vector3(videoTransform.localPosition.x,
                    y, videoTransform.localPosition.z);
            }
        }

        RotateVideos();
    }

    private void RotateVideos()
    {
        foreach (Transform videoObject in transform)
        {
            var targetPosition = target.transform.position;
            var videoTransform = videoObject.transform;
            // Get a vector towards the target for a new Quaternion oriented to 'lookAt' target.
            Quaternion srcRot = videoTransform.rotation;
            Vector3 directionToTarget = targetPosition - videoTransform.position;

            Quaternion dstRot = Quaternion.LookRotation(directionToTarget, videoTransform.up);
            videoObject.transform.eulerAngles =
                new Vector3(srcRot.eulerAngles.x, dstRot.eulerAngles.y, srcRot.eulerAngles.z);
        }
    }


    // Fade out and disable videos of other categories and fade in and enable videos of current category
    public void ResetVideos()
    {
        LeanTween.cancelAll();

        for (int i = 0, n = allVideos.Count; i < n; i++)
        {
            var videoController = allVideos[i];
            // Stop any video playback and reset visuals
            videoController.StopVideo();
            videoController.DisableCloseButton();
            videoController.ResetPosition();

            // Toggle video/interaction state based on category
            if (videoController.IsOfCategory(categoryManager.currentCategoryIndex))
            {
                videoController.Activate();
                videoController.FadeInVideo();
            }
            else
            {
                videoController.Deactivate();
                videoController.FadeOutVideo();
                videoController.UnloadAsset();
            }
        }

        //rotator.SetActive(true);
    }
}