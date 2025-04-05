using System;
using System.Collections.Generic;
using System.Reflection;

namespace GenericTelemetryProvider
{
    public abstract class StateMachineBase<T> where T : Enum
    {
        public Action<T> OnStateEnter { get; set; }

        private readonly Dictionary<(T, string), MethodInfo> _stateMethods;

        protected T activeState;

        protected StateMachineBase(T initialState)
        {
            _stateMethods = new Dictionary<(T, string), MethodInfo>();

            // Pre-cache method info for each enum value and action ("Enter", "Update", "Exit")
            foreach (T state in Enum.GetValues(typeof(T)))
            {
                foreach (string action in new[] { "Enter", "Update", "Exit" })
                {
                    string methodName = $"State_{state}_{action}";
                    // Search for both public and non-public instance methods
                    MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (method != null)
                    {
                        _stateMethods[(state, action)] = method;
                    }
                }
            }

            EnterState(initialState, false);
        }

        protected void InvokeStateMethod(T state, string action)
        {
            if (_stateMethods.TryGetValue((state, action), out MethodInfo method))
            {
                method.Invoke(this, null);
            }
            //else
            //{
            //    throw new InvalidOperationException($"No method defined for state '{state}' with action '{action}'");
            //}
        }

        public void EnterState(T state, bool callExit = true)
        {
            if(callExit)
                ExitState(activeState);

            activeState = state;
            InvokeStateMethod(state, "Enter");
            OnStateEnter?.Invoke(state);
        }

        public void UpdateState(T state)
        {
            InvokeStateMethod(state, "Update");
        }

        public void ExitState(T state)
        {
            InvokeStateMethod(state, "Exit");
        }

        public void Update()
        {
            UpdateState(activeState);
        }
    }
}
