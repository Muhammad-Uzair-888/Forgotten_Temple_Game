
using UnityEngine;

public class PlayerPickupDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;

    [SerializeField] private float pickupDistance = 5f;

    [SerializeField] private LayerMask pickupLayerMask;

    [SerializeField] private Transform objectGrabPointTransform;

    private ObjectGrabbable currentObjectGrabbable;

    private void Update()
    {
        // Press E to pick up an object
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentObjectGrabbable == null)
            {
                TryPickupObject();
            }
        }

        // Press Q to drop the held object
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentObjectGrabbable != null)
            {
                currentObjectGrabbable.Drop();

                currentObjectGrabbable = null;

                Debug.Log("Object dropped!");
            }
        }
    }

    private void TryPickupObject()
    {
        
        if (Physics.Raycast(
            playerCameraTransform.position,
            playerCameraTransform.forward,
            out RaycastHit raycastHit,
            pickupDistance,
            pickupLayerMask,
            QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Raycast hit: " + raycastHit.collider.name);

            ObjectGrabbable objectGrabbable =
                raycastHit.collider.GetComponentInParent<ObjectGrabbable>();

            if (objectGrabbable != null)
            {
                if (objectGrabbable.Grab(objectGrabPointTransform))
                {
                    currentObjectGrabbable = objectGrabbable;
                    Debug.Log("Object picked up successfully!");
                }
            }
            else
            {
                Debug.Log("This object has no ObjectGrabbable component.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit an Interactable object.");
        }

    }
}
