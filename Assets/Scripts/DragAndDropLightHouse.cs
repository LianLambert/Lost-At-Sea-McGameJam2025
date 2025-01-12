using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropLightHouse : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject lightHouseBasic;
    public GameObject lightHouseHorizontal;
    public GameObject lightHouseVertical;
    public  GameObject currentPrefab;
    public static bool isDragging = false;
    public static LightHouseType draggingLightHouseType;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.gameIsOver)
            return;

        if (!isDragging && GameManager.numLightHouses > 0)
        {
            isDragging = true;
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 0; // Make sure the prefab is at the right Z level


            var r = Random.Range(0, 3);
            if (r == 0)
            {
                draggingLightHouseType = LightHouseType.Basic;
                currentPrefab = Instantiate(lightHouseBasic, mousePosition, Quaternion.identity);
            }
            if (r == 1)
            {
                draggingLightHouseType = LightHouseType.Horizontal;
                currentPrefab = Instantiate(lightHouseHorizontal, mousePosition, Quaternion.identity);
            }
            if (r == 2)
            {
                draggingLightHouseType = LightHouseType.Vertical;
                currentPrefab = Instantiate(lightHouseVertical, mousePosition, Quaternion.identity);
            }
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
