using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{
    public FrogController playermove;
    public List<Collider> triggerList;

    // Start is called before the first frame update
    void Start()
    {
        playermove = GetComponentInParent<FrogController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playermove.isActiveAndEnabled)
        {
            if (triggerList.Count > 0)
            {
                playermove.isGrounded = true;
            }
            else
            {
                playermove.isGrounded = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Collider>().isTrigger && playermove.rb.velocity.y <= 0)
        {
            triggerList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        foreach (Collider coll in triggerList)
        {
            if (coll == other && !coll.isTrigger)
            {
                triggerList.Remove(coll);
                break;
            }
        }
    }
}
