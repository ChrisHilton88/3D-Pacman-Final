using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Respsonsible for all things related to the Pellet GameObject
public class PelletManager : MonoSingleton<PelletManager>
{
    private int _maxPellets = 5;
    private int _totalPellets;       
    private int _pelletTally;

    [SerializeField] private List<GameObject> _pelletList = new List<GameObject>();     // SetActive (true) to all Pellet GameObjects at the start of a new level
    [SerializeField] private List<GameObject> _powerPelletList = new List<GameObject>();

    [SerializeField] private GameObject _pelletprefab;
    [SerializeField] private InkyBehaviour _inkyBehaviour;
    [SerializeField] private ClydeBehaviour _clydeBehaviour;

    #region Properties
    public int TotalPellets
    {
        get { return _totalPellets; }
        private set { _totalPellets = value; }
    }
    public int PelletTally
    {
        get { return _pelletTally; }
        private set { _pelletTally = value; }
    }
    #endregion


    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        ItemCollection.OnItemCollected += InkyStartMoving;
        ItemCollection.OnItemCollected += ClydeStartMoving;
        RoundManager.OnRoundEnd += RoundEnd;
        RoundManager.OnRoundEnd += ActivatePellets;
        //RoundManager.OnRoundStart += ActivatePellets;
    }

    void Start()
    {
        TotalPellets = _maxPellets;
        PelletTally = 0;
    }

    #region Events
    void PelletCollected(int value)
    {
        if (TotalPellets > 0 && value == 10)
        {
            TotalPellets--;
            PelletTally++;      // Add 1 to the tally
            Debug.Log("Pellet Tally: " + PelletTally);

            if (TotalPellets <= 0)
            {
                RoundManager.Instance.NextLevel();
            }
        }
    }

    // Inky can start moving
    void InkyStartMoving(int value)
    {
        if (PelletTally >= _inkyBehaviour.StartRandomValue)
            _inkyBehaviour.StartMovement();
        else
            return;
    }

    // Clyde can start moving
    void ClydeStartMoving(int value)
    {
        if (PelletTally >= _clydeBehaviour.MovePelletCount)
            _clydeBehaviour.StartMovement();
        else
            return;
    }

    // Reset the values
    void RoundEnd()
    {
        TotalPellets = 5;
        PelletTally = 0;
    }

    // Activate all the pellets
    void ActivatePellets()
    {
        StartCoroutine(DelayPelletActivationRoutine());
    }
    #endregion

    IEnumerator DelayPelletActivationRoutine()
    {
        yield return null;

        for (int i = 0; i < _pelletList.Count; i++)
        {
            _pelletList[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < _powerPelletList.Count; i++)
        {
            _powerPelletList[i].gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected;
        ItemCollection.OnItemCollected -= InkyStartMoving;
        ItemCollection.OnItemCollected -= ClydeStartMoving;
        RoundManager.OnRoundEnd -= RoundEnd;
        RoundManager.OnRoundStart -= ActivatePellets;
    }
}
