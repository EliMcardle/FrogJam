using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class FrogController : MonoBehaviour
{
    [SerializeField] private float jumpCharge = 0f;
    [Tooltip("Controls how fast the jump charges")]
    [SerializeField] private float chargeRate = 0.1f;
    [Tooltip("Max height of player jump")]
    [SerializeField] private float maxHeight = 5f;
    [Tooltip("Horizontal jump force")]
    [SerializeField] private float horizJumpForce = 5f;

    [SerializeField] private float movespeed = 3f;

    public string nickname = "DEFAULT";
    [SerializeField] private PhotonView view;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private TMPro.TMP_Text playerNameText;
    private float xInput = 0;
    private Vector3 previousVelocity = Vector3.zero;
    public bool isGrounded = false;
    private bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        playerNameText.text = view.Controller.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine)
        {
            if(Input.GetKey(KeyCode.Space) && isGrounded)
            {
                rb.velocity = Vector3.zero;
                isJumping = true;
                jumpCharge += chargeRate * Time.deltaTime;
                if(jumpCharge > 1)
                {
                    jumpCharge = 1;
                }
            }
            else if(jumpCharge > 0 && isGrounded)
            {
                Jump();
                jumpCharge = 0;
            }

            if(!isGrounded)
            {
                jumpCharge = 0;
            }

            xInput = Input.GetAxisRaw("Horizontal");
        }
    }

    private void LateUpdate()
    {
        previousVelocity = rb.velocity;
    }

    private void FixedUpdate()
    {
        if(isGrounded && !isJumping)
        {
            rb.velocity = Vector3.right * xInput * movespeed;
        }

    }

    private void Jump()
    {
        Invoke("StopJump", 0.1f);
        rb.velocity = Vector3.up * jumpCharge * maxHeight + Vector3.right * xInput * horizJumpForce;
    }

    private void StopJump()
    {
        isJumping = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.contacts[0].normal.y == 0)
        {
            rb.velocity = new Vector3(collision.contacts[0].normal.x, rb.velocity.normalized.y, collision.contacts[0].normal.z) * rb.velocity.magnitude;
        }
    }
}
