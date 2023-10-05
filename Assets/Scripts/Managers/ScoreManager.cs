using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _increment = 10;
    private static int _totalScore;
    public int TotalScore
    {
        get { return _totalScore; }
        private set { _totalScore = value; }
    }


    void OnEnable()
    {
        PelletCollection.onPelletCollected += UpdateTotalScore;
    }

    void UpdateTotalScore()
    {
        _totalScore += _increment;
    }

    void OnDisable()
    {
        PelletCollection.onPelletCollected -= UpdateTotalScore;
    }
}
