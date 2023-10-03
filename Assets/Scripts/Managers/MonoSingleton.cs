using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Instance is NULL");
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }
}


