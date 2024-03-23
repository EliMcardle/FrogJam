using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EatableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject flyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnEatable", 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEatable()
    {
        GameObject eatable = PhotonNetwork.Instantiate(flyPrefab.name, new Vector3(Random.Range(-10f, 7f), Random.Range(1.5f, 5), 0), Quaternion.identity);
        Invoke("SpawnEatable", 3f);
    }
}
