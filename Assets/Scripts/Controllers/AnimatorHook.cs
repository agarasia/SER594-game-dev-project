using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator animator;
        StateManager stateManager;
        Rigidbody rigid;
        EnemyStates e_states;

        public float rootMotionMultiplier;
        bool rolling;
        float roll_t;
        float d;
        AnimationCurve animCurve;
        public void Init(StateManager st, EnemyStates eStates)
        {
            stateManager = st;
            e_states = eStates;
            if (st != null)
            {
                animator = st.animator;
                rigid = st.rigidBody;
                animCurve = stateManager.roll_curve;
                d = stateManager.delta;
            }
            if (eStates != null)
            {
                animator = eStates.animator;
                rigid = eStates.rigid;
                d = eStates.delta;
            }

        }

        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0;
        }

        public void CloseRoll()
        {
            if (rolling == false) return;

            rootMotionMultiplier = 1;
            roll_t = 0;
            rolling = false;
        }

        private void OnAnimatorMove()
        {
            if (stateManager == null && e_states == null) return;

            if (rigid == null)
                return;

            if (stateManager != null)
            {
                if (stateManager.canMove)
                    return;

                d = stateManager.delta;
            }

            if (e_states != null)
            {
                if (e_states.canMove)
                    return;

                d = e_states.delta;
            }



            rigid.drag = 0;

            if (rootMotionMultiplier == 0)
                rootMotionMultiplier = 1;

            if (rolling == false)
            {
                Vector3 delta = animator.deltaPosition;
                delta.y = 0;
                Vector3 velocity = (delta * rootMotionMultiplier) / d;
                rigid.velocity = velocity;
            }
            else
            {
                roll_t += d / 0.6f;
                if (roll_t > 1) roll_t = 1;
                if (stateManager.roll_curve == null) return;

                float zValue = animCurve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rootMotionMultiplier);
                rigid.velocity = v2;
            }
        }

        public void OpenDamageColliders()
        {
            if (stateManager == null) return;

            stateManager.inventoryManager.OpenAllDamageColliders();
        }

        public void CloseDamageColliders()
        {
            if (stateManager == null) return;
            stateManager.inventoryManager.CloseAllDamageColliders();
        }
    }
}
