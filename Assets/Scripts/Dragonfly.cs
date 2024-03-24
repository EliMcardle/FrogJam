using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Dragonfly : Eatable
{
    [SerializeField] private float maxRange = 10f;
    [SerializeField] private float flySpeed = 7f;
    [SerializeField] private Image bugImage;
    [SerializeField] private PhotonView view;
    private bool flyingLeft = true;
    private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        view.RPC("ChangeSprite", RpcTarget.All, flyingLeft);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x - spawnPoint.x > maxRange)
        {
            flyingLeft = true;
            view.RPC("ChangeSprite", RpcTarget.All, flyingLeft);
        }
        else if (transform.position.x - spawnPoint.x < -maxRange)
        {
            flyingLeft = false;
            view.RPC("ChangeSprite", RpcTarget.All, flyingLeft);
        }
    }

    private void FixedUpdate()
    {
        if(flyingLeft)
        {
            transform.position += Vector3.right * -flySpeed * Time.fixedDeltaTime;
        }
        else
        {
            transform.position += Vector3.right * flySpeed * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.isTrigger)
        {
            flyingLeft = !flyingLeft;
            view.RPC("ChangeSprite", RpcTarget.All, flyingLeft);
        }
    }

    [PunRPC]
    public void ChangeSprite(bool state)
    {
        if(state)
        {
            bugImage.rectTransform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            bugImage.rectTransform.eulerAngles = Vector3.zero;
        }
    }
}
