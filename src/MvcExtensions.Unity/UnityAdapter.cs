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
    using System.Linq;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// Defines an adapter class which is backed by Unity <seealso cref="IUnityContainer">Container</seealso>.
    /// </summary>
    public class UnityAdapter : ContainerAdapter
    {
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
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IUnityContainer Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterType(Type serviceType, Type implementationType, LifetimeType lifetime)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(implementationType, "implementationType");

            LifetimeManager lifeTimeManager;
            switch (lifetime)
            {
                case LifetimeType.PerRequest:
                    lifeTimeManager = new PerRequestLifetimeManager();
                    break;
                case LifetimeType.Singleton:
                    lifeTimeManager = new ContainerControlledLifetimeManager();
                    break;
                default:
                    lifeTimeManager = new TransientLifetimeManager();
                    break;
            }

            if (Container.Registrations.Any(registration => registration.RegisteredType.Equals(serviceType)))
            {
                Container.RegisterType(serviceType, implementationType, implementationType.FullName, lifeTimeManager);
            }
            else
            {
                Container.RegisterType(serviceType, implementationType, lifeTimeManager);
            }

            return this;
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterInstance(Type serviceType, object instance)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(instance, "instance");
            
            Container.RegisterInstance(serviceType, instance);

            return this;
        }

        /// <summary>
        /// Injects the matching dependences.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public override void Inject(object instance)
        {
            if (instance != null)
            {
                Container.BuildUp(instance.GetType(), instance);
            }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override object DoGetService(Type serviceType)
        {
            return Container.IsRegistered(serviceType) ? Container.Resolve(serviceType) : null;
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetServices(Type serviceType)
        {
            Invariant.IsNotNull(serviceType, "serviceType");

            var instances = new List<object>();

            if (Container.Registrations.Any(registration => registration.RegisteredType.Equals(serviceType) && string.IsNullOrEmpty(registration.Name)))
            {
                instances.Add(Container.Resolve(serviceType));
            }

            instances.AddRange(Container.ResolveAll(serviceType));

            return instances;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        protected override void DisposeCore()
        {
            Container.Dispose();
        }
    }
}