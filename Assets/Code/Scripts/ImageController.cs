// *********************************************
// *********************************************
// <info>
//   File: ImageController.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/11 10:39 AM
// </info>
// <copyright file="ImageController.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections;

using SourceBase.Utilities.Helpers;

using TriInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR.Interaction.Toolkit;

public interface IImageController
{
    public string Name { get; }
    public ManagedAction<IImageController, Texture, IImageController> OnTextureLoadedCallback { get; }
    public IEnumerator ObjectSelected();

    public GameObject pinObject { get; }
}

public class ImageController : MonoBehaviour, IImageController
{
    public static int counter = 0;
    private int id = 0;

    /// <summary>
    ///     The name of the associated image
    /// </summary>
    [ShowInInspector]
    public string Name => (textureReference == null || textureReference.Asset == null)
        ? gameObject.name
        : textureReference.Asset.name;

    public GameObject pinObject => gameObject;

    [SerializeField]
    private SpinnerController loadingEffect;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    public float imageMovingSpeed = 2f;
    public float imageScaleFactor;
    public float sphereScaleFactor = 200f;

    public FadeController fader;

    public int category;

    private Categories categoryManager;

    private Material material;

    [SerializeField]
    private XRSimpleInteractable interactable;

    private bool isTextureReady = false;

    private AsyncOperationHandle<Texture> assetHandle;

    [SerializeField]
    public AssetReference textureReference;

    [SerializeField]
    private Material viewedMaterial;

    private Texture texture;

    public ManagedAction<IImageController, Texture, IImageController> onTextureLoadedCallback = new();

    public ManagedAction<IImageController, Texture, IImageController> OnTextureLoadedCallback =>
        onTextureLoadedCallback;

    private ContentController contentController;

    [SerializeField]
    private GameObject outline;

    [SerializeField]
    private Texture skyboxTexture;

    void Reset()
    {
        interactable = GetComponentInChildren<XRSimpleInteractable>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (interactable == null)
            interactable = GetComponentInChildren<XRSimpleInteractable>();

        categoryManager = FindAnyObjectByType<Categories>();
        //categoryManager.onTransition += FadePins;

        material = gameObject.GetComponent<MeshRenderer>().materials[0];

        initialPosition = gameObject.transform.position;
        initialScale = gameObject.transform.localScale;
        XrReferences.Instance.RotateTowardsCamera(transform, 0, null, 180);
        fader = XrReferences.FadeToBlack;

        contentController = FindAnyObjectByType<ContentController>();

        id = counter++;
        var outlineMesh = outline.GetComponent<MeshRenderer>();
        outlineMesh.material.SetFloat("_Seed", id);
    }

    public void HoverEntered()
    {
        StartCoroutine(Hovering());
    }

    IEnumerator Hovering()
    {
        ScaleUp();
        yield return new WaitForSeconds(0.7f);

        if (!contentController.EnteringOther360Image())
        {
            contentController.SetEnteringImageStatus(true);
            StartCoroutine(Loading());
        }
    }

    private void ScaleUp()
    {
        gameObject.transform.localScale *= imageScaleFactor;
    }

    private void ScaleDown()
    {
        gameObject.transform.localScale = initialScale;
    }

    protected void LoadAsset()
    {
        UpdateAssetTargets();
        StartCoroutine(ObjectSelected());
    }

    private void UpdateAssetTargets()
    {
        Debug.Log("update asset targets");
        isTextureReady = true;
        if (skyboxTexture != null)
        {

            OnTextureLoadedCallback.Invoke(this, skyboxTexture, null);
        } else
        {
            Debug.Log("texture is null when selecting image");
        }
    }

    public void UnloadAsset()
    {
        if (!assetHandle.IsValid()) return;
        Addressables.Release(assetHandle);
        isTextureReady = false;
        OnTextureLoadedCallback.ResetInvocation();
    }

    IEnumerator Loading()
    {
        LoadAsset();

        loadingEffect.Show();
        loadingEffect.Load();
        yield return new WaitUntil(() => !loadingEffect.IsLoading());
        loadingEffect.Hide();
    }

    
    public void HoverExited()
    {
        //Debug.LogWarning("Stop all coroutines");
        contentController.SetEnteringImageStatus(false);
        StopAllCoroutines();
        ScaleDown();
        loadingEffect.Hide();
    }

    public IEnumerator ObjectSelected()
    {
        PopUpController popUpController = FindObjectOfType<PopUpController>();
        /*if (!popUpController.selectedPin)
        {
            popUpController.ShowInstructionWithImage(popUpController.turnAroundInstruction, popUpController.turnAroundImage, 3, 4);
        }*/
        popUpController.selectedPin = true;
        Debug.Log("select");
        yield return new WaitForSeconds(1);
        //categoryManager.in360Image = true;
        fader.Load360Image(this);
    }

    private void FadePins()
    {
        Color color = material.color;

        if (category == categoryManager.currentCategoryIndex)
        {
            GetComponentInChildren<XRSimpleInteractable>().enabled = true;
            //interactionAffordance.SetActive(true);
            color.a = 1;
            color.r = 1f;
            color.g = 0.65f;
            color.b = 0.3f;
            material.color = color;
            outline.SetActive(true);
        }
        else
        {
            GetComponentInChildren<XRSimpleInteractable>().enabled = false;
            //interactionAffordance.SetActive(false);
            color.a = 0.3f;
            color.r = 0.7f;
            color.g = 0.7f;
            color.b = 0.7f;
            material.color = color;
            outline.SetActive(false);
            UnloadAsset();
        }
    }

    public void RequestTexture(Action<Texture> textureCallback)
    {
        if (isTextureReady)
        {
            textureCallback(texture);
            return;
        }
    }
}