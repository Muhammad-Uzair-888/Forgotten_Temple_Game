using UnityEngine;
using UnityEngine.SceneManagement;
using NEDClass2024;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject titleScreenPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Gameplay References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;
    [SerializeField] private EscapeManager escapeManager;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (titleScreenPanel != null && titleScreenPanel.activeInHierarchy)
        {
            return;
        }

        if (settingsPanel != null && settingsPanel.activeInHierarchy)
        {
            return;
        }

        if (escapeManager != null && escapeManager.IsEscapeFinished)
        {
            return;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;
        Time.timeScale = 0f;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.cursorLocked = false;
            thirdPersonCamera.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pausePanel != null)
        {
            pausePanel.transform.SetAsLastSibling();
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (!isPaused)
        {
            return;
        }

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.enabled = true;
            thirdPersonCamera.cursorLocked = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
