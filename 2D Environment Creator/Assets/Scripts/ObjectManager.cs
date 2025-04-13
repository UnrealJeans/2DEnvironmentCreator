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
    private bool objectsLoaded;

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
        FirstSave.onClick.AddListener(SaveEnvironmentObjects);
        Load.onClick.AddListener(LoadObjectsForActiveEnvironment);
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

        // Initialize the placedObjects list if it is null
        if (placedObjects == null)
        {
            placedObjects = new List<GameObject>();
        }

        // Add the newly placed object to the placedObjects list
        placedObjects.Add(instanceOfPrefab);
    }



    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
        UITopMenu.SetActive(true);
    }

    private async void SaveEnvironmentObjects()
    {
        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("Environment ID is not set.");
            return;
        }

        // Fetch all existing objects for the current environment
        var existingObjectsResponse = await object2DApiClient.ReadObject2Ds(environmentId);
        if (existingObjectsResponse is WebRequestData<List<object2D>> existingObjectsData)
        {
            List<object2D> existingObjects = existingObjectsData.Data;

            // Delete each existing object
            foreach (var existingObject in existingObjects)
            {
                await object2DApiClient.DeleteObject2D(environmentId, existingObject.id);
                Debug.Log($"Deleted object with ID: {existingObject.id}");
            }

        }
        else
        {
            Debug.LogError("Failed to fetch existing objects for deletion.");
            return;
        }

        if (placedObjects == null || placedObjects.Count == 0)
        {
            Debug.LogWarning("No objects to save.");
            return;
        }
        // Save the new objects
        foreach (var placedObject in placedObjects)
        {
            object2D newObject = new object2D
            {
                id = Guid.NewGuid().ToString(),
                prefabId = placedObject.name,
                PositionX = Mathf.RoundToInt(placedObject.transform.position.x),
                PositionY = Mathf.RoundToInt(placedObject.transform.position.y),
                ScaleX = Mathf.RoundToInt(placedObject.transform.localScale.x * 100),
                ScaleY = Mathf.RoundToInt(placedObject.transform.localScale.y * 100),
                RotationZ = Mathf.RoundToInt(placedObject.transform.rotation.eulerAngles.z),
                environmentId = environmentId
            };

            Debug.Log($"Saving object: ID={newObject.id}, PositionX={newObject.PositionX}, PositionY={newObject.PositionY}");

            await object2DApiClient.CreateObject2D(newObject);
        }



        Debug.Log("Save operation completed.");
    }


    public async void LoadObjectsForActiveEnvironment()
    {
        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("Active environment ID is not set.");
            return;
        }
        DestroyAllPlacedObjects();
        if (objectsLoaded)
        {
            Debug.Log("Objects have already been loaded for this environment.");
            return;
        }

        Debug.Log($"Loading Object2Ds for environment ID: {environmentId}");
        var response = await object2DApiClient.ReadObject2Ds(environmentId);

        if (response is WebRequestData<List<object2D>> object2DListResponse)
        {
            List<object2D> object2DList = object2DListResponse.Data;
            Debug.Log($"Loaded {object2DList.Count} Object2Ds.");
            InstantiateObjectsForEnvironment(object2DList);
            objectsLoaded = true;
        }
        else
        {
            Debug.LogError("Failed to load Object2Ds.");
        }
    }

    private void InstantiateObjectsForEnvironment(List<object2D> object2DList)
    {
        if (object2DList == null)
        {
            Debug.LogError("object2DList is null.");
            return;
        }

        foreach (var object2D in object2DList)
        {
            if (object2D.environmentId == environmentId)
            {
                // Strip the "(Clone)" suffix if present
                string prefabName = object2D.prefabId.Replace("(Clone)", "").Trim();

                // Find the prefab by name
                GameObject prefab = prefabObjects.Find(p => p.name == prefabName);
                if (prefab == null)
                {
                    Debug.LogError($"Prefab with name {prefabName} not found.");
                    continue;
                }

                // Instantiate the prefab at the specified position
                Vector3 position = new Vector3(object2D.PositionX, object2D.PositionY, 0);
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);

                // Name the instance and parent it under a specific GameObject in the hierarchy
                instance.name = $"{prefab.name}Clone{object2D.id}";
                instance.transform.SetParent(this.transform);

                // Add the instance to the placedObjects list
                if (placedObjects == null)
                {
                    placedObjects = new List<GameObject>();
                }

                placedObjects.Add(instance);

                // Log the instantiation
                Debug.Log($"Instantiated {instance.name} at position {position}");
            }
        }
    }


    public void DestroyAllPlacedObjects()
    {
        foreach (var placedObject in placedObjects)
        {
            Destroy(placedObject);
        }
        placedObjects.Clear();
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
