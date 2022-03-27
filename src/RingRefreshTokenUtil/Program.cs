using RingSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RingRefreshTokenUtil
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string username = null;
            string password = null;
            string twoFactorCode = null;

            if (args is null || !args.Any())
            {
                var rawInput = Console.ReadLine();
                args = rawInput.Split(' ');
            }

            bool valid = args != null && args.Length >= 2;
            if (valid)
            {
                username = args[0];
                password = args[1];

                valid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
            }

            if (valid && args.Length > 2)
            {
                twoFactorCode = args[2];
            }

            if (!valid)
            {
                Console.WriteLine("Invalid arguments. Valid examples:\nRingRefreshTokenUtil.exe <username> <password>\nRingRefreshTokenUtil.exe <username> <password> <2fa code>");
                return;
            }

            var client = new RingClient();
            var refreshToken = await client.AuthenticateAsync(username, password, twoFactorCode).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                Console.WriteLine("A refresh token was not received. Check your email or text messages for a 2fa code and run again with the code. Example:\nRingRefreshTokenUtil.exe <username> <password> <2fa code>");
            }
            else
            {
                Console.WriteLine(refreshToken);
            }
        }
    }
}