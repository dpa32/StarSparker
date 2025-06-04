using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public static MiniMapManager Instance;

    public RectTransform playerIcon;
    public Vector2 mapWorldMin;
    public Vector2 mapWorldMax;
    public RectTransform miniMapRect; // ¹Ì´Ï¸Ê ¿µ¿ª RectTransform

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdatePlayerIconPosition(Vector3 playerWorldPos)
    {
        Vector2 playerPos2D = new Vector2(playerWorldPos.x, playerWorldPos.z);

        float normalizedX = Mathf.InverseLerp(mapWorldMin.x, mapWorldMax.x, playerPos2D.x);
        float normalizedY = Mathf.InverseLerp(mapWorldMin.y, mapWorldMax.y, playerPos2D.y);

        float miniMapWidth = miniMapRect.rect.width;
        float miniMapHeight = miniMapRect.rect.height;

        Vector2 miniMapPos = new Vector2(
            normalizedX * miniMapWidth,
            normalizedY * miniMapHeight
        );

        playerIcon.anchoredPosition = miniMapPos;
    }
}
