#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Unity
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Defines a <see cref="HttpApplication"/> which uses <seealso cref="UnityBootstrapper"/>.
    /// </summary>
    public class UnityMvcApplication : ExtendedMvcApplication
    {
        private static readonly object key = typeof(UnityMvcApplication).FullName;

        internal static IDictionary<PerRequestLifetimeManager, object> GetPerRequestLifetimeManagers()
        {
            IDictionary backingStore = BackingStore.Get();
            IDictionary<PerRequestLifetimeManager, object> instances;

            if (backingStore.Contains(key))
            {
                instances = backingStore[key] as IDictionary<PerRequestLifetimeManager, object>;
            }
            else
            {
                lock (backingStore)
                {
                    instances = backingStore.Contains(key) ?
                                backingStore[key] as IDictionary<PerRequestLifetimeManager, object> :
                                new Dictionary<PerRequestLifetimeManager, object>();

                    if (!backingStore.Contains(key))
                    {
                        backingStore.Add(key, instances);
                    }
                }
            }

            return instances;
        }

        /// <summary>
        /// Creates the bootstrapper.
        /// </summary>
        /// <returns></returns>
        protected override IBootstrapper CreateBootstrapper()
        {
            return new UnityBootstrapper(BuildManagerWrapper.Current);
        }

        /// <summary>
        /// Executes after the registered <see cref="PerRequestTask"/> disposes.
        /// </summary>
        protected override void OnPerRequestTasksDisposed()
        {
            RemoveAllInstances();
        }

        private static void RemoveAllInstances()
        {
            IDictionary<PerRequestLifetimeManager, object> lifetimeManagers = GetPerRequestLifetimeManagers();

            PerRequestLifetimeManager[] managers = new PerRequestLifetimeManager[lifetimeManagers.Count];

            lifetimeManagers.Keys.CopyTo(managers, 0);

            managers.Each(lifetimeManager => lifetimeManager.RemoveValue());

            lifetimeManagers.Clear();
        }
    }
}