using System.Collections;
using UnityEngine;

// Responsible for displaying the fruit on the map
public class BonusItemDisplay : MonoBehaviour
{
    private int _displayFirstBonusItem = 70, _displaySecondBonusItem = 170;        // When 70 pellets are eaten - First Bonus Item is displayed

    [SerializeField] private GameObject[] _bonusItemPrefabArray;        // Assign all the Bonus Item Prefabs
    [SerializeField] private GameObject _currentBonusItemPrefab;        // Used as a reference to the currently displayed item
    [SerializeField] private Transform _spawnPos;       // Set spawn position of all the items



    void OnEnable()
    {
        ItemCollection.OnItemCollected += DisplayBonusItem; 
    }

    void DisplayBonusItem(int value)
    {
        // First, check if the pellet total is anything but 70 & 170 - If so, do nothing
        if (PelletManager.Instance.PelletTally != _displayFirstBonusItem || PelletManager.Instance.PelletTally != _displaySecondBonusItem)
        {
            return;
        }
        else
        {
            RoundData currentRound = RoundManager.Instance.CheckRound();
            StartCoroutine(DisplayBonusItemRoutine(currentRound.time));
        }
    }

 
    // Needs - Timer only and maybe GameObject
    IEnumerator DisplayBonusItemRoutine(float timer)
    {
        GameObject newBonusItem = Instantiate(_bonusItemPrefabArray[0], _spawnPos.position, Quaternion.identity);
        yield return new WaitForSeconds(timer);
        Destroy(newBonusItem);
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= DisplayBonusItem; 
    }
}
