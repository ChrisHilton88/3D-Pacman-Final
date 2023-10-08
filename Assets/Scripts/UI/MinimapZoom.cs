using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MinimapZoom : MonoBehaviour
{
    private float _lerpDuration = 1f;
    private float _currentFOV;

    PlayerInputActions _playerInputActions;
    Coroutine _lerpCoroutine;
    Camera _cam;

    int[] _zoomLevel = { 60, 50, 40, 30 };
    int _currentFOVIndex;


    void OnEnable()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.UI.Enable();
        _playerInputActions.UI.MinimapZoom.started += MinimapZoomStarted;
        _playerInputActions.UI.MinimapDefault.started += MinimapDefaultStarted;
    }

    void Start()
    {
        _cam = GetComponentInChildren<Camera>();
        _cam.fieldOfView = 60;
        _currentFOV = _cam.fieldOfView;
        _currentFOVIndex = 0;
        _lerpCoroutine = null;
    }

    #region Events
    void MinimapZoomStarted(InputAction.CallbackContext context)
    {
        if (_lerpCoroutine == null)     // If coroutine reference is NULL - assign and run the coroutine
        {
            if (context.ReadValue<float>() == 1)        // Zoom in
            {
                (float newFOV, float previousFOV) zoomValues = ZoomIn();
                _lerpCoroutine = StartCoroutine(LerpThroughFOVS(zoomValues.previousFOV, zoomValues.newFOV));
            }
            else if (context.ReadValue<float>() == -1)      // Zoom out
            {
                (float newFOV, float previousFOV) zoomValues = ZoomOut();
                _lerpCoroutine = StartCoroutine(LerpThroughFOVS(zoomValues.previousFOV, zoomValues.newFOV));
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
        _cam.fieldOfView = _zoomLevel[_currentFOVIndex];
        Debug.Log(_zoomLevel[_currentFOVIndex]);
        return (_zoomLevel[_currentFOVIndex], _zoomLevel[previousFovIndex]);        
    }
    
    // Zoom out
    (float newFOV, float previousFOV) ZoomOut()
    {
        int previousFovIndex = _currentFOVIndex;
        _currentFOVIndex = (_currentFOVIndex - 1 + _zoomLevel.Length) % _zoomLevel.Length;      
        _cam.fieldOfView = _zoomLevel[_currentFOVIndex];

        return (_zoomLevel[_currentFOVIndex], _zoomLevel[previousFovIndex]);
    }

    // Return the Minimap view to the default FOV 0f 60 instantly
    void ZoomDefaultHomePosition()
    {
        _currentFOVIndex = 0;
        _cam.fieldOfView = _zoomLevel[_currentFOVIndex];
    }

    // Smooth out the transition between FOV's
    IEnumerator LerpThroughFOVS(float currentFOV, float targetFOV)
    {
        float interpolationProgress = 0;        // Starting time for progress

        while(interpolationProgress < 1.0f) 
        {
            interpolationProgress += Time.deltaTime / _lerpDuration;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, interpolationProgress);
            _cam.fieldOfView = newFOV;
            yield return null;      
        }

        _lerpCoroutine = null;
    }

    void OnDisable()
    {
        _playerInputActions.UI.MinimapZoom.performed -= MinimapZoomStarted;
        _playerInputActions.UI.MinimapDefault.started -= MinimapDefaultStarted;
        _playerInputActions.UI.Disable();
    }
}
