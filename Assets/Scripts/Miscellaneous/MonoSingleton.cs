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

    protected virtual void Initialize()
    {
        // Default implementation for initialization.
        // Override this method in inheriting classes to add custom initialization logic.
    }

    protected MonoSingleton()
    {
        if (_instance != null)
        {
            Debug.LogError("Trying to create another instance of a Singleton.");
            Destroy(this);
            return;
        }

        _instance = this as T;

        Initialize();
    }
}


