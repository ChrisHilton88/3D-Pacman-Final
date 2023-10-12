using System;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    // Incrementor to count how many times we have gone through the cycle
    private int _cycleCount;

    // Timer for the intervals
    private float _cycleTimer;
    private float _startTime;
    // Start mode is always Scatter

    Coroutine _cycleCoroutine;

    public static Action OnNewState;

    // 

    #region Properties
    public int CycleCount { get { return _cycleCount; } private set { _cycleCount = value; } }      
    public float CycleTimer {  get { return _cycleTimer; } private set {  _cycleTimer = value; } }
    #endregion



    void OnEnable()
    {
        OnNewState += CycleIncrement;
    }

    void Start()
    {
        CycleCount = 0;
        _startTime = Time.realtimeSinceStartup;
        if (_cycleCoroutine == null)
            _cycleCoroutine = StartCoroutine(CycleTimerRoutine());
        _cycleCoroutine = null;
    }

    void Update()
    {
        float timeSinceStartUp = Time.realtimeSinceStartup - _startTime;
    }

    void CycleIncrement()
    {
        CycleCount++;
    }

    IEnumerator CycleTimerRoutine()
    {
        if(CycleCount == 0)     // Set initial state at game launch (Scatter)
        {
            yield return new WaitForEndOfFrame();
        }
        else if (CycleCount >= 1 && CycleCount <= 8) 
        {
            yield return new WaitForSeconds(5f);
        }
        else
            Debug.Log("Incorrect Cycle Count");
            yield return null;

        OnNewState?.Invoke();
        CycleCount++;
        _cycleCoroutine = null;
    }

    void OnDisable()
    {
        OnNewState -= CycleIncrement;   
    }
}
