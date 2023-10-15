using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinimapZoom : MonoBehaviour
{
    private int _minZoomLevel = 30, _maxZoomLevel = 50;     // Variables to let the camera follow the player depending on orthographic size
    private int[] _zoomLevel = { 60, 50, 40, 30 };
    private int _currentFOVIndex;

    private float _lerpDuration = 1f;

    private bool _lerpComplete;

    private readonly Vector3 _defaultCamPosition = new Vector3(0, 96.5f, -4f);


    PlayerInputActions _playerInputActions;
    Coroutine _lerpCoroutine;
    Camera _cam;
    [SerializeField] private Transform _playerPos;


    void OnEnable()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.UI.Enable();
        _playerInputActions.UI.MinimapZoom.started += MinimapZoomStarted;
        _playerInputActions.UI.MinimapDefault.started += MinimapDefaultStarted;
    }

    void OnDisable()
    {
        _playerInputActions.UI.MinimapZoom.performed -= MinimapZoomStarted;
        _playerInputActions.UI.MinimapDefault.started -= MinimapDefaultStarted;
        _playerInputActions.UI.Disable();
    }

    void Start()
    {
        _cam = GetComponentInChildren<Camera>();
        _cam.orthographicSize = 60;
        _currentFOVIndex = 0;
        _lerpCoroutine = null;
    }

    void LateUpdate()
    {
        if (_cam.orthographicSize >= 30 && _cam.orthographicSize <= 50)
        {
            Vector3 playerPosition = _playerPos.position;
            Vector3 cameraPosition = transform.position;

            // Update the camera's global position to follow the player
            cameraPosition.x = playerPosition.x;
            cameraPosition.z = playerPosition.z;
            transform.position = cameraPosition;
        }
        else
        {
            // Return the camera to its default global position when outside the specified range
            transform.position = _defaultCamPosition;
        }
    }

    #region Events
    void MinimapZoomStarted(InputAction.CallbackContext context)
    {
        if (_lerpCoroutine == null)     // If coroutine reference is NULL - assign and run the coroutine
        {
            if (context.ReadValue<float>() == 1)        // Zoom in
            {
                (float newFOV, float previousFOV) zoomValues = ZoomIn();
                _lerpCoroutine = StartCoroutine(LerpThroughFOVS(zoomValues.previousFOV, zoomValues.newFOV, transform.position, _playerPos.position));
                Debug.Log("transform.position: " + transform.position);
                Debug.Log("Player Pos: " + _playerPos.position);
            }
            else if (context.ReadValue<float>() == -1)      // Zoom out
            {
                (float newFOV, float previousFOV) zoomValues = ZoomOut();
                _lerpCoroutine = StartCoroutine(LerpThroughFOVS(zoomValues.previousFOV, zoomValues.newFOV, transform.position, _playerPos.position));
            }
        }
        else
            return;     
    }

    void MinimapDefaultStarted(InputAction.CallbackContext context)
    {
        if(_lerpCoroutine == null)
        {
            if (context.ReadValue<float>() == 1)
            {
                ZoomDefaultHomePosition();
            }
            else
                return;
        }
    }
    #endregion

    // Zoom in
    (float newFOV, float previousFOV) ZoomIn()     
    {
        int previousFovIndex = _currentFOVIndex;        
        _currentFOVIndex = (_currentFOVIndex + 1) % _zoomLevel.Length;
        _cam.orthographicSize = _zoomLevel[_currentFOVIndex];
        Debug.Log(_zoomLevel[_currentFOVIndex]);
        return (_zoomLevel[_currentFOVIndex], _zoomLevel[previousFovIndex]);        
    }
    
    // Zoom out
    (float newFOV, float previousFOV) ZoomOut()
    {
        int previousFovIndex = _currentFOVIndex;
        _currentFOVIndex = (_currentFOVIndex - 1 + _zoomLevel.Length) % _zoomLevel.Length;      
        _cam.orthographicSize = _zoomLevel[_currentFOVIndex];

        return (_zoomLevel[_currentFOVIndex], _zoomLevel[previousFovIndex]);
    }

    // Return the Minimap view to the default FOV 0f 60 instantly
    void ZoomDefaultHomePosition()
    {
        _currentFOVIndex = 0;
        _cam.orthographicSize = _zoomLevel[_currentFOVIndex];
    }

    // Smooth out the transition between FOV's
    IEnumerator LerpThroughFOVS(float currentFOV, float targetFOV, Vector3 currentCameraPos, Vector3 targetCameraPos)
    {
        float interpolationProgress = 0;        // Starting time for progress

        Vector3 initialCameraPosition = currentCameraPos;

        while (interpolationProgress < 1.0f) 
        {
            interpolationProgress += Time.deltaTime / _lerpDuration;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, interpolationProgress);
            _cam.orthographicSize = newFOV;

            Vector3 newPosition = new Vector3(targetCameraPos.x, initialCameraPosition.y, targetCameraPos.z);
            transform.position = Vector3.Lerp(initialCameraPosition, newPosition, interpolationProgress);
            yield return null;      
        }

        _lerpCoroutine = null;
    }
}
