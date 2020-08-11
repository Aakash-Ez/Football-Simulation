using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    bool hasBall = false;
    Passing pass;
    float Self_Overlap;
    float Ability = 70;
    GameObject Pass_TO;
    Collider[] Overlap;
    Rigidbody rb;
    Vector3 finalPosition;
    Vector3 initialPosition;

    public GameObject Brain;
    public GameObject GoalPost;
    public float radius;
    public GameObject cylinder;
    public Rigidbody Ball_Rb;
    public float ShootForce;
    public bool isPassingToMe;

    void Start()
    {

        pass = Brain.GetComponent<Passing>();
        isPassingToMe = false;
        rb = GetComponent<Rigidbody>();
        finalPosition = transform.position;
    }

    void Update()
    {
        if (pass.passingTo)
        {
            if (pass.passingTo.name == transform.name)// && pass.TeamInPos==transform.tag)
            {
                isPassingToMe = true;
            }
            else
            {
                isPassingToMe = false;
            }
        }
     
        if (transform.Find("Sphere"))
        {
            hasBall = true;
            Physics.IgnoreCollision(Ball_Rb.GetComponent<Collider>(), GetComponent<Collider>(),true);
            if (isPassingToMe)
            {
                isPassingToMe = false;
            }
        }
        else
        {
            hasBall = false;
            Physics.IgnoreCollision(Ball_Rb.GetComponent<Collider>(), GetComponent<Collider>(),false);
        }

        float count = 0;
        Overlap = Physics.OverlapSphere(transform.position, 10.0f,2);

        foreach(Collider col in Overlap)
        {
            if (col.tag != transform.name)
            {
                count++;
            }
        }

        Self_Overlap = count * 0.33f;


        if (hasBall)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            //if (!Pass_TO)
            Pass_TO = pass.FormTable(gameObject, Self_Overlap, Ability, pass.Tactics);
            rb.velocity = new Vector3(0f, 0f, 0f);
            if (Pass_TO.transform.name != transform.name)
            {
                Debug.Log("Passing to " + Pass_TO.transform.name);
                transform.LookAt(Pass_TO.transform,transform.up);
                Ball_Rb.AddForce(Vector3.Normalize(Pass_TO.transform.position - transform.position) * ShootForce, ForceMode.Impulse);
                //Debug.Log(transform.name+" "+rb.velocity);
            }
            if (isPassingToMe)
            {
                isPassingToMe = false;
            }
        }
        else if (isPassingToMe)
        {

            if (!hasBall)
            {
                rb.MovePosition(transform.position + (Ball_Rb.transform.position - transform.position) * 0.05f);
                /*
                if (Ball_Rb.velocity != new Vector3(0f, 0f, 0f))
                    rb.AddForce(Vector3.Normalize(Vector3.Cross(Vector3.Normalize(Ball_Rb.velocity), new Vector3(0f, 1f, 0f))) * 0.5f, ForceMode.VelocityChange);
                else
                    rb.MovePosition(transform.position + (Ball_Rb.transform.position - transform.position) * 0.1f);*/
                //Debug.Log(transform.name+" "+Ball_Rb.velocity+" "+isPassingToMe+" "+hasBall);
            }
            else
            {
                isPassingToMe = false;
            }

        }
        else
        {
            RandomMovement();
        }
    }

    void RandomMovement()
    {

       // rb.MovePosition(transform.position + (GoalPost.transform.position - transform.position) * 0.01f);

        if (Mathf.Floor(finalPosition.x) == Mathf.Floor(transform.position.x) && Mathf.Floor(finalPosition.z) == Mathf.Floor(transform.position.z))
        {
            initialPosition = transform.position;
            finalPosition = new Vector3(0f, 1f , Random.Range(-30f, 30f ));
            finalPosition.x = GoalPost.transform.position.x;
            rb.MovePosition(transform.position + Vector3.Normalize(finalPosition-transform.position) * 0.005f);
            //Debug.Log(finalPosition + " : " + initialPosition + " : " + transform.position);
        }

        else
        {  
            rb.MovePosition(transform.position + (finalPosition - transform.position) * 0.02f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "Sphere")
        {
            if (collision.transform.parent==pass.ground.transform)
            {
                Ball_Rb.transform.SetParent(transform);
                Debug.Log(transform.name + " has the ball");
                pass.reset();
                pass.posessBall = gameObject;
                collision.gameObject.transform.localPosition = new Vector3(0.03f, -0.4710776f, 0.71f);
                if (isPassingToMe)
                {
                    isPassingToMe = false;
                }
                Debug.Log("PassReset");
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.name=="Sphere")
            transform.Find("Sphere").transform.parent = pass.ground.transform;
        pass.posessBall = null;
    }
}
