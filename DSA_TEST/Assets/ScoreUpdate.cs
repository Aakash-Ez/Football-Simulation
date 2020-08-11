using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUpdate : MonoBehaviour
{
    TextMeshProUGUI score;
    public GoalScript GoalsA;

    private void Start()
    {
        score = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        score.text = GoalsA.GetComponent<GoalScript>().TeamA.ToString();
    }
}
