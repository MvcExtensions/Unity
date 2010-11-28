#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Unity.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Mvc;

    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    using Moq;
    using Xunit;
    using Xunit.Extensions;

    public class UnityAdapterTests
    {
        private readonly Mock<IUnityContainer> container;
        private readonly UnityAdapter adapter;

        public UnityAdapterTests()
        {
            container = new Mock<IUnityContainer>();
            adapter = new UnityAdapter(container.Object);
        }

        [Fact]
        public void Dispose_should_also_dispose_container()
        {
            container.Setup(c => c.Dispose());

            adapter.Dispose();

            container.VerifyAll();
        }

        [Theory]
        [InlineData(LifetimeType.PerRequest)]
        [InlineData(LifetimeType.Singleton)]
        [InlineData(LifetimeType.Transient)]
        public void Should_be_able_to_register_type(LifetimeType lifetime)
        {
            if (lifetime == LifetimeType.PerRequest)
            {
                container.Setup(c => c.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<PerRequestLifetimeManager>(), It.IsAny<InjectionMember[]>())).Verifiable();
            }
            else if (lifetime == LifetimeType.Singleton)
            {
                container.Setup(c => c.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ContainerControlledLifetimeManager>(), It.IsAny<InjectionMember[]>())).Verifiable();
            }
            else if (lifetime == LifetimeType.Transient)
            {
                container.Setup(c => c.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<TransientLifetimeManager>(), It.IsAny<InjectionMember[]>())).Verifiable();
            }

            adapter.RegisterType(null, typeof(DummyObject), typeof(DummyObject), lifetime);

            container.Verify();
        }

        [Fact]
        public void Should_be_able_to_register_named_service()
        {
            container.Setup(c => c.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), "foo", It.IsAny<TransientLifetimeManager>(), It.IsAny<InjectionMember[]>())).Verifiable();

            adapter.RegisterType("foo", typeof(DummyObject), typeof(DummyObject), LifetimeType.Transient);

            container.Verify();
        }

        [Fact]
        public void Should_register_the_implementation_type_as_service_name_when_same_service_already_exists()
        {
            var registration = (ContainerRegistration)Activator.CreateInstance(typeof(ContainerRegistration), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { typeof(DummyObject), null, new PolicyList() }, CultureInfo.CurrentCulture);

            var registrations = new List<ContainerRegistration> { registration };

            container.SetupGet(c => c.Registrations).Returns(registrations);
            container.Setup(c => c.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), typeof(DummyObject).FullName, It.IsAny<TransientLifetimeManager>(), It.IsAny<InjectionMember[]>())).Verifiable();

            adapter.RegisterType(null, typeof(DummyObject), typeof(DummyObject), LifetimeType.Transient);

            container.Verify();
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("")]
        public void Should_be_able_to_register_instance(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                container.Setup(c => c.RegisterInstance(It.IsAny<Type>(), null, It.IsAny<object>(), It.IsAny<LifetimeManager>())).Verifiable();
            }
            else
            {
                container.Setup(c => c.RegisterInstance(It.IsAny<Type>(), key, It.IsAny<object>(), It.IsAny<LifetimeManager>())).Verifiable();
            }

            adapter.RegisterInstance(key, typeof(DummyObject), new DummyObject());

            container.Verify();
        }

        [Fact]
        public void Should_be_able_to_inject()
        {
            var dummy = new DummyObject();

            container.Setup(c => c.BuildUp(It.IsAny<Type>(), It.IsAny<DummyObject>(), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()));

            adapter.Inject(dummy);

            container.VerifyAll();
        }

        [Fact]
        public void Should_be_able_to_get_service_by_type()
        {
            container.Setup(c => c.Resolve(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()));

            adapter.GetService<DummyObject>();

            container.VerifyAll();
        }

        [Fact]
        public void Should_be_able_to_get_service_by_type_and_key()
        {
            container.Setup(c => c.Resolve(It.IsAny<Type>(), It.IsAny<string>()));

            adapter.GetService<DummyObject>("foo");

            container.VerifyAll();
        }

        [Fact]
        public void Should_be_able_to_get_services()
        {
            var registration = (ContainerRegistration)Activator.CreateInstance(typeof(ContainerRegistration), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { typeof(DummyObject), null, new PolicyList() }, CultureInfo.CurrentCulture);

            var registrations = new List<ContainerRegistration> { registration };

            container.SetupGet(c => c.Registrations).Returns(registrations);

            container.Setup(c => c.Resolve(typeof(DummyObject), It.IsAny<string>(), It.IsAny<ResolverOverride[]>())).Returns(new DummyObject()).Verifiable();
            container.Setup(c => c.ResolveAll(typeof(DummyObject), It.IsAny<ResolverOverride[]>())).Returns(new[] { new DummyObject() }).Verifiable();

            adapter.GetServices(typeof(DummyObject));

            container.Verify();
        }

        private class DummyObject
        {
        }
    }
}