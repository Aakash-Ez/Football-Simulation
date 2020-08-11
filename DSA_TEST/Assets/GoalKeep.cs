using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeep : MonoBehaviour
{
    Vector3 moveVector;
    bool goalKick;
    GameObject Pass_TO;
    Passing brain;

    public GameObject ball;
    public GameObject Brain;
    // Start is called before the first frame update
    void Start()
    {
        brain = Brain.GetComponent<Passing>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.LookAt(Vector3.zero);
        Debug.DrawRay(transform.localPosition, transform.forward * 10, Color.black);
        if (ball.transform.localPosition.z >= -8 && ball.transform.localPosition.z <= 8)
            moveVector.z = ball.transform.localPosition.z;
        //elseif()
        moveVector.x = transform.localPosition.x;
        moveVector.y = 0;
        GetComponent<Rigidbody>().MovePosition(transform.localPosition + Vector3.Normalize(moveVector - transform.localPosition)*9f*Time.deltaTime);

        if (goalKick)
        {
            Pass_TO = brain.FormTable(gameObject, 0, 70, brain.Tactics);
            if (Pass_TO.transform.name != transform.name)
            {
                transform.LookAt(Pass_TO.transform, transform.up);
                transform.Find("Sphere").transform.parent = null;
                brain.posessBall = null;
                ball.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(Pass_TO.transform.localPosition - transform.localPosition) * 4000.0f * Time.deltaTime, ForceMode.Impulse);
                goalKick = false;
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "Sphere")
        {
            if(collision.transform.parent == null)
            {
                ball.transform.SetParent(transform);
                brain.reset();
                brain.posessBall = gameObject;
                brain.PseudoPos = gameObject.tag;
                collision.gameObject.transform.localPosition = new Vector3(-0.283f, -0.2f, 0.962f);
                goalKick = true;
            }
            else if (collision.transform.parent == brain.ground.transform)
            {
                ball.transform.SetParent(transform);
                brain.reset();
                brain.posessBall = gameObject;
                brain.PseudoPos = gameObject.tag;
                collision.gameObject.transform.localPosition = new Vector3(-0.283f, -0.2f, 0.962f);
                goalKick = true;
            }
        }
    }
}
