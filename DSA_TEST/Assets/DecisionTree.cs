using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    //Private variables 
    Passing brain;
    int LayerMask;
    float Self_Overlap;
    Collider[] Overlap;
    Shooting shoot;
    Vector3 intipos;
    bool hasBall = false;
    GameObject Pass_TO;
    Rigidbody rb;

    //Public Variables
    public bool nearby = false;
    public GameObject goalscored;
    public bool vision;
    public float Ability = 70;
    public float PlayRange;
    private float ballprox;
    public string PrevPassPlayer;
    public float sprintSpeed = 2.0f;
    public int countPass;
    public GameObject Brain;
    public GameObject GoalPost;
    public Rigidbody Ball_Rb;
    public float ShootForce;
    public bool isPassingToMe;

    // Start is called before the first frame update
    void Start()
    {
        brain = Brain.GetComponent<Passing>();
        LayerMask = 1 << 2;
        LayerMask = ~LayerMask;
        shoot = GetComponent<Shooting>();
        rb = GetComponent<Rigidbody>();
        transform.LookAt(new Vector3(GoalPost.transform.localPosition.x, 1f, transform.localPosition.z),transform.up);
        PrevPassPlayer = "null";
        countPass = 0;
        vision = false;
        intipos = transform.localPosition;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
    }

    // Update is called once per frame
    private void Update()
    {

    }
    void FixedUpdate()
    {
            //If a goal has been scored
            if (brain.goalscored)
            {
                transform.localPosition = intipos;
                StartCoroutine(Example());
            
            }

            //If the ball is very close and no one else is possessing the ball 
            if (Vector3.Distance(Ball_Rb.transform.localPosition, transform.localPosition) < 2f && (Ball_Rb.transform.parent.tag != "Team A" && Ball_Rb.transform.parent.tag != "Team B"))
            {
                //Claim the bal's possession
                Ball_Rb.transform.SetParent(transform);
                Ball_Rb.transform.localPosition = new Vector3(-0.283f, -0.2f, 0.962f);
                brain.reset();
                brain.posessBall = gameObject;
                brain.PseudoPos = gameObject.tag;

            }

            //Look Towards Goal post to score 
            transform.LookAt(GoalPost.transform.localPosition);
            RaycastHit hit;
            if (Physics.Raycast(transform.localPosition, transform.forward, out hit, 80.0f, LayerMask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.name == "BluePost" || hit.collider.name == "RedPost")
                {
                    //If the path is clear
                    vision = true;
                }
                else
                {
                    vision = false;
                }

            }

            //If the player is nearest to the ball 
            if (brain.nearestTeamA.name == transform.name || brain.nearestTeamB.name == transform.name)
            {
                nearby = true; //Set nearby flag as true
            }
            else
            {
                nearby = false;
            }

            //If the ball is being passed 
            if (brain.passingTo)
            {
                //If the ball is being passed to me 
                if (brain.passingTo.name == transform.name)
                {
                    isPassingToMe = true;
                }
                else
                {
                    isPassingToMe = false;
                }
            }

            //If my team has the possession
            if (brain.PseudoPos == transform.tag)
            {
                if (transform.Find("Sphere"))   //If I have the Ball
                {
                    //If goal post is visible and is closeby 
                    if (vision && Vector3.Distance(GoalPost.transform.localPosition, transform.localPosition) <= 45)
                    {
                        shoot.shoot = true; //Shoot the ball 
                        brain.reset();
                    }

                    //Calculate the player density acround me
                    float count = 0;

                    //Find all the players around me 
                    Overlap = Physics.OverlapSphere(transform.localPosition, 10.0f, LayerMask);
                    foreach (Collider col in Overlap)
                    {
                        //If the player surrounding me does not belong to my team 
                        if (col.tag != transform.name)
                        {
                            count++;
                        }
                    }

                    Self_Overlap = count * 0.2f;

                    //If I am surrounded by players and the goal post is closeby
                    if (Vector3.Distance(GoalPost.transform.localPosition, transform.localPosition) <= 45 || Self_Overlap > 0.39)//Can Shoot
                    {
                        if (Vector3.Distance(GoalPost.transform.localPosition, transform.localPosition) <= 45 && Mathf.Abs(GoalPost.transform.localPosition.x - transform.localPosition.x) >= 2)
                        {
                            shoot.shoot = true; //Shoot the ball
                            brain.reset();
                        }

                        else //Pass the Ball
                        {
                            //Find the best player to pass the ball to 
                            Pass_TO = brain.FormTable(gameObject, Self_Overlap, Ability, brain.Tactics);

                            //Avoid back and forth passing
                            if (Pass_TO.name == PrevPassPlayer)
                            {
                                countPass++;
                                if (countPass > 2)
                                {
                                    Pass_TO = brain.nextPlayer();
                                    countPass = 0;
                                }
                            }
                            if (Pass_TO.transform.name != transform.name) //Check if the player to pass to is not same as himself
                            {
                                transform.LookAt(Pass_TO.transform, transform.up);
                                transform.Find("Sphere").transform.parent = null;
                                brain.posessBall = null;
                                Ball_Rb.AddForce((Pass_TO.transform.localPosition - transform.localPosition) * ShootForce / 10 * Time.deltaTime, ForceMode.Impulse);
                                PrevPassPlayer = Pass_TO.transform.name;
                                Debug.Log("Passing to " + Pass_TO.transform.name);

                            }
                            if (isPassingToMe) //Declare that pass was received 
                            {
                                isPassingToMe = false;
                            }
                        }
                    }
                    //If the player is free and the goal post is not nearby.
                    else
                    {
                    //Check if the space in front of the player is free or not
                        RaycastHit see;

                        //If the space in front of the player is free or not crowded 
                        if (!Physics.Raycast(transform.localPosition, Vector3.Normalize(GoalPost.transform.localPosition - transform.localPosition), out see, 30.0f))
                        {
                            Ball_Rb.AddForce(transform.forward * 90f * Time.deltaTime, ForceMode.Impulse);
                            transform.Find("Sphere").transform.parent = null;
                        }
                        else
                        {   
                            //Pass the ball to the best player 
                            Pass_TO = brain.FormTable(gameObject, Self_Overlap, Ability, brain.Tactics);
                            if (Pass_TO.name == PrevPassPlayer)
                            {
                                countPass++;
                                if (countPass > 2)
                                {
                                    Pass_TO = brain.nextPlayer();
                                    countPass = 0;
                                }
                            }
                            if (Pass_TO.transform.name != transform.name)
                            {
                                transform.LookAt(Pass_TO.transform, transform.up);
                                transform.Find("Sphere").transform.parent = null;
                                brain.posessBall = null;
                                Ball_Rb.AddForce(Vector3.Normalize(Pass_TO.transform.localPosition - transform.localPosition) * ShootForce * Time.deltaTime, ForceMode.Impulse);
                                PrevPassPlayer = Pass_TO.transform.name;

                            }
                        }
                    }
                }
                else if (isPassingToMe) // Going to Receive a Pass
                {
                    brain.PseudoPos = transform.tag;
                    //If didnt receive the ball 
                    if (!hasBall)
                    {
                        //Move towards the ball 
                        rb.MovePosition(transform.localPosition + Vector3.Normalize(Ball_Rb.transform.localPosition - transform.localPosition) * 1.5f * Time.deltaTime);
                    }
                    else
                    {
                        isPassingToMe = false;
                    }

                }

                //If is not passing to me 
                else // Get to a Better Position
                {
                    bool ballNearby = false;
                    if (Mathf.Abs(intipos.x - transform.localPosition.x) < PlayRange)
                    {
                        if (Mathf.Abs(transform.localPosition.x - Ball_Rb.transform.localPosition.x) < 30)
                        {
                            ballNearby = true;
                        }
                        else
                        {
                            ballNearby = false;
                        }
                        Vector3 lookToGoal = GoalPost.transform.localPosition;
                        Vector3 moveVector;
                        if (ballNearby)
                        {
                            moveVector = new Vector3(3f, 0f, 0f);
                            transform.LookAt(new Vector3(GoalPost.transform.localPosition.x, 1f, transform.localPosition.z), transform.up);
                            moveVector *= GoalPost.transform.localPosition.x / Mathf.Abs(GoalPost.transform.localPosition.x);
                            rb.MovePosition(transform.localPosition + moveVector * sprintSpeed * Time.deltaTime);
                        }
                        else
                        {
                            rb.MovePosition(transform.localPosition + Vector3.Normalize(intipos - transform.localPosition) * sprintSpeed * Time.deltaTime);
                        }

                    }
                    else if (Mathf.Abs(transform.localPosition.x - Ball_Rb.transform.localPosition.x) < 8)
                    {
                        Vector3 lookToGoal = GoalPost.transform.localPosition;
                        Vector3 moveVector = new Vector3(3f, 0f, 0f);
                        moveVector *= GoalPost.transform.localPosition.x / Mathf.Abs(GoalPost.transform.localPosition.x);
                        rb.MovePosition(transform.localPosition + moveVector * sprintSpeed * Time.deltaTime);
                        transform.LookAt(new Vector3(GoalPost.transform.localPosition.x, 1f, transform.localPosition.z), transform.up);
                    }
                }
            }

            //If my team does not have the possession of the ball 
            else
            {
                ballprox = Vector3.Distance(Ball_Rb.transform.localPosition, transform.localPosition);
                ballprox /= Mathf.Sqrt(36968f) * 10.0f;

                //If the ball is free and no one has possession move towards the ball 
                if ((brain.TeamInPos != "Team A" && brain.TeamInPos != "Team B") && ballprox < 5f && nearby)
                {
                    rb.MovePosition(transform.localPosition + Vector3.Normalize(Ball_Rb.transform.localPosition - transform.localPosition) * 25 * Time.deltaTime);
                }

                //If the ball is far away move towards the goal 
                else if (ballprox > 7f)
                {
                    if (transform.localPosition != intipos)
                        rb.MovePosition(transform.localPosition + Vector3.Normalize(GoalPost.transform.localPosition) * 25 * Time.deltaTime);
                }

                //If ball is nearby move towards the ball 
                else if (nearby)
                {

                    rb.MovePosition(transform.localPosition + Vector3.Normalize(Ball_Rb.transform.localPosition - transform.localPosition) * 25 * Time.deltaTime);//10

                }
            }
    }

    //Time Delay function
    IEnumerator Example()
    {
        yield return new WaitForSecondsRealtime(2);
        brain.goalscored = false;
    }

    //Check for collisions
    private void OnCollisionEnter(Collision collision)
    {
        //If the gameobject collided with is a ball 
        if (collision.transform.name == "Sphere")
        {
            //If nobody possesess the ball 
            if (collision.transform.parent == brain.ground.transform)
            {
                Ball_Rb.transform.SetParent(transform);
                brain.reset();
                brain.posessBall = gameObject;
                brain.PseudoPos = gameObject.tag;
                rb.MovePosition(transform.localPosition + new Vector3(0f, 0f, 5f) * Time.deltaTime);
                collision.gameObject.transform.localPosition = new Vector3(-0.283f, -0.2f, 0.962f);
                if (isPassingToMe)
                {
                    isPassingToMe = false;
                }
            }

            //If the player from the opponent's team is having the ball 
            else if(collision.transform.tag!=transform.tag)
            {
                //Compare the abilities 
                if (collision.gameObject.GetComponent<DecisionTree>())
                {
                    //If the opponent's ability is less than mine 
                    if (collision.transform.parent.gameObject.GetComponent<DecisionTree>().Ability < Ability)
                    {
                        //Steal the ball 
                        Ball_Rb.transform.SetParent(transform);
                        brain.reset();
                        brain.posessBall = gameObject;
                        brain.PseudoPos = gameObject.tag;
                        rb.MovePosition(transform.localPosition + new Vector3(0f, 0f, 5f) * Time.deltaTime);
                        collision.gameObject.transform.localPosition = new Vector3(-0.283f, -0.2f, 0.962f);
                        if (isPassingToMe)
                        {
                            isPassingToMe = false;
                        }
                    }
                    else
                    {
                        //Move Back
                        rb.MovePosition(transform.localPosition + new Vector3(0f, 0f, 25f) * Time.deltaTime);
                    }
                }
            }
        }
    }
 
}
