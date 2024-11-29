// *********************************************
// *********************************************
// <info>
//   File: CoordinatesMapper.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/11 11:31 AM
// </info>
// <copyright file="CoordinatesMapper.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(NestedPinsAssetsHandler))]
public class CoordinatesMapper : MonoBehaviour
{
    public GameObject polaroidPrefab;
    public GameObject nested360PinPrefab;
    private Texture originalSkyboxTexture;
    public float annotationsDistance = 10f;

    private Transform XRCamera;

    private string path;

    public List<Vector2> annotations;
    public Dictionary<string, Vector2> annotations360;

    public Canvas canvas;

    public XRSimpleInteractable goBack360;
    public GameObject goBackHome;

    Vector3 initialPosition;
    Vector3 initialScale;

    Vector3 initialGoBack360Scale;

    public float imageScaleFactor;

    private SpinnerController spinner;
    private NestedPinsAssetsHandler nestedPinsAssetsHandler;
    private IImageController originalImageController;
    private IImageController parentImageController;

    void Awake()
    {
        nestedPinsAssetsHandler = GetComponent<NestedPinsAssetsHandler>();
        XRCamera = XrReferences.XrCameraTransform;
    }

    void Start()
    {
        TextAsset textPath = (TextAsset) Resources.Load("annotationLocations");
        path = textPath.text;
        annotations = new List<Vector2>();
        annotations360 = new Dictionary<string, Vector2>();
        initialGoBack360Scale = goBack360.transform.localScale;
    }

    public void ClearAndDestroyAnnotations()
    {
        var annotationsTransformRoot = canvas.transform;
        var numberOfChildren = annotationsTransformRoot.childCount;
        var children = new GameObject[numberOfChildren];
        for (int i = 0; i < numberOfChildren; i++)
        {
            children[i] = annotationsTransformRoot.GetChild(i).gameObject;
        }

        for (int i = 0; i < numberOfChildren; i++)
        {
            var child = children[i];
            if (!child.CompareTag("mark")) continue;
            Destroy(child);
        }

        ClearAnnotations();
    }

    public void ClearAndHideAnnotations()
    {
        GameObject[] annotationObjects = GameObject.FindGameObjectsWithTag("mark");
        foreach (var annotationObject in annotationObjects)
        {
            annotationObject.SetActive(false);
        }

        ClearAnnotations();
    }

    public void ClearAnnotations(bool destroyAnnotationObject = true)
    {
        annotations.Clear();
        annotations360.Clear();
    }

    public void LoadAnnotations(IImageController source, Texture skyboxTexture, IImageController parentImageValue)
    {
        // Save image references
        originalImageController = source;
        parentImageController = parentImageValue;
        originalSkyboxTexture = skyboxTexture;

        // Clear any existing annotations
        ClearAndDestroyAnnotations();
        // Load all annotations for current image
        Debug.Log(path);
        // Find the starting line for current Images' annotations
        var lines = path.Split("\r\n");
        int lineCounter = 0;

        string temp = lines[lineCounter];
        while (!temp.Equals(source.Name) && lineCounter < lines.Length-1)
        {
            lineCounter++;
            temp = lines[lineCounter];
        }
        if (temp.Equals(source.Name) && lineCounter < lines.Length-1)
        {
            // Load the associated annotations
            lineCounter++;
            string ann = lines[lineCounter];
            while (ann != null && (ann.StartsWith("(") || ann.StartsWith(">")) && lineCounter < lines.Length - 1)
            {
                if (ann.StartsWith("("))
                {
                    annotations.Add(StringToVector2(ann));
                    lineCounter++;
                    ann = lines[lineCounter];
                }
                else if (ann.StartsWith(">"))
                {
                    StringTo360Vector2(ann);
                    lineCounter++; 
                    ann = lines[lineCounter];
                }
            }
        }
        else
        {
            Debug.Log("no annotations found for image");
        }

        var inNestedImage = parentImageController != null;

        goBack360.gameObject.SetActive(inNestedImage);
        if (!inNestedImage)
        {
            goBackHome.SetActive(true);
            LeanTween.scale(goBackHome, new Vector3(20, 20, 20), 1f);
        } else
        {
            goBackHome.SetActive(false);
        }

        // Create the annotations
        CreateAnnotations();
        if (annotations360 is { Count: 0 }) return;
        Create360Annotations();
    }

    // Turn annotation coordinates string into position vector
    private static Vector2 StringToVector2(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector2
        Vector3 result = new Vector2(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        return result;
    }

    // Turn 360 annotation coordinates string into position vector
    private void StringTo360Vector2(string sVector)
    {
        // Remove the parentheses
        int parIndex = sVector.IndexOf("(");
        var target360 = sVector.Substring(1, parIndex - 1);
        sVector = sVector.Substring(parIndex + 1, sVector.Length - (parIndex + 2));

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector2
        Vector2 result = new Vector2(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        annotations360.Add(target360, result);
    }

    // Turn cartesian to spherical coordinates
    private Vector3 CartesianToSphericalCoordinates(Vector2 position)
    {
        var point = GetPoint(annotationsDistance, (position.x * 360 - 180) * Mathf.Deg2Rad,
            ((1 - position.y) * 180 - 90) * Mathf.Deg2Rad);

        return new Vector3(point.x, point.y, point.z);
    }

    Vector3 GetPoint(float rho, float theta, float phi)
    {
        float x = rho * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = rho * Mathf.Sin(phi);
        float z = rho * Mathf.Cos(theta) * Mathf.Cos(phi);
        return new Vector3(x, y, z);
    }

    // Create annotation game object
    private void CreateAnnotations()
    {
        int i = 0;
        foreach (Vector2 ann in annotations)
        {
            Vector3 sphericalCoords = CartesianToSphericalCoordinates(ann);

            GameObject markObject = CreateMarkObject(sphericalCoords);
            markObject.name = i.ToString();

            CreateImageAnnotation(markObject, i);
            i++;
        }
    }

    // Instantiate annotation mark object, rotate object to look at user and add hover listeners
    private GameObject CreateMarkObject(Vector3 coords)
    {
        var markObject = Instantiate(polaroidPrefab, coords, Quaternion.identity, canvas.transform);
        markObject.tag = "mark";

        // Rotate object to face camera
        Vector3 rot = Quaternion.LookRotation(XRCamera.position - markObject.transform.position).eulerAngles;
        rot.x = rot.z = 0;
        markObject.transform.rotation = Quaternion.Euler(rot);

        return markObject;
    }

    // Load image into annotation object
    private void CreateImageAnnotation(GameObject markObject, int i)
    {
        // Set image annotation sprite
        Image annotationImage = markObject.GetComponentInChildren<Image>();
        annotationImage.sprite = Resources.Load<Sprite>("Images/" + originalImageController.Name + "/" + i.ToString());
        annotationImage.preserveAspect = true;
    }

    // Create 360 annotation game objects
    private GameObject Create360MarkObject(Vector3 coords)
    {
        GameObject markObject = Instantiate(nested360PinPrefab, new Vector3(coords.x, coords.y, coords.z),
            Quaternion.identity);
        markObject.tag = "mark";
        markObject.transform.SetParent(canvas.transform);

        Vector3 rot = Quaternion.LookRotation(XRCamera.transform.position - markObject.transform.position).eulerAngles;
        rot.x = 0;
        rot.z = 180;
        markObject.transform.rotation = Quaternion.Euler(rot);

        return markObject;
    }

    // Create 360 image annotation objects
    private void Create360Annotations()
    {
        var keys = annotations360.Keys;
        foreach (var target360 in keys)
        {
            Vector3 coords = CartesianToSphericalCoordinates(annotations360[target360]);
            // Instantiate annotation mark object
            GameObject markObject = Create360MarkObject(coords);
            markObject.name = target360;

            var imageController = markObject.GetComponentInChildren<ImageController360>();
            //imageController.SetTextureAssetHandler(nestedPinsAssetsHandler, originalImageController);
            string texPath = "360WithoutPeople/" + target360;
            Texture tex = Resources.Load<Texture>(texPath);
            imageController.SetTexture(tex, originalImageController);
        }
    }

    public void OnBakButtonHoverEntered()
    {
        ScaleUp();
        StartCoroutine(Hovering(goBack360));
    }

    IEnumerator Hovering(XRSimpleInteractable buttonInteractble)
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Loading(buttonInteractble));
    }

    private void ScaleUp()
    {
        goBack360.transform.localScale *= imageScaleFactor;
    }

    private void ScaleDown()
    {
        goBack360.transform.localScale = initialGoBack360Scale;
    }

    IEnumerator Loading(XRSimpleInteractable buttonInteractble)
    {
        spinner = buttonInteractble.GetComponentInChildren<SpinnerController>(true);

        spinner.Show();
        spinner.Load();

        yield return new WaitUntil(() => (spinner == null || !spinner.IsLoading()));

        if (spinner != null)
        {
            spinner.Hide();
            spinner = null;
        }

        HideBack360Button();
        StartCoroutine(parentImageController.ObjectSelected());
    }

    public void HideBack360Button()
    {
        goBack360.gameObject.SetActive(false);
    }

    public void OnBackButtonHoverExited()
    {
        ScaleDown();

        if (spinner != null)
        {
            spinner.Hide();
            spinner = null;
        }

        StopAllCoroutines();
    }

    public void ShowAnnotationContent(GameObject mark)
    {
        GameObject[] annotations = GameObject.FindGameObjectsWithTag("mark");
        foreach (var item in annotations)
        {
            if (item != mark)
            {
                item.SetActive(false);
            }
        }

        var interactable = mark.GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.RemoveAllListeners();
        interactable.hoverExited.RemoveAllListeners();

        initialPosition = mark.transform.position;
        initialScale = mark.transform.localScale;

        float targetScale = 8f;
        interactable.enabled = false;
        Vector3 newPosition = new Vector3(mark.transform.position.x - XRCamera.forward.x,
            XRCamera.position.y, mark.transform.position.z - XRCamera.forward.z);
        LeanTween.move(mark, newPosition, 1.5f);
        LeanTween.scale(mark, new Vector3(targetScale, targetScale, mark.transform.localScale.z), 1.5f);
        LeanTween.scale(goBackHome, Vector3.zero, 1f);
        LeanTween.scale(goBack360.gameObject, Vector3.zero, 1f);
        LeanTween.color(mark.GetComponentInChildren<Image>().rectTransform, Color.white, 1.5f)
            .setOnComplete(FadeComplete).setOnCompleteParam(mark);
    }

    private void FadeComplete(object mark)
    {
        GameObject markGO = (GameObject)mark;

        markGO.GetComponent<PolaroidController>().PolaroidAppears();
    }

    public void HideAnnotationContent()
    {
        LeanTween.scale(goBackHome, new Vector3(20, 20, 20), 1f);
        LeanTween.scale(goBack360.gameObject, new Vector3(7, 7, 7), 1f);
    }

    public void ScaleDownComplete()
    {
        foreach (Transform item in canvas.transform)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void HideHomeButton()
    {
        goBackHome.SetActive(false);
    }
}