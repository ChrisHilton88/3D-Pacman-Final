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
        RoundManager.OnStartRound += NewRoundStart;
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
    // Let's cache 2 lots of WaitForSeconds
    IEnumerator CycleTimerRoutine()
    {
        while (CycleCount <= _maxCycles) 
        {
            yield return _sevenSeconds;
            Debug.Log("Realtime elapsed: " + realTimeElapsed);        // Should be = 7
            CycleCount++;
            OnNewState?.Invoke();

            yield return _twentySeconds;
            Debug.Log("Realtime elapsed: " + realTimeElapsed);        // Should be = 27
            CycleCount++;
            OnNewState?.Invoke();

            yield return _sevenSeconds;
            Debug.Log("Realtime elapsed: " + realTimeElapsed);        // Should be = 34
            CycleCount++;
            OnNewState?.Invoke();

            yield return _twentySeconds;
            Debug.Log("Realtime elapsed: " + realTimeElapsed);        // Should be = 54
            CycleCount++;
            OnNewState?.Invoke();

            yield return _fiveSeconds;
            Debug.Log("Realtime elapsed: " + realTimeElapsed);        // Should be = 59
            CycleCount++;
            OnNewState?.Invoke();

            yield return _twentySeconds;
            Debug.Log("Realtime elapsed: " + realTimeElapsed);        // Should be = 79
            CycleCount++;
            OnNewState?.Invoke();

            yield return _fiveSeconds;
            Debug.Log("Realtime elapsed: " + realTimeElapsed);        // Should be = 84
            CycleCount++;
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
        RoundManager.OnStartRound -= NewRoundStart;
    }
}
