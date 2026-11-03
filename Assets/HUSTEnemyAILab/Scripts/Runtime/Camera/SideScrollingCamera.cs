using UnityEngine;

namespace HUST.EnemyAILab.CameraSystem
{
    public sealed class SideScrollingCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float fixedY;
        [SerializeField] private float minimumX = -7f;
        [SerializeField] private float maximumX = 7f;
        [SerializeField, Min(0.01f)] private float smoothTime = 0.15f;

        private float horizontalVelocity;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            float desiredX = Mathf.Clamp(target.position.x, minimumX, maximumX);
            float smoothedX = Mathf.SmoothDamp(
                transform.position.x,
                desiredX,
                ref horizontalVelocity,
                smoothTime);

            transform.position = new Vector3(smoothedX, fixedY, transform.position.z);
        }
    }
}
