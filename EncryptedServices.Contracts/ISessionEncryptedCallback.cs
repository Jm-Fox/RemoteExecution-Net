namespace EncryptedServices.Contracts
{
    public interface ISessionEncryptedCallback
    {
        void ConfirmEncryption(bool success);
    }
}