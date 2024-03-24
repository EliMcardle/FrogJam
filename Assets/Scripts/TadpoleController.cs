using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TadpoleController : MonoBehaviour
{
    [SerializeField] protected PhotonView view;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public float movespeed = 5f;
    [SerializeField] private float maxSpeed = 20f;
    private float xInput = 0;
    private float yInput = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = new Vector3(xInput, yInput, 0).normalized;
        rb.velocity += movespeed * moveVector * Time.fixedDeltaTime;

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}