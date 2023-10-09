using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    private int _increment = 10;
    private static int _totalScore;
    public int TotalScore
    {
        get { return _totalScore; }
        private set { _totalScore = value; }
    }

    private Dictionary<string, int> _bonusItemsDictionary = new Dictionary<string, int>();   
    public Dictionary<string, int> BonusItemsDictionary
    {
        get { return _bonusItemsDictionary; }
        private set { _bonusItemsDictionary = value; }
    }

    void OnEnable()
    {
        ItemCollection.onItemCollected += UpdateTotalScore;
    }

    void Start()
    {
        GenerateBonusItemsDictionary();
    }

    void GenerateBonusItemsDictionary()
    {
        _bonusItemsDictionary["Pellet"] = 10;
        _bonusItemsDictionary["Power Pellet"] = 50;
        _bonusItemsDictionary["Enemy"] = 200;       // Every consecutive enemy to the maximum of 4 is doubled - 200, 400, 800, 1600. Make sure to add in a calculation
        _bonusItemsDictionary["Cherry"] = 100;
        _bonusItemsDictionary["Strawberry"] = 300;
        _bonusItemsDictionary["Orange"] = 500;
        _bonusItemsDictionary["Apple"] = 700;
        _bonusItemsDictionary["Melon"] = 1000;
        _bonusItemsDictionary["Ship"] = 2000;
        _bonusItemsDictionary["Bell"] = 3000;
        _bonusItemsDictionary["Key"] = 5000;
    }

    void UpdateTotalScore(int value)
    {
        TotalScore += value;
    }

    void OnDisable()
    {
        ItemCollection.onItemCollected -= UpdateTotalScore;
    }
}
