#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Unity.Tests
{
    using System;

    using Moq;
    using Xunit;

    public class UnityMvcApplicationTests
    {
        [Fact]
        public void Should_be_able_to_create_bootstrapper()
        {
            var application = new UnityMvcApplication();

            Assert.NotNull(application.Bootstrapper);
            Assert.IsType<UnityBootstrapper>(application.Bootstrapper);
        }

        [Fact]
        public void Should_remove_services_when_per_request_tasks_disposes()
        {
            var application = new UnityMvcApplicationTestDouble();

            var value1 = new Mock<IDisposable>();
            var value2 = new Mock<IDisposable>();

            new PerRequestLifetimeManager().SetValue(value1.Object);
            new PerRequestLifetimeManager().SetValue(value2.Object);

            value1.Setup(v => v.Dispose()).Verifiable();
            value2.Setup(v => v.Dispose()).Verifiable();

            application.PerRequestTaskCompleted();

            value1.Verify();
            value2.Verify();
        }

        private sealed class UnityMvcApplicationTestDouble : UnityMvcApplication
        {
            public void PerRequestTaskCompleted()
            {
                OnPerRequestTasksDisposed();
            }
        }
    }
}