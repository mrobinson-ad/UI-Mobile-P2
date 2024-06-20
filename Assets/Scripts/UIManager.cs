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
    }

    private void OnLoginBtn_Clicked()
    {
        bool hasAt = emailField.value.IndexOf('@') > 0;

        if (string.IsNullOrEmpty(emailField.value) || string.IsNullOrEmpty(passwordField.value))
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = "Make sure email and password are filled";
        }
        else if (emailField.value == "admin@unity.com" && passwordField.value == "password")
        {
            statusTxt.style.color = Color.green;
            statusTxt.text = "Welcome admin";
        }
        else if (hasAt)
        {
            var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value);
            if (userEntry != null)
            {
                statusTxt.style.color = Color.green;
                statusTxt.text = "Welcome " + userEntry.Username;
            }
            else
            {
                statusTxt.style.color = Color.yellow;
                statusTxt.text = "Email not found, do you want to register?";
                registerBtn.style.display = DisplayStyle.Flex; // Show register button
            }
        }
        else
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = "Email is incorrect";
        }
    }

    private void OnRegisterBtn_Clicked()
    {
        // Extract username from email (string before '@')
        string[] emailParts = emailField.value.Split('@');
        string username = emailParts[0];

        // Register the new user
        UserManager.Instance.AddUserEntry(username, emailField.value, passwordField.value);

        // Provide feedback
        statusTxt.style.color = Color.green;
        statusTxt.text = "Registration successful for " + username;

        // Hide register button after registration
        registerBtn.style.display = DisplayStyle.None;
    }
}