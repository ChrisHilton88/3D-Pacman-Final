using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for displaying the fruit on the map
public class BonusItemDisplay : MonoBehaviour
{
    // TODO - Check what round it is so we can pass in appropriate Bonus Item
    // If 70 pellets eaten - Display first fruit
    // If 170 pellets eaten - Display second fruit (unless the first is still there - which should be impossible as the timer is so short)
    // reset at the start of each round

    private int _displayFirstBonusItem = 70;        // When 70 pellets are eaten - First Bonus Item is displayed
    private int _displaySecondBonusItem = 170;      // When 170 pelelts are eaten - Second Bonus Item is displayed

    [SerializeField] private GameObject[] _bonusItemPrefabArray;        // Assign all the Bonus Item Prefabs
    [SerializeField] private GameObject _currentBonusItemPrefab;        // Used as a reference to the currently displayed item

    [SerializeField] private Transform _spawnPos;       // Set spawn position of all the items



    // Which script should hold the reference to the GameObjects??
    // Let's keep it here and we can use values to cross reference 


    void OnEnable()
    {
        ItemCollection.onItemCollected += DisplayBonusItem; 
    }

    void DisplayBonusItem(int value)
    {
        // First, check if the values are anything but 70 & 170 - If so, do nothing
        if (PelletManager.Instance.PlayerPellets != _displayFirstBonusItem || PelletManager.Instance.PlayerPellets != _displaySecondBonusItem)
        {
            return;
        }
        // Second, check if the players total is 70
        else if (PelletManager.Instance.PlayerPellets == _displayFirstBonusItem)
        {
            // Display first item at the spawn point
            // TODO - Check what round it is so we can pass in the correct prefabs and timers. E.g. Round 1 displays Cherry first for 6 seconds
            //(GameObject prefab, float timer) = CheckRound();

            StartCoroutine(DisplayBonusItemRoutine(prefab, timer));
        }
        // Third, check if the players total is 170
        else if (PelletManager.Instance.PlayerPellets == _displaySecondBonusItem)
        {
            // Check if first item is still active in Hierarchy
            // If not - Display second item

            StartCoroutine(DisplayBonusItemRoutine(prefab, timer));
        }
    }

    // TODO - Once the RoundManager has been built - come back to fix this
    //(GameObject prefab, float timer) CheckRound()
    //{

    //}

    IEnumerator DisplayBonusItemRoutine(GameObject prefab, float timer)
    {
        Instantiate(prefab, _spawnPos.position, Quaternion.identity);

        // TODO - Check the prefab items rotation
        yield return new WaitForSeconds(timer);     
    }

    void OnDisable()
    {
        ItemCollection.onItemCollected -= DisplayBonusItem; 
    }
}
