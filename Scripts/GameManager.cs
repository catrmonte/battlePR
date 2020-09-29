using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Diagnostics;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public string playerPrefabLoc2;
    public PlayerController[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;
    public Material m;

    private int playersInGame;
    private int numPlayers = 1;
    //instance
    public static GameManager instance;

    public float postGameTime;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }
    public PlayerController GetPlayer(int playerId)
    {
        foreach(PlayerController player in players)
        {
            if (player != null & player.id == playerId)
                return player;
        }
        return null;
    }
    public PlayerController GetPlayer(GameObject playerObj)
    {
        foreach (PlayerController player in players)
        {
            if (player != null & player.gameObject == playerObj)
                return player;
        }
        return null;
    }

    [PunRPC]
    void SpawnPlayer()
    {
        UnityEngine.Debug.Log("num players: "+numPlayers);
        GameObject playerObj;

        if (numPlayers % 2 == 1)
        {
            UnityEngine.Debug.Log("blue team");
            playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            numPlayers++;
        }

        else
        {
            UnityEngine.Debug.Log("red team");
            playerObj = PhotonNetwork.Instantiate(playerPrefabLoc2, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            playerObj.GetComponent<MeshRenderer>().material = m;
            numPlayers++;
        }
        
        // initialize player for all other players
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer); 
    }

    public void CheckWinCondition()
    {
        if (alivePlayers == 1)
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id); 
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {
        GameUI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);
        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }

        
}
