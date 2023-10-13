using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    private int _newPlayerLife = 10000;
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

    [SerializeField] private PlayerLives _playerLives;



    void OnEnable()
    {
        ItemCollection.OnItemCollected += UpdateTotalScore;
    }

    void Start()
    {
        GenerateBonusItemsDictionary();
    }

    void GenerateBonusItemsDictionary()
    {
        BonusItemsDictionary["Pellet"] = 10;
        BonusItemsDictionary["Power Pellet"] = 50;
        BonusItemsDictionary["Enemy"] = 200;       // Every consecutive enemy to the maximum of 4 is doubled - 200, 400, 800, 1600. Make sure to add in a calculation
        BonusItemsDictionary["Cherry"] = 100;
        BonusItemsDictionary["Strawberry"] = 300;
        BonusItemsDictionary["Orange"] = 500;
        BonusItemsDictionary["Apple"] = 700;
        BonusItemsDictionary["Melon"] = 1000;
        BonusItemsDictionary["Ship"] = 2000;
        BonusItemsDictionary["Bell"] = 3000;
        BonusItemsDictionary["Key"] = 5000;
    }

    void UpdateTotalScore(int value)
    {
        TotalScore += value;

        if(TotalScore >= _newPlayerLife)
        {
            _playerLives.GainLife();
            UIManager.Instance.UpdateLivesDisplay();
        }
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= UpdateTotalScore;
    }
}
