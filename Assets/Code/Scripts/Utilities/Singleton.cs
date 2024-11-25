// *********************************************
// *********************************************
// <info>
//   File: Singleton.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 9:42 AM
//   Last Modification Date: 2023/10/06 5:14 PM
// </info>
// <copyright file="Singleton.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;

using UnityEngine;

namespace SourceBase.Utilities.Helpers
{
    /// <summary>
    ///     Inherit from this base class to create a singleton.
    ///     e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Variables

        // Check to see if we're about to be destroyed.
        protected static object m_Lock = new object();
        protected static bool isQuitting = false;

        protected static T m_Instance;

        #endregion Variables

        #region Instance Properties

        /// <summary>
        ///     Access singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (m_Lock)
                {
#if UNITY_EDITOR
                    try
                    {
                        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                            isQuitting = false;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                    }
#endif
                    if (!isInstanceNull || isQuitting) return m_Instance;

                    FindExistingInstance();

                    // Create new instance if one doesn't already exist.
                    if (isInstanceNull)
                    {
#if UNITY_EDITOR
                        try
                        {
                            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                            {
                                Debug.LogWarning(
                                    $"No singleton instance of type {typeof(T)} found!\nUnable to create one while not playing!");
                                return null;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
#endif
                        // Need to create a new GameObject to attach the singleton to.
                        Instantiate();
                    }

                    SetInstance(m_Instance, true);

                    return m_Instance;
                }
            }

            protected set
            {
                lock (m_Lock)
                {
#if UNITY_EDITOR
                    try
                    {
                        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                        {
#endif
                            m_Instance = value;
#if UNITY_EDITOR
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                    }
#endif
                }
            }
        }

        // get reference to instance that can be null
        // i.e. do not create new instance if it's null
        public static T NullableInstance => m_Instance;

        public static bool isInstanceNull => m_Instance == null;
        public static bool IsDifferentInstance(T newInstance) => m_Instance != newInstance;

        #endregion Instance Properties

        #region Unity Messages

        protected virtual void OnApplicationFocus(bool hasFocus)
        {
            isQuitting = false;
        }

        protected virtual void OnApplicationQuit()
        {
            isQuitting = true;

            lock (m_Lock)
            {
                m_Instance = null;
            }
        }

        #endregion Unity Messages

        #region Instances Management

        /// <summary>
        ///     Create a new instance
        /// </summary>
        protected static void Instantiate()
        {
            lock (m_Lock)
            {
                var singletonObject = new GameObject();
                Instance = singletonObject.AddComponent<T>();
                singletonObject.name = $"{typeof(T).ToString()} (Singleton)";
            }
        }

        /// <summary>
        ///     Set a new instance. If one already exists destroy the new one
        /// </summary>
        /// <param name="newInstance"></param>
        /// <param name="replaceExisting"></param>
        /// <returns></returns>
        public static bool SetInstance(T newInstance, bool replaceExisting = false)
        {
            // If new instance can replace the old new
            if (replaceExisting)
            {
                // if an instance exists destroy it
                DestroyExistingInstance(newInstance);
            }
            else
            {
                // instance should not replace existing instance
                // Check if one already exists, and if it does destroy the new one and return
                if (DestroyNewInstance(newInstance)) return false;
            }

            lock (m_Lock)
            {
                if (!IsDifferentInstance(newInstance)) return true;

                Instance = newInstance;

                return true;
            }
        }

        /// <summary>
        ///     Check to find if an instance already exists
        /// </summary>
        public static void FindExistingInstance()
        {
            lock (m_Lock)
            {
                if (!isInstanceNull) return;

                // Search for existing instance.
                Instance = (T)FindObjectOfType(typeof(T), true);
            }
        }

        /// <summary>
        ///     If an instance already exists, destroy the new instance
        /// </summary>
        /// <returns></returns>
        protected static bool DestroyNewInstance(T newInstance)
        {
            return DestroyInstance(newInstance, !isInstanceNull && IsDifferentInstance(newInstance));
        }

        /// <summary>
        ///     If an instance already exists, destroy it
        /// </summary>
        /// <returns></returns>
        protected static bool DestroyExistingInstance(T newInstance)
        {
            return DestroyInstance(m_Instance, !isInstanceNull && IsDifferentInstance(newInstance));
        }

        /// <summary>
        ///     Destroy the provided instance based on a provided condition
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected static bool DestroyInstance(T instance, bool condition = true)
        {
            if (!condition) return false;

#if UNITY_EDITOR
            try
            {
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                    DestroyImmediate(instance);
                else
#endif
                    Destroy(instance);
#if UNITY_EDITOR
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
#endif
            return true;
        }

        #endregion Instances Management
    }
}