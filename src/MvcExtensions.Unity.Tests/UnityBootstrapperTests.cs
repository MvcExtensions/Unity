#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Unity.Tests
{
    using Microsoft.Practices.Unity;

    using Moq;
    using Xunit;

    public class UnityBootstrapperTests
    {
        [Fact]
        public void Should_be_able_to_create_adapter()
        {
            var buildManager = new Mock<IBuildManager>();
            buildManager.SetupGet(bm => bm.Assemblies).Returns(new[] { GetType().Assembly });

            var bootstrapper = new UnityBootstrapper(buildManager.Object, new Mock<IBootstrapperTasksRegistry>().Object, new Mock<IPerRequestTasksRegistry>().Object);

            Assert.IsType<UnityAdapter>(bootstrapper.Adapter);
        }

        [Fact]
        public void Should_be_able_to_load_modules()
        {
            var buildManager = new Mock<IBuildManager>();
            buildManager.SetupGet(bm => bm.ConcreteTypes).Returns(new[] { typeof(DummyModule) });

            var bootstrapper = new UnityBootstrapper(buildManager.Object, new Mock<IBootstrapperTasksRegistry>().Object, new Mock<IPerRequestTasksRegistry>().Object);

            DummyModule.Loaded = false;

            Assert.IsType<UnityAdapter>(bootstrapper.Adapter);

            Assert.True(DummyModule.Loaded);
        }

        private sealed class DummyModule : IModule
        {
            public static bool Loaded { get; set; }

            public void Load(IUnityContainer container)
            {
                Loaded = true;
            }
        }
    }
}