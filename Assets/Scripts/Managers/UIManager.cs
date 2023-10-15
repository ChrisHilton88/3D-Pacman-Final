using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>   
{
    private int _currentBonusItemSlotIndex = 0;

    Coroutine _updatePelletAndScoreRoutine;
    Coroutine _updateLivesRoutine;
    Coroutine _updateNextLevelRoutine;
    Coroutine _gameOverFlickerRoutine;

    [SerializeField] private TextMeshProUGUI _gameOver;
    [SerializeField] private TextMeshProUGUI _totalPellets;
    [SerializeField] private TextMeshProUGUI _totalScore;
    [SerializeField] private PelletManager _pelletManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private PlayerLives _playerLives;
    [SerializeField] private BonusItemDisplay _bonusItemDisplay;

    [SerializeField] private Image[] _playerLifeIcons;
    [SerializeField] private Image[] _bonusItemIcons;
    [SerializeField] private Sprite[] _bonusItemSprites;



    void OnEnable()
    {
        ItemCollection.OnItemCollected += UpdatePelletAndScoreDisplay;
        EnemyCollision.OnEnemyCollision += UpdateLivesDisplay;
        EnemyCollision.OnEnemyCollision += GameOverFlicker;
        RoundManager.OnRoundEnd += UpdateNextLevel;
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= UpdatePelletAndScoreDisplay;
        EnemyCollision.OnEnemyCollision -= UpdateLivesDisplay;
        EnemyCollision.OnEnemyCollision -= GameOverFlicker;
        RoundManager.OnRoundEnd -= UpdateNextLevel;
    }

    void Start()
    {
        _gameOver.gameObject.SetActive(false);  
        _updatePelletAndScoreRoutine = null;
        _updateLivesRoutine = null;
        _updateNextLevelRoutine = null;
        _gameOverFlickerRoutine = null;

        if (_updateLivesRoutine == null)
            _updateLivesRoutine = StartCoroutine(PlayerLivesDisplayRoutine());
        else
            Debug.LogWarning("Coroutine not NULL for PlayerLivesDisplayRoutine");

        StartCoroutine(PelletScoreRoutine());
    }

    public void UpdateLivesDisplay()
    {
        StartCoroutine(PlayerLivesDisplayRoutine());
    }

    void UpdatePelletAndScoreDisplay(int value)
    {
        if(_updatePelletAndScoreRoutine == null && value == ScoreManager.Instance.BonusItemsDictionary["Pellet"])
        {
            _updatePelletAndScoreRoutine = StartCoroutine(PelletScoreRoutine());
        }
        else
        {
            _updatePelletAndScoreRoutine = StartCoroutine(BonusItemsScoreRoutine()); 
        }
    }

    void UpdateNextLevel()
    {
        if(_updateNextLevelRoutine == null)
            _updateNextLevelRoutine = StartCoroutine(NextLevelRoutine());
    }

    public void AddCollectedBonusItem(string tagname)
    {
        if (_bonusItemDisplay.BonusItemDictionary.ContainsKey(tagname))     // Check if the Dictionary has the tag name
        {
            int slotIndex = _bonusItemDisplay.BonusItemDictionary[tagname];     // Pass in the matching value to the key

            if (_currentBonusItemSlotIndex < _bonusItemSprites.Length)      // Check that the currentIndex incrementor is less than the lenght of the array
            {
                _bonusItemIcons[_currentBonusItemSlotIndex].sprite = _bonusItemSprites[slotIndex];        // Assign sprite from the array using the slotIndex value

                _currentBonusItemSlotIndex++;       // Move to the next slot for the next collected BonusItem
            }
        }
        else
        {
            Debug.LogWarning("Tag name doesn't match in BonusItemDisplay Dictionary - UIManager");
        }
    }

    void GameOverFlicker()
    {
        if (_gameOverFlickerRoutine == null)
            _gameOverFlickerRoutine = StartCoroutine(GameOverFlickerRoutine());
        else
            Debug.Log(_gameOverFlickerRoutine.ToString() + " is not equal to NULL - UIManager");
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
    IEnumerator PelletScoreRoutine()
    {
        yield return new WaitForEndOfFrame();
        _totalPellets.text = "Remaining Pellets: " + _pelletManager.TotalPellets.ToString();
        _totalScore.text = "Total Score: " + _scoreManager.TotalScore.ToString();
        _updatePelletAndScoreRoutine = null;
    }

    // Handles only updating the Total Score with the Bonus Items
    IEnumerator BonusItemsScoreRoutine()
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
        _updateNextLevelRoutine = null;
    }

    IEnumerator GameOverFlickerRoutine()
    {
        Debug.Log("Testing UIManager");
        yield return null;  

        while (GameManager.Instance.IsGameOver)
        {
            Debug.Log("Testing 2");
            yield return new WaitForSecondsRealtime(0.5f);
            _gameOver.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);
            _gameOver.gameObject.SetActive(false);
        }

        _gameOverFlickerRoutine = null;
        Debug.Log("Exiting");
    }
}
