using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class DamageCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            Debug.Log("A");
            EnemyStates enemyStates = other.transform.GetComponentInParent<EnemyStates>();
            if (enemyStates == null)
                return;

            Debug.Log("B");
            enemyStates.DoDamage(35);
        }
    }
}
