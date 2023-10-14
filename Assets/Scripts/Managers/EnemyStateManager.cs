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

    private bool frightenedStateEventOccurred = false;

    Coroutine _cycleCoroutine;
    WaitForSeconds _fiveSeconds = new WaitForSeconds(5);        // Cache for optimisation
    WaitForSeconds _sevenSeconds = new WaitForSeconds(7);
    WaitForSeconds _twentySeconds = new WaitForSeconds(20);

    [SerializeField] private Transform[] _frightenedPositions = new Transform[16];

    public static Action OnNewState;        // Responsible for the timing of when enemies should change states. Enemies should subscribe

    #region Properties
    public int CycleCount { get { return _cycleCount; } private set { _cycleCount = value; } }      
    public float CycleTimer {  get { return _cycleTimer; } private set {  _cycleTimer = value; } }
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
        CycleTimer = Time.time;
        Debug.Log("Cycle Time: " + CycleTimer);
        _cycleCoroutine = null;
        if (_cycleCoroutine == null)
            _cycleCoroutine = StartCoroutine(CycleTimerRoutine());
    }

    // Generates a random number to be used as an element reference in an array
    public int RandomNumber()
    {  
        _randomNumber = UnityEngine.Random.Range(_minNumber, _frightenedPositions.Length);
        return _randomNumber;
    }

    // Needs a parameter for a timer and we need to source this information from a stored table or something
    IEnumerator CycleTimerRoutine()
    {
        while (CycleCount <= _maxCycles && frightenedStateEventOccurred == false)        
        {
            switch (CycleCount)
            {
                case 1:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Change State 1");
                    Debug.Log("Time: " + Time.time);
                    break;
                case 2:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 2");
                    Debug.Log("Time: " + Time.time);
                    break;
                case 3:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Change State 3");
                    Debug.Log("Time: " + Time.time);
                    break;
                case 4:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 4");
                    Debug.Log("Time: " + Time.time);
                    break;
                case 5:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Change State 5");
                    Debug.Log("Time: " + Time.time);
                    break;
                case 6:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Change State 6");
                    Debug.Log("Time: " + Time.time);
                    break;
                case 7:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Change State 7");
                    Debug.Log("Time: " + Time.time);
                    break;
                default:
                    Debug.Log("Incorrect Cycle Count in CycleTimerRoutine() - EnemyStateManager");
                    break;
            }

            OnNewState?.Invoke();       // Trigger event for all enemies
        }

        _cycleCoroutine = null;     // Set to null so we can run again
    }

    #region Events
    // Event that handles increasing the CycleCount
    void CycleIncrement()
    {
        CycleCount++;
    }

    void FrightenedState()
    {
        Debug.Log("Frightened State occurred at: " + Time.time.ToString());
        frightenedStateEventOccurred = true;    
    }

    // Event that handles resetting the CycleCount back to 1 on New Round
    void NewRoundStart()
    {
        StopCoroutine(_cycleCoroutine);
        CycleCount = 1;
        _cycleCoroutine = null;
        if (_cycleCoroutine == null)
            _cycleCoroutine = StartCoroutine(CycleTimerRoutine());
    }
    #endregion
}
