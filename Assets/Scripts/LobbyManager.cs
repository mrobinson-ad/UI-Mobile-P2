using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using DG.Tweening;

// Manages Connection to Photon and the display of the connected users
public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Variable declaration
    [SerializeField] private UIDocument lobbyUI;
    [SerializeField] private VisualTreeAsset EntryTemplate;
    private VisualElement uiPanel;
    public EaseTypeWrapper easePanel; // Serialize the EaseTypeWrapper for panels

    private VisualElement root;
    private Button disconnectButton;
    private ListView list;
    private List<int> items = new List<int>();
    private string user;

    #endregion

    void Awake()
    { 
        root = GetComponent<UIDocument>().rootVisualElement;
        uiPanel = root.Q<VisualElement>("panel-background");
        disconnectButton = root.Q<Button>("disconnect-button");
        disconnectButton.clicked += Disconnect;
        // To do: study binding items
        #region Initializes list and binds it 
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
        #endregion 
        user = UserManager.Instance.currentUser;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Functions

     void Disconnect()
    {
        disconnectButton.clicked -= Disconnect;
 // logic to be changed, functionality is partial but unreliable
        StartDisconnectAnimation();
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
        Debug.Log("remove user" + actorNumber);
        items.RemoveAt(actorNumber -1); 
        list.RefreshItems();
        Debug.Log("list remaining " + items.Count);
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
        base.OnLeftRoom();
        PhotonNetwork.Disconnect();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    #endregion

        public bool StartDisconnectAnimation() 
    {
        float startValue = 0f; // Initial position value
        float endValue = -140f;  // Target position value

        // DOTween sequence initiates the animation with an interval to provide better feedback, the Side from which the element moves can be changed with the Side Enum
        // the values are in % and the easetype is serialized with an editor scrollable dropdown
        DOTween.Sequence()
            .AppendInterval(1) // Wait for 2 seconds
            .Append(uiPanel.DOMovePercent(Side.Top, startValue, endValue, 2f, easePanel.easeType))
            .AppendInterval(1)
            .OnComplete(() =>
            {
                photonView.RPC("RemoveUser", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
                Debug.Log("Movement complete");
                PhotonNetwork.LeaveRoom();
            });
        return true;
    }

}