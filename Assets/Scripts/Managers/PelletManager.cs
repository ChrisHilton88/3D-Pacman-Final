using UnityEngine;

// Respsonsible for all things related to the Pellet GameObject
public class PelletManager : MonoSingleton<PelletManager>
{
    private int _maxPellets = 240;
    private int _pelletIncrement = 1;
    private int _totalPellets;       // Global variable which holds reference to the total pool of pellets
    private int _pelletTally;

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

    [SerializeField] private GameObject _pelletprefab;
    [SerializeField] private InkyBehaviour _inkyBehaviour;


    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
    }

    void Start()
    {
        TotalPellets = _maxPellets;
        PelletTally = 0;
    }

    void PelletCollected(int value)
    {
        PelletTally++;

        if (TotalPellets > 0)
        {
            TotalPellets -= _pelletIncrement;
        }
        else
            Debug.Log("No more pellets");

        Debug.Log("Pellet Tally: " + PelletTally);

        if (PelletTally >= _inkyBehaviour.StartRandomValue)
        {
            Debug.Log("Inky Start: " + _inkyBehaviour.StartRandomValue);
            _inkyBehaviour.StartMovement();
            Debug.Log("Starting");
        }
        else
            return;
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected;  
    }
}
