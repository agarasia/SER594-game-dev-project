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
            Weapon w = stateManager.inventoryManager.currentWeapon;

            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetAction(w.actions[i].input);
                a.targetAnimation = w.actions[i].targetAnimation;
            }
        }

        public void UpdateActionsWithCurrentWeaponTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = stateManager.inventoryManager.currentWeapon;

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
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnimation;
        public string itemID;
    }
}
