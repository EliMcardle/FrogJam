using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMPro.TMP_InputField input;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ChangeUsername()
    {
        PhotonNetwork.LocalPlayer.NickName = input.text;
    }

    public void GoToLobby()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("connected to server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("Joined lobby");

        PhotonNetwork.JoinOrCreateRoom("test", null, null);

        Debug.Log("joined room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        StartGame();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
