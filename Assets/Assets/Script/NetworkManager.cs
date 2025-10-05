using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject namePanel;    // Panel where player enters name
    public GameObject roomPanel;    // Panel with room list / create room

    [Header("Inputs")]
    public TMP_InputField playerNameInput; // Input for nickname
    public TMP_InputField roomNameInput;   // Input for room name

    [Header("Room List UI")]
    public GameObject roomItemPrefab;      // Prefab for one room entry
    public Transform roomListContent;      // Parent (Content of ScrollView)

    void Start()
    {
        // Show only the name panel at first
        namePanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    void Awake()
{
    Debug.Log("NetworkManager Awake()");
}


    // Called when the "Connect" button is pressed
    public void OnClickConnect()
    {
        if (!string.IsNullOrEmpty(playerNameInput.text))
        {
            PhotonNetwork.NickName = playerNameInput.text; // set nickname
            Debug.Log("Player name set to: " + PhotonNetwork.NickName);

            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogWarning("Player name cannot be empty!");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("✅ Joined the Lobby!");
        namePanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    // Called when clicking "Create Room" button
    public void OnClickCreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4;

            PhotonNetwork.CreateRoom(roomNameInput.text, options);
            Debug.Log("Creating Room: " + roomNameInput.text);
        }
        else
        {
            Debug.LogWarning("Room name cannot be empty!");
        }
    }

    // Update room list whenever lobby gets new data
    public override void OnRoomListUpdate(System.Collections.Generic.List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContent)
            Destroy(child.gameObject);

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue;

            GameObject item = Instantiate(roomItemPrefab, roomListContent);

            // Use TMP instead of old Text
            TMP_Text roomNameText = item.GetComponentInChildren<TMP_Text>();
            if (roomNameText != null)
                roomNameText.text = room.Name;

            Button btn = item.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => PhotonNetwork.JoinRoom(room.Name));
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("✅ Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("Room"); // load your Room scene
    }
}
