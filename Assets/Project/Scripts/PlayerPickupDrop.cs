using System.Collections;
using UnityEngine;
using TMPro;

namespace NEDClass2024
{
    public class PlayerPickupDrop : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Camera & Raycast Settings")]
        [Tooltip("Camera used for centre-screen viewport raycasting.")]
        [SerializeField] private Camera playerCamera;

        [Tooltip("Max distance the player can reach to pick up or place objects.")]
        [SerializeField] private float pickupDistance = 5f;

        [Tooltip("Layer mask containing interactable objects (Gems, Pedestals).")]
        [SerializeField] private LayerMask interactableLayerMask;

        [Tooltip("Layer mask used for line-of-sight checks (Interactables + Environment/Walls).")]
        [SerializeField] private LayerMask lineOfSightLayerMask;

        [Header("Attachment Socket")]
        [Tooltip("Existing ArtifactHoldPoint in character's right hand.")]
        [SerializeField] private Transform objectGrabPointTransform;

        [Header("UI References")]
        [Tooltip("Interaction text prompt displaying contextual feedback.")]
        [SerializeField] private TextMeshProUGUI interactionPromptText;

        #endregion

        #region Private Fields

        private ObjectGrabbable currentHeldObject;
        private ObjectGrabbable targetedGrabbable;
        private Pedestal targetedPedestal;
        private Coroutine errorRoutine;

        #endregion

        #region Static Events

        public static event System.Action<ObjectGrabbable> OnObjectPickedUp;

        public static void RaiseObjectPickedUp(ObjectGrabbable obj)
        {
            OnObjectPickedUp?.Invoke(obj);
        }

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
                if (playerCamera == null)
                {
                    playerCamera = FindObjectOfType<Camera>();
                }
            }

            // Default LayerMasks if not configured in Inspector
            if (interactableLayerMask.value == 0)
            {
                int interactableLayer = LayerMask.NameToLayer("Interactable");
                if (interactableLayer != -1)
                {
                    interactableLayerMask = 1 << interactableLayer;
                }
            }

            if (lineOfSightLayerMask.value == 0)
            {
                // Default + Interactable
                lineOfSightLayerMask = LayerMask.GetMask("Default", "Interactable");
            }

            UpdatePromptUI();
        }

        private void Update()
        {
            PerformCentreRaycast();
            HandleInteractionInputs();
            UpdatePromptUI();
        }

        #endregion

        #region Raycasting & Target Detection

        private void PerformCentreRaycast()
        {
            targetedGrabbable = null;
            targetedPedestal = null;

            if (playerCamera == null)
                return;

            // Centre-screen ray from exact camera viewport centre
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            // Query all hits along the ray up to pickupDistance
            RaycastHit[] hits = Physics.RaycastAll(ray, pickupDistance, lineOfSightLayerMask, QueryTriggerInteraction.Ignore);

            if (hits.Length == 0)
                return;

            // Sort hits by distance from camera
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // Find first valid hit that is NOT part of the player character
            Transform playerRootTransform = transform.root;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                // Skip player colliders
                if (hit.collider.transform.root == playerRootTransform)
                    continue;

                // Check for Pedestal
                Pedestal pedestal = hit.collider.GetComponentInParent<Pedestal>();
                if (pedestal != null)
                {
                    targetedPedestal = pedestal;
                    return;
                }

                // Check for Grabbable Artifact
                ObjectGrabbable grabbable = hit.collider.GetComponentInParent<ObjectGrabbable>();
                if (grabbable != null && !grabbable.IsPlaced && !grabbable.IsHeld)
                {
                    targetedGrabbable = grabbable;
                    return;
                }

                // If hit something else with collider (e.g. wall/floor) on lineOfSightLayerMask,
                // line of sight is blocked by obstacle
                if (((1 << hit.collider.gameObject.layer) & interactableLayerMask) == 0)
                {
                    // Hit environment obstacle before interactable
                    return;
                }
            }
        }

        #endregion

        #region Interaction Inputs

        private void HandleInteractionInputs()
        {
            // Press E to pick up or place
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentHeldObject == null)
                {
                    TryPickup();
                }
                else
                {
                    TryPlace();
                }
            }

            // Press Q to drop currently held artifact
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (currentHeldObject != null)
                {
                    currentHeldObject.Drop();
                    currentHeldObject = null;
                    Debug.Log("[PlayerPickupDrop] Artifact dropped.");
                }
            }
        }

        private void TryPickup()
        {
            if (targetedGrabbable == null || objectGrabPointTransform == null)
                return;

            if (targetedGrabbable.Grab(objectGrabPointTransform))
            {
                currentHeldObject = targetedGrabbable;
                targetedGrabbable = null;
                Debug.Log($"[PlayerPickupDrop] Picked up: {currentHeldObject.Type}");
                OnObjectPickedUp?.Invoke(currentHeldObject);
            }
        }

        private void TryPlace()
        {
            if (targetedPedestal == null || currentHeldObject == null)
                return;

            if (targetedPedestal.IsOccupied)
            {
                ShowErrorMessage("Pedestal is already occupied.");
                return;
            }

            if (targetedPedestal.CanAcceptArtifact(currentHeldObject))
            {
                if (targetedPedestal.PlaceArtifact(currentHeldObject))
                {
                    currentHeldObject = null;
                    targetedPedestal = null;
                }
            }
            else
            {
                ShowErrorMessage("This relic does not belong here.");
            }
        }

        #endregion

        #region UI Prompt Feedback

        private void UpdatePromptUI()
        {
            if (interactionPromptText == null)
                return;

            // If an active temporary error is showing, don't overwrite it immediately
            if (errorRoutine != null)
                return;

            if (currentHeldObject == null)
            {
                // Empty-handed: check if aiming at an available artifact
                if (targetedGrabbable != null)
                {
                    interactionPromptText.text = "Press E to Pick Up";
                    return;
                }
            }
            else
            {
                // Holding an artifact: check if aiming at a pedestal
                if (targetedPedestal != null)
                {
                    if (targetedPedestal.IsOccupied)
                    {
                        interactionPromptText.text = "";
                    }
                    else if (targetedPedestal.AcceptedArtifact == currentHeldObject.Type)
                    {
                        interactionPromptText.text = "Press E to Place";
                    }
                    else
                    {
                        interactionPromptText.text = "This relic does not belong here.";
                    }
                    return;
                }
            }

            // Not aiming at anything interactable
            interactionPromptText.text = "";
        }

        private void ShowErrorMessage(string message)
        {
            if (interactionPromptText == null)
                return;

            if (errorRoutine != null)
            {
                StopCoroutine(errorRoutine);
            }

            errorRoutine = StartCoroutine(ErrorMessageRoutine(message, 2.5f));
        }

        private IEnumerator ErrorMessageRoutine(string message, float duration)
        {
            interactionPromptText.text = message;
            yield return new WaitForSeconds(duration);
            errorRoutine = null;
            UpdatePromptUI();
        }

        #endregion
    }
}
