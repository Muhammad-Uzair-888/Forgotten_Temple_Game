using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NEDClass2024;

public class EscapeManager : MonoBehaviour
{
    [SerializeField] private EscapeWallMover wallMover;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button victoryRestartButton;
    [SerializeField] private float countdownDuration = 5f;

    public bool escapeFinished = false;

    private bool countdownStarted = false;
    private Coroutine countdownRoutine;

    private void Awake()
    {
        PlayerPickupDrop.OnObjectPickedUp += HandleObjectPickedUp;

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(RestartLevel);
        if (victoryRestartButton != null)
            victoryRestartButton.onClick.AddListener(RestartLevel);
    }

    private void OnDestroy()
    {
        PlayerPickupDrop.OnObjectPickedUp -= HandleObjectPickedUp;
    }

    private void HandleObjectPickedUp(ObjectGrabbable obj)
    {
        if (obj == null || obj.Type != ArtifactType.Crown)
            return;
        if (countdownStarted)
            return;
        countdownStarted = true;

        Debug.Log("Crown picked up");

        if (wallMover != null)
            wallMover.StartClosing();

        countdownRoutine = StartCoroutine(CountdownRoutine());
    }

    public void OnPlayerEscaped()
    {
        if (escapeFinished)
            return;
        escapeFinished = true;

        Debug.Log("Countdown cancelled");

        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
        if (wallMover != null)
            wallMover.StopClosing();
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        escapeFinished = true;
        Debug.Log("Restarting level");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator CountdownRoutine()
    {
        float remaining = countdownDuration;
        int lastSecond = -1;

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        Debug.Log("Countdown started");

        while (remaining > 0f)
        {
            int currentSecond = Mathf.CeilToInt(remaining);
            if (currentSecond != lastSecond)
            {
                lastSecond = currentSecond;
                if (countdownText != null)
                    countdownText.text = currentSecond.ToString();
            }
            remaining -= Time.deltaTime;
            yield return null;
        }

        if (escapeFinished)
            yield break;
        escapeFinished = true;

        Debug.Log("Countdown finished, player failed");

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
        if (wallMover != null)
            wallMover.StopClosing();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
}
