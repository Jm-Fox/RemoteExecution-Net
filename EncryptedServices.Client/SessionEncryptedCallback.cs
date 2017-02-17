using System.Threading;
using EncryptedServices.Contracts;

namespace EncryptedServices.Client
{
    public class SessionEncryptedCallback : ISessionEncryptedCallback
    {
        public bool Success { get; private set; }

        private readonly AutoResetEvent handle = new AutoResetEvent(false);

        public void Wait()
        {
            handle.WaitOne();
        }

        public void ConfirmEncryption(bool success)
        {
            Success = success;
            handle.Set();
        }
    }
}
