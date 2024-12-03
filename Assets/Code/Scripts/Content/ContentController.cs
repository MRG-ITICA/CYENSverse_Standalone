// *********************************************
// *********************************************
// <info>
//   File: ContentController.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/11 11:08 AM
// </info>
// <copyright file="ContentController.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ContentController : MonoBehaviour
{
    private static int SkyboxCubeMapTextureId = Shader.PropertyToID("_Tex");

    public GameObject teleportCapsule;
    public CoordinatesMapper annotationController;

    public VideoGenerator videoGenerator;

    public float imageScaleFactor;

    public Material skyboxVideo;

    [SerializeField]
    private Material skyboxPin;

    public Categories categoryManager;

    public Menu menu;
    public WordCloud wordCloud;
    public Credits credits;

    [FormerlySerializedAs("startCullingMask")]
    public LayerMask uiCameraCullingMask;

    [FormerlySerializedAs("uiMask")]
    public LayerMask uiInteractionCullingMask;

    private bool enteringImage = false;

    [SerializeField]
    private FadeController fader;

    private float yRotationBeforeEnteringImage;
    private float yRotationBeforeExitingImage;

    [SerializeField]
    private GameObject floor;

    [SerializeField]
    private GameObject border;

    [SerializeField]
    private Material viewedMaterial;

    void Awake()
    {
        XrReferences.Instance.ChangeRayCastCullingMask(uiInteractionCullingMask);
        XrReferences.Instance.ChangeCameraCullingMask();

        Addressables.InitializeAsync();
    }

    private void OnEnable()
    {
        //RenderSettings.skybox = skyboxVideo;

        credits.OnCreditsSequenceFinished += RestartExperience;
        wordCloud.OnFutureSequenceEnded += ShowCredits;
    }

    private void OnDisable()
    {
        credits.OnCreditsSequenceFinished -= RestartExperience;
        wordCloud.OnFutureSequenceEnded -= ShowCredits;
    }

    private void OnDestroy()
    {
        skyboxPin.SetTexture(SkyboxCubeMapTextureId, null);
        RenderSettings.skybox = skyboxVideo;
    }

    public void SetEnteringImageStatus(bool status)
    {
        enteringImage = status;
    }

    public void EnteringImage(IImageController pinImageController)
    {
        Debug.Log("entering image");
        SetFloor360Mode(true);

        yRotationBeforeEnteringImage = XrReferences.XrCamera.transform.localEulerAngles.y;
        //videoGenerator.ResetVideos();
        teleportCapsule.SetActive(true);

        XrReferences.Instance.ChangeRayCastCullingMask(uiInteractionCullingMask.value);
        XrReferences.Instance.ChangeCameraCullingMask(uiCameraCullingMask.value);

        // Change skybox to selected 360 image
        pinImageController.OnTextureLoadedCallback.RegisterListener(SetSkyboxFromPinTexture);
        //Debug.Log($"Added listener to load skybox texture");
    }

    public void SetFloor360Mode(bool active)
    {
        if (active)
        {
            border.SetActive(true);
            floor.layer = 0;
            foreach (Transform child in floor.transform)
            {
                child.gameObject.layer = 0;
            }
            floor.transform.localScale = floor.transform.localScale / 6;
        } else
        {
            border.SetActive(false);
            floor.layer = 6;
            floor.transform.localScale = Vector3.one;
        }
    }

    public void SetSkyboxFromPinTexture(IImageController source, Texture skyboxTexture, IImageController parent)
    {
        Debug.Log("set skybox");
        Debug.Log("source :" + source.Name);
        Debug.Log(skyboxTexture.name);

        source.OnTextureLoadedCallback.UnregisterListener(SetSkyboxFromPinTexture);

        annotationController.LoadAnnotations(source, skyboxTexture, parent);
        Debug.Log($"Loaded texture {skyboxTexture.name} to skybox");
        skyboxPin.SetTexture(SkyboxCubeMapTextureId, skyboxTexture);
        RenderSettings.skybox = skyboxPin;
        
        Material material = source.pinObject.GetComponent<MeshRenderer>().materials[0];
        MarkPinAsViewed(material);

        SetEnteringImageStatus(false);
    }

    private void MarkPinAsViewed(Material material)
    {
        Color color = viewedMaterial.color;
        material.color = color;
    }

    public void ClearAnnotations()
    {
        teleportCapsule.SetActive(false);
        annotationController.ClearAndDestroyAnnotations();
    }

    public void ExitingImageToHome()
    {
        SetFloor360Mode(false);
        enteringImage = false;
        ClearAnnotations();
        yRotationBeforeExitingImage = XrReferences.XrCamera.transform.localEulerAngles.y;
        float offset = yRotationBeforeEnteringImage - yRotationBeforeExitingImage;

        XrReferences.XrCameraOffset.Rotate(0, offset, 0);

        // Change skybox back to original category texture
        RenderSettings.skybox = skyboxVideo;

        //videoGenerator.ResetVideos();

        XrReferences.Instance.ChangeRayCastCullingMask();
    }

    public void ShowCredits()
    {
        credits.Show();
    }

    public void RestartExperience()
    {
        StartCoroutine(FadeBeforeRestart());
        //credits.SetActive(false);
        
        /*menu.SetActive(true);
        leftRayInteractor.raycastMask = uiMask;
        rightRayInteractor.raycastMask = uiMask;*/
    }

    public IEnumerator FadeBeforeRestart()
    {
        fader.FadeFromTransparency(4f);
        yield return new WaitForSeconds(4);
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    public bool EnteringOther360Image()
    {
        return enteringImage;
    }
}