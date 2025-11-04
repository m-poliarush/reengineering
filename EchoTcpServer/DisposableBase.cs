using System;

namespace EchoServer
{
    public abstract class DisposableBase : IDisposable
    {
        private bool _disposed = false;

        protected bool IsDisposed => _disposed;

        ~DisposableBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeManagedResources();
            }

            DisposeUnmanagedResources();

            _disposed = true;
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeUnmanagedResources()
        {
        }

        protected void CheckDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
    }
}