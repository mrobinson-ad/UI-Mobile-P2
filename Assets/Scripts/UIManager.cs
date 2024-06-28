using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{

    #region UI variables
    private VisualElement root;
    private TextField emailField;
    private TextField passwordField;
    private Button loginBtn;
    private Button registerBtn;
    private Label statusTxt; // Label used to provide feedback depending on the text field values when buttons are clicked
    private VisualElement uiPanel; //Includes all components of the Visual Tree Asset except the root background
    #endregion

    public EaseTypeWrapper easePanel; // Serialize the EaseTypeWrapper for panels

    [SerializeField] GameObject lobbyUI; //UIDocument to display when logged in correctly


    private void Awake()
    {
        // Set all variables references
        root = GetComponent<UIDocument>().rootVisualElement;
        emailField = root.Q<TextField>("EmailField");
        passwordField = root.Q<TextField>("PasswordField");
        loginBtn = root.Q<Button>("loginBtn");
        registerBtn = root.Q<Button>("registerBtn");
        statusTxt = root.Q<Label>("statusTxt");
        uiPanel = root.Q<VisualElement>("loginPanel");

        //Subscribe to the Clicked events
        loginBtn.clicked += OnLoginBtn_Clicked;
        registerBtn.clicked += OnRegisterBtn_Clicked;

    }

    #region Button events


    private void OnLoginBtn_Clicked()
    {
        #region Login if tree

        //If tree checks the fields' values and changes status that is evaluated in the switch for better clarity
        bool hasAt = emailField.value.IndexOf('@') > 0; //Check if email has an @ (upgrade to a real email verification)
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
            var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value); //Uses UserManager to check if email and password match a single UserEntry
            if (userEntry != null)
            {
                status = "loginSuccess";
            }
            else
            {
                var existingUser = UserManager.Instance.GetUserByEmail(emailField.value); //If ValidateUser fails checks if the email is registered to set incorrectPassword and if not prompts the user to register
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
        #endregion

        #region Login switch
        //Displays different status label depending on the status with DOTween animations (shorthand made compatible with UI Toolkit with DOTweenExtensions)
        switch (status) 
        {

            case "emptyFields":

                statusTxt.style.color = Color.red;
                statusTxt.text = "Make sure email and password are filled";
                statusTxt.DOShake(1f, 5f);
                break;

            case "invalidEmail":

                statusTxt.style.color = Color.red;
                statusTxt.text = "Email is invalid";
                statusTxt.DOShake(1f, 5f);
                break;

            case "loginSuccess":

                var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value); //Maybe refactor since the same value setting is made in the if tree
                statusTxt.style.color = Color.green;
                statusTxt.text = "Welcome " + userEntry.Username; 
                StartLoginAnimation();

                break;

            case "incorrectPassword":

                statusTxt.style.color = Color.red;
                statusTxt.text = "Password is incorrect";
                statusTxt.DOShake(1f, 5f);
                break;

            case "emailNotFound":

                statusTxt.style.color = Color.yellow;
                FadeIn(statusTxt, 2f);
                statusTxt.text = "Email not found. Would you like to register?";
                break;

            default:

                statusTxt.style.color = Color.red;
                statusTxt.text = "An unknown error occurred.";
                statusTxt.DOShake(1f, 5f);
                break;

        }
        #endregion
    }
    #region Register if tree
    private void OnRegisterBtn_Clicked()
    {
        // Extract username from email (string before '@')
        string[] emailParts = emailField.value.Split('@');
        string username = emailParts[0];

        // Check if the email already exists
        var existingUser = UserManager.Instance.GetUserByEmail(emailField.value); 
        if (existingUser != null) // Informs the user that the email is already in use
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = existingUser.Username + " is already registered. Please choose a different email.";
            statusTxt.DOShake(1f, 5f);
            return;
        }

        // Register the new user
        if (!string.IsNullOrEmpty(emailField.value) && !string.IsNullOrEmpty(passwordField.value))
        {
            UserManager.Instance.AddUserEntry(username, emailField.value, passwordField.value); // Adds an entry in UserManager that is savec locally in a PlayerPref.json (in the future these tables should be stored in a server DB)

            statusTxt.style.color = Color.green;
            FadeIn(statusTxt, 2f);
            statusTxt.text = "Registration successful for " + username;

        }
        else
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = "Make sure email and password are filled";
            statusTxt.DOShake(1f, 5f);
        }
    }
    #endregion

    #endregion

    #region DOTween animations
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

    private void FadeIn(VisualElement ve, float duration) //Made so you can input a VisualElement and duration and use it to fade in from an alpha 0 to alpha 1 while keeping DOFade more general
    {
        ve.style.opacity = 0;
        ve.DOFade(1f, 2f);
    }
    public bool  StartLoginAnimation() //return bool for unit testing
    {
        
        var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value); //Maybe refactor for the same reason as the switch case
        statusTxt.style.color = Color.green;
        FadeIn(statusTxt, 2f);
        statusTxt.text = "Welcome " + userEntry.Username;

        float startValue = -15f; // Initial position value
        float endValue = -110f;  // Target position value

        // DOTween sequence initiates the animation with an interval to provide better feedback, the Side from which the element moves can be changed with the Side Enum
        // the values are in % and the easetype is serialized with an editor scrollable dropdown
        DOTween.Sequence()
            .AppendInterval(2) // Wait for 2 seconds
            .Append(uiPanel.DOMovePercent(Side.Bottom, startValue, endValue, 2f, easePanel.easeType))
            .OnComplete(() =>
            {
                lobbyUI.SetActive(true);
                Debug.Log("Movement complete");
                gameObject.SetActive(false);
            });
        return true;
    }

    //public Action OnAnimationStart
    //public Action OnAnimationEnd
    #endregion
}