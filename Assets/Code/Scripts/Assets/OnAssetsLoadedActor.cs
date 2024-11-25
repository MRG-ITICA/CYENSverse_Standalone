// *********************************************
// *********************************************
// <info>
//   File: OnAssetsLoadedActor.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSVERSE/Assembly-CSharp
//   Creation Date: 2023/09/29 6:29 PM
//   Last Modification Date: 2023/09/29 6:31 PM
// </info>
// <copyright file="OnAssetsLoadedActor.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using TriInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class OnAssetsLoadedActor : MonoBehaviour
{
    #region Variables

    /// <summary>
    ///     The assets loader that send progress update callbacks
    /// </summary>
    [FormerlySerializedAs("loader")]
    [Title("Assets management")]
    [SerializeField]
    private AssetsLoader assetsLoader;

    [FormerlySerializedAs("OnAssetsLoadedEvent")]
    [Title("Event listeners")]
    /// <summary>
    ///     Delay before loading the next scene
    /// </summary>
    [SerializeField]
    private UnityEvent OnAssetsLoadedActions;

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

    #region OnAssetsLoadedActor

    /// <summary>
    ///     Callback when all assets have been loaded
    /// </summary>
    /// <param name="value"></param>
    void OnAssetsLoaded()
    {
        OnAssetsLoadedActions?.Invoke();
    }

    #endregion OnAssetsLoadedActor
}