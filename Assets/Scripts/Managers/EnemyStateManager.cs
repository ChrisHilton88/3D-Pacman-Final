using System;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoSingleton<EnemyStateManager>   
{
    private int _cycleCount;        // Incrementor for cycle count
    private int _maxCycles = 7;     // Maximum cycles per level
    private int _randomNumber;
    private int _minNumber = 0;

    private float _cycleTimer;

    private bool _frightenedStateActive = false;

    Coroutine _cycleRoutine;
    Coroutine _newRoundStartRoutine;
    WaitForSeconds _fiveSeconds = new WaitForSeconds(5);        // Cache for optimisation
    WaitForSeconds _sevenSeconds = new WaitForSeconds(7);
    WaitForSeconds _twentySeconds = new WaitForSeconds(20);

    [SerializeField] private Transform[] _frightenedPositions = new Transform[16];

    public static Action OnNewState;        // Responsible for the timing of when enemies should change states. Enemies should subscribe

    #region Properties
    public int CycleCount { get { return _cycleCount; } private set { _cycleCount = value; } }      
    public float startTime {  get { return _cycleTimer; } private set {  _cycleTimer = value; } }
    public bool FrightenedStateActive { get { return _frightenedStateActive; } private set { _frightenedStateActive= value; } } 
    public Transform[] FrightenedPositions { get { return _frightenedPositions;} private set {  _frightenedPositions = value; } }       
    #endregion



    void OnEnable()
    {
        ItemCollection.OnFrightened += FrightenedState;
        OnNewState += CycleIncrement;
        RoundManager.OnRoundEnd += NewRoundStart;
    }
    void OnDisable()
    {
        ItemCollection.OnFrightened -= FrightenedState;
        OnNewState -= CycleIncrement;
        RoundManager.OnRoundEnd -= NewRoundStart;
    }

    void Start()
    {
        CycleCount = 1;
        startTime = Time.time;
        _cycleRoutine = null;
        _newRoundStartRoutine = null;
        if (_cycleRoutine == null)
            _cycleRoutine = StartCoroutine(CycleTimerRoutine());
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

    void FrightenedState()
    {
        _frightenedStateActive = true;  
        // Need to find a way to be told that the coroutine has finished so that we can set the state back to false.
        // At the same time we need to pause the other coroutine and have it resume when this timer finishes.
    }

    // Event that handles resetting the CycleCount back to 1 on New Round
    void NewRoundStart()
    {
        if (_newRoundStartRoutine == null)
            _newRoundStartRoutine = StartCoroutine(NewRoundStartDelayRoutine());
        else
            Debug.Log("_newRoundStartRoutine is NOT NULL - EnemyStateManager");
    }
    #endregion

    // Chase & Scatter States
    IEnumerator CycleTimerRoutine()
    {
        while (CycleCount <= _maxCycles && _frightenedStateActive == false)
        {
            switch (CycleCount)
            {
                case 1:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Change State 1 (ToChase): " + Time.time);
                    Debug.Log("Frightened State: " + _frightenedStateActive);
                    break;
                case 2:
                    yield return _twentySeconds;    // ToScatter            // Stuck in here after collecting Power Pellet in Chase state
                    Debug.Log("Change State 2 (ToScatter): " + Time.time);
                    Debug.Log("Frightened State: " + _frightenedStateActive);
                    break;
                case 3:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Change State 3 (ToChase): " + Time.time);
                    break;
                case 4:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 4 (ToScatter): " + Time.time);
                    break;
                case 5:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Change State 5 (ToChase): " + Time.time);
                    break;
                case 6:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 6 (ToScatter): " + Time.time);
                    break;
                case 7:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Change State 7 (ToChase): " + Time.time);
                    break;
                default:
                    Debug.Log("Incorrect Cycle Count in CycleTimerRoutine() - EnemyStateManager");
                    break;
            }

            yield return new WaitUntil(() => !_frightenedStateActive);
            OnNewState?.Invoke();       // Trigger event for all enemies
        }

        _cycleRoutine = null;     // Set to null so we can run again
    }

    IEnumerator NewRoundStartDelayRoutine()
    {
        yield return null;

        CycleCount = 1;

        if (_cycleRoutine != null)
        {
            StopCoroutine(_cycleRoutine);
            _cycleRoutine = null;
        }
        else
            Debug.Log("_cycleCoroutine is NULL - EnemyStateManager");

        if (_cycleRoutine == null)
        {
            _cycleRoutine = StartCoroutine(CycleTimerRoutine());
            Debug.Log("Starting new Coroutine");
        }
        else
            Debug.Log("_cycleCoroutine is NOT NULL - EnemyStateManager");

        _newRoundStartRoutine = null;
    }
}
