using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulsLike
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool isInvincible;
        public bool canMove;
        public bool isDead;


        public Animator animator;
        EnemyTarget enemyTarget;
        AnimatorHook animatorHook;
        public Rigidbody rigid;
        public float delta;

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();


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

            if (canMove)
                animator.applyRootMotion = false;
        }

        public void DoDamage(float v)
        {
            if (isInvincible)
                return;

            health -= v;
            isInvincible = true;
            animator.Play("damage_1");
            animator.applyRootMotion = true;
            animator.SetBool("canMove", false);

        }
    }
}