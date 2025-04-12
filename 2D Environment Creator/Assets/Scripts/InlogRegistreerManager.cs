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

    // Input fields for login
    public TMP_InputField loginEmailInputField;
    public TMP_InputField loginPasswordInputField;
    public Button PasswordToggle;

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




        // Check if the user is already logged in
        //string token = PlayerPrefs.GetString("authToken", "");
        //if (!string.IsNullOrEmpty(token))
        //{
        //    isLoggedIn = true;
        //}
    }

    public async void Register()
    {
        User user = new User
        {
            email = registerEmailInputField.text,
            password = registerPasswordInputField.text
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
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Register error: " + errorMessage);
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
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
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
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











