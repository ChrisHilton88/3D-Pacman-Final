using System;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    private int _cycleCount;        // Incrementor for cycle count
    private int _maxCycles = 7;     // Maximum cycles per level

    private float _cycleTimer;

    Coroutine _cycleCoroutine;
    WaitForSeconds _fiveSeconds = new WaitForSeconds(5);        // Cache for optimisation
    WaitForSeconds _sevenSeconds = new WaitForSeconds(7);
    WaitForSeconds _twentySeconds = new WaitForSeconds(20);

    #region Properties
    public int CycleCount { get { return _cycleCount; } private set { _cycleCount = value; } }      
    public float CycleTimer {  get { return _cycleTimer; } private set {  _cycleTimer = value; } }
    #endregion

    public static Action OnNewState;        // Responsible for the timing of when enemies should change states. Enemies should subscribe



    void OnEnable()
    {
        OnNewState += CycleIncrement;
        RoundManager.OnRoundStart += NewRoundStart;
    }
    void OnDisable()
    {
        OnNewState -= CycleIncrement;
        RoundManager.OnRoundStart -= NewRoundStart;
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


    // Needs a parameter for a timer and we need to source this information from a stored table or something
    IEnumerator CycleTimerRoutine()
    {
        while (CycleCount <= _maxCycles)        // 7 instead of 8 cycles as the player already starts in one
        {
            switch (CycleCount)
            {
                case 1:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Test 1" + CycleTimer);
                    break;
                case 2:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Test 2" + CycleTimer);
                    break;
                case 3:
                    yield return _sevenSeconds;     // ToChase
                    Debug.Log("Test 3" + CycleTimer);
                    break;
                case 4:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Test 4" + CycleTimer);
                    break;
                case 5:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Test 5" + CycleTimer);
                    break;
                case 6:
                    yield return _twentySeconds;    // ToScatter
                    Debug.Log("Test 6" + CycleTimer);
                    break;
                case 7:
                    yield return _fiveSeconds;      // ToChase
                    Debug.Log("Test 7" + CycleTimer);
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

    // Event that handles resetting the CycleCount back to 1 on New Round
    void NewRoundStart()
    {
        CycleCount = 1;
        _cycleCoroutine = null;
        if (_cycleCoroutine == null)
            _cycleCoroutine = StartCoroutine(CycleTimerRoutine());
    }
    #endregion
}
