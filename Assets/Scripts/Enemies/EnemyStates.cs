using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulsLike
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool canBeParried = true;
        public bool parriedIsOn = true;
        // public bool doParry = false;
        public bool isInvincible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;
        StateManager parriedBy;



        public Animator animator;
        EnemyTarget enemyTarget;
        AnimatorHook animatorHook;
        public Rigidbody rigid;
        public float delta;

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        float timer;


        public void Start()
        {
            health = 100;
            animator = GetComponentInChildren<Animator>();
            enemyTarget = GetComponent<EnemyTarget>();
            enemyTarget.Init(this);

            rigid = GetComponent<Rigidbody>();

            animatorHook = animator.GetComponent<AnimatorHook>();
            if (animatorHook == null)
                animatorHook = animator.gameObject.AddComponent<AnimatorHook>();
            animatorHook.Init(null, this);

            InitRagDoll();
            parriedIsOn = false;
        }

        void InitRagDoll()
        {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if (rigs[i] == rigid)
                    continue;

                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagdoll()
        {


            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;

            }


            Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            rigid.isKinematic = true;


            StartCoroutine("CloseAnimator");
        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForEndOfFrame();
            animator.enabled = false;
            this.enabled = false;

        }

        public void Update()
        {
            delta = Time.deltaTime;
            canMove = animator.GetBool("canMove");

            if (dontDoAnything)
            {

                dontDoAnything = !canMove;
                return;
            }

            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    EnableRagdoll();
                }
            }
            if (isInvincible)
            {
                isInvincible = !animator.GetBool("canMove");
            }

            if (parriedBy != null && parriedIsOn == false)
            {
                parriedBy.parryTarget = null;
                parriedBy = null;
            }

            if (canMove)
            {
                parriedIsOn = false;
                animator.applyRootMotion = false;
                //Debug
                timer += Time.deltaTime;
                if (timer > 3)
                {
                    DoAction();
                    timer = 0;
                }
            }

        }

        public void DoAction()
        {
            animator.Play("oh_attack_1");
            animator.applyRootMotion = true;
            animator.SetBool("canMove", false);
        }

        public void DoDamage(float v)
        {
            if (isInvincible)
                return;

            health -= v;
            isInvincible = true;
            animator.Play("damage_2");
            animator.applyRootMotion = true;
            animator.SetBool("canMove", false);

        }

        public void CheckForParry(Transform target, StateManager stateManager)
        {
            if (canBeParried == false || parriedIsOn == false || isInvincible)
            {
                Debug.Log("Parry not possible: canBeParried = " + canBeParried + ", isInvincible = " + isInvincible);
                return;
            }

            Vector3 dir = transform.position - target.position;
            dir.Normalize();
            float dot = Vector3.Dot(target.forward, dir);
            Debug.Log($"Dot Product: {dot}");

            if (dot < 0)
            {
                Debug.Log("Parry failed: incorrect direction.");
                return;
            }

            Debug.Log("Parry successful! Triggering parry animation.");
            isInvincible = true;
            animator.Play("attack_interrupt");
            animator.applyRootMotion = true;
            animator.SetBool("canMove", false);
            stateManager.parryTarget = this;
            parriedBy = stateManager;
            return;
        }

        public void IsGettingParried()
        {
            health -= 500;
            dontDoAnything = true;
            animator.SetBool("canMove", false);
            animator.Play("parry_recieved");
        }

    }
}