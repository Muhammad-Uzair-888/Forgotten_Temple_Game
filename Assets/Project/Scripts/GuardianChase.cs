
using UnityEngine;

public class GuardianChase : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float catchDistance = 0.5f;

    [Header("References")]
    [SerializeField] private EscapeManager escapeManager;

    private Transform playerTarget;
    private Animator animator;

    private Collider[] guardianColliders;
    private Collider[] playerColliders;

    private bool isChasing = false;
    private bool hasCaughtPlayer = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);

        guardianColliders = GetComponentsInChildren<Collider>(true);

        if (escapeManager == null)
        {
            escapeManager = FindObjectOfType<EscapeManager>();
        }
    }

    public void Awaken(Transform target)
    {
        if (isChasing || hasCaughtPlayer)
            return;

        if (target == null)
        {
            Debug.LogError("GuardianChase: Player target is missing!");
            return;
        }

        playerTarget = target;

        playerColliders =
            playerTarget.GetComponentsInChildren<Collider>(true);

        isChasing = true;

        Debug.Log("Guardian awakened and started chasing the player!");

        if (animator != null &&
            animator.HasState(0, Animator.StringToHash("SleepEnd")))
        {
            animator.CrossFade("SleepEnd", 0.1f);
        }
    }

    public void StopChase()
    {
        isChasing = false;
    }

    private void Update()
    {
        if (!isChasing || hasCaughtPlayer || playerTarget == null)
            return;

        // Keep the guardian at its current height.
        Vector3 targetPosition = new Vector3(
            playerTarget.position.x,
            transform.position.y,
            playerTarget.position.z
        );

        Vector3 direction = targetPosition - transform.position;

        // Rotate towards the player.
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move towards the player.
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            chaseSpeed * Time.deltaTime
        );

        // Check whether the guardian has caught the player.
        if (IsPlayerWithinCatchDistance())
        {
            CatchPlayer();
        }
    }

    private bool IsPlayerWithinCatchDistance()
    {
        if (playerTarget == null)
            return false;

        if (guardianColliders == null || playerColliders == null)
        {
            return false;
        }

        bool foundValidColliderPair = false;

        float allowedDistance = catchDistance * catchDistance;

        // Check the distance between the actual collider surfaces.
        foreach (Collider guardianCol in guardianColliders)
        {
            if (guardianCol == null ||
                !guardianCol.enabled ||
                !guardianCol.gameObject.activeInHierarchy)
            {
                continue;
            }

            foreach (Collider playerCol in playerColliders)
            {
                if (playerCol == null ||
                    !playerCol.enabled ||
                    !playerCol.gameObject.activeInHierarchy)
                {
                    continue;
                }

                foundValidColliderPair = true;

                Vector3 playerPoint =
                    playerCol.ClosestPoint(guardianCol.bounds.center);

                Vector3 guardianPoint =
                    guardianCol.ClosestPoint(playerPoint);

                float distanceSquared =
                    (guardianPoint - playerPoint).sqrMagnitude;

                if (distanceSquared <= allowedDistance)
                {
                    return true;
                }
            }
        }

        // Fallback if usable colliders are not available.
        if (!foundValidColliderPair)
        {
            Vector2 guardianPosition =
                new Vector2(transform.position.x, transform.position.z);

            Vector2 playerPosition =
                new Vector2(playerTarget.position.x, playerTarget.position.z);

            return Vector2.Distance(
                guardianPosition,
                playerPosition
            ) <= catchDistance;
        }

        return false;
    }

    private void CatchPlayer()
    {
        if (hasCaughtPlayer)
            return;

        hasCaughtPlayer = true;
        isChasing = false;

        Debug.Log("GUARDIAN CAUGHT THE PLAYER!");

        if (escapeManager == null)
        {
            escapeManager = FindObjectOfType<EscapeManager>();
        }

        if (escapeManager != null)
        {
            escapeManager.TriggerGameOver();
        }
        else
        {
            Debug.LogError(
                "GuardianChase: EscapeManager reference is missing!"
            );
        }
    }
}
