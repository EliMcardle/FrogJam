using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragonfly : Eatable
{
    [SerializeField] private float maxRange = 10f;
    [SerializeField] private float flySpeed = 7f;
    private bool flyingLeft = true;
    private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x - spawnPoint.x > maxRange)
        {
            flyingLeft = true;
        }
        else if (transform.position.x - spawnPoint.x < -maxRange)
        {
            flyingLeft = false;
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
        }
    }
}
