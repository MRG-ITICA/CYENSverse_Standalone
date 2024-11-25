// *********************************************
// *********************************************
// <info>
//   File: CategoriesData.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 9:47 AM
//   Last Modification Date: 2023/10/09 9:24 PM
// </info>
// <copyright file="CategoriesData.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Video;

[Serializable]
public class CategoryData
{
    public string name;
    public List<Texture> videoCaps;
    public List<AssetReference> videoAssetReferences;
    public List<AudioClip> audioClips;
    public List<VideoClip> graphicVideos;
    public List<VideoClip> graphicTransitionVideos;
    internal float duration;

    public void ComputeDuration(float delay, float audioMultiplier)
    {
        var totalDuration = delay;
        for (int i = 0, n = audioClips.Count; i < n; i++)
        {
            totalDuration += audioClips[i].length + 3;
        }

        duration = totalDuration * audioMultiplier;
    }
}