// *********************************************
// *********************************************
// <info>
//   File: AssetsLoader.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverseUrp/Assembly-CSharp
//   Creation Date: 2023/10/02 7:26 PM
//   Last Modification Date: 2023/10/03 1:59 PM
// </info>
// <copyright file="AssetsLoader.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections;
using System.Collections.Generic;

using SourceBase.Utilities.Helpers;

using TriInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

[DefaultExecutionOrder(int.MinValue)]
public class AssetsLoader : Singleton<AssetsLoader>
{
    #region Variables

    #region Assets Management

    #region Data Types

    #region Asset

    [Serializable]
    private abstract class CategoryAsset
    {
        public string category = string.Empty;
        public string assetPath = string.Empty;
        public AsyncOperationHandle handle;
    }

    private class CategoryAsset<T> : CategoryAsset
    {
        public static readonly CategoryAsset<T> empty = default;
        public T asset = default(T);
    }

    private class CategoryImageAsset : CategoryAsset<Texture>
    {
    }

    private class CategoryAudioAsset : CategoryAsset<AudioClip>
    {
    }

    private class CategoryVideoAsset : CategoryAsset<VideoClip>
    {
    }

    #endregion Asset

    #region Category

    private interface ICategory
    {
        /// <summary>
        ///     The name of the category
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     An enumerator over the assets of the category;
        /// </summary>
        public IEnumerable<CategoryAsset> Assets { get; }

        /// <summary>
        ///     Add an asset to the category
        /// </summary>
        /// <param name="asset"></param>
        public void Add(CategoryAsset asset);

        /// <summary>
        ///     Remove an asset from the category
        /// </summary>
        /// <param name="asset"></param>
        public void Remove(CategoryAsset asset);

        /// <summary>
        ///     Clear all assets from the category
        /// </summary>
        /// <param name="asset"></param>
        public void Clear();

        /// <summary>
        ///     Release all loaded assets
        /// </summary>
        public void ReleaseAll();

        /// <summary>
        ///     The number of loaded assets in the category
        /// </summary>
        public int Count { get; }

        /// <summary>
        ///     Get a loaded assets based on its <paramref name="index" />
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CategoryAsset this[int index] { get; }
    }

    [Serializable]
    private class CategoryAssets<T> : ICategory where T : CategoryAsset
    {
        public string name = string.Empty;

        /// <inheritdoc />
        public string Name => name;

        public readonly List<T> assets = new();

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="nameValue"></param>
        public CategoryAssets(string nameValue)
        {
            name = nameValue;
        }

        /// <inheritdoc />
        public IEnumerable<CategoryAsset> Assets => assets;

        /// <inheritdoc />
        public void Add(CategoryAsset asset) => assets.Add((T)asset);

        /// <inheritdoc />
        public void Remove(CategoryAsset asset) => assets.Remove((T)asset);

        /// <inheritdoc />
        public void Clear() => assets.Clear();

        /// <inheritdoc />
        public void ReleaseAll()
        {
            for (int i = 0, n = assets.Count; i < n; i++)
            {
                Addressables.Release(assets[i].handle);
            }

            Clear();
        }

        /// <inheritdoc />
        public int Count => assets.Count;

        /// <inheritdoc />
        public CategoryAsset this[int index] => assets[index];
    }

    #endregion Category

    #endregion Data Types

    /// <summary>
    ///     Dictionary with all the loaded assets.
    ///     It is separated based on type (e.g., video, audio, etc.),
    ///     and then by category (e.g., heritage, biodiversity)
    /// </summary>
    [ShowInInspector, InlineEditor]
    private Dictionary<string, Dictionary<string, ICategory>> loadedAssets = new();

    [SerializeField]
    private bool loadAllAssetsOnStart = true;

    #endregion Assets Management

    #region Assets Tracking

    /// <summary>
    ///     The number of loaded assets so far
    /// </summary>
    private int numberOfAssetsLoaded = 0;

    /// <summary>
    ///     Are there any available assets that have been loaded
    /// </summary>
    public int AreAvailableAssets => numberOfAssetsLoaded;

    /// <summary>
    ///     Callback Action triggered when all assets have been loaded
    /// </summary>
    public ManagedAction OnAssetsLoaded = new();

    /// <summary>
    ///     Callback Action triggered when load process progresses
    /// </summary>
    public ManagedAction<float> OnLoadingProgress = new();

    #endregion Assets Tracking

    #endregion Variables

    #region Unity Messages

    /// <summary>
    ///     Awake is called when an enabled script instance is being loaded.
    /// </summary>
    void Awake()
    {
        SetInstance(this);
    }

    /// <summary>
    ///     Start is called just before any of the Update methods is called the first time
    /// </summary>
    void Start()
    {
        if (!loadAllAssetsOnStart)
        {
            NotifyAssetsLoading();
            return;
        }

        LoadAllVideos();
    }


    /// <summary>
    ///     This function is called when the scriptable object will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        UnloadVideos();
    }

    #endregion Unity Messages

    #region Setup

    private CategoryVideoAsset AddAddressableAsset(string category, string key, AsyncOperationHandle handle,
        VideoClip clip)
    {
        // Split path to elements
        var pathElements = key.Split("/");

        if (pathElements.Length < 3)
        {
            Debug.LogWarning($"Asset path for {key} does not have correct number of elements.");
            return default(CategoryVideoAsset);
        }

        var typeIndex = pathElements.Length - 3;
        var type = pathElements[typeIndex].ToLower();

        if (!loadedAssets.TryGetValue(type, out var categoriesByType))
        {
            loadedAssets.Add(type, new());
            categoriesByType = loadedAssets[type];
        }

        category = category.ToLower();
        if (!categoriesByType.TryGetValue(category, out var assetsForCategory))
        {
            categoriesByType.Add(category, new CategoryAssets<CategoryVideoAsset>(category));
            assetsForCategory = categoriesByType[category];
        }

        var asset = new CategoryVideoAsset()
        {
            assetPath = key,
            category = category,
            asset = clip,
            handle = handle
        };
        assetsForCategory.Add(asset);
        numberOfAssetsLoaded++;

        return asset;
    }

    public void LoadAllVideos()
    {
        StartCoroutine(LoadAllVideosAsync());
    }

    public IEnumerator LoadAllVideosAsync()
    {
        var perCategoryProgress = 1f / 3;
        var progress = 0f;
        OnLoadingProgress.Invoke(progress);
        yield return StartCoroutine(LoadCategoryVideosAsync("heritage", progress, perCategoryProgress));

        progress += perCategoryProgress;
        OnLoadingProgress.Invoke(progress);
        yield return StartCoroutine(LoadCategoryVideosAsync("perceptions", progress, perCategoryProgress));

        progress += perCategoryProgress;
        OnLoadingProgress.Invoke(progress);
        yield return StartCoroutine(LoadCategoryVideosAsync("biodiversity", progress, perCategoryProgress));

        NotifyAssetsLoading();
    }

    private void NotifyAssetsLoading()
    {
        OnLoadingProgress.Invoke(1);
        OnAssetsLoaded?.Invoke();
    }

    public void LoadCategoryVideos(string category, float progress = 0, float perCategoryProgress = 1)
    {
        StartCoroutine(LoadCategoryVideosAsync(category, progress, perCategoryProgress));
    }

    /// <summary>
    ///     Load all videos of a <paramref name="category" />
    /// </summary>
    private IEnumerator LoadCategoryVideosAsync(string category, float progress = 0, float perCategoryProgress = 1)
    {
        // Get the keys for the videos of the category
        var keys = new List<string>() { "videos", category };

        // Get the locations of the videos
        var locations = Addressables.LoadResourceLocationsAsync(keys,
            Addressables.MergeMode.Intersection, typeof(VideoClip));

        yield return locations;

        var loadOps = new List<AsyncOperationHandle>(locations.Result.Count);
        var numberOfNewAssets = locations.Result.Count;
        var perAssetProgress =
            (numberOfNewAssets > 0) ? (perCategoryProgress / numberOfNewAssets) : perCategoryProgress;

        var assetLocations = locations.Result;
        for (int i = 0, n = assetLocations.Count; i < n; i++)
        {
            var location = assetLocations[i];


            var loadHandle = Addressables.LoadAssetAsync<VideoClip>(location);
            loadHandle.Completed += clipHandle =>
            {
                AddAddressableAsset(category, location.PrimaryKey, loadHandle, clipHandle.Result);
                progress += perAssetProgress;
                OnLoadingProgress?.Invoke(progress);
            };
            loadOps.Add(loadHandle);
        }

        yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);
    }

    public void UnloadVideos()
    {
        foreach (var assetTypesValues in loadedAssets)
        {
            foreach (var categoryAssetsValues in assetTypesValues.Value)
            {
                var categoryAssets = categoryAssetsValues.Value;
                categoryAssets.ReleaseAll();
            }

            assetTypesValues.Value.Clear();
        }

        loadedAssets.Clear();
    }

    public List<VideoClip> GetCategoryVideos(string category)
    {
        var clips = new List<VideoClip>();
        return GetCategoryVideos(category, clips);
    }

    public List<VideoClip> GetCategoryVideos(string category, List<VideoClip> clips)
    {
        if (!loadedAssets.TryGetValue("videos", out var videoAssets)) return clips;
        if (!videoAssets.TryGetValue(category.ToLower(), out var categoryAssets)) return clips;

        if (categoryAssets is not CategoryAssets<CategoryVideoAsset> categoryVideos) return clips;

        clips.Sort((clip1, clip2) => clip1.name.CompareTo(clip2.name));

        for (int i = 0, n = categoryVideos.Count; i < n; i++)
        {
            if (categoryVideos[i] is not CategoryVideoAsset videoAsset) continue;
            clips.Add(videoAsset.asset);
        }

        return clips;
    }

    /// <summary>
    ///     Get all the loaded videos
    /// </summary>
    /// <returns></returns>
    public List<VideoClip> GetAllVideos()
    {
        return GetAllVideos(new List<VideoClip>());
    }

    /// <summary>
    ///     Get all the loaded videos
    /// </summary>
    /// <returns></returns>
    public List<VideoClip> GetAllVideos(List<VideoClip> videos)
    {
        if (!loadedAssets.TryGetValue("videos", out var videoAssets)) return videos;

        foreach (var categoryAssetsValues in videoAssets.Keys)
        {
            GetCategoryVideos(categoryAssetsValues, videos);
        }

        return videos;
    }

    #endregion Setup
}