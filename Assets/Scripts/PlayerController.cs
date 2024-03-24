using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float powerDifference = 50f;
    [SerializeField] private PhotonView view;
    [SerializeField] private TadpoleController tadpole;
    [SerializeField] private FrogController frog;
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected TMPro.TMP_Text playerPowerText;
    [SerializeField] private Image playerImage;
    private bool isFrog = true;

    public float power = 10f;
    private bool facingRight = true;
    [SerializeField] private Sprite sittingRight;
    [SerializeField] private Sprite sittingLeft;
    [SerializeField] private Sprite jumpingRight;
    [SerializeField] private Sprite jumpingLeft;
    [SerializeField] private Sprite tadpoleUp;
    [SerializeField] private Sprite tadpoleDown;
    [SerializeField] private Sprite tadpoleMid;
    [SerializeField] private bool tadpoleAnim;

    [SerializeField] private float tadpoleAnimTime; // time to wait between each tadpole animation change in seconds
    [SerializeField] private float powerRatio;
    [SerializeField] private int greenFloor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.x > 0)
        {
            facingRight = true;
        }
        else if(rb.velocity.x < 0)
        {
            facingRight = false;
        }

        if(view.IsMine && Input.GetKeyDown(KeyCode.P))
        {
            power += 50;
            view.RPC("SyncPower", RpcTarget.AllBuffered, power);
        }

        if(view.IsMine)
        {
            if (isFrog)
            {
                if (frog.isGrounded && facingRight)
                {
                    view.RPC("ChangeSprite", RpcTarget.AllBuffered, 0);
                }
                else if (frog.isGrounded && !facingRight)
                {
                    view.RPC("ChangeSprite", RpcTarget.AllBuffered, 1);
                }
                else if (!frog.isGrounded && facingRight)
                {
                    view.RPC("ChangeSprite", RpcTarget.AllBuffered, 2);
                }
                else if (!frog.isGrounded && !facingRight)
                {
                    view.RPC("ChangeSprite", RpcTarget.AllBuffered, 3);
                }
            }

            else
            {
                if (!tadpoleAnim)
                {
                tadpoleAnim = true;
                
                //StartCoroutine(TadpoleSpriteCoroutine());
                }
            }
        }
    }

    public IEnumerator TadpoleSpriteCoroutine(int state)
    {
        if (state == 4)
        {
            
        }
        else if (state == 5)
        {
            
        }
        
        else if (state == 6)
        {
            
        }

        yield return new WaitForSeconds(tadpoleAnimTime);
        
        tadpoleAnim = false;
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

        Color powerColor = new Color();
        if(power< greenFloor)
        {
            powerRatio = power / greenFloor;
            float inverse = 1 - powerRatio;
            Debug.Log(powerRatio);
            Debug.Log(inverse);
            powerColor = new Color(0, powerRatio, inverse);
        }

        else if( power >= greenFloor && power < 2 * greenFloor)
        {
            powerRatio = (power - greenFloor) / (greenFloor);
            float inverse = 1 - powerRatio;
            powerColor = new Color(powerRatio, inverse, 0);
        }

        else
        {
            powerColor = new Color(1, 0, 0);
        }


        playerPowerText.color = powerColor;
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
        int randInt = Random.Range(0, FrogPlayerManager.Instance.spawnPoints.Count);
        player.transform.position = FrogPlayerManager.Instance.spawnPoints[randInt].position;
    }

    [PunRPC]
    public void ChangeSprite(int state)
    {
        if(state == 0)
        {
            playerImage.sprite = sittingRight;
        }
        else if(state == 1)
        {
            playerImage.sprite = sittingLeft;
        }
        else if (state == 2)
        {
            playerImage.sprite = jumpingRight;
        }
        else if (state == 3)
        {
            playerImage.sprite = jumpingLeft;
        }
        else if (state == 4)
        {
            playerImage.sprite = tadpoleMid;
        }
    }
}
