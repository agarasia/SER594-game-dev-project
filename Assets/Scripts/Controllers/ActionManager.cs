using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoulsLike.InventoryManager;

namespace SoulsLike
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();
        StateManager stateManager;
        public ItemAction consumeables;

        public void Init(StateManager st)
        {
            stateManager = st;
            UpdateActionsWithCurrentWeapon();
        }

        public void UpdateActionsWithCurrentWeapon()
        {
            EmptyAllSlots();

            if (stateManager.inventoryManager.hasLeftHandWeapon)
            {
                UpdateActionsWithLeftHand();
                return;
            }
            Weapon w = stateManager.inventoryManager.rightHandWeapon;

            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetAction(w.actions[i].input);
                a.targetAnimation = w.actions[i].targetAnimation;
            }
        }

        public void UpdateActionsWithLeftHand()
        {
            Weapon r_w = stateManager.inventoryManager.rightHandWeapon;
            Weapon l_w = stateManager.inventoryManager.leftHandWeapon;

            Action rb = GetAction(ActionInput.rb);
            Action rt = GetAction(ActionInput.rt);
            rb.targetAnimation = r_w.GetAction(r_w.actions, ActionInput.rb).targetAnimation;
            rt.targetAnimation = r_w.GetAction(r_w.actions, ActionInput.rt).targetAnimation;

            Action lb = GetAction(ActionInput.lb);
            Action lt = GetAction(ActionInput.lt);
            lb.targetAnimation = l_w.GetAction(l_w.actions, ActionInput.rb).targetAnimation;
            lt.targetAnimation = l_w.GetAction(l_w.actions, ActionInput.rt).targetAnimation;

            if (l_w.LeftHandMirror)
            {
                lb.mirror = true;
                lt.mirror = true;
            }

        }

        public void UpdateActionsWithCurrentWeaponTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = stateManager.inventoryManager.rightHandWeapon;

            for (int i = 0; i < w.twoHandedActions.Count; i++)
            {
                Action a = GetAction(w.twoHandedActions[i].input);
                a.targetAnimation = w.twoHandedActions[i].targetAnimation;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
                a.targetAnimation = null;
                a.mirror = false;
            }
        }

        ActionManager()
        {
            if (actionSlots.Count != 0)
                return;

            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return GetAction(a_input);
        }

        private Action GetAction(ActionInput input)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == input)
                    return actionSlots[i];

            }
            return null;
        }

        public ActionInput GetActionInput(StateManager st)
        {
            if (st.rb)
                return ActionInput.rb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lb)
                return ActionInput.lb;
            if (st.lt)
                return ActionInput.lt;

            return ActionInput.rb;
        }
    }

    public enum ActionInput
    {
        rb, rt, lb, lt, x
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public string targetAnimation;
        public bool mirror = false;
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnimation;
        public string itemID;
    }
}
