using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        /*if(collision.transform.tag!="Wall")
        {
            Debug.Log(collision.collider.name + " collided with the wall with velocity " + collision.rigidbody.velocity);
            collision.rigidbody.AddForce(-collision.rigidbody.velocity);
        }*/
    }
}
