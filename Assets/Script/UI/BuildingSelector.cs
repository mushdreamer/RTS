using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    [SerializeField] private HouseNeedsUIController houseNeedsUI;
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Prevent clicking through UI
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                House hitHouse = hit.collider.GetComponent<House>();
                if (hitHouse != null)
                {
                    houseNeedsUI.ShowNeedsForHouse(hitHouse);
                }
                else
                {
                    houseNeedsUI.HidePanel();
                }
            }
            else
            {
                houseNeedsUI.HidePanel();
            }
        }
    }
}