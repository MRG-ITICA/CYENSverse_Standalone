// *********************************************
// *********************************************
// <info>
//   File: LoadNextSceneOnAssetsLoaded.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSVERSE/Assembly-CSharp
//   Creation Date: 2023/09/28 6:46 PM
//   Last Modification Date: 2023/09/29 6:12 PM
// </info>
// <copyright file="LoadNextSceneOnAssetsLoaded.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using TriInspector;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoadNextSceneOnAssetsLoaded : MonoBehaviour
{
    #region Variables

    /// <summary>
    ///     The assets loader that send progress update callbacks
    /// </summary>
    [FormerlySerializedAs("loader")]
    [Title("Assets management")]
    [SerializeField]
    private AssetsLoader assetsLoader;

    [Title("Scene loading")]
    /// <summary>
    ///     Delay before loading the next scene
    /// </summary>
    [SerializeField]
    private float loadingDelay = 1.5f;

    #endregion Variables

    #region Unity Messages

    /// <summary>
    ///     Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component
    ///     the first time.
    /// </summary>
    void Reset()
    {
        assetsLoader = FindAnyObjectByType<AssetsLoader>();
    }

    /// <summary>
    ///     Awake is called either when an active GameObject that contains the script is initialized when a Scene loads, or
    ///     when a previously inactive GameObject is set to active, or after a GameObject created with Object.Instantiate is
    ///     initialized.
    /// </summary>
    void Awake()
    {
        if (SceneManager.sceneCountInBuildSettings > 1) return;
        Debug.LogWarning($"Not enough scenes to load next scene from {name}");
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

    #region Scene Loading

    /// <summary>
    ///     Callback when all assets have been loaded
    /// </summary>
    /// <param name="value"></param>
    void OnAssetsLoaded()
    {
        StartCoroutine(LoadNextSceneAsync(loadingDelay));
    }

    /// <summary>
    ///     Load the next scene after an optional delay
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator LoadNextSceneAsync(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadSceneAsync(1);
        Destroy(this);
    }

    #endregion Scene Loading
}