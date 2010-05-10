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
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Defines an adapter class which with backed by Unity <seealso cref="IUnityContainer">Container</seealso>.
    /// </summary>
    public class UnityAdapter : ServiceLocatorImplBase, IServiceRegistrar, IServiceInjector, IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityAdapter(IUnityContainer container)
        {
            Invariant.IsNotNull(container, "container");

            Container = container;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="UnityAdapter"/> is reclaimed by garbage collection.
        /// </summary>
        [DebuggerStepThrough]
        ~UnityAdapter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IUnityContainer Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [DebuggerStepThrough]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns></returns>
        public virtual IServiceRegistrar RegisterType(string key, Type serviceType, Type implementationType, LifetimeType lifetime)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(implementationType, "implementationType");

            LifetimeManager lifeTimeManager = (lifetime == LifetimeType.PerRequest) ?
                                              new PerRequestLifetimeManager() :
                                              ((lifetime == LifetimeType.Singleton) ?
                                              new ContainerControlledLifetimeManager() :
                                              new TransientLifetimeManager() as LifetimeManager);

            if (string.IsNullOrEmpty(key))
            {
                if (Container.Registrations.Any(registration => registration.RegisteredType == serviceType))
                {
                    Container.RegisterType(serviceType, implementationType, implementationType.FullName, lifeTimeManager);
                }
                else
                {
                    Container.RegisterType(serviceType, implementationType, lifeTimeManager);
                }
            }
            else
            {
                Container.RegisterType(serviceType, implementationType, key, lifeTimeManager);
            }

            return this;
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public virtual IServiceRegistrar RegisterInstance(string key, Type serviceType, object instance)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(instance, "instance");

            if (string.IsNullOrEmpty(key))
            {
                Container.RegisterInstance(serviceType, instance);
            }
            else
            {
                Container.RegisterInstance(serviceType, key, instance);
            }

            return this;
        }

        /// <summary>
        /// Injects the matching dependences.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Inject(object instance)
        {
            if (instance != null)
            {
                Container.BuildUp(instance.GetType(), instance);
            }
        }

        /// <summary>
        /// Gets the matching instance for the given type and key.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return string.IsNullOrEmpty(key) ? Container.Resolve(serviceType) : Container.Resolve(serviceType, key);
        }

        /// <summary>
        /// Gets all the instances for the given type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            Invariant.IsNotNull(serviceType, "serviceType");

            List<object> instances = new List<object>();

            if (Container.Registrations.Any(registration => registration.RegisteredType == serviceType && string.IsNullOrEmpty(registration.Name)))
            {
                instances.Add(Container.Resolve(serviceType));
            }

            instances.AddRange(Container.ResolveAll(serviceType));

            return instances;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [DebuggerStepThrough]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                Container.Dispose();
            }

            disposed = true;
        }
    }
}