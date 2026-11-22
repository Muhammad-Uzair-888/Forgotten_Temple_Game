
using System.Collections;
using UnityEngine;

public class EntranceDoorTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform doorRoot;

    [Header("Door Movement")]
    [SerializeField] private Vector3 openLocalOffset =
        new Vector3(0f, 5f, 0f);

    [SerializeField] private float openDuration = 3f;

    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasOpened)
            return;

        if (playerRoot == null || doorRoot == null)
        {
            Debug.LogError("Player Root or Door Root is not assigned!");
            return;
        }

        // Detect the player, including its child colliders.
        bool isPlayer =
            other.transform == playerRoot ||
            other.transform.IsChildOf(playerRoot);

        if (!isPlayer)
            return;

        hasOpened = true;
        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        Vector3 startPosition = doorRoot.localPosition;
        Vector3 targetPosition = startPosition + openLocalOffset;

        float elapsed = 0f;
        float duration = Mathf.Max(openDuration, 0.1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            doorRoot.localPosition =
                Vector3.Lerp(startPosition, targetPosition, smoothT);

            yield return null;
        }

        doorRoot.localPosition = targetPosition;

        Debug.Log("Entrance door fully opened!");
    }
}
