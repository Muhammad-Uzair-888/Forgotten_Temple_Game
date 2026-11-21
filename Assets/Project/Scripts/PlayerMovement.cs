
using UnityEngine;

namespace NEDClass2024
{
    public class PlayerMovement : MonoBehaviour
    {
        public int WalkSpeed = 3;

        public float RunSpeed = 7f;

        public int RotationalSpeed = 10;

        public Transform Camera;

        private Animator playerAnimator;

        private Rigidbody rigidBody;

        private bool isRunning = false;

        private Vector3 moveDirection;

        #region Private Fields

        private string horizontalAxis = "Horizontal";

        private string verticalAxis = "Vertical";

        private string AnimatorSpeedParam = "Speed";

        private string AnimatorRunParam = "Run";

        #endregion

        private void Start()
        {
            playerAnimator = GetComponentInChildren<Animator>();

            rigidBody = GetComponent<Rigidbody>();

            if (playerAnimator == null ||
                rigidBody == null ||
                Camera == null)
            {
                Debug.LogError("Missing Animator, Rigidbody or Camera reference!");
                enabled = false;
                return;
            }

            playerAnimator.applyRootMotion = false;
        }

        private void Update()
        {
            float horizontal, vertical;

            Inputs(out horizontal, out vertical);

            PlayerMovementHandling(horizontal, vertical);

            float animationSpeed = moveDirection.magnitude;

            if (animationSpeed < 0.05f)
                animationSpeed = 0f;

            playerAnimator.SetFloat(
                AnimatorSpeedParam,
                animationSpeed
            );

            playerAnimator.SetBool(
                AnimatorRunParam,
                isRunning && animationSpeed > 0.1f
            );
        }

        private void PlayerMovementHandling(float horizontal, float vertical)
        {
            Vector3 cameraForward =
                Vector3.ProjectOnPlane(Camera.forward, Vector3.up).normalized;

            Vector3 cameraRight =
                Vector3.ProjectOnPlane(Camera.right, Vector3.up).normalized;

            moveDirection = horizontal * cameraRight +
                            vertical * cameraForward;

            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            if (moveDirection.sqrMagnitude < 0.01f)
                moveDirection = Vector3.zero;
        }

        private void FixedUpdate()
        {
            Vector3 currentVelocity = rigidBody.velocity;

            Vector3 targetVelocity = moveDirection *
                (isRunning ? RunSpeed : WalkSpeed);

            rigidBody.velocity = new Vector3(
                targetVelocity.x,
                currentVelocity.y,
                targetVelocity.z
            );

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(moveDirection, Vector3.up);

                rigidBody.MoveRotation(
                    Quaternion.Slerp(
                        rigidBody.rotation,
                        targetRotation,
                        RotationalSpeed * Time.fixedDeltaTime
                    )
                );
            }
        }

        private void Inputs(out float horizontal, out float vertical)
        {
            horizontal = Input.GetAxisRaw(horizontalAxis);

            vertical = Input.GetAxisRaw(verticalAxis);

            isRunning = Input.GetKey(KeyCode.LeftShift);
        }
    }
}
