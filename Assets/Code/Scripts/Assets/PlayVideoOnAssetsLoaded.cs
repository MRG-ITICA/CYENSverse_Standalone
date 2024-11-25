// *********************************************
// *********************************************
// <info>
//   File: PlayVideoOnAssetsLoaded.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSVERSE/Assembly-CSharp
//   Creation Date: 2023/09/29 6:09 PM
//   Last Modification Date: 2023/09/29 6:45 PM
// </info>
// <copyright file="PlayVideoOnAssetsLoaded.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections.Generic;

using TriInspector;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class PlayVideoOnAssetsLoaded : MonoBehaviour
{
    #region Variables

    /// <summary>
    ///     The assets loader that send progress update callbacks
    /// </summary>
    [FormerlySerializedAs("loader")]
    [Title("Assets management")]
    [SerializeField]
    private AssetsLoader assetsLoader;

    [Title("Video player")]
    /// <summary>
    ///     Delay before loading the next scene
    /// </summary>
    [SerializeField]
    private VideoPlayer player;

    private List<VideoClip> videos = new();
    private int videoIndex;

    #endregion Variables

    #region Unity Messages

    /// <summary>
    ///     Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component
    ///     the first time.
    /// </summary>
    void Reset()
    {
        assetsLoader = FindAnyObjectByType<AssetsLoader>();
        player = GetComponent<VideoPlayer>();
    }

    /// <summary>
    ///     Awake is called either when an active GameObject that contains the script is initialized when a Scene loads, or
    ///     when a previously inactive GameObject is set to active, or after a GameObject created with Object.Instantiate is
    ///     initialized.
    /// </summary>
    void Awake()
    {
        player ??= GetComponent<VideoPlayer>();
        if (player != null) return;
        Debug.LogWarning($"No video player found by {name}");
        enabled = false;
    }

    /// <summary>
    ///     This function is called when the object becomes enabled and active
    /// </summary>
    void OnEnable()
    {
        if (assetsLoader == null)
        {
            assetsLoader = AssetsLoader.Instance;
            if (assetsLoader == null)
            {
                Debug.LogWarning($"No asset loader found by {name}");
                enabled = false;
                return;
            }
        }

        assetsLoader.OnAssetsLoaded.RegisterListener(OnAssetsLoaded);
    }

    /// <summary>
    ///     This function is called when the behaviour becomes disabled or inactive
    /// </summary>
    void OnDisable()
    {
        if (assetsLoader == null) return;
        assetsLoader.OnAssetsLoaded.UnregisterListener(OnAssetsLoaded);
    }

    #endregion Unity Messages

    #region Video Player

    /// <summary>
    ///     Callback when all assets have been loaded
    /// </summary>
    /// <param name="value"></param>
    void OnAssetsLoaded()
    {
        videoIndex = 0;
        videos = assetsLoader.GetAllVideos(videos);

        PlayVideo();
    }

    /// <summary>
    ///     Play current video based on <see cref="videoIndex" />
    /// </summary>
    void PlayVideo()
    {
        if (videoIndex < 0 || videoIndex >= videos.Count) return;

        var videoClip = videos[videoIndex];
        player.clip = videoClip;
        player.loopPointReached += PlayNextVideo;
        player.Play();
    }

    /// <summary>
    ///     Play the next video in the queue
    /// </summary>
    /// <param name="source"></param>
    private void PlayNextVideo(VideoPlayer source)
    {
        videoIndex++;
        PlayVideo();
    }

    #endregion Video Player
}