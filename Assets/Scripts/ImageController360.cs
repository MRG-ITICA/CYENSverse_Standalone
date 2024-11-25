// *********************************************
// *********************************************
// <info>
//   File: ImageController360.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/11 11:17 AM
// </info>
// <copyright file="ImageController360.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using SourceBase.Utilities.Helpers;

using TriInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR.Interaction.Toolkit;

public class ImageController360 : MonoBehaviour, IImageController
{
    /// <summary>
    ///     The name of the associated image
    /// </summary>
    [ShowInInspector]
    public string Name => (textureReference == null || textureReference.Asset == null)
        ? gameObject.name
        : textureReference.Asset.name;
    public GameObject pinObject => gameObject;

    [SerializeReference]
    private XRSimpleInteractable interactable;

    /// <summary>
    ///     The interactable for the pin
    /// </summary>
    public XRSimpleInteractable Interactable => interactable;

    public SpinnerController loadingEffect;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    public float imageMovingSpeed = 2f;
    public float imageScaleFactor;
    public float sphereScaleFactor = 200f;

    public FadeController fader;
    private NestedPinsAssetsHandler nestedPinsAssetsHandler;

    private AsyncOperationHandle<Texture> assetHandle;

    [SerializeField]
    private AssetReference textureReference;

    private bool isTextureReady = false;

    private Texture texture;

    public ManagedAction<IImageController, Texture, IImageController> onTextureLoadedCallback = new();

    public ManagedAction<IImageController, Texture, IImageController> OnTextureLoadedCallback =>
        onTextureLoadedCallback;

    private IImageController parentImage;

    void Reset()
    {
        interactable = GetComponentInChildren<XRSimpleInteractable>();
        loadingEffect = GetComponentInChildren<SpinnerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = gameObject.transform.position;
        initialScale = gameObject.transform.localScale;
        XrReferences.Instance.RotateTowardsCamera(transform, 0, null, 180);
        fader = XrReferences.FadeToBlack;
    }

    private void OnDestroy()
    {
        UnloadAsset();
        //nestedPinsAssetsHandler.ReturnFromNested360Image(parentImage);
    }

    public void SetTextureAssetHandler(NestedPinsAssetsHandler nestedPinsAssetsHandlerValue,
        IImageController parentImageValue)
    {
        parentImage = parentImageValue;
        nestedPinsAssetsHandler = nestedPinsAssetsHandlerValue;
    }

    public void HoverEntered()
    {
        ScaleUp();
        StartCoroutine(Hovering());
    }

    IEnumerator Hovering()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Loading());
    }

    private void ScaleUp()
    {
        gameObject.transform.localScale *= imageScaleFactor;
    }

    private void ScaleDown()
    {
        gameObject.transform.localScale = initialScale;
    }

    protected bool LoadAsset()
    {
        if (isTextureReady && assetHandle.IsValid())
        {
            //Debug.Log($"Asset for image {Name} is already loaded");
            StartCoroutine(ObjectSelected());
            return true;
        }
        nestedPinsAssetsHandler.LoadAsset(gameObject.name, OnAssetLoaded);
        return false;
    }

    protected void OnAssetLoaded(AsyncOperationHandle<Texture> assetHandleValue, Texture textureValue)
    {
        assetHandle = assetHandleValue;
        texture = textureValue;
        isTextureReady = true;
        StartCoroutine(ObjectSelected());
        OnTextureLoadedCallback.Invoke(this, texture, parentImage);
    }

    public void UnloadAsset()
    {
        if (!assetHandle.IsValid()) return;
        Addressables.Release(assetHandle);
        texture = null;
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
        StartCoroutine(ObjectSelected());
    }

    public void HoverExited()
    {
        StopAllCoroutines();
        ScaleDown();
        loadingEffect.Hide();
    }

    public IEnumerator ObjectSelected()
    {
        yield return new WaitForSeconds(1);
        fader.Load360Image(this);
    }
}