using UnityEngine;
using TMPro;

namespace NEDClass2024
{
    public class PlayerInteraction : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Interaction Settings")]
        [SerializeField] private Transform interactionOrigin;
        [SerializeField] private float interactionRadius = 2.5f;
        [SerializeField] private LayerMask interactableLayerMask;

        [Header("Hold Settings")]
        [Tooltip("Hold socket in player right hand.")]
        [SerializeField] private Transform artifactHoldPoint;

        [Header("UI Settings")]
        [SerializeField] private TMP_Text interactionPrompt;
        [SerializeField] private string pickupMessage = "Press E to Pick Up";
        [SerializeField] private string dropMessage = "Press Q to Drop";
        [SerializeField] private string placeMessage = "Press E to Place";
        [SerializeField] private string wrongRelicMessage = "This relic does not belong here.";

        #endregion

        #region Private Fields

        private readonly Collider[] nearbyObjects = new Collider[32];
        private ObjectGrabbable nearestArtifact;
        private ObjectGrabbable currentHeldArtifact;
        private Pedestal nearestPedestal;

        #endregion

        #region Properties

        public ObjectGrabbable CurrentHeldArtifact => currentHeldArtifact;
        public ObjectGrabbable NearestArtifact => nearestArtifact;
        public Pedestal NearestPedestal => nearestPedestal;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            if (artifactHoldPoint == null)
            {
                GameObject holdPointGo = GameObject.Find("ArtifactHoldPoint");
                if (holdPointGo != null)
                {
                    artifactHoldPoint = holdPointGo.transform;
                }
            }

            if (interactionOrigin == null || interactionPrompt == null || artifactHoldPoint == null)
            {
                Debug.LogError("[PlayerInteraction] Missing references!");
                enabled = false;
                return;
            }

            interactionPrompt.gameObject.SetActive(false);
        }

        private void Update()
        {
            DetectNearbyObjects();
            HandleInput();
            UpdateInteractionUI();
        }

        #endregion

        #region Proximity Detection

        private void DetectNearbyObjects()
        {
            int objectCount = Physics.OverlapSphereNonAlloc(
                interactionOrigin.position,
                interactionRadius,
                nearbyObjects,
                interactableLayerMask,
                QueryTriggerInteraction.Collide
            );

            nearestArtifact = null;
            nearestPedestal = null;

            float nearestDistance = float.MaxValue;

            if (currentHeldArtifact == null)
            {
                // When empty-handed: search for nearest unplaced, unheld artifact
                for (int i = 0; i < objectCount; i++)
                {
                    Collider currentCollider = nearbyObjects[i];

                    if (currentCollider == null)
                        continue;

                    ObjectGrabbable artifact = currentCollider.GetComponentInParent<ObjectGrabbable>();

                    if (artifact == null || artifact.IsHeld || artifact.IsPlaced)
                        continue;

                    Vector3 closestPoint = currentCollider.ClosestPoint(interactionOrigin.position);
                    float distance = Vector3.Distance(interactionOrigin.position, closestPoint);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestArtifact = artifact;
                    }
                }
            }
            else
            {
                // When carrying an artifact: search for nearest unoccupied Pedestal
                for (int i = 0; i < objectCount; i++)
                {
                    Collider currentCollider = nearbyObjects[i];

                    if (currentCollider == null)
                        continue;

                    Pedestal pedestal = currentCollider.GetComponentInParent<Pedestal>();

                    if (pedestal == null || pedestal.IsOccupied)
                        continue;

                    Vector3 closestPoint = currentCollider.ClosestPoint(interactionOrigin.position);
                    float distance = Vector3.Distance(interactionOrigin.position, closestPoint);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPedestal = pedestal;
                    }
                }
            }
        }

        #endregion

        #region Input & Interaction

        private void HandleInput()
        {
            // Press E: Pick up artifact OR Place artifact on pedestal
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentHeldArtifact == null)
                {
                    if (nearestArtifact != null)
                    {
                        PickupArtifact(nearestArtifact);
                    }
                }
                else
                {
                    if (nearestPedestal != null)
                    {
                        PlaceArtifactOnPedestal(nearestPedestal);
                    }
                }
            }

            // Press Q: Drop carried artifact
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (currentHeldArtifact != null)
                {
                    DropArtifact();
                }
            }
        }

        private void PickupArtifact(ObjectGrabbable artifact)
        {
            if (artifact == null || artifactHoldPoint == null)
                return;

            if (artifact.Grab(artifactHoldPoint))
            {
                currentHeldArtifact = artifact;
                nearestArtifact = null;
                Debug.Log($"[PlayerInteraction] Picked up artifact: {artifact.name}");
            }
        }

        private void DropArtifact()
        {
            if (currentHeldArtifact == null)
                return;

            Debug.Log($"[PlayerInteraction] Dropped artifact: {currentHeldArtifact.name}");
            currentHeldArtifact.Drop();
            currentHeldArtifact = null;
        }

        private void PlaceArtifactOnPedestal(Pedestal pedestal)
        {
            if (pedestal == null || currentHeldArtifact == null)
                return;

            if (pedestal.IsOccupied)
                return;

            if (pedestal.CanAcceptArtifact(currentHeldArtifact))
            {
                ObjectGrabbable artifactToPlace = currentHeldArtifact;
                if (pedestal.PlaceArtifact(artifactToPlace))
                {
                    Debug.Log($"[PlayerInteraction] Successfully placed {artifactToPlace.name} on {pedestal.name}");
                    currentHeldArtifact = null;
                    nearestPedestal = null;
                }
            }
            else
            {
                Debug.Log($"[PlayerInteraction] {currentHeldArtifact.name} does not belong on {pedestal.name}.");
            }
        }

        #endregion

        #region UI

        private void UpdateInteractionUI()
        {
            if (interactionPrompt == null)
                return;

            if (currentHeldArtifact != null)
            {
                if (nearestPedestal != null)
                {
                    if (nearestPedestal.CanAcceptArtifact(currentHeldArtifact))
                    {
                        interactionPrompt.text = placeMessage;
                    }
                    else
                    {
                        interactionPrompt.text = wrongRelicMessage;
                    }
                    interactionPrompt.gameObject.SetActive(true);
                }
                else
                {
                    interactionPrompt.text = dropMessage;
                    interactionPrompt.gameObject.SetActive(true);
                }
            }
            else if (nearestArtifact != null)
            {
                interactionPrompt.text = pickupMessage;
                interactionPrompt.gameObject.SetActive(true);
            }
            else
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (interactionOrigin == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                interactionOrigin.position,
                interactionRadius
            );
        }

        #endregion
    }
}
