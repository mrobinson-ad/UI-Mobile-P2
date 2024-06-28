using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class UITesting
{

    [UnityTest]
    // checks if status message is empty on start
    public IEnumerator UITestingStatusMsg()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        Label label = uIDocument.rootVisualElement.Q<Label>("statusTxt");

        Assert.IsEmpty(label.text);
    }

    [UnityTest]
    //checks if initial email field value is correct
    public IEnumerator UITestingEmailField()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        TextField email = uIDocument.rootVisualElement.Q<TextField>("EmailField");

        Assert.AreEqual("example@email.com", email.text);
    }

    [UnityTest]
    //Checks if initial email field value is correct
    public IEnumerator UITestingPassField()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        TextField password = uIDocument.rootVisualElement.Q<TextField>("PasswordField");

        Assert.AreEqual(password.value, "****");
    }

    [UnityTest]
    // Checks if the correct error is displayed when entering an incorrect email 
    public IEnumerator UITestingInvalidMail()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        Label label = uIDocument.rootVisualElement.Q<Label>("statusTxt");
        TextField email = uIDocument.rootVisualElement.Q<TextField>("EmailField");
        Button loginBtn = uIDocument.rootVisualElement.Q<Button>("loginBtn");
        email.value = "invalidmail";

        using (var clicked = new NavigationSubmitEvent() { target = loginBtn })
            loginBtn.SendEvent(clicked);

        Assert.AreEqual(label.text, "Email is invalid");
    }

    [UnityTest]
    // Checks if the correct error is displayed when the email field is empty
    public IEnumerator UITestingEmptyMail()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        Label label = uIDocument.rootVisualElement.Q<Label>("statusTxt");
        TextField email = uIDocument.rootVisualElement.Q<TextField>("EmailField");
        Button loginBtn = uIDocument.rootVisualElement.Q<Button>("loginBtn");
        email.value = "";

        using (var clicked = new NavigationSubmitEvent() { target = loginBtn })
            loginBtn.SendEvent(clicked);

        Assert.AreEqual(label.text, "Make sure email and password are filled");
    }

    [UnityTest]
    // Checks if the correct error is displayed when the password field is empty
    public IEnumerator UITestingEmptyPassword()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        Label label = uIDocument.rootVisualElement.Q<Label>("statusTxt");
        TextField password = uIDocument.rootVisualElement.Q<TextField>("PasswordField");
        Button loginBtn = uIDocument.rootVisualElement.Q<Button>("loginBtn");
        password.value = "";

        using (var clicked = new NavigationSubmitEvent() { target = loginBtn })
            loginBtn.SendEvent(clicked);

        Assert.AreEqual(label.text, "Make sure email and password are filled");
    }

    [UnityTest]
    // Tests if the login animation is started correctly when a valid email/password combination is entered

    public IEnumerator UITestingSuccessfullLogin()
    {
        SceneManager.LoadScene(0);
        yield return null;
        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        TextField email = uIDocument.rootVisualElement.Q<TextField>("EmailField");
        TextField password = uIDocument.rootVisualElement.Q<TextField>("PasswordField");
        Button loginBtn = uIDocument.rootVisualElement.Q<Button>("loginBtn");
        email.value = "matt@unity.com";
        password.value = "1234";

        using (var clicked = new NavigationSubmitEvent() { target = loginBtn })
            loginBtn.SendEvent(clicked);

        //Assert.IsTrue(UIManager.StartLoginAnimation())

    }
}
