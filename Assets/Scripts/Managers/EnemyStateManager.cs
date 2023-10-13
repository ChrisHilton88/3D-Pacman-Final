using System;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    private int _cycleCount;        // Incrementor for cycle count
    private int _maxCycles = 8;     // Maximum cycles per level

    private float _cycleTimer;
    private float _startTime;
    float realTimeElapsed;

    Coroutine _cycleCoroutine;
    WaitForSeconds _fiveSeconds = new WaitForSeconds(5);
    WaitForSeconds _sevenSeconds = new WaitForSeconds(7);
    WaitForSeconds _twentySeconds = new WaitForSeconds(20);

    #region Properties
    public int CycleCount { get { return _cycleCount; } private set { _cycleCount = value; } }      
    public float CycleTimer {  get { return _cycleTimer; } private set {  _cycleTimer = value; } }
    #endregion

    public static Action OnNewState;        // Responsible for the timing of when enemies should change states. Enemies should subscribe


    // New round needs to be called at the start of each round
    // Enemies need to be subscribed to OnNewState Action
    // From time since start of game to seven seconds = Scatter mode 
    // At 7 seconds, since game start - Call event and change state to Chase (opposite of what they are in)
    // Wait another 20 seconds - Call event again

    void OnEnable()
    {
        OnNewState += CycleIncrement;
        RoundManager.OnRoundStart += NewRoundStart;
    }

    void Start()
    {
        CycleCount = 1;
        _startTime = Time.time;
        _cycleCoroutine = null;
        if (_cycleCoroutine == null)
            _cycleCoroutine = StartCoroutine(CycleTimerRoutine());
    }

    void Update()
    {
        realTimeElapsed = Time.time - _startTime;      
    }

    

    // Need to know what round it is so we can pass in timer variables
    void NewRoundStart()
    {
        CycleCount = 1;
        _cycleCoroutine = null;
        if (_cycleCoroutine == null)
            _cycleCoroutine = StartCoroutine(CycleTimerRoutine());
    }

    // Needs a parameter for a timer and we need to source this information from a stored table or something
    IEnumerator CycleTimerRoutine()
    {
        while (CycleCount <= _maxCycles) 
        {
            switch (CycleCount)
            {
                case 1:
                    Debug.Log(CycleCount);
                    yield return _sevenSeconds;
                    break;
                case 2:
                    Debug.Log(CycleCount);

                    yield return _twentySeconds;
                    break;
                case 3:
                    Debug.Log(CycleCount);

                    yield return _sevenSeconds;
                    break;
                case 4:
                    Debug.Log(CycleCount);

                    yield return _twentySeconds;
                    break;
                case 5:
                    Debug.Log(CycleCount);

                    yield return _fiveSeconds;
                    break;
                case 6:
                    yield return _twentySeconds;
                    break;
                case 7:
                    yield return _fiveSeconds;
                    break;
                case 8: 
                    yield return _twentySeconds;
                    break;
                default:
                    Debug.Log("Incorrect Cycle Count in switch statement");
                    break;
            }

            OnNewState?.Invoke();
        }

        _cycleCoroutine = null;     // Set cache to null so we can run again
    }

    #region Events
    void CycleIncrement()
    {
        CycleCount++;
    }
    #endregion

    void OnDisable()
    {
        OnNewState -= CycleIncrement;
        RoundManager.OnRoundStart -= NewRoundStart;
    }
}
