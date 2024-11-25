using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class InventoryManager : MonoBehaviour
    {
        public Weapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public Weapon leftHandWeapon;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;

            EquipWeapon(rightHandWeapon, false);
            EquipWeapon(leftHandWeapon, true);

            CloseAllDamageColliders();
        }


        public void EquipWeapon(Weapon w, bool isLeft = false)
        {
            string targerIdle = w.oh_idle;
            targerIdle += (isLeft) ? "_l" : "_r";
            states.animator.SetBool("mirror", isLeft);
            states.animator.Play("changeWeapon");
            states.animator.Play(targerIdle);
        }



        public void OpenAllDamageColliders()
        {
            if (rightHandWeapon.weaponHook != null)
                rightHandWeapon.weaponHook.OpenDamageColliders();

            if (leftHandWeapon.weaponHook != null)
                leftHandWeapon.weaponHook.OpenDamageColliders();
        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapon.weaponHook != null)
                rightHandWeapon.weaponHook.CloseDamageColliders();

            if (leftHandWeapon.weaponHook != null)
                leftHandWeapon.weaponHook.CloseDamageColliders();
        }

        [System.Serializable]
        public class Weapon
        {
            public string oh_idle;
            public string th_idle;
            public List<Action> actions;
            public List<Action> twoHandedActions;
            public bool LeftHandMirror;
            public GameObject weaponModel;
            public WeaponHook weaponHook;

            public Action GetAction(List<Action> l, ActionInput inp)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].input == inp)
                    {
                        return l[i];
                    }
                }

                return null;
            }


        }

    }
}
