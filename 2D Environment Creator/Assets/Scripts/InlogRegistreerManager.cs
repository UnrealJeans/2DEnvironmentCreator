using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InlogRegistreerManager : MonoBehaviour
{
    // scenes
    public GameObject Scene1;
    public GameObject Scene2;
    public GameObject Scene2ProfielSelecteren;
    public GameObject scene2ProfielToevoegen;
    public GameObject Scene4;

    // Input fields for registration
    public TMP_InputField registerEmailInputField;
    public TMP_InputField registerPasswordInputField;

    // Input fields for login
    public TMP_InputField loginEmailInputField;
    public TMP_InputField loginPasswordInputField;

    public Button loginExit;
    public Button registerExit;

    public Button registerButton;
    public Button loginButton;

    public Button gaTerug;
    public Button gaDoorZonderAccount;
    public Toggle showPasswordToggleInlog; // Toggle om het wachtwoord te verbergen of weer te geven
    public Toggle showPasswordToggleRegister;

    // Buttons to switch between login and register screens
    public Button switchToLoginButton;
    public Button switchToRegisterButton;
    public Button StartGame;

    public Button BootBackButton;

    // Panels for login and registration
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject MainMenuButtons;
    public GameObject NotLoggedInWarning;

    // Warning components
    public RawImage passwordWarningImage;
    public TextMeshProUGUI passwordWarningText;

    // Api voor users
    public UserApiClient userApiClient;

    private bool isLoggedIn = false;

    private void Start()
    {
        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
        showPasswordToggleInlog.onValueChanged.AddListener(TogglePasswordVisibilityInlog);
        showPasswordToggleRegister.onValueChanged.AddListener(TogglePasswordVisibilityRegister);
        loginPasswordInputField.contentType = TMP_InputField.ContentType.Password; // Standaard wachtwoord verbergen
        registerPasswordInputField.contentType = TMP_InputField.ContentType.Password; // Standaard wachtwoord verbergen

        switchToLoginButton.onClick.AddListener(ShowLoginPanel);
        switchToRegisterButton.onClick.AddListener(ShowRegisterPanel);

        loginExit.onClick.AddListener(HideLoginPanel);
        registerExit.onClick.AddListener(HideRegisterPanel);

        StartGame.onClick.AddListener(StartGameHandler);
        gaTerug.onClick.AddListener(HideNotLoggedInWarning);
        gaDoorZonderAccount.onClick.AddListener(ProceedWithoutAccount);
        BootBackButton.onClick.AddListener(BootBackEvent);

        registerPasswordInputField.onValueChanged.AddListener(ValidateRegisterPassword);
        ValidateRegisterPassword(registerPasswordInputField.text); // Initial validation

        // Hide warning components initially
        passwordWarningImage.gameObject.SetActive(false);
        passwordWarningText.gameObject.SetActive(false);

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
                ProceedWithAccount();
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
                ProceedWithAccount();
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    private void ShowLoginPanel()
    {
        Debug.Log("Showing login panel");
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        MainMenuButtons.SetActive(false);
    }

    private void ShowRegisterPanel()
    {
        Debug.Log("Showing register panel");
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        MainMenuButtons.SetActive(false);
    }

    public void HideLoginPanel()
    {
        Debug.Log("Hiding login panel");
        loginPanel.SetActive(false);
        MainMenuButtons.SetActive(true);
    }



    private void HideRegisterPanel()
    {
        Debug.Log("Hiding register panel");
        registerPanel.SetActive(false);
        MainMenuButtons.SetActive(true);
    }

    private void ShowNotLoggedInWarning()
    {
        Debug.Log("Showing not logged in warning");
        NotLoggedInWarning.SetActive(true);
    }

    private void HideNotLoggedInWarning()
    {
        Debug.Log("Hiding not logged in warning");
        NotLoggedInWarning.SetActive(false);
    }

    private void ProceedWithoutAccount()
    {
        Debug.Log("Proceeding without account");
        Scene1.SetActive(false);
        Scene4.SetActive(true);


    }

    private void ProceedWithAccount()
    {
        Debug.Log("Proceeding with account");
        Scene1.SetActive(false);
        Scene2.SetActive(true);

        Scene2ProfielSelecteren.SetActive(true);
        scene2ProfielToevoegen.SetActive(false);
    }

    private void StartGameHandler()
    {
        if (isLoggedIn)
        {
            ProceedWithAccount();

        }
        else
        {
            ShowNotLoggedInWarning();

        }
    }

    private void TogglePasswordVisibilityInlog(bool isVisible)
    {
        loginPasswordInputField.contentType = isVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        loginPasswordInputField.ForceLabelUpdate(); // Forceer een update om de wijziging door te voeren
    }

    private void TogglePasswordVisibilityRegister(bool isVisible)
    {
        registerPasswordInputField.contentType = isVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        registerPasswordInputField.ForceLabelUpdate(); // Forceer een update om de wijziging door te voeren
    }

    private void ValidateRegisterPassword(string password)
    {
        if (password.Length < 1 || password.Length > 16)
        {
            passwordWarningImage.gameObject.SetActive(true);
            passwordWarningText.gameObject.SetActive(true);
        }
        else
        {
            passwordWarningImage.gameObject.SetActive(false);
            passwordWarningText.gameObject.SetActive(false);
        }
    }
    private void BootBackEvent()
    {
        if (isLoggedIn)
        {
            Scene4.SetActive(false);
            Scene1.SetActive(true);
            Scene2ProfielSelecteren.SetActive(true);
            scene2ProfielToevoegen.SetActive(true);

        }
        else
        {
            NotLoggedInWarning.SetActive(false);
            Scene4.SetActive(false);
            Scene1.SetActive(true);

        }


    }
}








