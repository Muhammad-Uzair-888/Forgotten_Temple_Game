using System.Collections;
using UnityEngine;
using TMPro;

public class Puzzle2Manager : MonoBehaviour
{
    [Header("Eternity Crown")]
    [SerializeField] private GameObject eternityCrown;
    public LeverInteractable lever1;
    public LeverInteractable lever2;
    public LeverInteractable lever3;

    public bool correctLever1 = true;
    public bool correctLever2 = false;
    public bool correctLever3 = true;

    public Transform doorRoot;
    public Vector3 doorOpenOffset = new Vector3(0f, 5f, 0f);
    public float doorOpenDuration = 3f;

    public TMP_Text messageText;

    bool puzzleSolved = false;

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (puzzleSolved)
        {
            return;
        }

        if (lever1 == null || lever2 == null || lever3 == null)
        {
            return;
        }

        if (lever1.IsMoving || lever2.IsMoving || lever3.IsMoving)
        {
            return;
        }

        bool correct =
            lever1.isPulled == correctLever1 &&
            lever2.isPulled == correctLever2 &&
            lever3.isPulled == correctLever3;

        if (correct)
        {
            SolvePuzzle();
        }
    }

    void SolvePuzzle()
    {
        puzzleSolved = true;

        Debug.Log("Puzzle 2 solved! Activating Crown.");

        if (eternityCrown != null)
        {
            eternityCrown.SetActive(true);
            Debug.Log("Crown activated!");
        }
        else
        {
            Debug.LogError("Eternity Crown is NOT assigned!");
        }

        if (messageText != null)
        {
            messageText.text = "The gate opens";
            messageText.gameObject.SetActive(true);
        }

        if (doorRoot != null)
        {
            StartCoroutine(OpenDoorRoutine());
        }
    }

    IEnumerator OpenDoorRoutine()
    {
        Vector3 startPos = doorRoot.localPosition;
        Vector3 targetPos = startPos + doorOpenOffset;
        Debug.Log("[Puzzle2Manager] Gate start: " + startPos + " target: " + targetPos);
        float duration = Mathf.Max(doorOpenDuration, 0.1f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            doorRoot.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        doorRoot.localPosition = targetPos;

        yield return new WaitForSeconds(2f);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}