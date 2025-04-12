using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InlogRegistreerManager : MonoBehaviour
{
    // scenes
    public GameObject Scene1;
    public GameObject Scene2;

    // Input fields for registration
    public TMP_InputField registerEmailInputField;
    public TMP_InputField registerPasswordInputField;
    public GameObject RegisterError;

    // Input fields for login
    public TMP_InputField loginEmailInputField;
    public TMP_InputField loginPasswordInputField;
    public GameObject LoginError;

    public Button loginExit;
    public Button registerExit;

    public Button registerButton;
    public Button loginButton;


    // Buttons to switch between login and register screens
    public Button switchToLoginButton;
    public Button switchToRegisterButton;


    // Panels for login and registration
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject MainMenu;



    // Api voor users
    public UserApiClient userApiClient;

    private bool isLoggedIn = false;

    private void Start()
    {
        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
        switchToLoginButton.onClick.AddListener(ShowLoginPanel);
        switchToRegisterButton.onClick.AddListener(ShowRegisterPanel);
        loginExit.onClick.AddListener(HideLoginPanel);
        registerExit.onClick.AddListener(HideRegisterPanel);
    }

    public async void Register()
    {
        string password = registerPasswordInputField.text;

        // Validate the password
        if (!IsPasswordValid(password))
        {
            Debug.Log("Register error: Password does not meet the required criteria.");
            RegisterError.SetActive(true); // Show the RegisterError GameObject
            return; // Exit the method
        }

        User user = new User
        {
            email = registerEmailInputField.text,
            password = password
        };

        IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Register success! Response: " + dataResponse.Data);
                string token = dataResponse.Data;
                PlayerPrefs.SetString("authToken", token); // Save the token
                PlayerPrefs.Save();
                isLoggedIn = true;

                // Transition to Scene2
                Scene1.SetActive(false);
                Scene2.SetActive(true);

                // Ensure RegisterError is hidden on successful registration
                RegisterError.SetActive(false);
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Register error: " + errorMessage);

                // Show the RegisterError GameObject
                RegisterError.SetActive(true);
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    // Helper method to validate the password
    private bool IsPasswordValid(string password)
    {
        if (password.Length < 10)
            return false;

        bool hasUppercase = false;
        bool hasLowercase = false;
        bool hasDigit = false;
        bool hasSpecialChar = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUppercase = true;
            if (char.IsLower(c)) hasLowercase = true;
            if (char.IsDigit(c)) hasDigit = true;
            if ("!?@#$%&".Contains(c)) hasSpecialChar = true;

            // If all conditions are met, no need to continue checking
            if (hasUppercase && hasLowercase && hasDigit && hasSpecialChar)
                return true;
        }

        return false;
    }

    public async void Login()
    {
        User user = new User
        {
            email = loginEmailInputField.text,
            password = loginPasswordInputField.text
        };

        IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Login success! Response: " + dataResponse.Data);
                string token = dataResponse.Data;
                PlayerPrefs.SetString("authToken", token); // Save the token
                PlayerPrefs.Save();
                isLoggedIn = true;

                // Transition to Scene2
                Scene1.SetActive(false);
                Scene2.SetActive(true);

                // Ensure LoginError is hidden on successful login
                LoginError.SetActive(false);
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);

                // Show the LoginError GameObject
                LoginError.SetActive(true);
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }


    public void TogglePasswordVisibilityInlog(bool isVisible)
    {
        loginPasswordInputField.contentType = isVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        loginPasswordInputField.ForceLabelUpdate(); // Forceer een update om de wijziging door te voeren
    }

    private void ShowLoginPanel()
    {
        Debug.Log("Showing login panel");
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        MainMenu.SetActive(false);
    }

    private void ShowRegisterPanel()
    {
        Debug.Log("Showing register panel");
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void HideLoginPanel()
    {
        Debug.Log("Hiding login panel");
        loginPanel.SetActive(false);
        MainMenu.SetActive(true);
    }



    private void HideRegisterPanel()
    {
        Debug.Log("Hiding register panel");
        registerPanel.SetActive(false);
        MainMenu.SetActive(true);
    }
}











