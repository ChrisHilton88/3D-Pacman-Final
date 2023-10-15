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
        CurrentRound = 1;  
        SetInitialLevelValues();
    }

    // Only needs to be set once at the start of the game
    RoundData[] SetInitialLevelValues()
    {
        Levels[0] = new RoundData(1, "Cherry", 6);
        Levels[1] = new RoundData(2, "Strawberry", 5);
        Levels[2] = new RoundData(3, "Orange", 4);
        Levels[3] = new RoundData(4, "Orange", 3);
        Levels[4] = new RoundData(5, "Apple", 2);
        Levels[5] = new RoundData(6, "Apple", 5);
        Levels[6] = new RoundData(7, "Melon", 2);
        Levels[7] = new RoundData(8, "Melon", 2);
        Levels[8] = new RoundData(9, "Ship", 1);
        Levels[9] = new RoundData(10, "Ship", 5);
        Levels[10] = new RoundData(11, "Bell", 2);
        Levels[11] = new RoundData(12, "Bell", 1);
        Levels[12] = new RoundData(13, "Key", 1);
        Levels[13] = new RoundData(14, "Key", 3);
        Levels[14] = new RoundData(15, "Key", 1);
        Levels[15] = new RoundData(16, "Key", 1);
        Levels[16] = new RoundData(17, "Key", 0);
        Levels[17] = new RoundData(18, "Key", 1);
        Levels[18] = new RoundData(19, "Key", 0);
        Levels[19] = new RoundData(20, "Key", 0);
        Levels[20] = new RoundData(21, "Key", 0);

        return _levels;
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

        foreach (RoundData roundData in Levels)
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
