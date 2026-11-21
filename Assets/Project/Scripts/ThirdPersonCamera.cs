
using UnityEngine;

namespace NEDClass2024
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform Target;

        public float Distance = 7.5f;

        public float LookHeight = 3.8f;

        public float ShoulderOffset = 0.4f;

        public float Sensitivity = 3f;

        public float FollowSpeed = 15f;

        public float InitialPitch = 18f;

        public float MinPitch = 5f;

        public float MaxPitch = 55f;

        private float yaw;
        private float pitch;

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

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Cursor.lockState != CursorLockMode.Locked &&
                Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
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

            Vector3 lookPoint = Target.position +
                                Vector3.up * LookHeight;

            Vector3 offset = rotation *
                new Vector3(ShoulderOffset, 0.6f, -Distance);

            Vector3 desiredPosition = lookPoint + offset;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                FollowSpeed * Time.deltaTime
            );

            transform.LookAt(lookPoint);
        }
    }
}
