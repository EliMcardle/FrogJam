using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float powerDifference = 50f;
    [SerializeField] public PhotonView view;
    [SerializeField] public TadpoleController tadpole;
    [SerializeField] public FrogController frog;
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected TMPro.TMP_Text playerPowerText;
    [SerializeField] public Image playerImage;
    [SerializeField] public Collider coll;
    private bool isFrog = true;

    public float power = 10f;
    private bool facingRight = true;
    [SerializeField] private Sprite sittingRight;
    [SerializeField] private Sprite sittingLeft;
    [SerializeField] private Sprite jumpingRight;
    [SerializeField] private Sprite jumpingLeft;
    [SerializeField] private Sprite tadpoleRight;
    [SerializeField] private Sprite tadpoleLeft;
    [SerializeField] private Sprite tadpoleUp;
    [SerializeField] private Sprite tadpoleDown;
    
    [SerializeField] private float tadpoleAnimTime; // time to wait for one full cycle
    [SerializeField] private float powerRatio;
    [SerializeField] private int greenFloor;

    [SerializeField] private AudioSource playerSource;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip waterEnterClip;
    [SerializeField] private AudioClip waterExitClip;
    [SerializeField] private AudioClip eatFlyClip;
    [SerializeField] private AudioClip eatFrogClip;
    [SerializeField] private AudioClip frogJump;
    public bool hasDiedOnce;
    
    public bool isDead = false;
    public bool inWater = false;

    [SerializeField] private float audioRadius = 20f;

    private void Start()
    {
        if(!view.IsMine)
        {
            GetComponent<AudioListener>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead) //IF THE PLAYER IS DEAD, NOTHING BELOW THIS RUNS!!!
        {

            if (!hasDiedOnce)
            {
                playerSource.clip = deathClip;
                playerSource.Play();
                hasDiedOnce = true;
            }
            
            rb.velocity = Vector3.zero;
            tadpole.enabled = false;
            frog.enabled = false;
            coll.enabled = false;
            playerImage.enabled = false;

            if(view.IsMine)
            {
                FrogPlayerManager.Instance.respawnButton.SetActive(true);
            }

            return;
        }

        if(rb.velocity.x > 0)
        {
            facingRight = true;
        }
        else if(rb.velocity.x < 0)
        {
            facingRight = false;
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
                if (Mathf.Abs(rb.velocity.x) >= Mathf.Abs(rb.velocity.y))
                {
                    // move horizontal
                    if (facingRight)
                    {
                        //right
                        view.RPC("ChangeSprite", RpcTarget.AllBuffered, 4);
                    }

                    else
                    {
                        //left
                        view.RPC("ChangeSprite", RpcTarget.AllBuffered, 5);
                    }
                }

                else
                {
                    // move vertical

                    if (rb.velocity.y > 0)
                    {
                        // up
                        view.RPC("ChangeSprite", RpcTarget.AllBuffered, 6);
                    }

                    else
                    {
                        //down
                        view.RPC("ChangeSprite", RpcTarget.AllBuffered, 7);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water" && isFrog)
        {
            playerSource.clip = waterEnterClip;
            playerSource.Play();
            List<PlayerController> nearbyPlayers = Contains(Physics.OverlapSphere(transform.position, audioRadius));
            FrogPlayerManager.Instance.SendSound(nearbyPlayers, 3);
            isFrog = false;
            frog.enabled = false;
            tadpole.enabled = true;
            rb.useGravity = false;
            inWater = true;
        }

        if (other.tag == "Eatable" && view.IsMine)
        {
            playerSource.clip = eatFlyClip;
            playerSource.Play();
            List<PlayerController> nearbyPlayers = Contains(Physics.OverlapSphere(transform.position, audioRadius));
            FrogPlayerManager.Instance.SendSound(nearbyPlayers, 1);
            power += other.GetComponent<Eatable>().value;
            view.RPC("DestroyObject", RpcTarget.MasterClient, other.gameObject.GetComponent<PhotonView>().ViewID);
            view.RPC("SyncPower", RpcTarget.AllBuffered, power);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Water" && !isFrog)
        {
            playerSource.clip = waterExitClip;
            playerSource.Play();
            List<PlayerController> nearbyPlayers = Contains(Physics.OverlapSphere(transform.position, audioRadius));
            FrogPlayerManager.Instance.SendSound(nearbyPlayers, 4);
            inWater = false;
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
                otherPlayer.isDead = true;
                view.RPC("KillPlayer", RpcTarget.All, otherPlayer.gameObject.GetComponent<PhotonView>().ViewID);
                view.RPC("SyncPower", RpcTarget.AllBuffered, power);
                
                playerSource.clip = eatFrogClip;
                playerSource.Play();
                List<PlayerController> nearbyPlayers = Contains(Physics.OverlapSphere(transform.position, audioRadius));
                FrogPlayerManager.Instance.SendSound(nearbyPlayers, 2);
            }
        }
    }

    private List<PlayerController> Contains(Collider[] colls)
    {
        List<PlayerController> templist = new List<PlayerController>();

        foreach (Collider coll in colls)
        {
            if (coll.gameObject.GetComponent<PlayerController>())
            {
                templist.Add(coll.gameObject.GetComponent<PlayerController>());
            }
        }

        return templist;
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
        player.isDead = true;
        player.power = 10;
        player.view.RPC("SyncPower", RpcTarget.AllBuffered, player.power);
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
            playerImage.sprite = tadpoleRight;
        }
        
        else if (state == 5)
        {
            playerImage.sprite = tadpoleLeft;
        }
        
        else if (state == 6)
        {
            playerImage.sprite = tadpoleUp;
        }
        
        else if (state == 7)
        {
            playerImage.sprite = tadpoleDown;
        }
        
    }

    [PunRPC]
    public void ReceiveSound(int soundIndex)
    {
        if(view.IsMine)
        {
            if(soundIndex == 0)
            {
                playerSource.clip = frogJump;
                playerSource.Play();
            }
            else if(soundIndex == 1)
            {
                playerSource.clip = eatFlyClip;
                playerSource.Play();
            }
            else if (soundIndex == 2)
            {
                playerSource.clip = eatFrogClip;
                playerSource.Play();
            }
            else if (soundIndex == 3)
            {
                playerSource.clip = waterEnterClip;
                playerSource.Play();
            }
            else if (soundIndex == 4)
            {
                playerSource.clip = waterExitClip;
                playerSource.Play();
            }
        }
    }
}
