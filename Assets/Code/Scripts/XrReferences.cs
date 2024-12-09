// *********************************************
// *********************************************
// <info>
//   File: XrReferences.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/10 8:31 AM
// </info>
// <copyright file="XrReferences.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using SourceBase.Utilities.Helpers;
using System.Collections;
using TriInspector;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[DefaultExecutionOrder(int.MinValue)]
public class XrReferences : Singleton<XrReferences>
{
    #region Variables

    [Button(ButtonSizes.Large, "Reset references")]
    void ResetReferences()
    {
        ClearReferences();
        SetReferences();
    }

    [SerializeField]
    private Transform xrOriginTransform;

    [Title("Camera")]
    public static Transform XrOriginTransform => Instance?.xrOriginTransform;

    public static Transform XrCameraOffset => Instance.xrCameraOffset;

    [SerializeField]
    private Transform xrCameraOffset;

    [SerializeField]
    private Camera xrCamera;

    public static Camera XrCamera => Instance.xrCamera;

    private Transform xrCameraTransform;
    public static Transform XrCameraTransform => Instance.xrCameraTransform;

    [Title("Faders")]
    [SerializeField]
    private FadeController fadeToBlack;

    public static FadeController FadeToBlack => Instance.fadeToBlack;


    [SerializeField]
    private FadeController fadeToWhite;

    public static FadeController FadeToWhite => Instance.fadeToWhite;

    [Title("Hands")]
    [SerializeField]
    private GameObject leftHand;

    public static GameObject LeftHand => Instance.leftHand;

    [SerializeField]
    private Transform leftHandVisual;

    public static Transform LeftHandVisual => Instance.leftHandVisual;

    [SerializeField]
    private XRRayInteractor leftRayInteractor;

    public static XRRayInteractor LeftRayInteractor => Instance.leftRayInteractor;

    [SerializeField]
    private GameObject rightHand;

    public static GameObject RightHand => Instance.rightHand;

    [SerializeField]
    private Transform rightHandVisual;

    public static Transform RightHandVisual => Instance.rightHandVisual;

    [SerializeField]
    private XRRayInteractor rightRayInteractor;

    public static XRRayInteractor RightRayInteractor => Instance.rightRayInteractor;

    #endregion Variables

    #region Unity Messages

    void Reset()
    {
        SetInstance(this, true);
        SetReferences();
    }

    void Awake()
    {
        SetInstance(this, true);
        SetReferences();
        StartCoroutine(CheckHeadset());
    }

    #endregion Unity Messages

    #region Xr References

    /// <summary>
    ///     Clear references related to XR components
    /// </summary>
    private void ClearReferences()
    {
        xrOriginTransform = null;
        xrCameraOffset = null;
        xrCamera = null;
        xrCameraTransform = null;
        leftHand = null;
        leftHandVisual = null;
        leftRayInteractor = null;
        rightHand = null;
        rightHandVisual = null;
        rightRayInteractor = null;
    }

    /// <summary>
    ///     Set the Xr References
    /// </summary>
    private void SetReferences()
    {
        // Find XR origin reference
        var xrOrigin = SetXrOrigin();

        // Get camera references
        SetCameraReferences(xrOrigin);

        // Get ray interactor references
        SetHandReferences();
    }

    /// <summary>
    ///     Get the xr origin
    /// </summary>
    /// <returns></returns>
    private XROrigin SetXrOrigin()
    {
        // if the origin is already set just return it
        if (xrOriginTransform != null) return xrOriginTransform.GetComponent<XROrigin>();

        // otherwise find it
        var xrOrigin = FindAnyObjectByType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("[XrReferences:SetReferences] Unable to find XrOrigin!");
            return xrOrigin;
        }

        xrOriginTransform = xrOrigin.transform;
        return xrOrigin;
    }

    /// <summary>
    ///     Get the camera references
    /// </summary>
    /// <param name="xrOrigin"></param>
    private void SetCameraReferences(XROrigin xrOrigin)
    {
        if (xrOrigin != null)
        {
            xrCameraOffset ??= xrOrigin.CameraFloorOffsetObject?.transform;
            xrCamera ??= xrOrigin.Camera;
        }

        xrCameraTransform ??= xrCamera.transform;
    }

    /// <summary>
    ///     Get the hand references
    /// </summary>
    private void SetHandReferences()
    {
        // if the ray interactors are already set, do nothing
        if (leftRayInteractor != null && rightRayInteractor != null) return;

        // otherwise, find them through the XR Input Modality Manager
        var xrInputModalityManager = FindAnyObjectByType<XRInputModalityManager>();
        if (xrInputModalityManager == null)
        {
            Debug.LogError("[XrReferences:SetReferences] Unable to find hand references!");
            return;
        }

        leftHand = xrInputModalityManager.leftHand;
        leftHandVisual = GetHandVisual(leftHand);
        leftRayInteractor ??= GetHandRayInteractor(leftHand);

        rightHand = xrInputModalityManager.rightHand;
        rightHandVisual = GetHandVisual(rightHand);
        rightRayInteractor ??= GetHandRayInteractor(rightHand);
    }

    /// <summary>
    ///     Get the Xr Ray Interactor based on the root object of a an Xr Hand
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    private XRRayInteractor GetHandRayInteractor(GameObject hand)
    {
        var handInteractionGroup = hand.GetComponentInChildren<XRInteractionGroup>();
        if (handInteractionGroup == null) return null;
        var groupMembers = handInteractionGroup.startingGroupMembers;
        for (int i = 0, n = groupMembers.Count; i < n; i++)
        {
            if (groupMembers[i] is not XRRayInteractor xrRayInteractor) continue;
            return xrRayInteractor;
        }

        return null;
    }

    /// <summary>
    ///     Get the Hand visual transform of a an Xr Hand
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    private Transform GetHandVisual(GameObject hand)
    {
        var handTrackingEvent = hand.GetComponentInChildren<XRHandTrackingEvents>();
        return handTrackingEvent?.transform;
    }

    #endregion Xr References

    #region XR Camera

    /// <summary>
    ///     Set the transform to rotate to look toward the camera
    /// </summary>
    /// <param name="targetTransform"></param>
    public void RotateTowardsCamera(Transform targetTransform, float? xAngle, float? yAngle, float? zAngle)
    {
        Vector3 rot = Quaternion.LookRotation(targetTransform.position - xrCameraTransform.position).eulerAngles;

        if (xAngle.HasValue)
            rot.x = xAngle.Value;

        if (yAngle.HasValue)
            rot.y = yAngle.Value;

        if (zAngle.HasValue)
            rot.z = zAngle.Value;

        targetTransform.rotation = Quaternion.Euler(rot);
    }

    /// <summary>
    ///     Set target transform to look toward the camera and
    ///     positioned at a set distance from it
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <param name="distance"></param>
    public void PositionTowardsCamera(Transform targetTransform, float distance = 6)
    {
        SetTargetPosition(targetTransform, distance);
        SetTargetRotation(targetTransform);
    }

    /// <summary>
    ///     Set the credits position so they are in front of the camera
    /// </summary>
    /// <param name="targetTransform"></param>
    private void SetTargetPosition(Transform targetTransform, float distance = 6)
    {
        var cameraForward = xrCameraTransform.forward;
        var targetPosition = xrCameraTransform.position + cameraForward * distance;

        targetPosition.y = targetTransform.position.y;

        targetTransform.position = targetPosition;
    }

    /// <summary>
    ///     Set the credits rotation so they face the camera
    /// </summary>
    /// <param name="targetTransform"></param>
    private void SetTargetRotation(Transform targetTransform)
    {
        var targetRotation = targetTransform.localEulerAngles;
        targetRotation.y = xrCameraTransform.eulerAngles.y;
        targetTransform.localEulerAngles = targetRotation;
    }

    /// <summary>
    ///     Set the culling mask of the camera to Everything
    /// </summary>
    /// <param name="maskValue"></param>
    public void ChangeCameraCullingMask()
    {
        xrCamera.cullingMask = LayerMask.NameToLayer("Everything");
    }

    /// <summary>
    ///     Change the culling mask of the camera
    /// </summary>
    /// <param name="maskValue"></param>
    public void ChangeCameraCullingMask(int maskValue)
    {
        xrCamera.cullingMask = maskValue;
    }

    /// <summary>
    ///     Set the culling mask of the camera to Everything
    /// </summary>
    /// <param name="maskValue"></param>
    public void ChangeRayCastCullingMask()
    {
        leftRayInteractor.raycastMask = LayerMask.NameToLayer("Everything");
        rightRayInteractor.raycastMask = LayerMask.NameToLayer("Everything");
    }

    /// <summary>
    ///     Change the culling mask of the camera
    /// </summary>
    /// <param name="maskValue"></param>
    public void ChangeRayCastCullingMask(int maskValue)
    {
        leftRayInteractor.raycastMask = maskValue;
        rightRayInteractor.raycastMask = maskValue;
    }

    #endregion XR Camera

    public bool FacingPins()
    {
        float angle = Mathf.Abs(xrCamera.transform.eulerAngles.y);
        return angle < 30 || angle > 330;
    }

    public bool FacingVideos()
    {
        float angle = Mathf.Abs(xrCamera.transform.eulerAngles.y);
        return angle % 360 < 270 && angle % 360 > 90;
    }

    private IEnumerator CheckHeadset()
    {
        var cooldown = new WaitForSeconds(1f);

        while (true)
        {
            InputDevice headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (headDevice.isValid)
            {
                bool userPresent = false;
                bool presenceFeatureSupported = headDevice.TryGetFeatureValue(CommonUsages.userPresence, out userPresent);
                Debug.Log("presence feature supported " + presenceFeatureSupported + " userPresent is " + userPresent);
                //OnHeadsetStatusChanged?.Invoke(userPresent);  <--- custom event to inform others
            }

            yield return cooldown;
        }
    }
}