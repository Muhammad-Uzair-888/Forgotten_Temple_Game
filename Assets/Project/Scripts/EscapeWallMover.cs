using System.Collections;
using UnityEngine;

public class EscapeWallMover : MonoBehaviour
{
    [Header("Wall References")]
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;

    [Header("Movement")]
    [SerializeField] private float closeDuration = 3f;
    [SerializeField] private float leftClosedLocalX = 1.5f;
    [SerializeField] private float rightClosedLocalX = -1.5f;

    [Header("Debug (remove after test)")]
    [SerializeField] private bool enableDebugKey = true;

    private bool hasStarted = false;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;
    private Coroutine closeRoutine;

    private void Start()
    {
        if (leftWall != null)
            leftOpenPos = leftWall.localPosition;
        if (rightWall != null)
            rightOpenPos = rightWall.localPosition;
    }

    private void Update()
    {
        if (enableDebugKey && Input.GetKeyDown(KeyCode.F5))
        {
            StartClosing();
        }
    }

    public void StartClosing()
    {
        if (hasStarted)
            return;
        hasStarted = true;
        Debug.Log("[EscapeWallMover] StartClosing triggered.");
        closeRoutine = StartCoroutine(CloseWallsRoutine());
    }

    public void StopClosing()
    {
        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
            closeRoutine = null;
        }
        Debug.Log("[EscapeWallMover] Closing stopped.");
    }

    private IEnumerator CloseWallsRoutine()
    {
        Vector3 leftTarget = new Vector3(leftClosedLocalX, leftOpenPos.y, leftOpenPos.z);
        Vector3 rightTarget = new Vector3(rightClosedLocalX, rightOpenPos.y, rightOpenPos.z);
        float elapsed = 0f;

        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / closeDuration));
            if (leftWall != null)
                leftWall.localPosition = Vector3.Lerp(leftOpenPos, leftTarget, t);
            if (rightWall != null)
                rightWall.localPosition = Vector3.Lerp(rightOpenPos, rightTarget, t);
            yield return null;
        }

        if (leftWall != null)
            leftWall.localPosition = leftTarget;
        if (rightWall != null)
            rightWall.localPosition = rightTarget;

        Debug.Log("[EscapeWallMover] Walls fully closed.");
    }
}
