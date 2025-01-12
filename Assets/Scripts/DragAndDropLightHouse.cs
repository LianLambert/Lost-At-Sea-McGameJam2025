using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropLightHouse : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject prefab;
    public  GameObject currentPrefab;
    public static bool isDragging = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDragging)
        {
            isDragging = true;
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 0; // Make sure the prefab is at the right Z level
            currentPrefab = Instantiate(prefab, mousePosition, Quaternion.identity);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && currentPrefab != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 0;
            currentPrefab.transform.position = mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("Hit object: " + hit.collider.name);
            }

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
