using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    [Header("API zooi")]
    private List<GameObject> placedObjects;
    public EnvironmentApiClient apiClient;
    public object2DApiClient object2DApiClient;
    public string environmentId;
    public string objectId;


    [Header("UI Elements")]
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    public GameObject UITopMenu;

    [Header("UITopMenu")]
    public Button Save;
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

        // Add listener to the slider
        rotationSlider.onValueChanged.AddListener(SetRotation);

        // Initialize the rotation text and preview
        UpdateRotationText();
        UpdateRotationPreview();
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
        UITopMenu.SetActive(false);
    }

    // Methode om de huidige scène te resetten
    public void Reset()
    {
        // Laad de huidige scène opnieuw
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
