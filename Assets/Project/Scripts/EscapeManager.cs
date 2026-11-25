using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using NEDClass2024;
public class EscapeManager : MonoBehaviour
{
    public EscapeWallMover wallMover;
    public TextMeshProUGUI countdownText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public float countdownDuration = 10f;
    public PlayerMovement playerMovement;

    bool countdownStarted = false;
    bool escapeFinished = false;
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

        if (wallMover != null)
        {
            wallMover.StartClosing();
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

        escapeFinished = true;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (wallMover != null)
        {
            wallMover.StopClosing();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Debug.Log("Countdown finished, player failed");
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called");
        Application.Quit();
    }
}