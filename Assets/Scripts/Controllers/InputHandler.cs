using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool b_input;
        bool a_input;
        bool x_input;
        bool y_input;

        bool rb_input;
        bool lb_input;
        bool rt_input;
        bool lt_input;
        float rt_axis;
        float lt_axis;

        bool leftAxisDown;
        bool rightAxisDown;

        float b_timer;
        float rt_timer;
        float lt_timer;

        StateManager stateManager;
        CameraManager cameraManager;

        float delta;

        void Start()
        {
            stateManager = GetComponent<StateManager>();
            stateManager.Init();

            cameraManager = CameraManager.singleton;
            cameraManager.Init(stateManager);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            stateManager.FixedTick(delta);
            cameraManager.Tick(delta);
        }

        private void ResetInputAndStates()
        {
            if (b_input == false)
                b_timer = 0;

            if (stateManager.rollInput)
                stateManager.rollInput = false;

            if (stateManager.run)
                stateManager.run = false;
        }

        void Update()
        {
            delta = Time.deltaTime;
            stateManager.Tick(delta);
            ResetInputAndStates();
        }

        private void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            b_input = Input.GetButton("b_input");
            a_input = Input.GetButton("a_input");
            x_input = Input.GetButton("x_input");
            y_input = Input.GetButtonUp("y_input");
            rt_input = Input.GetButton("rt_input");
            rt_axis = Input.GetAxis("rt_input");
            if (rt_axis != 0)
                rt_input = true;
            lt_input = Input.GetButton("lt_input");
            lt_axis = Input.GetAxis("lt_input");
            if (lt_axis != 0)
                lt_input = true;
            rb_input = Input.GetButton("rb_input");
            lb_input = Input.GetButton("lb_input");

            rightAxisDown = Input.GetButtonUp("r3_input");

            if (b_input)
                b_timer += delta;
        }

        void UpdateStates()
        {
            stateManager.horizontal = horizontal;
            stateManager.vertical = vertical;

            Vector3 v = stateManager.vertical * cameraManager.transform.forward;
            Vector3 h = stateManager.horizontal * cameraManager.transform.right;
            stateManager.moveDirection = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            stateManager.moveAmount = Mathf.Clamp01(m);

            if (x_input)
                b_input = false;

            if (b_input && b_timer > 0.3f)
            {
                stateManager.run = (stateManager.moveAmount > 0);
            }

            if (b_input == false && b_timer > 0 && b_timer < 0.3f)
                stateManager.rollInput = true;

            stateManager.itemInput = x_input;
            stateManager.rt = rt_input;
            stateManager.rb = rb_input;
            stateManager.lt = lt_input;
            stateManager.lb = lb_input;

            if (y_input)
            {
                stateManager.isTwoHanded = !stateManager.isTwoHanded;
                stateManager.HandleTwoHanded();
            }

            if (stateManager.lockOnTarget != null)
            {
                if (stateManager.lockOnTarget.eStates.isDead)
                {
                    stateManager.lockOn = false;
                    stateManager.lockOnTarget = null;
                    stateManager.lockOnTransform = null;
                    cameraManager.lockOn = false;
                    cameraManager.lockOnTarget = null;

                }
            }



            if (rightAxisDown)
            {
                stateManager.lockOn = !stateManager.lockOn;
                if (stateManager.lockOnTarget == null)
                {
                    stateManager.lockOn = false;
                }


                cameraManager.lockOnTarget = stateManager.lockOnTarget;
                stateManager.lockOnTransform = cameraManager.lockOnTransform;
                cameraManager.lockOn = stateManager.lockOn;
            }
        }
    }
}
