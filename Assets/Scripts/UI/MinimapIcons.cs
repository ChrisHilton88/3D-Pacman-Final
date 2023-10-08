using UnityEngine;

public class MinimapIcons : MonoBehaviour
{
    public float minimapScale = 1000.0f;

    [SerializeField] private Transform _playerPos;
    [SerializeField] private GameObject _playerIcon;

    RectTransform _playerIconRectTransform;



    void Start()
    {
        _playerIconRectTransform = _playerIcon.GetComponent<RectTransform>();   
    }

    void LateUpdate()
    {
        UpdatePlayerIcon();
    }

    void UpdatePlayerIcon()
    {
        Vector2 minimapPos = WorldToMinimapPosition();
        _playerIconRectTransform.anchoredPosition = minimapPos;
    }

    // Helper function to convert world position to minimap position
    Vector2 WorldToMinimapPosition()        // WorldPos we want to convert
    {
        Vector2 minimapPos = new Vector2(_playerPos.position.x, _playerPos.position.z) * minimapScale;        // X & Z values of the 3D world
        return minimapPos;
    }


    // this method takes a world position, calculates its relative position with respect to the player, scales it down based on minimapScale, and returns a 2D vector representing the corresponding
    // position on the minimap. It's a common operation in minimap systems to convert between world coordinates and minimap coordinates for displaying icons, markers, or other elements on the minimap.
}
