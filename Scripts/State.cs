using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.StateMachine
{
    public abstract class State
    {
        public StateMachine SM { get; set; }

        public abstract string StateId { get; }

        /// <summary>
        /// No registration
        /// </summary>
        /// <param name="stateMachine"></param>
        public State(StateMachine stateMachine)
        {
            this.SM = stateMachine;
            SM.RegisterState(this);
        }

        public virtual void SetCustomProperties(object arg, string tag = "") { }

        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void PreUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
    }
}
