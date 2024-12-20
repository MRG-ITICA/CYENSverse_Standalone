using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PolaroidController : MonoBehaviour
{

    #region Close button
    [SerializeField]
    private XRSimpleInteractable closeButton;
    [SerializeField]
    private SpinnerController closeButtonLoadingEffect;
    #endregion

    [SerializeField]
    private SpinnerController imageSpinner;

    private CoordinatesMapper mapper;

    Vector3 initialPosition;
    Vector3 initialScale;

    public void HoverEntered()
    {
        StartCoroutine(Hovering());
    }

    public IEnumerator Hovering()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Load(gameObject));
    }

    // Load and show the annotation content
    public IEnumerator Load(GameObject mark)
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        imageSpinner.Show();
        imageSpinner.Load();
        yield return new WaitUntil(() => imageSpinner == null || !imageSpinner.IsLoading());

        if (imageSpinner != null)
        {
            imageSpinner.Hide();
        }
        mapper = FindObjectOfType<CoordinatesMapper>();
        mapper.ShowAnnotationContent(mark);
    }

    public void PolaroidAppears()
    {
        closeButton.gameObject.SetActive(true);
    }

    public void CloseButtonHoverExited()
    {
        StopAllCoroutines();
        closeButtonLoadingEffect.Hide();
    }

    public void HoverExit()
    {
        if (imageSpinner != null)
        {
            imageSpinner.Hide();
        }

        StopAllCoroutines();
    }

    IEnumerator CloseButtonLoading()
    {
        closeButtonLoadingEffect.Show();
        closeButtonLoadingEffect.Load();
        yield return new WaitUntil(() => !closeButtonLoadingEffect.IsLoading());
        closeButtonLoadingEffect.Hide();

        closeButton.gameObject.SetActive(false);
        StartCoroutine(LoadHide());
    }

    public void CloseButtonHoverEntered()
    {
        StartCoroutine(CloseButtonHovering());
    }

    IEnumerator CloseButtonHovering()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CloseButtonLoading());
    }

    public IEnumerator LoadHide()
    {
        yield return new WaitForSeconds(0.5f);
        HideAnnotationContent(initialPosition, initialScale);
    }

    private void HideAnnotationContent(Vector3 initialPosition, Vector3 initialScale)
    {
        mapper.HideAnnotationContent();

        XRSimpleInteractable interactable = transform.GetChild(0).GetComponentInChildren<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.RemoveAllListeners();
            interactable.enabled = false;
            interactable.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Unable to find interactable for annotation {name}");
        }

        LeanTween.move(gameObject, initialPosition, 1.5f);
        LeanTween.scale(gameObject, new Vector3(initialScale.x, initialScale.y, gameObject.transform.localScale.z), 1.5f)
            .setOnComplete(ScaleDownComplete).setOnCompleteParam(gameObject);
    }

    public void ScaleDownComplete(object mark)
    {
        GameObject markGO = (GameObject)mark;
        if (markGO.TryGetComponent<XRSimpleInteractable>(out var interactable))
        {
            interactable.hoverEntered.AddListener(delegate { StartCoroutine(Load(markGO)); });
            interactable.hoverExited.AddListener(delegate { HoverExit(); });
            interactable.enabled = true;
        }
        else
        {
            Debug.LogWarning($"Unable to find interactable for annotation {markGO.name}");
        }
        mapper.ScaleDownComplete();
    }
}
