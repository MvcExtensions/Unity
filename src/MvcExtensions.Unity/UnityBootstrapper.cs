#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Unity
{
    using System;
    using System.Linq;
    using System.Web;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// Defines a <seealso cref="Bootstrapper">Bootstrapper</seealso> which is backed by <seealso cref="UnityAdapter"/>.
    /// </summary>
    public class UnityBootstrapper : Bootstrapper
    {
        private static readonly Type moduleType = typeof(IModule);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityBootstrapper"/> class.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        public UnityBootstrapper(IBuildManager buildManager) : base(buildManager)
        {
        }

        /// <summary>
        /// Creates the container adapter.
        /// </summary>
        /// <returns></returns>
        protected override ContainerAdapter CreateAdapter()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<HttpContextBase, HttpContextWrapper>(new InjectionFactory(c => new HttpContextWrapper(HttpContext.Current)));

            UnityAdapter adapter = new UnityAdapter(container);

            return adapter;
        }

        /// <summary>
        /// Loads the container specific modules.
        /// </summary>
        protected override void LoadModules()
        {
            IUnityContainer container = ((UnityAdapter)Adapter).Container;

            BuildManager.ConcreteTypes
                        .Where(type => moduleType.IsAssignableFrom(type) && type.HasDefaultConstructor())
                        .Select(type => Activator.CreateInstance(type))
                        .Cast<IModule>()
                        .Each(module => module.Load(container));
        }
    }
}