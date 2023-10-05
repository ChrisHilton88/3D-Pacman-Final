using UnityEngine;

// Respsonsible for all things related to the Pellet GameObject
public class PelletManager : MonoSingleton<PelletManager>   
{
    private int _maxPellets = 240;
    private int _pelletIncrement = 1;
    private static int _totalPellets;       // Global variable which holds reference to the total pool of pellets
    private int _playerPellets;

    #region Properties
    public int TotalPellets
    {
        get { return _totalPellets; }   
        private set { _totalPellets = value;}
    }
    public int PlayerPellets
    {
        get { return _playerPellets; }
        private set { _playerPellets = value; }
    }
    #endregion

    [SerializeField] private GameObject _pelletprefab;

    void OnEnable()
    {
        PelletCollection.onPelletCollected += PelletCollected;
    }

    void Start()
    {
        TotalPellets = _maxPellets;
    }

    void PelletCollected()
    {
        if (TotalPellets > 0)
        {
            TotalPellets -= _pelletIncrement;
            PlayerPellets += _pelletIncrement;
            Debug.Log(TotalPellets);
            Debug.Log(PlayerPellets);   
        }
        else
            Debug.Log("No more pellets");
    }
}
