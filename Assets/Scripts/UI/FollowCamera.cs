using UnityEngine;

namespace UI
{
    public class FollowCamera : MonoBehaviour
    {
        private const float FollowSpeed = 2f;
        private const float AngleThreshold = 15f;
        private const float DelayBeforeFollow = 0.5f;
        
        [SerializeField] 
        private Vector3 positionOffset;
        
        [SerializeField] 
        private Vector3 rotationOffset;

        [SerializeField] 
        private Transform cam;

        private float _delayTimer;
        private bool _isMoving;

        private void OnEnable()
        {
            Snap();
        }

        private void Update()
        {
            if (!_isMoving && GetAngleFromTarget() > AngleThreshold)
            {
                _isMoving = true;
                _delayTimer = 0f;
            }

            _delayTimer += Time.deltaTime;

            if (_isMoving && _delayTimer >= DelayBeforeFollow)
            {
                var t = 1f - Mathf.Exp(-FollowSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), t);
                transform.rotation = Quaternion.Slerp(transform.rotation, GetTargetRotation(), t);
            }
                
            if (GetAngleFromTarget() < 1f)
            {
                _isMoving = false;
            }

        }

        private void Snap()
        {
            transform.position = GetTargetPosition();
            transform.rotation = GetTargetRotation();
            _isMoving = false;
            _delayTimer = 0f;
        }

        private Vector3 GetTargetPosition()
        {
            var offsetX = cam.right * positionOffset.x;
            var offsetY = cam.up * positionOffset.y;
            var offsetZ = cam.forward * positionOffset.z;
            
            return cam.position + offsetX + offsetY + offsetZ;
        }

        // might not need but just in case
        private Quaternion GetTargetRotation()
        {
            return cam.rotation * Quaternion.Euler(rotationOffset);
        }
        
        private float GetAngleFromTarget()
        {
            var currentDirection = transform.position - cam.position;
            var targetDirection = GetTargetPosition() - cam.position;
            var currAngle = Vector3.Angle(targetDirection, currentDirection);

            return currAngle;
        }
    }
}
