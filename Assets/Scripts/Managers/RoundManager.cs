using System;
using UnityEngine;

public class RoundManager : MonoSingleton<RoundManager>
{
    private int _currentRound;
    public int CurrentRound
    {
        get { return _currentRound; }
        private set { _currentRound = value; }
    }
    private int _maxRounds = 22;

    private RoundData[] _levels = new RoundData[21];
    public RoundData[] Levels
    {
        get { return _levels; }
        set { _levels = value; }
    }

    // Event for the NextLevel after player collects all 240 pellets - This
    // Event for RestartLevel after the Player gets hit by an enemy - EnemyCollision
    // Event for GameOver


    public static Action OnRoundEnd;
    public static Action OnRoundStart;


    void OnEnable()
    {
        OnRoundEnd += IncrementRound;
    }

    void Start()
    {
        _currentRound = 1;  
        SetInitialLevelValues();
    }

    // Only needs to be set once at the start of the game
    RoundData[] SetInitialLevelValues()
    {
        new RoundData(1, GetBonusItemValue("Cherry"), 6);
        new RoundData(2, GetBonusItemValue("StrawBerry"), 5);
        new RoundData(3, GetBonusItemValue("Orange"), 4);
        new RoundData(4, GetBonusItemValue("Orange"), 3);
        new RoundData(5, GetBonusItemValue("Apple"), 2);
        new RoundData(6, GetBonusItemValue("Apple"), 5);
        new RoundData(7, GetBonusItemValue("Melon"), 2);
        new RoundData(8, GetBonusItemValue("Melon"), 2);
        new RoundData(9, GetBonusItemValue("Ship"), 1);
        new RoundData(10, GetBonusItemValue("Ship"), 5);
        new RoundData(11, GetBonusItemValue("Bell"), 2);
        new RoundData(12, GetBonusItemValue("Bell"), 1);
        new RoundData(13, GetBonusItemValue("Key"), 1);
        new RoundData(14, GetBonusItemValue("Key"), 3);
        new RoundData(15, GetBonusItemValue("Key"), 1);
        new RoundData(16, GetBonusItemValue("Key"), 1);
        new RoundData(17, GetBonusItemValue("Key"), 0);
        new RoundData(18, GetBonusItemValue("Key"), 1);
        new RoundData(19, GetBonusItemValue("Key"), 0);
        new RoundData(20, GetBonusItemValue("Key"), 0);
        new RoundData(21, GetBonusItemValue("Key"), 0);

        return _levels;
    }

    string GetBonusItemValue(string itemName)
    {
        if (ScoreManager.Instance.BonusItemsDictionary.ContainsKey(itemName))
        {
            return ScoreManager.Instance.BonusItemsDictionary[itemName].ToString();
        }
        else
        {
            return "Item not found";
        }
    }

    // When player collects all pellets in a level - Go to the next level
    public void NextLevel()
    {
        OnRoundEnd?.Invoke();
    }

    // Called when the player collects all pellets in a level
    public void IncrementRound()
    {
        if (CurrentRound >= _maxRounds)
        {
            CurrentRound = 1;
            Debug.Log("Current Round: " + CurrentRound);
        }
        else
        {
            CurrentRound++;
            Debug.Log("Current Round: " + CurrentRound);
        }
    }

    public RoundData CheckRound()
    {
        RoundData currentRoundData = null;

        foreach (var roundData in _levels)
        {
            if (roundData.round == CurrentRound)
            {
                currentRoundData = roundData;
                break;      // Exits loop once matching round data is found 
            }
        }

        return currentRoundData;    
    }

    void OnDisable()
    {
        OnRoundEnd -= IncrementRound;
    }
}
