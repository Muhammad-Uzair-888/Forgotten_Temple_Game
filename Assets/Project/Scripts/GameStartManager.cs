using UnityEngine;
using NEDClass2024;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private GameObject titleScreenPanel;
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerPickupDrop playerPickupDrop;
    [SerializeField] private EntranceDoorTrigger entranceDoorTrigger;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    private void Start()
    {
        if (titleScreenPanel != null)
            titleScreenPanel.SetActive(true);
        if (gameplayHUD != null)
            gameplayHUD.SetActive(false);
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (playerPickupDrop != null)
            playerPickupDrop.enabled = false;
        if (entranceDoorTrigger != null)
        {
            entranceDoorTrigger.enabled = false;
            Collider triggerCollider = entranceDoorTrigger.GetComponent<Collider>();
            if (triggerCollider != null)
                triggerCollider.enabled = false;
        }
        if (thirdPersonCamera != null)
            thirdPersonCamera.cursorLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        if (titleScreenPanel != null)
            titleScreenPanel.SetActive(false);
        if (gameplayHUD != null)
            gameplayHUD.SetActive(true);
        if (thirdPersonCamera != null)
            thirdPersonCamera.cursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerPickupDrop != null)
            playerPickupDrop.enabled = true;
        if (entranceDoorTrigger != null)
        {
            entranceDoorTrigger.enabled = true;
            Collider triggerCollider = entranceDoorTrigger.GetComponent<Collider>();
            if (triggerCollider != null)
                triggerCollider.enabled = true;
        }
    }
}
