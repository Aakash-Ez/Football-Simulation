using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUpdateB : MonoBehaviour
{
    TextMeshProUGUI score;
    public GoalScript GoalsB;

    private void Start()
    {
        score = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        score.text = GoalsB.GetComponent<GoalScript>().TeamB.ToString();
    }
}
