using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace Environments
{
    public class ProfielManagerScript : MonoBehaviour
    {
        [Header("Scenes")]
        public GameObject EnvironmentSelectieScherm;
        public GameObject EnvironmentAanmakenScherm;
        public GameObject StartScherm;

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
        private List<Environment> environments = new List<Environment>();

        void Start()
        {
            Reset();
            EnvironmentToevoegenButton.onClick.AddListener(EnvironmentToevoegenScene);
            TerugNaarSelectie.onClick.AddListener(NaarEnvironmentSelectie);
            MaakEnvironmentButton.onClick.AddListener(MaakEnvironment);
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

        public void NaarEnvironmentSelectie()
        {
            EnvironmentSelectieScherm.SetActive(true);
            EnvironmentAanmakenScherm.SetActive(false);
            DisplayEnvironments();
        }

        public void MaakEnvironment()
        {
            Debug.Log("MaakEnvironment() function started!");

            if (EnvironmentNaam == null || string.IsNullOrWhiteSpace(EnvironmentNaam.text))
            {
                Debug.LogError("EnvironmentNaam is NULL or empty!");
                return;
            }

            // Create a new environment and add it to the list
            Environment newEnvironment = new Environment
            {
                id = Guid.NewGuid().ToString(),
                name = EnvironmentNaam.text
            };
            environments.Add(newEnvironment);

            Debug.Log("Mock: Environment Created with name: " + newEnvironment.name);

            NaarEnvironmentSelectie();
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
                        button.onClick.AddListener(() => DeleteEnvironment(environment));
                        Debug.Log($"Delete button assigned for environment: {environment.name}");
                        break;
                    }
                }

                aantalEnvironmentsAangemaakt++;
            }
        }

        private void DeleteEnvironment(Environment environment)
        {
            Debug.Log($"Deleting environment: {environment.name}");

            // Remove the environment from the list
            environments.Remove(environment);

            // Refresh the UI
            DisplayEnvironments();
        }
    }

    [Serializable]
    public class Environment
    {
        public string id;
        public string name;
    }
}
