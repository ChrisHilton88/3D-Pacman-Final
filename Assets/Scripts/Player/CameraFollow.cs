using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _camPos;


    void Update()
    {
        transform.position = _camPos.position;
    }
}
