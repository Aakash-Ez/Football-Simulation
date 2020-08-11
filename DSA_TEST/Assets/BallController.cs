using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject Brain;
    public GameObject ground;
    Passing brain;
    // Start is called before the first frame update
    void Start()
    {
        brain = Brain.GetComponent<Passing>();
    }

    // Update is called once per frame
    void Update()
    {        

        //Debug.Log(GetComponent<Rigidbody>().velocity);
        if(transform.parent==null)
        {
            transform.parent = ground.transform;
        }

        if (transform.parent!=ground.transform)
        {
         //  GetComponent<Rigidbody>().velocity = new Vector3 (0f,0f,0f);
            transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        }
        else
        {
            transform.parent = ground.transform;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
    }
    private void OnTransformParentChanged()
    {
        if(transform.parent!=ground.transform)
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        //brain.TeamInPos = null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            brain.reset();
            Vector3 vel = GetComponent<Rigidbody>().velocity;
            GetComponent<Rigidbody>().AddForce( Vector3.zero - transform.localPosition * 0.01f * Time.deltaTime, ForceMode.Impulse);
        }
    }

}
