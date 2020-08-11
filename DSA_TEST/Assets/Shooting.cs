using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject GoalPost;
    public GameObject GoalKeeper;
    public Rigidbody Ball;
    public float distance;
    public bool shoot;

    Transform leftPost;
    Transform rightPost;
    float GKpos;
    float PPos;
    float result;
    // Start is called before the first frame update
    void Start()
    {
        leftPost = GoalPost.transform.Find("LeftPost");     //Position of the left pole of the goal post
        rightPost = GoalPost.transform.Find("RightPost");   //Position of the right pole of the goal post
        shoot = false;
    }

    private void FixedUpdate()
   // public void shoot()
    {
       if (shoot) //If the player wants to shoot 
       {
            //Calculate the parameters for shooting regression variables 
            //Player's Position from the left pole of the goal
            PPos = Mathf.Abs(transform.localPosition.z - leftPost.position.z) / Vector3.Distance(leftPost.position, rightPost.position);

            //Goalkeeper's Position from the left pole of the goal
            GKpos = Mathf.Abs(GoalKeeper.transform.localPosition.z - leftPost.position.z) / Vector3.Distance(leftPost.position, rightPost.position);

            //Player's distance from the goal post 
            distance = Mathf.Abs(transform.localPosition.x - GoalPost.transform.localPosition.x) / 164f;

            //Substitute in the equations 
            float class1 = 0.5270f + 4.500f * GKpos - 4.4412f * PPos + 0.5554f * distance;
            float class2 = 0.2176f + GKpos * 3.9933f + PPos * (-3.8231f) + distance * (-2.7943f);

            result = Mathf.Round(Mathf.Round(1 / (1 + Mathf.Exp(-class1))) * (Mathf.Round(1 / (1 + Mathf.Exp(-class1))) + Mathf.Round(1 / (1 + Mathf.Exp(-class2)))));
            Debug.Log(transform.name+" "+result);

            //Perform according to the result acquired after regression
            switch (result)
            {
                case (0): //Shoot towards the nearby post 
                    Ball.transform.parent = null;
                    transform.LookAt(leftPost.position);
                    Ball.AddForce(Vector3.Normalize(leftPost.position - transform.localPosition) * 8000f* Time.deltaTime, ForceMode.Impulse);
                    break;
                case (1): //Shoot at the center of the post 
                    Ball.transform.parent = null;
                    transform.LookAt(GoalPost.transform.localPosition);
                    Ball.AddForce(Vector3.Normalize(GoalPost.transform.localPosition - transform.localPosition) * 8000f * Time.deltaTime, ForceMode.Impulse);
                    break;
                case (2): //Shoot at far end of the post 
                    Ball.transform.parent = null;
                    transform.LookAt(rightPost.position);
                    Ball.AddForce(Vector3.Normalize(rightPost.position - transform.localPosition) * 8000f * Time.deltaTime, ForceMode.Impulse);
                    break;
            }
            shoot = false; //reset shooting as false
        }
    }
    
}
