using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
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
            other.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            PhotonNetwork.Destroy(other.gameObject);
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

    [PunRPC]
    public void SyncPower(float myPower)
    {
        power = myPower;
        playerPowerText.text = power.ToString();
    }
}
