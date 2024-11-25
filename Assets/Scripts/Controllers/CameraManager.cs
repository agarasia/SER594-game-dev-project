using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class CameraManager : MonoBehaviour
    {
        public bool lockOn;
        public float followSpeed = 7;
        public float mouseSpeed = 2;
        public float controllerSpeed = 3;

        public Transform target;
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;

        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform cameraTransform;
        StateManager stateManager;

        float turnSmoothening = .1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;
        public float lookAngle;
        public float tiltAngle;

        bool usedRightAxis;

        public void Init(StateManager st)
        {
            stateManager = st;
            target = st.transform;
            cameraTransform = Camera.main.transform;
            pivot = cameraTransform.parent;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;

            if (lockOnTarget != null)
            {
                {
                    if (lockOnTransform == null)
                    {
                        lockOnTransform = lockOnTarget.GetTarget();
                        stateManager.lockOnTransform = lockOnTransform;
                    }

                    if (Mathf.Abs(c_h) > 0.6f)
                    {
                        if (!usedRightAxis)
                        {
                            lockOnTransform = lockOnTarget.GetTarget((c_h > 0));
                            stateManager.lockOnTransform = lockOnTransform;
                            usedRightAxis = true;
                        }
                    }
                }
            }

            if (usedRightAxis)
            {
                if (Mathf.Abs(c_h) > 0.6f)
                {
                    usedRightAxis = false;
                }
            }

            if (c_h != 0 && c_v != 0)
            {
                h = c_h;
                v = -c_v;
                targetSpeed = controllerSpeed;
            }

            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }

        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            target.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if (turnSmoothening > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothening);
                smoothY = Mathf.SmoothDamp(smoothX, v, ref smoothYvelocity, turnSmoothening);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            if (lockOn && lockOnTarget != null)
            {
                Vector3 targetDirection = lockOnTransform.position - transform.position;
                targetDirection.Normalize();
                //targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                    targetDirection = transform.forward;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, d * 9);
                lookAngle = transform.eulerAngles.y;
                return;
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }

        public static CameraManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
