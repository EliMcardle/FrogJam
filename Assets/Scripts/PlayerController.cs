using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float powerDifference = 50f;
    [SerializeField] private PhotonView view;
    [SerializeField] private TadpoleController tadpole;
    [SerializeField] private FrogController frog;
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected TMPro.TMP_Text playerPowerText;
    private bool isFrog = true;

    public float power = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine && Input.GetKeyDown(KeyCode.P))
        {
            power += 50;
            view.RPC("SyncPower", RpcTarget.AllBuffered, power);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water" && isFrog)
        {
            isFrog = false;
            frog.enabled = false;
            tadpole.enabled = true;
            rb.useGravity = false;
        }

        if (other.tag == "Eatable" && view.IsMine)
        {
            power += other.GetComponent<Eatable>().value;
            view.RPC("DestroyObject", RpcTarget.MasterClient, other.gameObject.GetComponent<PhotonView>().ViewID);
            view.RPC("SyncPower", RpcTarget.AllBuffered, power);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water" && !isFrog)
        {
            isFrog = true;
            frog.enabled = true;
            tadpole.enabled = false;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && view.IsMine)
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if(otherPlayer.power < power - powerDifference)
            {
                power += otherPlayer.power;
                view.RPC("KillPlayer", RpcTarget.All, otherPlayer.gameObject.GetComponent<PhotonView>().ViewID);
                view.RPC("SyncPower", RpcTarget.AllBuffered, power);
            }
        }
    }

    [PunRPC]
    public void SyncPower(float myPower)
    {
        power = myPower;
        playerPowerText.text = power.ToString();
    }

    [PunRPC]
    public void DestroyObject(int viewID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
    }

    [PunRPC]
    public void KillPlayer(int viewID)
    {
        PlayerController player = PhotonView.Find(viewID).gameObject.GetComponent<PlayerController>();
        player.power = 10;
        player.view.RPC("SyncPower", RpcTarget.AllBuffered, player.power);
        player.transform.position = new Vector3(Random.Range(-3, 5), 0, 0);
    }
}
