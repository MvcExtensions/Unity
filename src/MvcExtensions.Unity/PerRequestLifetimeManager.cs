#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Unity
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// Defines a <see cref="LifetimeManager"/> which returns the same object for a web request.
    /// </summary>
    public class PerRequestLifetimeManager : LifetimeManager
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            IDictionary<PerRequestLifetimeManager, object> lifetimeManagers = UnityMvcApplication.GetPerRequestLifetimeManagers();
            object value;

            lifetimeManagers.TryGetValue(this, out value);

            return value;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        public override void SetValue(object newValue)
        {
            if (newValue == null)
            {
                RemoveValue();
                return;
            }

            IDictionary<PerRequestLifetimeManager, object> lifetimeManagers = UnityMvcApplication.GetPerRequestLifetimeManagers();
            object value;

            if (lifetimeManagers.TryGetValue(this, out value))
            {
                if ((value != null) && ReferenceEquals(value, newValue))
                {
                    // Setting the same object so exit
                    return;
                }

                DisposeValue(value);
            }

            lifetimeManagers[this] = newValue;
        }

        /// <summary>
        /// Removes the value.
        /// </summary>
        public override void RemoveValue()
        {
            IDictionary<PerRequestLifetimeManager, object> lifetimeManagers = UnityMvcApplication.GetPerRequestLifetimeManagers();

            object value;

            if (!lifetimeManagers.TryGetValue(this, out value))
            {
                return;
            }

            DisposeValue(value);
            lifetimeManagers.Remove(this);
        }

        private static void DisposeValue(object value)
        {
            if (value == null)
            {
                return;
            }

            IDisposable disposable = value as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}