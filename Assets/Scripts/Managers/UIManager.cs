using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>   
{
    [SerializeField] private TextMeshProUGUI _totalPellets;
    [SerializeField] private TextMeshProUGUI _totalScore;
    [SerializeField] private PelletManager _pelletManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private PlayerLives _playerLives;

    [SerializeField] private Image[] _playerLifeIcons;

    Coroutine _updatePelletAndScoreRoutine;
    Coroutine _updateLivesRoutine;
    Coroutine _updateNextLevel;


    void OnEnable()
    {
        ItemCollection.OnItemCollected += UpdatePelletAndScoreDisplay;
        EnemyCollision.OnEnemyCollision += UpdateLivesDisplay;
        RoundManager.OnRoundEnd += UpdateNextLevel;
    }

    void Start()
    {
        _updatePelletAndScoreRoutine = null;
        _updateLivesRoutine = null;
        _updateNextLevel = null;    

        if (_updateLivesRoutine == null)
            _updateLivesRoutine = StartCoroutine(PlayerLivesDisplayRoutine());
        else
            return;

        StartCoroutine(PelletDisplayRoutine());
    }

    public void UpdateLivesDisplay()
    {
        StartCoroutine(PlayerLivesDisplayRoutine());
    }

    void UpdatePelletAndScoreDisplay(int value)
    {
        if(_updatePelletAndScoreRoutine == null && value == ScoreManager.Instance.BonusItemsDictionary["Pellet"])
        {
            _updatePelletAndScoreRoutine = StartCoroutine(PelletDisplayRoutine());
        }
        else
        {
            _updatePelletAndScoreRoutine = StartCoroutine(BonusItemsDisplayRoutine()); 
        }
    }

    void UpdateNextLevel()
    {
        if(_updateNextLevel == null)
            _updateNextLevel = StartCoroutine(NextLevelRoutine());
    }

    IEnumerator PlayerLivesDisplayRoutine()
    {
        yield return null;

        for (int i = 0; i < _playerLifeIcons.Length; i++)       // This will always run 4 times each call
        {
            _playerLifeIcons[i].gameObject.SetActive(i < _playerLives.CurrentPlayerLives);      // element 0 gameobject setactive(0 < 3)
        }

        _updateLivesRoutine = null;
    }

    // There is a delay in showing values so it needs to be updated at the end of each frame
    // Handles updating Pellets
    IEnumerator PelletDisplayRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
        _updatePelletAndScoreRoutine = null;
    }

    // Handles only updating the Total Score with the Bonus Items
    IEnumerator BonusItemsDisplayRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
        _updatePelletAndScoreRoutine = null;
    }

    // Resets the values on the UI
    IEnumerator NextLevelRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();
        _updateNextLevel = null;
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= UpdatePelletAndScoreDisplay;
        EnemyCollision.OnEnemyCollision -= UpdateLivesDisplay;
        RoundManager.OnRoundEnd -= UpdateNextLevel;
    }
}
