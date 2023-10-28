using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Respsonsible for all things related to the Pellet GameObject
public class PelletManager : MonoSingleton<PelletManager>
{
    private int _maxPellets = 240;
    private int _totalPellets;       
    private int _pelletTally;

    Coroutine _activatePelletsRoutine;
    AudioSource _audioSource;   
    
    [SerializeField] private AudioClip _audioClip;   
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
        {
            _inkyBehaviour.StartMovement();
        }
        else
            return;
    }

    // Clyde can start moving
    void ClydeStartMoving(int value)
    {
        if (PelletTally >= _clydeBehaviour.MovePelletCount)
        {
            _clydeBehaviour.StartMovement();
        }
        else
            return;
    }

    // Reset the values
    void RoundEnd()
    {
        TotalPellets = 240;
        PelletTally = 0;
        _activatePelletsRoutine = null;
    }

    // Activate all the pellets
    void ActivatePellets()
    {
        if (_activatePelletsRoutine == null)
            _activatePelletsRoutine = StartCoroutine(DelayPelletActivationRoutine());
        else
            Debug.Log("_activatePelletsRoutine is NOT NULL - PelletManager");
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
