using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class FrogPlayerManager : MonoBehaviourPun
{
    [SerializeField] private GameObject playerPrefab;
    public List<Transform> spawnPoints;
    public GameObject respawnButton;
    public static FrogPlayerManager Instance;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SendSound(List<PlayerController> players, int soundIndex)
    {
        foreach(PlayerController player in players)
        {
            player.view.RPC("ReceiveSound", RpcTarget.All, soundIndex);
        }
    }

    public void RespawnButton()
    {
        
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("Player");

        PhotonView localPlayer = null;

        foreach (GameObject player in tempList)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                localPlayer = player.GetComponent<PhotonView>();
                break;
            }
        }

        if (localPlayer == null)
        {
            Debug.LogError("A disaster has occured");
            return;
        }
        
        photonView.RPC("RespawnPlayer", RpcTarget.All, localPlayer.ViewID);
    }

    [PunRPC]
    public void RespawnPlayer(int viewID)
    {
        PlayerController player = PhotonView.Find(viewID).gameObject.GetComponent<PlayerController>();

        player.isDead = false;
        player.hasDiedOnce = false;
        player.coll.enabled = true;
        player.playerImage.enabled = true;
        int randInt = Random.Range(0, spawnPoints.Count);
        player.transform.position = spawnPoints[randInt].position;

        if(player.inWater)
        {
            player.tadpole.enabled = true;
            player.frog.enabled = false;
        }
        else
        {
            player.tadpole.enabled = false;
            player.frog.enabled = true;
        }
    }

    public void JoinFrog()
    {
        int randInt = Random.Range(0, spawnPoints.Count);
        GameObject frog = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[randInt].position, Quaternion.identity);
        frog.GetComponent<FrogController>().nickname = PhotonNetwork.LocalPlayer.NickName;
        Camera.main.transform.SetParent(frog.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
    }
}
