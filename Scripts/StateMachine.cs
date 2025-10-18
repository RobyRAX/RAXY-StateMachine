using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;


#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace RAXY.StateMachine
{
    public abstract class StateMachine
    {
        public State CurrentState { get; set; }
        public Dictionary<string, State> StateDict { get; set; } = new();

#if UNITY_EDITOR
        [ShowInInspector, HideReferenceObjectPicker, HideLabel]
        private StateMachineDrawer _drawer;
#endif

        public StateMachine()
        {
#if UNITY_EDITOR
            _drawer = new StateMachineDrawer(this);
#endif
        }

        /// <summary>
        /// Change the State without changing the Sub State
        /// </summary>
        public virtual void ChangeState(State newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void ChangeState(string stateId)
        {
            if (StateDict.TryGetValue(stateId, out State selectedState))
            {
                ChangeState(selectedState);
            }
            else
            {
                Debug.LogWarning($"{stateId} doesn't exist");
            }
        }

        public void RegisterState(State state)
        {
            if (!StateDict.ContainsKey(state.StateId))
            {
                StateDict.Add(state.StateId, state);
            }
        }
    }

#if UNITY_EDITOR
    [Serializable]
    public class StateMachineDrawer
    {
        [NonSerialized]
        private readonly StateMachine target;

        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, ElementColor = "GetElementColor", ListElementLabelName = "Label", OnTitleBarGUI = "DrawRefreshButton")]
        [ShowInInspector, LabelText("States (Editor Only)")]
        [HideReferenceObjectPicker]
        private List<StateDrawerEntry> _editorStates = new();

        public StateMachineDrawer(StateMachine target)
        {
            this.target = target;
            _ = DelayInitAsync();
        }

        async UniTask DelayInitAsync()
        {
            await UniTask.WaitForSeconds(0.5f);
            RefreshFromTarget();
        }

        // Refresh the list from the target dictionary
        private void RefreshFromTarget()
        {
            if (target == null)
                return;

            _editorStates = target.StateDict
                .Select(kvp => new StateDrawerEntry { State = kvp.Value })
                .ToList();
        }

        private void DrawRefreshButton()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                RefreshFromTarget();
            }
        }

        private Color GetElementColor(int index)
        {
            if (target == null || index < 0 || index >= _editorStates.Count)
                return Color.clear; // don't draw anything

            bool isActive = _editorStates[index].State == target.CurrentState;

            if (isActive)
            {
                return new Color(0.12f, 0.45f, 0.90f, 0.18f);
            }
            else
            {
                bool isEven = (index % 2 == 0);

                Color baseColor;
                if (EditorGUIUtility.isProSkin)
                    baseColor = isEven ? new Color(0.219f, 0.219f, 0.219f) : new Color(0.192f, 0.192f, 0.192f);
                else
                    baseColor = isEven ? new Color(0.925f, 0.925f, 0.925f) : new Color(0.961f, 0.961f, 0.961f);

                return baseColor;
            }
        }
    }

    [Serializable]
    public class StateDrawerEntry
    {
        [HideLabel]
        [HideReferenceObjectPicker]
        public State State;

        public string Label
        {
            get
            {
                if (State.SM != null)
                {
                    if (State.SM.CurrentState == State)
                    {
                        return $"● {State.StateId}";
                    }
                    else
                    {
                        return State.StateId;
                    }
                }
                else
                {
                    return State.StateId;
                }
            }
        }
    }
#endif
}