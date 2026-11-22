using System.Collections;
using UnityEngine;
using TMPro;

namespace NEDClass2024
{
    public class Puzzle1Manager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Pedestals")]
        [SerializeField] private Pedestal pedestalCrystal;
        [SerializeField] private Pedestal pedestalDiamond;
        [SerializeField] private Pedestal pedestalRuby;

        [Header("Exit Door")]
        [Tooltip("The door GameObject or door slab to open when puzzle is solved.")]
        [SerializeField] private Transform exitDoorTransform;
        [SerializeField] private Vector3 doorOpenLocalOffset = new Vector3(0f, 5f, 0f);
        [SerializeField] private float doorOpenDuration = 3f;

        [Header("Feedback UI & Audio")]
        [SerializeField] private TextMeshProUGUI puzzleMessageText;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip completionSound;

        #endregion

        #region Private Fields

        private Vector3 doorClosedLocalPos;
        private Vector3 doorTargetLocalPos;
        private bool isDoorOpening = false;

        #endregion

        #region Properties

        public bool Puzzle1Completed { get; private set; }
        public bool IsDoorOpening => isDoorOpening;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            if (puzzleMessageText != null)
            {
                puzzleMessageText.text = "";
            }

            if (exitDoorTransform != null)
            {
                doorClosedLocalPos = exitDoorTransform.localPosition;
                doorTargetLocalPos = doorClosedLocalPos + doorOpenLocalOffset;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called by a Pedestal whenever an artifact is placed.
        /// </summary>
        public void OnPedestalStateChanged()
        {
            if (Puzzle1Completed)
                return;

            bool crystalDone = pedestalCrystal != null && pedestalCrystal.IsOccupied;
            bool diamondDone = pedestalDiamond != null && pedestalDiamond.IsOccupied;
            bool rubyDone = pedestalRuby != null && pedestalRuby.IsOccupied;

            if (crystalDone && diamondDone && rubyDone)
            {
                CompletePuzzle();
            }
        }

        #endregion

        #region Private Methods

        private void CompletePuzzle()
        {
            Puzzle1Completed = true;
            Debug.Log("[Puzzle1Manager] PUZZLE 1 COMPLETED! Opening exit door...");

            // Play completion sound if available
            if (audioSource != null && completionSound != null)
            {
                audioSource.PlayOneShot(completionSound);
            }

            // Display UI message
            if (puzzleMessageText != null)
            {
                StartCoroutine(ShowCompletionMessageRoutine("PUZZLE COMPLETED!", 4f));
            }

            // Open exit door
            if (exitDoorTransform != null)
            {
                StartCoroutine(OpenDoorRoutine());
            }
        }

        private IEnumerator ShowCompletionMessageRoutine(string message, float duration)
        {
            puzzleMessageText.text = message;
            yield return new WaitForSeconds(duration);
            puzzleMessageText.text = "";
        }

        private IEnumerator OpenDoorRoutine()
        {
            isDoorOpening = true;
            Vector3 startPos = exitDoorTransform.localPosition;
            float elapsed = 0f;
            float duration = Mathf.Max(doorOpenDuration, 0.1f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Smooth step curve for realistic stone door movement
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                exitDoorTransform.localPosition = Vector3.Lerp(startPos, doorTargetLocalPos, smoothT);
                yield return null;
            }

            exitDoorTransform.localPosition = doorTargetLocalPos;
            isDoorOpening = false;
            Debug.Log("[Puzzle1Manager] Exit door fully opened.");
        }

        #endregion
    }
}
