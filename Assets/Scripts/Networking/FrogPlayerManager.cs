using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class FrogPlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public List<Transform> spawnPoints;

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
        int randInt = Random.Range(0, spawnPoints.Count);
        GameObject frog = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[randInt].position, Quaternion.identity);
        frog.GetComponent<FrogController>().nickname = PhotonNetwork.LocalPlayer.NickName;
    }
}
