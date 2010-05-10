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

    public class PerRequestLifetimeManagerTests
    {
        private readonly PerRequestLifetimeManager lifetimeManager;

        public PerRequestLifetimeManagerTests()
        {
            lifetimeManager = new PerRequestLifetimeManager();
        }

        [Fact]
        public void GetValue_should_return_null_when_value_does_not_exists()
        {
            Assert.Null(lifetimeManager.GetValue());
        }

        [Fact]
        public void Should_be_able_to_set_new_value()
        {
            Assert.DoesNotThrow(() => lifetimeManager.SetValue(new object()));
        }

        [Fact]
        public void GetValue_should_return_value_when_value_exists()
        {
            var value = new object();

            lifetimeManager.SetValue(value);

            Assert.Same(value, lifetimeManager.GetValue());
        }

        [Fact]
        public void Should_be_able_to_remove_value()
        {
            var value = new object();

            lifetimeManager.SetValue(value);
            lifetimeManager.RemoveValue();

            Assert.Null(lifetimeManager.GetValue());
        }

        [Fact]
        public void RemoveValue_should_also_dipose_value_when_disposable()
        {
            var value = new Mock<IDisposable>();
            value.Setup(v => v.Dispose()).Verifiable();

            lifetimeManager.SetValue(value.Object);
            lifetimeManager.RemoveValue();

            value.Verify();
        }

        [Fact]
        public void Setting_null_value_to_existing_value_should_remove_value()
        {
            lifetimeManager.SetValue(new object());
            lifetimeManager.SetValue(null);

            Assert.Null(lifetimeManager.GetValue());
        }

        [Fact]
        public void Setting_new_value_to_existing_disposable_value_should_dispose_existing_value()
        {
            var value = new Mock<IDisposable>();
            value.Setup(v => v.Dispose()).Verifiable();

            lifetimeManager.SetValue(value.Object);
            lifetimeManager.SetValue(new object());

            value.Verify();
        }

        [Fact]
        public void Setting_same_value_should_do_nothing()
        {
            var value = new object();

            lifetimeManager.SetValue(value);
            lifetimeManager.SetValue(value);

            Assert.Same(value, lifetimeManager.GetValue());
        }
    }
}