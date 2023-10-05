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
        PelletCollection.onPelletCollected += UpdateTotalPelletDisplay;
    }

    void Start()
    {
        StartCoroutine(InitialiseUIValues());   
    }

    void UpdateTotalPelletDisplay()
    {
        StartCoroutine(PelletDisplayRoutine()); 
    }

    // There was a delay in loading initial values so it needs to be one of the last things
    IEnumerator InitialiseUIValues()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();        
        _playerPellets.text = "Player Pellets: " + _pelletManager.PlayerPellets.ToString();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
    }

    IEnumerator PelletDisplayRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();
        _playerPellets.text = "Player Pellets: " + _pelletManager.PlayerPellets.ToString();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
    }

    void OnDisable()
    {
        PelletCollection.onPelletCollected -= UpdateTotalPelletDisplay;
    }
}
