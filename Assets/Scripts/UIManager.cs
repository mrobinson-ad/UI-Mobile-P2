using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Collections;

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

    }

    #region Button events


    private void OnLoginBtn_Clicked()
    {
        #region Login if tree
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
        #endregion

        #region Login switch

        switch (status)
        {

            case "emptyFields":

                statusTxt.style.color = Color.red;
                statusTxt.text = "Make sure email and password are filled";
                statusTxt.DOShake(1f, 5f);
                break;

            case "invalidEmail":

                statusTxt.style.color = Color.red;
                statusTxt.text = "Email is incorrect";
                statusTxt.DOShake(1f, 5f);
                break;

            case "loginSuccess":

                var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value);
                statusTxt.style.color = Color.green;
                //FadeIn(statusTxt, 2f);
                statusTxt.text = "Welcome " + userEntry.Username;
                //StartCoroutine(LoginAnimation(uiPanel));
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
        if (existingUser != null)
        {
            statusTxt.style.color = Color.red;
            statusTxt.text = existingUser.Username + " is already registered. Please choose a different email.";
            statusTxt.DOShake(1f, 5f);
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

    private void FadeIn(VisualElement ve, float duration)
    {
        ve.style.opacity = 0;
        ve.DOFade(1f, 2f);
    }
    public void StartLoginAnimation()
    {
        var userEntry = UserManager.Instance.ValidateUser(emailField.value, passwordField.value);
        statusTxt.style.color = Color.green;
        FadeIn(statusTxt, 2f);
        statusTxt.text = "Welcome " + userEntry.Username;

        float startValue = -15f; // Initial position value
        float endValue = -110f;  // Target position value

        // Start the animation sequence
        DOTween.Sequence()
            .AppendInterval(2) // Wait for 2 seconds
            .Append(uiPanel.DOMovePercent(Side.Bottom, startValue, endValue, 2f, easePanel.easeType))
            .AppendInterval(3)
            .OnComplete(() =>
            {
                Debug.Log("Movement complete");
                gameObject.SetActive(false);
            });

    }
    #endregion
}