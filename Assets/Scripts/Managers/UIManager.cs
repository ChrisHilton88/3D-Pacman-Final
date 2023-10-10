using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>   
{
    [SerializeField] private TextMeshProUGUI _totalPellets;
    [SerializeField] private TextMeshProUGUI _playerPellets;
    [SerializeField] private TextMeshProUGUI _totalScore;
    [SerializeField] private PelletManager _pelletManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private PlayerLives _playerLives;

    [SerializeField] private Image[] _playerLifeIcons;



    void OnEnable()
    {
        ItemCollection.OnItemCollected += UpdatePelletAndScoreDisplay;
        EnemyCollision.OnEnemyCollision += UpdateLivesDisplay;
    }

    void Start()
    {
        StartCoroutine(PlayerLivesDisplayRoutine());
        StartCoroutine(PelletDisplayRoutine());   
    }

    void UpdateLivesDisplay()
    {
        for (int i = 0; i < _playerLifeIcons.Length; i++)
        {
            // Access the Image array and activate/deactivate images
            _playerLifeIcons[i].gameObject.SetActive(i < _playerLives.CurrentPlayerLives);
            Debug.Log(_playerLifeIcons[i].gameObject.activeInHierarchy);
        }
    }

    void UpdatePelletAndScoreDisplay(int value)
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

    IEnumerator PlayerLivesDisplayRoutine()
    {
        yield return new WaitForEndOfFrame();
        UpdateLivesDisplay();
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
        ItemCollection.OnItemCollected -= UpdatePelletAndScoreDisplay;
        EnemyCollision.OnEnemyCollision -= UpdateLivesDisplay;
    }
}
