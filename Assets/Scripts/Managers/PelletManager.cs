using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Respsonsible for all things related to the Pellet GameObject
public class PelletManager : MonoSingleton<PelletManager>
{
    private int _maxPellets = 10;
    private int _totalPellets;       
    private int _pelletTally;

    private bool _inkySpawned = false;
    private bool _clydeSpawned = false;

    Coroutine _activatePelletsRoutine;
    AudioSource _audioSource;   
    
    [SerializeField] private AudioClip _audioClip;   
    [SerializeField] private List<GameObject> _pelletList = new List<GameObject>();     // SetActive (true) to all Pellet GameObjects at the start of a new level
    [SerializeField] private List<GameObject> _powerPelletList = new List<GameObject>();

    [SerializeField] private GameObject _pelletprefab;
    [SerializeField] private InkyBehaviour _inkyBehaviour;
    [SerializeField] private ClydeBehaviour _clydeBehaviour;

    #region Properties
    public int TotalPellets { get { return _totalPellets; } private set { _totalPellets = value; } }
    public int PelletTally { get { return _pelletTally; } private set { _pelletTally = value; } }   
    public bool InkySpawned { get { return _inkySpawned; } private set { _inkySpawned = value; } }
    public bool ClydeSpawned { get { return _clydeSpawned; } private set { _clydeSpawned = value; } }


    #endregion


    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        ItemCollection.OnItemCollected += InkyStartMoving;
        ItemCollection.OnItemCollected += ClydeStartMoving;
        RoundManager.OnRoundEnd += RoundEnd;
        RoundManager.OnRoundEnd += ActivatePellets;
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected;
        ItemCollection.OnItemCollected -= InkyStartMoving;
        ItemCollection.OnItemCollected -= ClydeStartMoving;
        RoundManager.OnRoundEnd -= RoundEnd;
        RoundManager.OnRoundEnd -= ActivatePellets;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
        _activatePelletsRoutine = null;
        TotalPellets = _maxPellets;
        PelletTally = 0;
        _audioSource.clip = _audioClip;
    }

    #region Events
    void PelletCollected(int value)
    {
        if (TotalPellets > 0 && value == 10)
        {
            _audioSource.Play();
            TotalPellets--;
            PelletTally++;      // Add 1 to the tally

            if (TotalPellets <= 0)
            {
                RoundManager.Instance.NextLevel();
            }
        }
    }

    // Inky can start moving
    void InkyStartMoving(int value)
    {
        if (PelletTally >= _inkyBehaviour.StartRandomValue && InkySpawned == false)
        {
            _inkyBehaviour.StartMovement();
            InkySpawned = true; 
        }
        else
            return;
    }

    // Clyde can start moving
    void ClydeStartMoving(int value)
    {
        if (PelletTally >= _clydeBehaviour.MovePelletCount && ClydeSpawned == false)
        {
            _clydeBehaviour.StartMovement();
            ClydeSpawned = true;    
        }
        else
            return;
    }

    // Reset the values
    void RoundEnd()
    {
        TotalPellets = 10;
        PelletTally = 0;
        _activatePelletsRoutine = null;
        InkySpawned = false;
        ClydeSpawned = false;
    }

    // Activate all the pellets
    void ActivatePellets()
    {
        if (_activatePelletsRoutine == null)
            _activatePelletsRoutine = StartCoroutine(DelayPelletActivationRoutine());
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

        _activatePelletsRoutine = null;
    }
}
