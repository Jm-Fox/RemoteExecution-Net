using EncryptedServices.Contracts;
using Lidgren.Network;
using RemoteExecution;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading;

namespace EncryptedServices.Client
{
    internal class Program
    {
        private static readonly NetPeer EmptyPeer = new NetPeer(new NetPeerConfiguration("*"));

        static void Main(string[] args)
        {
            BasicLidgrenCryptoProviderResolver resolver = new BasicLidgrenCryptoProviderResolver();
            EncryptedConfigurator.Configure(resolver);

            IOperationDispatcher callbackDispatcher = new OperationDispatcher();
            SessionEncryptedCallback callback = new SessionEncryptedCallback();
            callbackDispatcher.RegisterHandler<ISessionEncryptedCallback>(callback);

            using (ClientConnection client = new ClientConnection("net://localhost:3133/EncryptedServices", callbackDispatcher))
            {
                Console.WriteLine("Opening client...");
                Console.WriteLine();
                client.Open();
                
                IAuthenticator authenticator = client.RemoteExecutor.Create<IAuthenticator>();

                Console.WriteLine("Note that encryption is not strictly enforced. Enforcement is left up to the developer.");
                Console.WriteLine("11 + 6 = " + authenticator.Add(11, 6));
                Console.WriteLine();

                Console.WriteLine("Generating secret key...");
                byte[] secretBytes = CreateByteKey(96);
                string secretString = Convert.ToBase64String(secretBytes);
                Console.WriteLine("Secret key is " + secretString);
                Console.WriteLine();

                Console.WriteLine("Encrypting secret key...");
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(authenticator.GetRsaParameters().ToParams());
                byte[] encryptedBytes = rsa.Encrypt(secretBytes, false);
                Console.WriteLine("Encrypted key is " + Convert.ToBase64String(encryptedBytes));
                Console.WriteLine();

                Console.WriteLine("Establishing encrypted session...");
                authenticator.EncryptSession(encryptedBytes);
                
                Console.WriteLine("Awaiting encryption confirmation...");
                callback.Wait();
                Console.WriteLine("Encryption confirmed; registering crypto provider...");
                resolver.Register(new IPEndPoint(IPAddress.Loopback, 3133),
                    new DebugNetAESEncryption(EmptyPeer, secretString));
                Console.WriteLine("Connection is encrypted.");
                const int wait = 160;
                Console.WriteLine($"Pausing for {wait} milliseconds to ensure server has time to register crypto provider...");
                Thread.Sleep(wait);
                Console.WriteLine("Done waiting.");
                Console.WriteLine();

                Console.WriteLine("5 + 6 = " + authenticator.Add(5, 6));
                Console.WriteLine();

                Console.WriteLine("Done. Press enter to exit.");
                Console.ReadLine();
            }
        }

        private static byte[] CreateByteKey(int size)
        {
            byte[] secretBytes = new byte[size];
            RandomNumberGenerator.Create().GetBytes(secretBytes);
            return secretBytes;
        }
    }
}
