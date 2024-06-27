using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
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

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
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
                label.text = i.ToString() + " " + PhotonNetwork.PlayerList[i].NickName;
            }
        };
        user = UserManager.Instance.currentUser;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Functions

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
        LoginUser();
    }

    public override void OnLeftRoom()
    {
        photonView.RPC("RemoveUser", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        WaitForSeconds(2f);
        base.OnLeftRoom();
    }

    #endregion

    private IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
    }

}