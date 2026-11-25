
using UnityEngine;

namespace NEDClass2024
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform Target;

        public float Distance = 6f;
        public float LookHeight = 2.8f;
        public float ShoulderOffset = 0f;
        public float LookAheadDistance = 6f;

        public float Sensitivity = 3f;

        public float FollowSpeed = 15f;

        public float InitialPitch = 18f;

        public float MinPitch = 5f;

        public float MaxPitch = 55f;

        private float yaw;
        private float pitch;
        public bool cursorLocked = false;

        private void Start()
        {
            if (Target == null)
            {
                Debug.LogError("Camera Target is missing!");
                enabled = false;
                return;
            }

            yaw = Target.eulerAngles.y;
            pitch = InitialPitch;
        }

        private void Update()
        {
            if (cursorLocked && Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (cursorLocked)
            {
                yaw += Input.GetAxis("Mouse X") * Sensitivity;

                pitch -= Input.GetAxis("Mouse Y") * Sensitivity;

                pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);
            }
        }

        
        private void LateUpdate()
        {
            if (Target == null) return;

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            // Camera's pivot position
            Vector3 pivotPosition = Target.position +
                                    Vector3.up * LookHeight;

            // Camera position behind the player
            Vector3 offset = rotation *
                new Vector3(ShoulderOffset, 0.6f, -Distance);

            Vector3 desiredPosition = pivotPosition + offset;

            // Aim slightly ahead of the player
            Vector3 aimPoint = pivotPosition +
                rotation * Vector3.forward * LookAheadDistance;

            // Smooth camera movement
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                FollowSpeed * Time.deltaTime
            );

            // Look towards the point ahead
            transform.LookAt(aimPoint);
        }

    }
}
