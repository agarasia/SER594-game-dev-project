using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool isInvincible;
        public bool canMove;


        public Animator animator;
        EnemyTarget enemyTarget;
        AnimatorHook animatorHook;
        public Rigidbody rigid;
        public float delta;
        

        public void Start()
        {
            animator = GetComponentInChildren<Animator>();
            enemyTarget = GetComponent<EnemyTarget>();
            enemyTarget.Init(animator);

            rigid = GetComponent<Rigidbody>();

            animatorHook = animator.GetComponent<AnimatorHook>();
            if (animatorHook == null)
                animatorHook = animator.gameObject.AddComponent<AnimatorHook>();
            animatorHook.Init(null, this);
        }

        public void Update()
        {
            delta = Time.deltaTime;
            canMove = animator.GetBool("canMove");
            if(isInvincible)
            {
                isInvincible = !animator.GetBool("canMove");
            }

            if (canMove == false)
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

        }
    }
}