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
        ItemCollection.OnItemCollected += UpdateUIDisplay;
    }

    void Start()
    {

        StartCoroutine(PelletDisplayRoutine());   
    }

    void UpdateUIDisplay(int value)
    {
        if(value == ScoreManager.Instance.BonusItemsDictionary["Pellet"])
        {
            StartCoroutine(PelletDisplayRoutine());
        }
        else
        {
            StartCoroutine(BonusItemsDisplayRoutine()); 
        }
    }

    // There is a delay in showing values so it needs to be updated at the end of each frame
    // Handles updating Pellets
    IEnumerator PelletDisplayRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();
        _playerPellets.text = "Player Pellets: " + _pelletManager.PlayerPellets.ToString();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
    }

    // Handles only updating the Total Score with the Bonus Items
    IEnumerator BonusItemsDisplayRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= UpdateUIDisplay;
    }
}
