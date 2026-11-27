using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using NEDClass2024;
public class EscapeManager : MonoBehaviour
{
    [Header("Game Over UI")]
    [SerializeField] private Button restartButton;
    public EscapeWallMover wallMover;
    public TextMeshProUGUI countdownText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public float countdownDuration = 15f;
    public PlayerMovement playerMovement;
    public ThirdPersonCamera thirdPersonCamera;
    public GuardianChase guardian;

    bool countdownStarted = false;
    bool escapeFinished = false;
    public bool IsEscapeFinished => escapeFinished;
    Coroutine countdownRoutine;

    void Awake()
    {
        PlayerPickupDrop.OnObjectPickedUp += HandleObjectPickedUp;
    }

    void OnDestroy()
    {
        PlayerPickupDrop.OnObjectPickedUp -= HandleObjectPickedUp;
    }

    void Start()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartLevel);
            restartButton.onClick.AddListener(RestartLevel);
        }
        else
        {
            Debug.LogError("Restart Button is not assigned!");
        }
    }

    void HandleObjectPickedUp(ObjectGrabbable obj)
    {
        if (obj == null || obj.Type != ArtifactType.Crown)
        {
            return;
        }

        if (countdownStarted)
        {
            return;
        }

        countdownStarted = true;
        Debug.Log("Crown picked up");
        Debug.Log("Countdown started");

        if (guardian != null && playerMovement != null)
        {
            guardian.Awaken(playerMovement.transform);
        }

        countdownRoutine = StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        float remaining = countdownDuration;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        while (remaining > 0f)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.CeilToInt(remaining).ToString();
            }

            yield return null;
            remaining -= Time.deltaTime;
        }

        if (escapeFinished)
        {
            yield break;
        }

        if (wallMover != null)
        {
            wallMover.StartClosing();
        }

        TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        if (escapeFinished)
        {
            return;
        }

        escapeFinished = true;

        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }

        if (guardian != null)
        {
            guardian.StopChase();
        }

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

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.transform.SetAsLastSibling();
            gameOverPanel.SetActive(true);
        }

        Debug.Log("Game Over");
    }

    public void PlayerReachedExit()
    {
        if (!countdownStarted || escapeFinished)
        {
            return;
        }

        escapeFinished = true;
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (guardian != null)
        {
            guardian.StopChase();
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.cursorLocked = false;
            thirdPersonCamera.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
        }

        if (wallMover != null)
        {
            wallMover.StopClosing();
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Debug.Log("Player escaped");
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called");
        Application.Quit();
    }
}