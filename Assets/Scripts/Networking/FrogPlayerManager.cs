using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class FrogPlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinFrog()
    {
        GameObject frog = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-3, 5), 0, 0), Quaternion.identity);
        frog.GetComponent<FrogController>().nickname = PhotonNetwork.LocalPlayer.NickName;
    }
}
