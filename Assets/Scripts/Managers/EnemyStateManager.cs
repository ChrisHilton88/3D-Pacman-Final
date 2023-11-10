using System;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoSingleton<EnemyStateManager>
{
    private int _randomNumber;
    private int _minNumber = 0;
    [SerializeField] private int _cycleCount;        // Incrementor for cycle count

    private float _cycleTimer;
    private float _startTime;
    [SerializeField] private float _enemyCycleTimer;

    private bool _frightenedStateActive = false;        // Bool to check if state is active while eating enemies
    private bool _hasChangedState = false;

    [SerializeField] private EnemyStateRoundTimer[] _enemyStateRoundTimer;
    [SerializeField] private EnemyStateRoundTimer _currentSO;
    [SerializeField] private Transform[] _frightenedPositions = new Transform[16];

    public static Action OnNewState;        // Responsible for the timing of when enemies should change states. Enemies should subscribe

    #region Properties
    public int CycleCount { get { return _cycleCount; } private set { _cycleCount = value; } }
    public float CycleTimer { get { return _cycleTimer; } private set { _cycleTimer = value; } }
    public float EnemyCycleTimer { get { return _enemyCycleTimer; } private set { _enemyCycleTimer = value; } }
    public float StartTime { get { return _startTime; } private set { _startTime = value; } }   
    public bool FrightenedStateActive { get { return _frightenedStateActive; } private set { _frightenedStateActive = value; } }
    public bool HasChangedState { get { return _hasChangedState; } private set { _hasChangedState = value; } }
    public Transform[] FrightenedPositions { get { return _frightenedPositions; } private set { _frightenedPositions = value; } }
    #endregion



    private void OnEnable()
    {
        ItemCollection.OnFrightened += FrightenedStateOn;
        OnNewState += CycleIncrement;
        RoundManager.OnRoundEnd += NewRoundStart;
    }
    private void OnDisable()
    {
        ItemCollection.OnFrightened -= FrightenedStateOn;
        OnNewState -= CycleIncrement;
        RoundManager.OnRoundEnd -= NewRoundStart;
    }

    private void Start()
    {
        ScriptableObjectHandler();
        CycleTimer = 0;     // Start timer at 0 at start of game
        EnemyCycleTimer = 0;     // Start timer at 0 at start of game
        CycleCount = 0;
        StartTime = Time.time;
    }

    private void Update()
    {
        CycleTimer += Time.deltaTime;
        Debug.Log("Bool: " + HasChangedState);

        if(CycleTimer > EnemyCycleTimer && HasChangedState == false)     
        {
            Debug.LogError("Cycle Timer: " + CycleTimer);
            Debug.LogError("EnemyCycleTimer: " + EnemyCycleTimer);
            Debug.LogError("State: " + HasChangedState);
            HasChangedState = true;
            EnemyCycleTimer += EnemyStateTimer();       
        }    
    }

    // Handles deciding which timer to use depending on the round
    private void ScriptableObjectHandler()
    {
        switch (RoundManager.Instance.CurrentRound)     // Checks current round
        {
            case 1:
                _currentSO = _enemyStateRoundTimer[0];       // Assigns appropriate enemy state round SO data
                break;
            case int round when round >= 2 && round <= 4:
                _currentSO = _enemyStateRoundTimer[1];
                break;
            case int round when round >= 5 && round <= 21:
                _currentSO = _enemyStateRoundTimer[2];
                break;
            default:
                break;
        }
    }

    float EnemyStateTimer()
    {
        float newTime = 0;

        Debug.LogError("CurrentSO: " + _currentSO);
        Debug.LogError("CycleCount: " + CycleCount);
        Debug.LogError("Length: " + _currentSO.stateTimers.Length);

        if (_currentSO != null && CycleCount >= 0 && CycleCount < _currentSO.stateTimers.Length)
        {
            newTime = _currentSO.stateTimers[CycleCount];      
            OnNewState?.Invoke();
            HasChangedState = false;
        }
        else
        {
            Debug.Log("CycleCount is out of bounds or _currentSO is null");
            newTime = 0;
        }

        return newTime;
    }

    // Generates a random number to be used as an element reference in an array
    public int RandomNumber()
    {  
        _randomNumber = UnityEngine.Random.Range(_minNumber, _frightenedPositions.Length);
        return _randomNumber;
    }

    #region Events
    // Event that handles increasing the CycleCount
    void CycleIncrement()
    {
        CycleCount++;
    }

    // TODO: Update this to a bool switch
    void FrightenedStateOn()
    {
        FrightenedStateActive = true;    
    }

    void NewRoundStart()
    {
        StartCoroutine(TestRoutine());
    }
    #endregion

    IEnumerator TestRoutine()
    {
        ScriptableObjectHandler();
        yield return null;
        CycleTimer = 0;
        EnemyCycleTimer = 0;
        CycleCount = 0;
        HasChangedState = false;
        FrightenedStateActive = false;
    }
}
