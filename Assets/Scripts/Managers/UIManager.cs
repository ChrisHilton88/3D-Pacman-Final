using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>   
{
    [SerializeField] private TextMeshProUGUI _totalPellets;
    [SerializeField] private TextMeshProUGUI _playerPellets;
    [SerializeField] private TextMeshProUGUI _totalScore;
    [SerializeField] private PelletManager _pelletManager;
    [SerializeField] private ScoreManager _scoreManager;


    void OnEnable()
    {
        ItemCollection.onItemCollected += UpdateUIDisplay;
    }

    void Start()
    {
        StartCoroutine(InitialiseUIValues());   
    }

    void UpdateUIDisplay(int value)
    {
        // Check if it is a Pellet through the value parameter
        // If it is run this
        if (value == 10)     // meaning a "Pellet"
        {
            StartCoroutine(PelletDisplayRoutine());
        }
        else
        {
            StartCoroutine(BonusItemDisplayeRoutine());
        }
    }

    // There was a delay in loading initial values so it needs to be one of the last things
    IEnumerator InitialiseUIValues()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();        
        _playerPellets.text = "Player Pellets: " + _pelletManager.PlayerPellets.ToString();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
    }

    // Update display if the player collects a Pellet
    IEnumerator PelletDisplayRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();
        _playerPellets.text = "Player Pellets: " + _pelletManager.PlayerPellets.ToString();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
        Debug.Log("Updating Pellet Display & Total Score ONLY");
    }

    // Doesn't update Pellets as Bonus Items do not count towards the Total Pellets
    IEnumerator BonusItemDisplayeRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
        Debug.Log("Updating Total Score Display ONLY");
    }

    void OnDisable()
    {
        ItemCollection.onItemCollected -= UpdateUIDisplay;
    }
}
