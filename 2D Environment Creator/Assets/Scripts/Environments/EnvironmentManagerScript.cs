using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Environments
{
    public class EnvironmentManagerScript : MonoBehaviour
    {
        [Header("Scenes")]
        public GameObject EnvironmentSelectieScherm;
        public GameObject EnvironmentAanmakenScherm;
        public GameObject StartScherm;
        public GameObject Environment;

        [Header("EnvironmentSelectieScherm Buttons")]
        public Button EnvironmentToevoegenButton;
        public Button TerugNaarMenu;

        [Header("EnvironmentAanmakenScherm Buttons")]
        public Button MaakEnvironmentButton;
        public Button TerugNaarSelectie;
        public TMP_InputField EnvironmentNaam;

        [Header("Prefab zooi")]
        public int aantalEnvironmentsAangemaakt = 0;
        public GameObject EnvironmentPrison;
        public TMP_Text textPrefab; // The text prefab to display environment names
        public Button DeleteEnvironmentButton; // Delete button prefab
        public Button OpenEnvironment;

        public Transform[] SpawnPosities;
        public GameObject[] Environments;

        private int spawnIndex = 0;

        [Header("API zooi")]
        private List<Environment> environments = new List<Environment>();
        // Reference to the EnvironmentApiClient
        public EnvironmentApiClient apiClient;

        [Header("Debug")]
        public TMP_Text Test;

        void Start()
        {
            Reset();
            EnvironmentToevoegenButton.onClick.AddListener(EnvironmentToevoegenScene);
            TerugNaarSelectie.onClick.AddListener(async () => await NaarEnvironmentSelectie());
            MaakEnvironmentButton.onClick.AddListener(MaakEnvironment);
            FetchEnvironments();
        }

        public void Reset()
        {
            EnvironmentSelectieScherm.SetActive(true);
            EnvironmentAanmakenScherm.SetActive(false);
        }

        public void EnvironmentToevoegenScene()
        {
            EnvironmentSelectieScherm.SetActive(false);
            EnvironmentAanmakenScherm.SetActive(true);
        }

        public async Task NaarEnvironmentSelectie()
        {
            EnvironmentSelectieScherm.SetActive(true);
            EnvironmentAanmakenScherm.SetActive(false);

            await FetchEnvironments();
            DisplayEnvironments();
        }

        public async void MaakEnvironment()
        {
            Debug.Log("MaakEnvironment() function started!");

            if (EnvironmentNaam == null || string.IsNullOrWhiteSpace(EnvironmentNaam.text))
            {
                Debug.LogError("EnvironmentNaam is NULL or empty!");
                return;
            }

            // Create a new environment using the API
            var newEnvironment = new Environment
            {
                id = Guid.NewGuid().ToString(),
                name = EnvironmentNaam.text
            };

            try
            {
                var response = await apiClient.CreateEnvironment(newEnvironment);
                if (response is WebRequestData<Environment> createdEnvironmentData)
                {
                    environments.Add(createdEnvironmentData.Data);
                    Debug.Log("Environment created successfully via API: " + createdEnvironmentData.Data.name);
                }
                else
                {
                    Debug.LogError("Failed to create environment via API.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while creating environment: " + ex.Message);
            }

            await NaarEnvironmentSelectie();
        }

        private async Task FetchEnvironments()
        {
            Debug.Log("Fetching environments from API...");

            try
            {
                var response = await apiClient.ReadEnvironments();
                if (response is WebRequestData<List<Environment>> environmentsData)
                {
                    environments = environmentsData.Data;
                    Debug.Log("Environments fetched successfully from API.");
                }
                else
                {
                    Debug.LogError("Failed to fetch environments from API.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while fetching environments: " + ex.Message);
            }
        }

        private void DisplayEnvironments()
        {
            // Clear existing environment objects
            foreach (Transform child in EnvironmentPrison.transform)
            {
                Destroy(child.gameObject);
            }

            aantalEnvironmentsAangemaakt = 0;

            // Display each environment
            foreach (Environment environment in environments)
            {
                Transform spawnPosition = SpawnPosities[aantalEnvironmentsAangemaakt % SpawnPosities.Length];
                GameObject newEnvironment = Instantiate(Environments[aantalEnvironmentsAangemaakt % Environments.Length], spawnPosition.position, Quaternion.identity, EnvironmentPrison.transform);

                // Find the correct TMP_Text component in the prefab
                TMP_Text[] textComponents = newEnvironment.GetComponentsInChildren<TMP_Text>(true);
                foreach (TMP_Text textComponent in textComponents)
                {
                    if (textComponent.gameObject.name == textPrefab.name) // Match by name to ensure it's the correct textPrefab
                    {
                        textComponent.text = environment.name; // Set the environment name
                        break;
                    }
                }

                // Find the delete button in the prefab and assign the delete functionality
                Button[] buttons = newEnvironment.GetComponentsInChildren<Button>(true);
                foreach (Button button in buttons)
                {
                    if (button.name == "DeleteEnvironmentButton")
                    {
                        button.onClick.AddListener(async () => await DeleteEnvironment(environment));
                        Debug.Log($"Delete button assigned for environment: {environment.name}");
                    }
                    else if (button.name == "OpenEnvironment")
                    {
                        button.onClick.AddListener(() => OpenEnvironmentScreen(environment));
                        Debug.Log($"Open button assigned for environment: {environment.name}");
                    }
                }

                aantalEnvironmentsAangemaakt++;
            }
        }

        private void OpenEnvironmentScreen(Environment environment)
        {
            Debug.Log($"Opening environment: {environment.name}");

            // Update the Test TMP_Text component with the environment ID
            if (Test != null)
            {
                Test.text = $"Environment ID: {environment.id}";
                Debug.Log($"Test text updated to: {environment.id}");
            }
            else
            {
                Debug.LogError("Test TMP_Text component is not assigned!");
            }

            // Activate the Environment screen and deactivate others
            EnvironmentSelectieScherm.SetActive(false);
            EnvironmentAanmakenScherm.SetActive(false);
            Environment.SetActive(true);

            // Optionally, you can pass the environment data to the Environment screen here
            // Example: UpdateEnvironmentScreen(environment);
        }


        private async Task DeleteEnvironment(Environment environment)
        {
            Debug.Log($"Deleting environment: {environment.name}");

            try
            {
                // Call the API to delete the environment
                var response = await apiClient.DeleteEnvironment(environment.id);
                if (response != null)
                {
                    // Remove the environment from the local list
                    environments.Remove(environment);
                    Debug.Log($"Environment deleted successfully from API: {environment.name}");
                }
                else
                {
                    Debug.LogError($"Failed to delete environment from API: {environment.name}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while deleting environment: {ex.Message}");
            }

            // Refresh the UI
            DisplayEnvironments();
        }
    }
}
