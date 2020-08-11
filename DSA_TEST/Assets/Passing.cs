using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passing : MonoBehaviour
{
    //Initialising Private Game Objects
    Vector3[] TeamAposition = new Vector3[5];
    Vector3[] TeamBposition = new Vector3[5];
    float[] TeamAOverlap = new float[5];
    float[] TeamBOverlap = new float[5];
    float[] Pass_Prob = new float[5];
    int layerMask;
    float smallest;
    //Initialising Public Game Objects
    public bool gamepause=true;
    public bool goalscored=false;
    public bool goalKick;
    public GameObject[] Pass_Team = new GameObject[5];
    public GameObject[] TeamA;
    public GameObject[] TeamB;
    public GameObject passingTo;
    public GameObject NextPlayer;
    public GameObject posessBall;
    [Range(-100, 100)]
    public float Tactics;
    public string TeamInPos;
    public GameObject ground;
    public GameObject ball;
    public GameObject nearestTeamA;
    public GameObject nearestTeamB;
    public string PseudoPos;

    // Start is called before the first frame update
    void Start()
    {
        //Setting Flags to NULL
        passingTo = null;
        TeamInPos = null;

        //Setting Ignore Raycast LayerMask
        layerMask = 1 << 2;
        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
            float count = 0;
        //If a goal is scored Reset all the variables 
            if (goalscored)
            {
                gamereset();
            }
            //Find the player possessing the ball
            if (ball.transform.parent)
            {
                //If a player a has the ball
                if (ball.transform.parent != ground.transform)
                {
                    TeamInPos = ball.transform.parent.tag;
                }
            }
            int i = 0;

        //Update the distance array of Team A
            foreach (GameObject player in TeamA)
            {
                TeamAposition[i] = player.transform.localPosition;
                Collider[] Overlap;
                Overlap = Physics.OverlapSphere(player.transform.localPosition, 10.0f, layerMask);
                foreach (Collider col in Overlap)
                {
                    if (player.tag != col.tag)
                        count++;
                }
                TeamAOverlap[i] = count * 0.1428f;
                i++;
            }
            i = 0;
            
        //Update the distance array of Team B
            foreach (GameObject player in TeamB)
            {
                TeamBposition[i] = player.transform.localPosition;
                Collider[] Overlap;
                Overlap = Physics.OverlapSphere(player.transform.localPosition, 10.0f, layerMask);
                foreach (Collider col in Overlap)
                {
                    if (player.tag != col.tag)
                        count++;
                }
                TeamBOverlap[i] = count * 0.1428f;
                i++;
            }

         //Find the player nearest to the ball 
            smallest = Mathf.Sqrt(36968);       //set smallest distance to the max distance possible on the ground ie the diagnol length of the ground
            GameObject nearby1 = null;
            GameObject nearby2 = null;

            //Find the player of Team A nearest to the ball 
            foreach (GameObject nearest in TeamA)
            {
                if (Vector3.Distance(nearest.transform.localPosition, ball.transform.localPosition) < smallest)
                {
                    nearby1 = nearest;
                    smallest = Vector3.Distance(nearest.transform.localPosition, ball.transform.localPosition);
                }
            }
            nearestTeamA = nearby1;
            smallest = Mathf.Sqrt(36968); //reset smallest 

            //Find the player of Team B nearest to the ball 
            foreach (GameObject nearest in TeamB)
            {
                if (Vector3.Distance(nearest.transform.localPosition, ball.transform.localPosition) < smallest)
                {
                    nearby2 = nearest;
                    smallest = Vector3.Distance(nearest.transform.localPosition, ball.transform.localPosition);
                }
            }
            nearestTeamB = nearby2;

    }

//Form the parameter Table for Passing algorithm 
    public GameObject FormTable(GameObject Ball_player, float Self_Overlap, float Ability, float Tactics)
    {
        int i = 0;
        int j = 0;
        bool flag = false;
        float distance;     
        float Overlapdiff;
        float Calc;
        float Vision;

        //If the player possessing the ball belongs to Team A
        if (Ball_player.tag=="Team A")
        {
            //Access the parameter Table for Team A
            foreach (Vector3 player_pos in TeamAposition)
            {
                //1. Find the difference of player density around every teammate and the person possessing the ball
                Overlapdiff = TeamAOverlap[i] - Self_Overlap;

                //2. Check if the path towards teammates is free or not 
                RaycastHit hit;
                if (!Physics.SphereCast(Ball_player.transform.localPosition,5.0f,player_pos-Ball_player.transform.localPosition,out hit))
                {
                    Vision = 1;
                }
                else
                {
                    Vision = 0;
                }

                //3. Find the distance between the player possessing the ball and his team mates
                distance = (Vector3.Distance(Ball_player.transform.localPosition, player_pos));
                distance /= (10 * Mathf.Pow(36896f, 0.5f));
                distance = ((1 / (1 + Mathf.Exp(Tactics / 30))) + distance*0.5f) / 2;
                distance = (1 / Mathf.Pow(0.5f, 4f)) * Mathf.Pow(Mathf.Abs(distance - 0.5f), 4f);

                //4. Substitute values in the Regression equation to find the result
                Calc = Ability * 0.01665971f + distance * (-4.37853368f) + Overlapdiff * (-9.61545001f) + Self_Overlap * (-0.43434114f) - 5.08785837f + Vision*1.59044574f;

                //5. Avoid Passing to Self
                if (TeamA[i].transform.name != Ball_player.transform.name)
                {
                    Pass_Prob[j] = Calc;
                    Pass_Team[j++] = TeamA[i];
                    flag = true;
                }
                i++;
            }
        }
        else
        {
            //If the player possessing the ball belongs to Team B access the parameter table for team  B
            foreach (Vector3 player_pos in TeamBposition)
            {
                //Same as 1.
                Overlapdiff = TeamBOverlap[i] - Self_Overlap;
                
                //Same as 2.
                RaycastHit hit;
                if (!Physics.SphereCast(Ball_player.transform.localPosition, 5.0f, player_pos - Ball_player.transform.localPosition, out hit))
                {
                    Vision = 1;
                }
                else
                {
                    Vision = 0;
                }

                //Same as 3.
                distance = (Vector3.Distance(Ball_player.transform.localPosition, player_pos));
                distance /= (10 * Mathf.Pow(36896f, 0.5f));
                distance = (1 / (1 + Mathf.Exp(Tactics / 30)) + distance) / 2;
                distance = (1 / Mathf.Pow(0.5f, 4f)) * Mathf.Pow(Mathf.Abs(distance - 0.5f), 4f);

                //Same as 4.
                Calc = Ability * 0.01665971f + distance * (-4.37853368f) + Overlapdiff * (-9.61545001f) + Self_Overlap * (-0.43434114f) - 5.08785837f + Vision*1.59044574f;

                //Same as 5.
                if (TeamB[i].transform.name != Ball_player.transform.name)
                {
                    Pass_Prob[j] = Calc;
                    Pass_Team[j++] = TeamB[i];
                    flag = true;
                }
                i++;
            }

        }

        //If found a player to pass
        if (flag)
        {
            //Find the best player and second best player to pass to  by choosing the player with largest result out of the regression equation
            float large = Pass_Prob[0];
            float seclarge = Pass_Prob[0];
            int index = 0,secindex=0;
            for(i=0;i<j;i++)
            {
                if (large < Pass_Prob[i])
                {
                    large = Pass_Prob[i];
                    index = i;
                }

            }
            if(seclarge==large)
            {
                seclarge = Pass_Prob[1];
                secindex = 1;
            }
            for (i = 0; i < j; i++)
            {
                if (i != index)
                {
                    if (seclarge < Pass_Prob[i])
                    {

                        seclarge = Pass_Prob[i];
                        secindex = i;
                    }

                }
            }
            NextPlayer = Pass_Team[secindex];   //Second Best player
            passingTo = Pass_Team[index];       //Best Player
            return (Pass_Team[index]);          //Return the player to which pass is to be given 

        }
        return (Ball_player);   //Return self
    }

    //Function to return the second best player to pass to 
    public GameObject nextPlayer()
    {
        passingTo = NextPlayer;
        return NextPlayer;
    }

    //Function to reset the pass variables
    public void reset()
    {
        passingTo = null;
        posessBall = null;
        PseudoPos = null;
        NextPlayer = null;
    }

    //If goalkeeper has the ball 
    void GoalKick()
    {
        if (ball.transform.parent.name == "Keeper")
        {
            goalKick = true;
        }
    }

    //Function to reset all the variables
    void gamereset()
    {
        passingTo = null;
        posessBall = null;
        PseudoPos = null;
        NextPlayer = null;
        ball.transform.parent = ground.transform;
        ball.transform.localPosition = Vector3.zero;
        gamepause = true;
    }
}
