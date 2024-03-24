using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EatableSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> possibleSpawns;
    [Tooltip("Each index of spawn chance correlates to a possible spawn of the SAME INDEX!")]
    [SerializeField] List<float> spawnChance;

    [SerializeField] private float spawnrate = 3;

    private BoxCollider spawnCollider;

    // Start is called before the first frame update
    void Start()
    {
        spawnCollider = GetComponent<BoxCollider>();
        if(PhotonNetwork.IsMasterClient)
        {
            Invoke("SpawnEatable", 0.5f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEatable()
    {
        float xPos = Random.Range(transform.position.x + transform.localScale.x / 2, transform.position.x - transform.localScale.x / 2);
        float yPos = Random.Range(transform.position.y + transform.localScale.y / 2, transform.position.y - transform.localScale.y / 2);
        float randFloat = Random.Range(0f, 100f);
        int i = 0;
        foreach(float chance in spawnChance)
        {
            if(randFloat <= chance)
            {
                GameObject eatable = PhotonNetwork.Instantiate(possibleSpawns[i].name, new Vector3(xPos, yPos, 0), Quaternion.identity);
                break;
            }
            i++;
        }


        if(PhotonNetwork.IsMasterClient)
        {
            Invoke("SpawnEatable", spawnrate);
        }
    }
}
