using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private VisualElement root;
    private TextField emailField;
    private TextField passwordField;
    private Button loginBtn;
    private Button registerBtn;
    private Label statusTxt;
    private VisualElement uiPanel;

    public EaseTypeWrapper easePanel; // Serialize the EaseTypeWrapper
    

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        emailField = root.Q<TextField>("EmailField");
        passwordField = root.Q<TextField>("PasswordField");
        loginBtn = root.Q<Button>("loginBtn");
        registerBtn = root.Q<Button>("registerBtn");
        statusTxt = root.Q<Label>("statusTxt");
        uiPanel = root.Q<VisualElement>("loginPanel");

        loginBtn.clicked += OnLoginBtn_Clicked;
        registerBtn.clicked += OnRegisterBtn_Clicked;

        // Initially hide register button
        registerBtn.style.display = DisplayStyle.None;
    }

   private void OnLoginBtn_Clicked()
{
    bool hasAt = emailField.value.IndexOf('@') > 0;
    string status = "default";

    if (string.IsNullOrEmpty(emailField.value) || string.IsNullOrEmpty(passwordField.value))
    {
        status = "emptyFields";
    }
    else if (!hasAt)
    {
        status = "invalidEmail";
    }
    else
    {
        var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value);
        if (userEntry != null)
        {
            status = "loginSuccess";
        }
        else
        {
            var existingUser = UserManager.Instance.GetUserByEmail(emailField.value);
            if (existingUser != null)
            {
                status = "incorrectPassword";
            }
            else
            {
                status = "emailNotFound";
            }
        }
    }

    switch (status)
    {

        case "emptyFields":

            statusTxt.style.color = Color.red;
            statusTxt.text = "Make sure email and password are filled";
            statusTxt.DOShake(1f,5f);
            break;

        case "invalidEmail":

            statusTxt.style.color = Color.red;
            statusTxt.text = "Email is incorrect";
            statusTxt.DOShake(1f,5f);
            break;

        case "loginSuccess":

            var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value);
            statusTxt.style.color = Color.green;
            FadeIn(statusTxt, 2f);
            statusTxt.text = "Welcome " + userEntry.Username;
            StartCoroutine(LoginAnimation(uiPanel));

            break;

        case "incorrectPassword":

            statusTxt.style.color = Color.red;
            statusTxt.text = "Password is incorrect";
            statusTxt.DOShake(1f,5f);
            break;

        case "emailNotFound":

            statusTxt.style.color = Color.yellow;
            FadeIn(statusTxt, 2f);
            statusTxt.text = "Email not found. Would you like to register?";
            registerBtn.style.display = DisplayStyle.Flex; // Show register button
            break;

        default:

            statusTxt.style.color = Color.red;
            statusTxt.text = "An unknown error occurred.";
            statusTxt.DOShake(1f,5f);
            break;
            
    }
}

    private void OnRegisterBtn_Clicked()
    {
        // Extract username from email (string before '@')
        string[] emailParts = emailField.value.Split('@');
        string username = emailParts[0];

        // Check if the email already exists
        var existingUser = UserManager.Instance.GetUserByEmail(emailField.value);
        if (existingUser != null)
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = existingUser.Username + " is already registered. Please choose a different email.";
            statusTxt.DOShake(1f,5f);
            return;
        }

        // Register the new user
        if (!string.IsNullOrEmpty(emailField.value) && !string.IsNullOrEmpty(passwordField.value))
        {
            UserManager.Instance.AddUserEntry(username, emailField.value, passwordField.value);

            // Provide feedback
            statusTxt.style.color = Color.green;
            FadeIn(statusTxt, 2f);
            statusTxt.text = "Registration successful for " + username;

            // Hide register button after registration
            registerBtn.style.display = DisplayStyle.None;
        }
        else
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = "Make sure email and password are filled";
            statusTxt.DOShake(1f,5f);
        }
    }

    private IEnumerator LoginAnimation(VisualElement ve)
    {
        yield return new WaitForSeconds(2);
        float panelPos = -15f;
        DOTween.To(() => panelPos, x => panelPos = x, -110, 2f).SetEase(easePanel.easeType);
        ve.style.bottom = Length.Percent(panelPos);
        while (panelPos != -110)
        {
            ve.style.bottom = Length.Percent(panelPos);
            yield return new WaitForSeconds(.02f);
        }
    }

    private void FadeIn(VisualElement ve, float duration)
    {
        ve.style.opacity = 0;
        ve.DOFade(1f, 2f);
    }
}