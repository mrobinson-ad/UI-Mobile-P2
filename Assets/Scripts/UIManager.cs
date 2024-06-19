using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private VisualElement root;
    private TextField emailField;
    private TextField passwordField;
    private Button loginBtn;

    private Label statusTxt;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        emailField = root.Q<TextField>("EmailField");
        passwordField = root.Q<TextField>("PasswordField");
        loginBtn = root.Q<Button>("loginBtn");
        statusTxt = root.Q<Label>("statusTxt");
        loginBtn.clicked += OnLoginBtn_Clicked;
        
    }

    private void OnLoginBtn_Clicked()
    {
        bool hasAt = emailField.value.IndexOf('@') > 0;
        if (emailField.value.ToString() == "" | emailField.value.ToString() == "Enter text ..." | passwordField.value.ToString() == "")
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = "Make sure email and password are filled";
        }
        else if (emailField.value.ToString() == "admin@unity.com" && passwordField.value.ToString() == "password")
        {
            statusTxt.style.color = Color.green;
            statusTxt.text = "Welcome admin";
        }
        else if (emailField.value.IndexOf('@') > 0 )
        {
            statusTxt.style.color = Color.yellow;
            statusTxt.text = "Email not found, do you want to register?";
        }
        else if (hasAt  == false)
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = "Email is incorrect";
        }

    }

}
