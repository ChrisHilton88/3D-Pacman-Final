using System;
using UnityEngine;

public class PelletCollection : MonoBehaviour
{

    public static Action onPelletCollected;


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("test 1");

        if (other.CompareTag("Pellet"))
        {
            onPelletCollected?.Invoke();
            other.gameObject.SetActive(false);
            Debug.Log("test 2");
        }
    }
}
