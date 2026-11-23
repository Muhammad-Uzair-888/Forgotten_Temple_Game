using UnityEngine;

public class EscapeExitDetector : MonoBehaviour
{
    [SerializeField] private EscapeManager escapeManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;
        Debug.Log("[EscapeExitDetector] Player entered exit trigger.");
        if (escapeManager != null)
            escapeManager.OnPlayerEscaped();
    }
}
