using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour
{
    public VideoClip menuSkyboxVideo;
    public VideoPlayer skyboxPlayer;

    [SerializeField]
    private Material categorySkybox;

    private void OnEnable()
    {
        RenderSettings.skybox = categorySkybox;
        skyboxPlayer.clip = menuSkyboxVideo;
    }
}
