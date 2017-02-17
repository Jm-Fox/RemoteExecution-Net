using System;
using EncryptedServices.Contracts;
using RemoteExecution;

namespace EncryptedServices.Server
{
    class Program
    {
        public static readonly BasicLidgrenCryptoProviderResolver ProviderResolver = new BasicLidgrenCryptoProviderResolver();
        static void Main(string[] args)
		{
			EncryptedConfigurator.Configure(ProviderResolver);
			using (var host = new Host("net://127.0.0.1:3133/EncryptedServices"))
			{
				host.Start();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
    }
}
