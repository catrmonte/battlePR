﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers = 10;
    //singleton instance
    public static NetworkManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        //Turn off any manager created that isnt this
        if (instance != null && instance != this)
        {
           gameObject.SetActive(false);
        } else
        {
           instance = this;
           DontDestroyOnLoad(gameObject);
        }
          
    
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();    
    }

    // attempt to create a room
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;

        PhotonNetwork.CreateRoom(roomName, options);
    }
    public override void OnConnectedToMaster()
    {
         PhotonNetwork.JoinLobby();
    }

    public void JoinRoom (string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GameManager.instance.alivePlayers--;
        GameUI.instance.UpdatePlayerInfoText();
        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
    }
}
