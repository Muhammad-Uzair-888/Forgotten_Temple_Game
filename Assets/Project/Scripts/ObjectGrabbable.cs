
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    public enum ArtifactType
    {
        SunDisc,
        ScarabSeal,
        AnkhRelic
    }

    [SerializeField] private ArtifactType artifactType;

    private Rigidbody objectRigidbody;
    private Collider[] objectColliders;

    public ArtifactType Type => artifactType;
    public bool IsPlaced { get; private set; }
    public bool IsHeld { get; private set; }

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        objectColliders = GetComponentsInChildren<Collider>();

        if (objectRigidbody == null)
            Debug.LogError("Artifact needs a Rigidbody!");
    }

    public bool Grab(Transform handPoint)
    {
        if (objectRigidbody == null || IsHeld || IsPlaced)
            return false;

        IsHeld = true;

        objectRigidbody.velocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;

        objectRigidbody.isKinematic = true;
        objectRigidbody.useGravity = false;

        SetColliders(false);

        AttachAndSnap(handPoint);

        return true;
    }

    public void Drop()
    {
        if (!IsHeld || IsPlaced)
            return;

        transform.SetParent(null, true);

        IsHeld = false;

        SetColliders(true);

        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;

        objectRigidbody.velocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;
    }

    public void PlaceAt(Transform socket)
    {
        if (!IsHeld || socket == null)
            return;

        AttachAndSnap(socket);

        IsHeld = false;
        IsPlaced = true;

        objectRigidbody.velocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;
        objectRigidbody.isKinematic = true;
        objectRigidbody.useGravity = false;

        SetColliders(false);
    }

    private void AttachAndSnap(Transform target)
    {
        Vector3 worldScale = transform.lossyScale;

        transform.SetParent(target, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Vector3 parentScale = target.lossyScale;

        transform.localScale = new Vector3(
            worldScale.x / Mathf.Max(parentScale.x, 0.0001f),
            worldScale.y / Mathf.Max(parentScale.y, 0.0001f),
            worldScale.z / Mathf.Max(parentScale.z, 0.0001f)
        );
    }

    private void SetColliders(bool state)
    {
        foreach (Collider col in objectColliders)
            col.enabled = state;
    }
}
