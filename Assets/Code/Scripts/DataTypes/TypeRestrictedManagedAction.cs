// *********************************************
// *********************************************
// <info>
//   File: TypeRestrictedManagedAction.cs
//   Author: Fotos  Frangoudes
//   Project: TONE/Assembly-CSharp
//   Creation Date: 2023/06/23 8:45 AM
//   Last Modification Date: 2023/07/04 4:07 PM
// </info>
// <copyright file="TypeRestrictedManagedAction.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;

namespace SourceBase.Utilities.Helpers
{
    /// <summary>
    ///     Managed Action with a restriction on the type of objects that can listen to the action
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public partial class TypeRestrictedManagedAction<S> : ManagedAction
    {
        #region TypeRestrictedManagedAction

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public override bool RegisterListener(Action listener)
        {
            // Check the listener is of the correct type
            return listener.Target.GetType().IsSubclassOf(typeof(S)) && base.RegisterListener(listener);
        }

        #endregion TypeRestrictedManagedAction
    }

    /// <summary>
    ///     Managed Action with single argument
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public partial class TypeRestrictedManagedAction<S, T> : ManagedAction<T>
    {
        #region TypeRestrictedManagedAction

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public override bool RegisterListener(Action<T> listener)
        {
            // Check the listener is of the correct type
            return listener.Target.GetType().IsSubclassOf(typeof(S)) && base.RegisterListener(listener);
        }

        #endregion TypeRestrictedManagedAction
    }

    /// <summary>
    ///     Managed Action with two arguments
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public partial class TypeRestrictedManagedAction<S, T1, T2> : ManagedAction<T1, T2>
    {
        #region TypeRestrictedManagedAction

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public override bool RegisterListener(Action<T1, T2> listener)
        {
            // Check the listener is of the correct type
            return listener.Target.GetType().IsSubclassOf(typeof(S)) && base.RegisterListener(listener);
        }

        #endregion TypeRestrictedManagedAction
    }

    /// <summary>
    ///     Managed Action with three arguments
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public partial class TypeRestrictedManagedAction<S, T1, T2, T3> : ManagedAction<T1, T2, T3>
    {
        #region TypeRestrictedManagedAction

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public override bool RegisterListener(Action<T1, T2, T3> listener)
        {
            // Check the listener is of the correct type
            return listener.Target.GetType().IsSubclassOf(typeof(S)) && base.RegisterListener(listener);
        }

        #endregion TypeRestrictedManagedAction
    }

    /// <summary>
    ///     Managed Action with four arguments
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public partial class TypeRestrictedManagedAction<S, T1, T2, T3, T4> : ManagedAction<T1, T2, T3, T4>
    {
        #region TypeRestrictedManagedAction

        /// <summary>
        ///     Register a new listener to the action
        /// </summary>
        /// <param name="listener"></param>
        public override bool RegisterListener(Action<T1, T2, T3, T4> listener)
        {
            // Check the listener is of the correct type
            return listener.Target.GetType().IsSubclassOf(typeof(S)) && base.RegisterListener(listener);
        }

        #endregion TypeRestrictedManagedAction
    }
}