using System;
using System.Collections.Generic;
using Environments;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    [Header("API zooi")]
    public EnvironmentManagerScript environmentManagerScript;
    private List<GameObject> placedObjects;
    public EnvironmentApiClient apiClient;
    public object2DApiClient object2DApiClient;
    public string environmentId;



    [Header("UI Elements")]
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    public GameObject UITopMenu;

    [Header("UITopMenu")]
    public Button FirstSave;
    public Button Save;
    public Button Load;
    public Button Return;



    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;

    [Header("Size")]
    // De grootte van de prefab objecten die geplaatst kunnen worden
    private bool Small = false; // 0.4 0.4 0.4
    public Button sizeSmall;
    private bool Medium = true; // Dit is de standaard waarde // 0.8 0.8 0.8
    public Button sizeMedium;
    private bool Large = false; // 1.5 1.5 1.5
    public Button sizeLarge;

    [Header("Rotation")]
    // Rotatie van het object
    private int Rotation = 0; // Dit is de standaard waarde
    public Slider rotationSlider; // Slider for rotation
    public TMPro.TMP_Text rotationText; // Text to display the rotation value
    public GameObject rotationPreview; // Object containing the rotation text

    // Lijst met objecten die geplaatst zijn in de wereld


    private void Start()
    {
        // Add listeners to the buttons
        sizeSmall.onClick.AddListener(() => SetSize("Small"));
        sizeMedium.onClick.AddListener(() => SetSize("Medium"));
        sizeLarge.onClick.AddListener(() => SetSize("Large"));
        environmentId = environmentManagerScript.Test.text.Replace("Environment ID: ", "").Trim();

        // Add listener to the slider
        rotationSlider.onValueChanged.AddListener(SetRotation);

        // Initialize the rotation text and preview
        UpdateRotationText();
        UpdateRotationPreview();

        // Add listeners for save/load buttons
        FirstSave.onClick.AddListener(FirstSaveEnvironment);
        Save.onClick.AddListener(SaveEnvironment);
        Load.onClick.AddListener(LoadEnvironment);
    }



    private void SetSize(string size)
    {
        // Set the corresponding size bool to true and others to false
        Small = size == "Small";
        Medium = size == "Medium";
        Large = size == "Large";
    }

    private void SetRotation(float value)
    {
        // Update the rotation value based on the slider
        Rotation = Mathf.RoundToInt(value);

        // Update the rotation text and preview
        UpdateRotationText();
        UpdateRotationPreview();
    }

    private void UpdateRotationText()
    {
        // Update the rotation text to display the current rotation value
        if (rotationText != null)
        {
            rotationText.text = $"Rotation: {Rotation}°";
        }
    }

    private void UpdateRotationPreview()
    {
        // Update the rotation of the rotationPreview GameObject
        if (rotationPreview != null)
        {
            rotationPreview.transform.rotation = Quaternion.Euler(0, 0, Rotation);
        }
    }

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        // Verberg het zijmenu
        UISideMenu.SetActive(false);
        UITopMenu.SetActive(false);

        // Instantieer het prefab object op de positie (0,0,0) met geen rotatie
        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);

        // Haal het Object2D component op van het nieuw geplaatste object
        Object2D object2D = instanceOfPrefab.GetComponent<Object2D>();

        // Stel de objectManager van het object in op deze instantie van ObjectManager
        object2D.objectManager = this;

        // Zet de isDragging eigenschap van het object op true zodat het gesleept kan worden
        object2D.isDragging = true;

        // Set the size of the object based on the active size
        if (Small)
        {
            instanceOfPrefab.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        else if (Medium)
        {
            instanceOfPrefab.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else if (Large)
        {
            instanceOfPrefab.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        // Apply the rotation to the object
        instanceOfPrefab.transform.rotation = Quaternion.Euler(0, 0, Rotation);
    }


    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
        UITopMenu.SetActive(true);
    }

    // Methode om de huidige scène te resetten
    public void Reset()
    {
        // Laad de huidige scène opnieuw
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void FirstSaveEnvironment()
    {
        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("Environment ID is not set. Cannot perform FirstSave.");
            return;
        }

        if (savedObjects.Any(o => o.environmentId == environmentId))
        {
            Debug.LogError("Environment already has saved objects. Use Save instead.");
            return;
        }

        SaveEnvironmentData();
        Debug.Log("First save completed successfully.");
    }

    private void SaveEnvironment()
    {
        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("Environment ID is not set. Cannot perform Save.");
            return;
        }

        // Remove existing objects for this environment before saving
        savedObjects.RemoveAll(o => o.environmentId == environmentId);

        SaveEnvironmentData();
        Debug.Log("Environment saved successfully.");
    }

    private void LoadEnvironment()
    {
        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("Environment ID is not set. Cannot perform Load.");
            return;
        }

        // Find saved objects for the current environment
        var objectsToLoad = savedObjects.Where(o => o.environmentId == environmentId).ToList();

        if (objectsToLoad.Count == 0)
        {
            Debug.LogError("No saved objects found for this environment.");
            return;
        }

        // Clear existing objects in the scene
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.Clear();

        // Instantiate saved objects
        foreach (var objData in objectsToLoad)
        {
            GameObject prefab = prefabObjects.Find(p => p.name == objData.PrefabId);
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, new Vector3(objData.PositionX, objData.PositionY, 0), Quaternion.Euler(0, 0, objData.RotationZ));
                instance.transform.localScale = new Vector3(objData.ScaleX, objData.ScaleY, 1);
                placedObjects.Add(instance);
            }
        }

        Debug.Log("Environment loaded successfully.");
    }

    private void SaveEnvironmentData()
    {
        foreach (GameObject obj in placedObjects)
        {
            Object2D object2D = obj.GetComponent<Object2D>();
            if (object2D != null)
            {
                savedObjects.Add(new object2D
                {
                    id = object2D.id,
                    PrefabId = obj.name,
                    PositionX = Mathf.RoundToInt(obj.transform.position.x),
                    PositionY = Mathf.RoundToInt(obj.transform.position.y),
                    ScaleX = Mathf.RoundToInt(obj.transform.localScale.x),
                    ScaleY = Mathf.RoundToInt(obj.transform.localScale.y),
                    RotationZ = Mathf.RoundToInt(obj.transform.rotation.eulerAngles.z),
                    environmentId = environmentId
                });
            }
        }
    }


}
