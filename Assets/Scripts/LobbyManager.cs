using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private UIDocument lobbyUI;
    [SerializeField] private VisualTreeAsset EntryTemplate;

    private VisualElement root;
    private Button addButton;
    private ListView list;
    private List<int> items = new List<int>();

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        addButton = root.Q<Button>("add-button");
        list = root.Q<ListView>("list-view");
        addButton.clicked += AddClicked;
        AddClicked();
    }

    void AddClicked()
    {
        items.Add(items.Count);

        list.Clear();

        list.makeItem = () => EntryTemplate.Instantiate();
        list.itemsSource = items;
        list.bindItem = (root, i) =>
        {
            root.Q<Label>().text = i.ToString() + " " + UserManager.Instance.currentUser;
        };
        list.fixedItemHeight= 60;
        list.selectionType = SelectionType.None;
        list.RefreshItems();
    }
}
