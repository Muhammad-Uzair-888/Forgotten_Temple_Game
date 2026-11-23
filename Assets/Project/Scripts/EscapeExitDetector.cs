using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using NEDClass2024;
public class EscapeExitDetector : MonoBehaviour
{
    public EscapeManager escapeManager;

    int playerLayer;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != playerLayer)
        {
            return;
        }

        if (escapeManager == null)
        {
            return;
        }

        escapeManager.PlayerReachedExit();
    }
}