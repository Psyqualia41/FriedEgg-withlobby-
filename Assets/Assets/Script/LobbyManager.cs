using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;  

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_InputField roomNameInput;       
    public GameObject roomListContent;
    public GameObject roomListItemPrefab;

    
    public void CreateRoom()
    {
        string roomName = roomNameInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
        }
    }

    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        
        foreach (Transform child in roomListContent.transform)
            Destroy(child.gameObject);

        
        foreach (RoomInfo room in roomList)
        {
            GameObject item = Instantiate(roomListItemPrefab, roomListContent.transform);

            
            TMP_Text textComponent = item.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = room.Name;
            }

            
            Button btn = item.GetComponent<Button>();
            btn.onClick.AddListener(() => JoinRoom(room.Name));
        }
    }

    void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
    }
}
