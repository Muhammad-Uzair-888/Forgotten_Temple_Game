
using System.Collections;
using UnityEngine;

public class EntranceDoorTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform doorRoot;
    [SerializeField] private AudioSource gateAudioSource;

    [Header("Door Movement")]
    [SerializeField] private Vector3 openLocalOffset =
        new Vector3(0f, 5f, 0f);

    [SerializeField] private float openDuration = 3f;

    [Header("Gate Sound")]
    [SerializeField] private float soundDuration = 5f;

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

        // Detect the player, including child colliders.
        bool isPlayer =
            other.transform == playerRoot ||
            other.transform.IsChildOf(playerRoot);

        if (!isPlayer)
            return;

        hasOpened = true;

        // Play the gate sound when opening begins.
        if (gateAudioSource != null && gateAudioSource.clip != null)
        {
            gateAudioSource.Play();

            StartCoroutine(StopGateSoundAfterDelay());
        }

        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator StopGateSoundAfterDelay()
    {
        yield return new WaitForSeconds(soundDuration);

        if (gateAudioSource != null)
        {
            gateAudioSource.Stop();
        }
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
