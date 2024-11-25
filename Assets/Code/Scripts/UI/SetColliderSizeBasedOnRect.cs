using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(BoxCollider))]
public class SetColliderSizeBasedOnRect : MonoBehaviour
{
    [SerializeField]
    private float textColliderPadding = 5f;

    [SerializeField]
    private float colliderDepth = 1f;

    [Button(ButtonSizes.Medium, "Set collider size")]
    private void SetColliderSizeButton()
    {
        SetColliderSize();
    }

    void Reset()
    {
        SetColliderSize();
    }

    void Awake()
    {
        SetColliderSize();
    }

    private void SetColliderSize()
    {
        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;
        var rect = rectTransform.rect;
        var collider = GetComponent<BoxCollider>();
        collider.size = new Vector3(rect.width + textColliderPadding, rect.height + textColliderPadding, colliderDepth);
    }
}
