using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Fsm
{
    public abstract class FsmManagerBase : MonoBehaviour
    {
        protected Dictionary<int, FsmStateBase> m_States = new Dictionary<int, FsmStateBase>();
        protected int m_CurrentStateKey;

        protected void Awake()
        {
            InitStates();
            foreach (var pair in m_States)
            {
                pair.Value.Init();
            }
            m_CurrentStateKey = GetEntryStateKey();
        }

        protected void Update()
        {
            m_States.TryGetValue(m_CurrentStateKey, out var currentState);
            if (currentState == null)
                return;

            var stateKey = currentState.Execute();
            // if returns an invalid key, keeps its current state
            if (stateKey != m_CurrentStateKey && m_States.TryGetValue(stateKey, out var nextState))
            {
                currentState.Exit(nextState);
                nextState.Enter(currentState);
                m_CurrentStateKey = stateKey;
            }
        }

        /// <summary>
        /// Override this function to fill states to the dictionary m_States.
        /// </summary>
        protected abstract void InitStates();
        /// <summary>
        /// Override this function to tell the manager which state you want it to be the entry state.
        /// Return the state's key.
        /// </summary>
        protected abstract int GetEntryStateKey();
    }
}
