// *********************************************
// *********************************************
// <info>
//   File: ManagedAction.cs
//   Author: Fotos  Frangoudes
//   Project: TONE/Assembly-CSharp
//   Creation Date: 2023/08/24 9:08 AM
//   Last Modification Date: 2023/08/29 6:54 PM
// </info>
// <copyright file="ManagedAction.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;

namespace SourceBase.Utilities.Helpers
{
    /// <summary>
    ///     Managed Action
    /// </summary>
    public partial class ManagedAction
    {
        #region Variables

        protected Action action;

        /// <summary>
        ///     Returns if the actions was already invoked
        /// </summary>
        public bool WasInvoked => isInvoked;

        protected bool isInvoked = false;

        #endregion Variables

        #region ManagedAction

        /// <summary>
        ///     Resets the invocation of the action
        /// </summary>
        public void ResetInvocation()
        {
            isInvoked = false;
        }

        /// <summary>
        ///     Invoke the action
        /// </summary>
        public void Invoke()
        {
            isInvoked = true;
            action?.Invoke();
        }

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public virtual bool RegisterListener(Action listener)
        {
            action += listener;
            if (isInvoked) listener?.Invoke();

            return true;
        }

        /// <summary>
        ///     Unregister an existing listener of the action
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(Action listener)
        {
            action -= listener;
        }

        /// <summary>
        ///     Remove all callbacks listening to the action
        /// </summary>
        public void RemoveAllListeners()
        {
            var listeners = action.GetInvocationList();

            for (int i = 0, n = listeners.Length; i < n; i++)
            {
                action -= (Action)listeners[i];
            }

            action = null;
        }

        #endregion ManagedAction
    }

    /// <summary>
    ///     Managed Action with single argument
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class ManagedAction<T>
    {
        #region Variables

        protected Action<T> action;

        /// <summary>
        ///     Returns if the actions was already invoked
        /// </summary>
        public bool WasInvoked => isInvoked;

        protected bool isInvoked = false;
        protected T lastValue;

        #endregion Variables

        #region ManagedAction

        /// <summary>
        ///     Resets the invocation of the action
        /// </summary>
        public void ResetInvocation()
        {
            isInvoked = false;
            lastValue = default(T);
        }

        /// <summary>
        ///     Invoke the action
        /// </summary>
        public void Invoke(T value)
        {
            isInvoked = true;
            lastValue = value;
            action?.Invoke(value);
        }

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public virtual bool RegisterListener(Action<T> listener)
        {
            action += listener;
            if (isInvoked) listener?.Invoke(lastValue);
            return true;
        }

        /// <summary>
        ///     Unregister an existing listener of the action
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(Action<T> listener)
        {
            action -= listener;
        }

        /// <summary>
        ///     Remove all callbacks listening to the action
        /// </summary>
        public void RemoveAllListeners()
        {
            var listeners = action.GetInvocationList();

            for (int i = 0, n = listeners.Length; i < n; i++)
            {
                action -= (Action<T>)listeners[i];
            }

            action = null;
        }

        #endregion ManagedAction
    }

    /// <summary>
    ///     Managed Action with two arguments
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public partial class ManagedAction<T1, T2>
    {
        #region Variables

        protected Action<T1, T2> action;

        /// <summary>
        ///     Returns if the actions was already invoked
        /// </summary>
        public bool WasInvoked => isInvoked;

        protected bool isInvoked = false;
        protected T1 t1LastValue;
        protected T2 t2LastValue;

        #endregion Variables

        #region ManagedAction

        /// <summary>
        ///     Resets the invocation of the action
        /// </summary>
        public void ResetInvocation()
        {
            isInvoked = false;
            t1LastValue = default(T1);
            t2LastValue = default(T2);
        }

        /// <summary>
        ///     Invoke the action
        /// </summary>
        public void Invoke(T1 t1Value, T2 t2Value)
        {
            isInvoked = true;
            t1LastValue = t1Value;
            t2LastValue = t2Value;
            action?.Invoke(t1Value, t2Value);
        }

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public virtual bool RegisterListener(Action<T1, T2> listener)
        {
            action += listener;
            if (isInvoked) listener?.Invoke(t1LastValue, t2LastValue);
            return true;
        }

        /// <summary>
        ///     Unregister an existing listener of the action
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(Action<T1, T2> listener)
        {
            action -= listener;
        }

        /// <summary>
        ///     Remove all callbacks listening to the action
        /// </summary>
        public void RemoveAllListeners()
        {
            if (action == null) return;

            var listeners = action.GetInvocationList();
            for (int i = 0, n = listeners.Length; i < n; i++)
            {
                action -= (Action<T1, T2>)listeners[i];
            }

            action = null;
        }

        #endregion ManagedAction
    }

    /// <summary>
    ///     Managed Action with three arguments
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public partial class ManagedAction<T1, T2, T3>
    {
        #region Variables

        protected Action<T1, T2, T3> action;

        /// <summary>
        ///     Returns if the actions was already invoked
        /// </summary>
        public bool WasInvoked => isInvoked;

        protected bool isInvoked = false;

        protected T1 t1LastValue;
        protected T2 t2LastValue;
        protected T3 t3LastValue;

        #endregion Variables

        #region ManagedAction

        /// <summary>
        ///     Resets the invocation of the action
        /// </summary>
        public void ResetInvocation()
        {
            isInvoked = false;
            t1LastValue = default(T1);
            t2LastValue = default(T2);
            t3LastValue = default(T3);
        }

        /// <summary>
        ///     Invoke the action
        /// </summary>
        public void Invoke(T1 t1Value, T2 t2Value, T3 t3Value)
        {
            isInvoked = true;
            t1LastValue = t1Value;
            t2LastValue = t2Value;
            t3LastValue = t3Value;
            action?.Invoke(t1Value, t2Value, t3Value);
        }

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public virtual bool RegisterListener(Action<T1, T2, T3> listener)
        {
            action += listener;
            if (isInvoked) listener?.Invoke(t1LastValue, t2LastValue, t3LastValue);
            return true;
        }

        /// <summary>
        ///     Unregister an existing listener of the action
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(Action<T1, T2, T3> listener)
        {
            action -= listener;
        }

        /// <summary>
        ///     Remove all callbacks listening to the action
        /// </summary>
        public void RemoveAllListeners()
        {
            if (action == null) return;

            var listeners = action.GetInvocationList();
            for (int i = 0, n = listeners.Length; i < n; i++)
            {
                action -= (Action<T1, T2, T3>)listeners[i];
            }

            action = null;
        }

        #endregion ManagedAction
    }

    /// <summary>
    ///     Managed Action with four arguments
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public partial class ManagedAction<T1, T2, T3, T4>
    {
        #region Variables

        protected Action<T1, T2, T3, T4> action;

        /// <summary>
        ///     Returns if the actions was already invoked
        /// </summary>
        public bool WasInvoked => isInvoked;

        protected bool isInvoked = false;

        protected T1 t1LastValue;
        protected T2 t2LastValue;
        protected T3 t3LastValue;
        protected T4 t4LastValue;

        #endregion Variables

        #region ManagedAction

        /// <summary>
        ///     Resets the invocation of the action
        /// </summary>
        public void ResetInvocation()
        {
            isInvoked = false;
            t1LastValue = default(T1);
            t2LastValue = default(T2);
            t3LastValue = default(T3);
            t4LastValue = default(T4);
        }

        /// <summary>
        ///     Invoke the action
        /// </summary>
        public void Invoke(T1 t1Value, T2 t2Value, T3 t3Value, T4 t4Value)
        {
            isInvoked = true;
            t1LastValue = t1Value;
            t2LastValue = t2Value;
            t3LastValue = t3Value;
            t4LastValue = t4Value;
            action?.Invoke(t1Value, t2Value, t3Value, t4Value);
        }

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public virtual bool RegisterListener(Action<T1, T2, T3, T4> listener)
        {
            action += listener;
            if (isInvoked) listener?.Invoke(t1LastValue, t2LastValue, t3LastValue, t4LastValue);
            return true;
        }

        /// <summary>
        ///     Unregister an existing listener of the action
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(Action<T1, T2, T3, T4> listener)
        {
            action -= listener;
        }

        /// <summary>
        ///     Remove all callbacks listening to the action
        /// </summary>
        public void RemoveAllListeners()
        {
            if (action == null) return;

            var listeners = action.GetInvocationList();
            for (int i = 0, n = listeners.Length; i < n; i++)
            {
                action -= (Action<T1, T2, T3, T4>)listeners[i];
            }

            action = null;
        }

        #endregion ManagedAction
    }
}