using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for displaying the fruit on the map
public class BonusItemDisplay : MonoBehaviour
{
    private int _displayFirstBonusItem = 70, _displaySecondBonusItem = 170;        // When 70 pellets are eaten - First Bonus Item is displayed

    private Dictionary<string, int> _bonusItemDictionary = new Dictionary<string, int>();

    [SerializeField] private GameObject _bonusItemDisplayPortal;
    [SerializeField] private GameObject[] _bonusItemPrefabArray;        // Assign all the Bonus Item Prefabs
    [SerializeField] private Transform _spawnPos;       // Set spawn position of all the items

    #region Properties
    public Dictionary<string, int> BonusItemDictionary {  get { return _bonusItemDictionary; } private set { _bonusItemDictionary = value; } }
    #endregion

    // Check Dictionary in DisplayBonusItem() coroutine 
    // Disable gameobject when timer finishes

    void OnEnable()
    {
        ItemCollection.OnItemCollected += DisplayBonusItem; 
    }

    void Start()
    {
        DisableAllPrefabs();
        GenerateDictionary();
    }

    void DisableAllPrefabs()
    {
        for (int i = 0; i < _bonusItemPrefabArray.Length; i++)
        {
            _bonusItemPrefabArray[i].SetActive(false);
        }
    }

    void DisplayBonusItem(int value)
    {
        // First, check if the pellet total is anything but 70 & 170 - If so, do nothing
        if (PelletManager.Instance.PelletTally == _displayFirstBonusItem || PelletManager.Instance.PelletTally == _displaySecondBonusItem)
        {
            RoundData currentRound = RoundManager.Instance.CheckRound();
            Debug.Log("Displaying Round: " + currentRound.round);
            Debug.Log("Displaying tag: " + currentRound.tag);
            Debug.Log("Displaying timer: " + currentRound.time);
            StartCoroutine(DisplayBonusItemRoutine(currentRound));         // Getting associated time
        }
        else
        {
            Debug.Log("Bonus Item Display Count: " + PelletManager.Instance.PelletTally);
        }
    }

    void GenerateDictionary()
    {
        BonusItemDictionary["Apple"] = 0;
        BonusItemDictionary["Bell"] = 1;
        BonusItemDictionary["Cherry"] = 2;
        BonusItemDictionary["Key"] = 3;
        BonusItemDictionary["Orange"] = 4;
        BonusItemDictionary["Ship"] = 5;
        BonusItemDictionary["Strawberry"] = 6;
        BonusItemDictionary["Melon"] = 7;
    }

    // Disply appropriate bonus item based on Round, timer.
    IEnumerator DisplayBonusItemRoutine(RoundData currentRound)
    {
        string tag = currentRound.tag;     // Pass this tag into the Dictionary below to find the matching value
        int timer = currentRound.time;     // Pass into WaitForSeconds

        int dictionaryValue = BonusItemDictionary[tag];

        if (dictionaryValue >= 0 && dictionaryValue < _bonusItemPrefabArray.Length)
        {
            for (int i = 0; i < _bonusItemPrefabArray.Length; i++)
            {
                _bonusItemPrefabArray[i].SetActive(i == dictionaryValue);
            }

            _bonusItemDisplayPortal.SetActive(true);
            yield return new WaitForSeconds(timer);
            _bonusItemPrefabArray[dictionaryValue].SetActive(false);
            _bonusItemDisplayPortal.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Dictionary value is out of range for _bonusItemArray.");
        }
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= DisplayBonusItem; 
    }
}
