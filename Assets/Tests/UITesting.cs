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
    public IEnumerator UITestingStatusMsg()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        Label label= uIDocument.rootVisualElement.Q<Label>("statusTxt");

        Assert.IsEmpty(label.text);
    }

    [UnityTest]
    public IEnumerator UITestingEmailField()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        TextField email = uIDocument.rootVisualElement.Q<TextField>("EmailField");

       Assert.AreEqual("example@email.com", email.text);
    }

    [UnityTest]
        public IEnumerator UITestingPassField()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        TextField password = uIDocument.rootVisualElement.Q<TextField>("PasswordField");

        Assert.AreEqual(password.value, "****");
    }

        [UnityTest]
        public IEnumerator UITestingInvalidMail()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        Label label= uIDocument.rootVisualElement.Q<Label>("statusTxt");
        TextField email = uIDocument.rootVisualElement.Q<TextField>("EmailField");
        Button loginBtn = uIDocument.rootVisualElement.Q<Button>("loginBtn");
        email.value = "invalidmail";

        using (var clicked = new NavigationSubmitEvent() {target = loginBtn})
            loginBtn.SendEvent(clicked);  

        Assert.IsNotEmpty(label.text, label.text);
    }

            [UnityTest]
        public IEnumerator UITestingEmptyMail()
    {
        SceneManager.LoadScene(0);
        yield return null;

        UIDocument uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        Label label= uIDocument.rootVisualElement.Q<Label>("statusTxt");
        TextField email = uIDocument.rootVisualElement.Q<TextField>("EmailField");
        Button loginBtn = uIDocument.rootVisualElement.Q<Button>("loginBtn");
        email.value = "";

        using (var clicked = new NavigationSubmitEvent() {target = loginBtn})
            loginBtn.SendEvent(clicked);  
        
        Assert.IsNotEmpty(label.text, label.text);
    }
}
