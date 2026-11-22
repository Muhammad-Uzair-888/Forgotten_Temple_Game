using UnityEngine;

namespace NEDClass2024
{
    public enum ArtifactType
    {
        Crystal,
        Diamond,
        Ruby
    }

    [RequireComponent(typeof(Rigidbody))]
    public class ObjectGrabbable : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Artifact Settings")]
        [SerializeField] private ArtifactType artifactType;

        #endregion

        #region Private Fields

        private Rigidbody objectRigidbody;
        private Collider[] objectColliders;
        private int originalLayer;

        #endregion

        #region Properties

        public ArtifactType Type => artifactType;
        public bool IsPlaced { get; private set; }
        public bool IsHeld { get; private set; }

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            EnsureComponents();
            originalLayer = gameObject.layer;

            if (objectRigidbody == null)
            {
                Debug.LogError($"[ObjectGrabbable] Missing Rigidbody on {gameObject.name}!");
            }
        }

        private void EnsureComponents()
        {
            if (objectRigidbody == null)
                objectRigidbody = GetComponent<Rigidbody>();

            if (objectColliders == null || objectColliders.Length == 0)
                objectColliders = GetComponentsInChildren<Collider>();
        }

        #endregion

        #region Public Methods

        public bool Grab(Transform handPoint)
        {
            EnsureComponents();

            if (objectRigidbody == null || IsHeld || IsPlaced || handPoint == null)
                return false;

            IsHeld = true;

            // Zero out physics motion if dynamic
            if (!objectRigidbody.isKinematic)
            {
                objectRigidbody.velocity = Vector3.zero;
                objectRigidbody.angularVelocity = Vector3.zero;
            }
            objectRigidbody.isKinematic = true;
            objectRigidbody.useGravity = false;

            // Disable colliders so it doesn't push the player or get stuck
            SetColliders(false);

            // Attach to hand socket
            AttachAndSnap(handPoint);

            return true;
        }

        public void Drop()
        {
            EnsureComponents();

            if (!IsHeld || IsPlaced)
                return;

            transform.SetParent(null, true);
            IsHeld = false;

            // Re-enable colliders and physics
            SetColliders(true);
            objectRigidbody.isKinematic = false;
            objectRigidbody.useGravity = true;
            objectRigidbody.velocity = Vector3.zero;
            objectRigidbody.angularVelocity = Vector3.zero;
        }

        public void PlaceAt(Transform socket)
        {
            EnsureComponents();

            if (!IsHeld || socket == null)
                return;

            AttachAndSnap(socket);

            IsHeld = false;
            IsPlaced = true;

            // Lock in place permanently
            if (!objectRigidbody.isKinematic)
            {
                objectRigidbody.velocity = Vector3.zero;
                objectRigidbody.angularVelocity = Vector3.zero;
            }
            objectRigidbody.isKinematic = true;
            objectRigidbody.useGravity = false;

            // Keep colliders off or trigger to prevent physical nudging
            SetColliders(false);
        }

        #endregion

        #region Private Methods

        private void AttachAndSnap(Transform target)
        {
            Vector3 worldScale = transform.lossyScale;

            transform.SetParent(target, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Preserve world scale across differently-scaled parents
            Vector3 parentScale = target.lossyScale;
            transform.localScale = new Vector3(
                worldScale.x / Mathf.Max(parentScale.x, 0.0001f),
                worldScale.y / Mathf.Max(parentScale.y, 0.0001f),
                worldScale.z / Mathf.Max(parentScale.z, 0.0001f)
            );
        }

        private void SetColliders(bool state)
        {
            if (objectColliders == null)
                objectColliders = GetComponentsInChildren<Collider>();

            foreach (Collider col in objectColliders)
            {
                if (col != null)
                    col.enabled = state;
            }
        }

        #endregion
    }
}
