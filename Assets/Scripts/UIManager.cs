using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private VisualElement root;
    private TextField emailField;
    private TextField passwordField;
    private Button loginBtn;
    private Button registerBtn;
    private Label statusTxt;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        emailField = root.Q<TextField>("EmailField");
        passwordField = root.Q<TextField>("PasswordField");
        loginBtn = root.Q<Button>("loginBtn");
        registerBtn = root.Q<Button>("registerBtn");
        statusTxt = root.Q<Label>("statusTxt");

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
            break;

        case "invalidEmail":

            statusTxt.style.color = Color.red;
            statusTxt.text = "Email is incorrect";
            break;

        case "loginSuccess":

            var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value);
            statusTxt.style.color = Color.green;
            statusTxt.text = "Welcome " + userEntry.Username;
            break;

        case "incorrectPassword":

            statusTxt.style.color = Color.red;
            statusTxt.text = "Password is incorrect";
            break;

        case "emailNotFound":

            statusTxt.style.color = Color.yellow;
            statusTxt.text = "Email not found. Would you like to register?";
            registerBtn.style.display = DisplayStyle.Flex; // Show register button
            break;

        default:

            statusTxt.style.color = Color.red;
            statusTxt.text = "An unknown error occurred.";
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
            return;
        }

        // Register the new user
        if (!string.IsNullOrEmpty(emailField.value) && !string.IsNullOrEmpty(passwordField.value))
        {
            UserManager.Instance.AddUserEntry(username, emailField.value, passwordField.value);

            // Provide feedback
            statusTxt.style.color = Color.green;
            statusTxt.text = "Registration successful for " + username;

            // Hide register button after registration
            registerBtn.style.display = DisplayStyle.None;
        }
        else
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = "Make sure email and password are filled";
        }
    }
}