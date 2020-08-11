using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public int TeamA;
    public int TeamB;
    public GameObject Brain;
    Passing brain;

    private void Start()
    {
        brain = Brain.GetComponent<Passing>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Sphere")
        {
            brain.goalscored = true;
            if(transform.tag=="Team A")
            {
                TeamB++;
            }
            else
            {
                TeamA++;
            }
        }

    }
}
