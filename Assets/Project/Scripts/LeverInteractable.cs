using System.Collections;
using UnityEngine;
using TMPro;

public class LeverInteractable : MonoBehaviour
{
    public Transform handle;
    public Transform player;
    public TMP_Text promptText;

    public float interactRadius = 3f;
    public float rotateSpeed = 90f;
    public Vector3 restRotation = Vector3.zero;
    public Vector3 pulledRotation = new Vector3(-60f, 0f, 0f);

    public bool isPulled = false;

    bool isMoving = false;

    void Start()
    {
        if (handle == null)
        {
            handle = transform;
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.Find("PlayerRoot");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("[LeverInteractable] PlayerRoot not found on " + gameObject.name);
            }
        }

        handle.localRotation = Quaternion.Euler(isPulled ? pulledRotation : restRotation);

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        bool playerNear = Vector3.Distance(transform.position, player.position) < interactRadius;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(playerNear && !isMoving);
        }

        if (playerNear && !isMoving && Input.GetKeyDown(KeyCode.E))
        {
            isPulled = !isPulled;
            StartCoroutine(RotateRoutine(isPulled ? pulledRotation : restRotation));
        }
    }

    IEnumerator RotateRoutine(Vector3 targetEuler)
    {
        isMoving = true;

        Quaternion target = Quaternion.Euler(targetEuler);

        while (Quaternion.Angle(handle.localRotation, target) > 0.5f)
        {
            handle.localRotation = Quaternion.RotateTowards(
                handle.localRotation,
                target,
                rotateSpeed * Time.deltaTime
            );

            yield return null;
        }

        handle.localRotation = target;
        isMoving = false;
    }
}

