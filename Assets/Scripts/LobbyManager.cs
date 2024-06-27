using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private UIDocument lobbyUI;
    [SerializeField] private VisualTreeAsset EntryTemplate;

    private VisualElement root;
    private Button disconnectButton;
    private ListView list;
    private List<int> items = new List<int>();
    private string user;

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        disconnectButton = root.Q<Button>("disconnect-button");
        disconnectButton.clicked += Disconnect;
        list = root.Q<ListView>("list-view");
        list.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
        list.fixedItemHeight = 90;
        list.selectionType = SelectionType.None;

        list.makeItem = () => EntryTemplate.Instantiate();
        list.bindItem = (element, i) => 
        {
            var label = element.Q<Label>();
            if (label != null)
            {   
                label.text = PhotonNetwork.PlayerList[i].NickName;
            }
            if (label.text == user)
            {
                label.style.color = Color.green;
            }
        };
        user = UserManager.Instance.currentUser;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Functions

     void Disconnect()
    {
        photonView.RPC("RemoveUser", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        StartCoroutine(WaitForSeconds(2f));
        PhotonNetwork.LeaveRoom();
    }
    public void LoginUser()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    [PunRPC]
    void AddUser()
    {
        items.Add(items.Count);
        list.itemsSource = items;
        list.RefreshItems();
    }

    [PunRPC]
    void RemoveUser(int actorNumber)
    {
        items.RemoveAt(actorNumber);
        list.RefreshItems();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log(user + "Has joined the room");
        Player player = PhotonNetwork.LocalPlayer;
        player.NickName = user;
        photonView.RPC("AddUser", RpcTarget.AllBuffered);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log(user + "Has connected to master successfully");
        LoginUser(); //Remember to change this line
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    #endregion

    private IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
    }

}