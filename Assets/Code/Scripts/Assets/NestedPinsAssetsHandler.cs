// *********************************************
// *********************************************
// <info>
//   File: NestedPinsAssetsHandler.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/10 9:56 AM
//   Last Modification Date: 2023/10/10 3:35 PM
// </info>
// <copyright file="NestedPinsAssetsHandler.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

[DefaultExecutionOrder(int.MaxValue)]
public class NestedPinsAssetsHandler : MonoBehaviour
{
    private CoordinatesMapper coordinatesMapping;
    private List<IResourceLocation> pinAssetsLocations = new();

    // Start is called before the first frame update
    void Start()
    {
        coordinatesMapping = GetComponent<CoordinatesMapper>();
        GetNestedPinsReferences();
    }

    private void GetNestedPinsReferences()
    {
        StartCoroutine(GetNestedPinsReferencesAsync());
    }

    /// <summary>
    ///     Load all videos of a <paramref name="category" />
    /// </summary>
    private IEnumerator GetNestedPinsReferencesAsync()
    {
        // Get the keys for the videos of the category
        var keys = new List<string>() { "360", "pin", "nested" };

        // Get the locations of the videos
        var pinTextures = Addressables.LoadResourceLocationsAsync(keys,
            Addressables.MergeMode.Intersection, typeof(Texture));

        yield return pinTextures;

        var loadOps = new List<AsyncOperationHandle>(pinTextures.Result.Count);

        var assetLocations = pinTextures.Result;
        var numberOfAssets = assetLocations.Count;

        for (int i = 0; i < numberOfAssets; i++)
        {
            var location = assetLocations[i];
            pinAssetsLocations.Add(location);
        }

        yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);
    }

    public void LoadAsset(string id, Action<AsyncOperationHandle<Texture>, Texture> OnAssetLoadCallback)
    {
        IResourceLocation pinLocation = null;

        for (int i = 0, n = pinAssetsLocations.Count; i < n; i++)
        {
            pinLocation = pinAssetsLocations[i];
            if (pinLocation.PrimaryKey != id) continue;
            StartCoroutine(LoadAssetAsync(pinLocation, OnAssetLoadCallback));
        }
    }

    public IEnumerator LoadAssetAsync(IResourceLocation location,
        Action<AsyncOperationHandle<Texture>, Texture> OnAssetLoadCallback)
    {
        var loadHandle = Addressables.LoadAssetAsync<Texture>(location);
        loadHandle.Completed += textureHandle => { OnAssetLoadCallback(loadHandle, loadHandle.Result); };
        yield return loadHandle;
    }
}