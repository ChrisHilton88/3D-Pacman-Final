using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    public static Action<int> OnItemCollected;      // Event is triggered when the Player collects any Item
    public static Action OnFrightened;              // Event is triggered when the Player collects a Power Pellet

    [SerializeField] private BonusItemDisplay _bonusItemDisplay;


    void OnTriggerEnter(Collider other)
    {
        string tagToFind = other.tag;       // Cache the other.tag reference
        (string key, int value) = GetKeyAndValueInDictionary(tagToFind);        // Cache the Tuple

        if (tagToFind != null)      // A tag exists
        {
            switch (tagToFind)
            {
                // TODO - Come back and add logic to Enemy - Depending on whether in frightened state or not
                // Tags that we don't need to do anything with, just simply ignore
                case "Enemy":
                case "Untagged":
                case "Portal":
                case "Tunnel":
                case "MainCamera":
                case "Player":
                case "StartBox":
                    break;

                // Seperate event for tag "Power Pellet" 
                case "Power Pellet":
                    OnFrightened?.Invoke();
                    OnItemCollected?.Invoke(value);
                    other.gameObject.SetActive(false);  
                    break;

                // Tags that we need to add to UIDisplay and add to Score
                case "Pellet":
                case "Cherry":
                case "Strawberry":
                case "Orange":
                case "Apple":
                case "Melon":
                case "Ship":
                case "Bell":
                case "Key":
                    OnItemCollected?.Invoke(value);
                    UIManager.Instance.AddCollectedBonusItem(key);
                    other.gameObject.SetActive(false);  
                    break;
            }
        }
        else       // Tag doesn't exist
        {
            Debug.LogWarning("Tag not found in OnTriggerEnter switch statement - ItemCollection");
        }
    }

    (string key, int value) GetKeyAndValueInDictionary(string tagToFind)
    {
        foreach(KeyValuePair<string, int> kvp in ScoreManager.Instance.BonusItemsDictionary)
        {
            if (kvp.Key == tagToFind)       // If a Key matches the collider Tag
            {
                return (kvp.Key, kvp.Value);     // Return the Key and Value
            }
        }

        return (null, 0);      // Tag not found in the dictionary. Choose a value that doesn't exist so we can match with the Tuple signature.
    }
}
