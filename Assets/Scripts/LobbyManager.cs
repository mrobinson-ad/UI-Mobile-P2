using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private UIDocument lobbyUI;
    [SerializeField] private VisualTreeAsset EntryTemplate;

    private VisualElement root;
    private Button addButton;
    private ListView list;
    private List<int> items = new List<int>();
    private string user;
    bool isConnecting;

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        addButton = root.Q<Button>("add-button");
        list = root.Q<ListView>("list-view");
        user = UserManager.Instance.currentUser;
        //addButton.clicked += AddClicked;
        PhotonNetwork.ConnectUsingSettings();
    }
    #region Code To Refactor
    public void LoginUser()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    [PunRPC]
    
    void AddClicked()
    {
        Debug.Log("another User Joined");
        items.Add(items.Count);

        list.makeItem = () => EntryTemplate.Instantiate();
        list.itemsSource = items;
        list.bindItem = (root, i) =>
        { 
            root.Q<Label>().text = i.ToString() + " " + PhotonNetwork.PlayerList[i].NickName;
        };
        list.fixedItemHeight= 90;
        list.selectionType = SelectionType.None;
        list.RefreshItems();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log(user + "Has joined the room");
        Player player= PhotonNetwork.LocalPlayer;
        player.NickName = user;
        photonView.RPC("AddClicked", RpcTarget.AllBuffered);
        
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        LoginUser();
    }

    #endregion

}
