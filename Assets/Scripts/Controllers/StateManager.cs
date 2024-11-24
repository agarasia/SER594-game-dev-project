using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
   public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public Vector3 moveDirection;
        public float moveAmount;
        public bool rt, rb, lt, lb;
        public bool rollInput;
        public bool itemInput;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool usingItem;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;

        [Header("Stats")]
        public float moveSpeed = 3;
        public float runSpeed = 5.5f;
        public float rotateSpeed = 9;
        public float distanceFromGround = .3f;
        public bool isTwoHanded;
        public float rollSpeed = 1;

        [HideInInspector]
        public AnimatorHook animatorHook;
        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public Rigidbody rigidBody;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        float _actionDelay;

        public void Init()
        {
            SetupAnimator();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.angularDrag = 999;
            rigidBody.drag = 4;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init();

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            animatorHook = activeModel.GetComponent<AnimatorHook>();
            if(animatorHook == null)
                animatorHook = activeModel.AddComponent<AnimatorHook>();
            animatorHook.Init(this, null);
            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            animator.SetBool("onGround", true);
        }

        private void SetupAnimator()
        {
            if (activeModel == null)
            {
                animator = GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.Log("No Model Found");
                }
                else
                {
                    activeModel = animator.gameObject;
                }
            }

            if(animator == null)
                animator = activeModel.GetComponent<Animator>();

            animator.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;

            usingItem = animator.GetBool("interacting");

            DetectItemAction();
            DetectAction();

            if( inAction) 
            {
                animator.applyRootMotion = true;
                _actionDelay += delta;
                if (_actionDelay > 0.5f) 
                { 
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }
            }

            canMove = animator.GetBool("canMove");

            if (!canMove)
                return;

            //animatorHook.rootMotionMultiplier = 1;
            animatorHook.CloseRoll();
            HandleRolls();

            animator.applyRootMotion = false;

            rigidBody.drag = (moveAmount > 0  || onGround == false) ? 0 : 4;

            float targetSpeed = moveSpeed;
            if (usingItem)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
            }

            if (run)
                targetSpeed = runSpeed;
                
            
            if(onGround)
                rigidBody.velocity = moveDirection * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;


            Vector3 targetDirection = (lockOn == false)? moveDirection :
                (lockOnTransform != null) ? lockOnTransform.transform.position - transform.position
                : moveDirection;

            targetDirection.y = 0;
            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            animator.SetBool("lockOn", lockOn);

            if (lockOn == false)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDirection);
        }

        private void HandleLockOnAnimations(Vector3 moveDirection)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDirection);
            float h = relativeDir.x;
            float v = relativeDir.z;

            animator.SetFloat("vertical", v, 0.2f, delta);
            animator.SetFloat("horizontal", h, 0.2f, delta);
        }

        public void DetectItemAction()
        {
            if (canMove == false || usingItem)
            { return; }

            if (itemInput == false)
                return;

            ItemAction slot = actionManager.consumeables;
            string targetAnimation = slot.targetAnimation;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            usingItem = true;
            animator.CrossFade(targetAnimation, 0.2f);
        }

        public void DetectAction()
        {
            if (canMove == false || usingItem)
            { return; }

            if (rb == false && rt == false && lb == false && lt == false)
                return;

            string targetAnimation = null;

            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
                return;
            targetAnimation = slot.targetAnimation;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            canMove = false;
            inAction = true;
            animator.CrossFade(targetAnimation, 0.2f);
            //rigidBody.velocity = Vector3.zero;
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();

            animator.SetBool("onGround", onGround);
        }

        void HandleMovementAnimations()
        {
            animator.SetBool("running", run);
            animator.SetFloat("vertical", moveAmount, 0.4f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * distanceFromGround);
            Vector3 dir = -Vector3.up;
            float dist = distanceFromGround + 0.3f;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, dist))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }
            return r;
        }

        public void HandleTwoHanded()
        {
            animator.SetBool("twoHanded", isTwoHanded);

            if (isTwoHanded)
            {
                actionManager.UpdateActionsWithCurrentWeaponTwoHanded();
            }
            else
            {
                actionManager.UpdateActionsWithCurrentWeapon();
            }

        }

        public void HandleRolls()
        {
            if (!rollInput || usingItem)
                return;

            float v = vertical;
            float h = horizontal;

            //if(!lockOn)
            //{
            //    v = moveAmount > 0.3f ? 1 : 0;
            //    h = 0;
            //}
            //else
            //{
            //    if (Mathf.Abs(v) < 0.3f)
            //        v = 0;
            //    if (Mathf.Abs(h) < 0.3f)
            //        h = 0;
            //}

            if (v != 0)
            {
                if (moveDirection == Vector3.zero)
                    moveDirection = transform.forward;
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = targetRotation;
                animatorHook.InitForRoll();
                animatorHook.rootMotionMultiplier = rollSpeed;
            }
            else
            {
                animatorHook.rootMotionMultiplier = 1.3f;
            }

            animator.SetFloat("vertical", v);
            animator.SetFloat("horizontal", h);

            canMove = false;
            inAction = true;
            animator.CrossFade("DodgeRolls", 0.2f);

        }
    }
}
