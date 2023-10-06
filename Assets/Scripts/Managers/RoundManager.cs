using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private RoundData[] levels;        // Open in Inspector to assign values
    

    void Start()
    {
        LoopThroughArray(); 
    }

    // TODO - Hard code all values, as making adjustments to the array can cause the values to be lost in the Inspector
    void LoopThroughArray()
    {
        foreach (RoundData level in levels)
        {
            Debug.Log($"Round: {level.round}, Bonus: {level.bonus}, Time: {level.time}");
        }
    }
}
