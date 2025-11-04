using NUnit.Framework;
using EchoServer;
using System;

namespace EchoServer.Tests
{
    [TestFixture]
    public class DisposableBaseTests
    {
        private class DummyDisposable : DisposableBase
        {
        }

        private class AdvancedDummyDisposable : DisposableBase
        {
            public static bool FinalizerHookCalled = false;

            public bool IsDisposedPublic => IsDisposed;

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                {
                    FinalizerHookCalled = true;
                }
                base.Dispose(disposing);
            }
        }

        [SetUp]
        public void SetUp()
        {
            AdvancedDummyDisposable.FinalizerHookCalled = false;
        }

        [Test]
        public void IsDisposedProperty_ShouldReflectState()
        {
            var d = new AdvancedDummyDisposable();
            Assert.That(d.IsDisposedPublic, Is.False);
            d.Dispose();
            Assert.That(d.IsDisposedPublic, Is.True);
        }

        [Test]
        public void Dispose_ShouldCoverEmptyBaseVirtualMethods()
        {
            var d = new DummyDisposable();
            d.Dispose();
            Assert.Pass();
        }

        [Test]
        public void Finalizer_ShouldRun_WhenDisposeIsNotCalled()
        {
            CreateAndForgetObject();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.That(AdvancedDummyDisposable.FinalizerHookCalled, Is.True);
        }

        private void CreateAndForgetObject()
        {
            var d = new AdvancedDummyDisposable();
        }
    }
}