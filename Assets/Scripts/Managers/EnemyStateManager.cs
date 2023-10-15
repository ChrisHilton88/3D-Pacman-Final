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

    private bool _frightenedState = false;

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
    public bool FrightenedState { get { return _frightenedState; } private set { _frightenedState = value; } }  
    public Transform[] FrightenedPositions { get { return _frightenedPositions;} private set {  _frightenedPositions = value; } }       
    #endregion



    void OnEnable()
    {
        ItemCollection.OnFrightened += FrightenedStateOn;
        OnNewState += CycleIncrement;
        RoundManager.OnRoundEnd += NewRoundStart;
    }
    void OnDisable()
    {
        ItemCollection.OnFrightened -= FrightenedStateOn;
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

    // Needs a parameter for a timer and we need to source this information from a stored table or something
    

    #region Events
    // Event that handles increasing the CycleCount
    void CycleIncrement()
    {
        CycleCount++;
    }

    void FrightenedStateOn()
    {
        _frightenedState = true;    
    }

    public void FrightenedStateOff()
    {
        _frightenedState = false;
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

    IEnumerator CycleTimerRoutine()
    {
        while (CycleCount <= _maxCycles && _frightenedState == false)
        {
            switch (CycleCount)
            {
                case 1:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Change State 1");
                    break;
                case 2:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 2");
                    break;
                case 3:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Change State 3");
                    break;
                case 4:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 4");
                    break;
                case 5:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Change State 5");
                    break;
                case 6:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 6");
                    break;
                case 7:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Change State 7");
                    break;
                default:
                    Debug.Log("Incorrect Cycle Count in CycleTimerRoutine() - EnemyStateManager");
                    break;
            }

            yield return new WaitUntil(() => !_frightenedState);
            OnNewState?.Invoke(); 
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
            Debug.Log("Starting new routine");
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
