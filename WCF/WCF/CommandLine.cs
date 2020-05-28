using System;

namespace WCF
{
    public class CommandLine
    {
        public bool Client { get; set; }

        public bool Server { get; set; }

        public int Port { get; set; }

        public string Upn { get; set; }

        public string Host { get; set; }

        public bool IsValid()
        {
            if (!Server && !Client)
            {
                Console.WriteLine("-server or -client must be specified");
                return false;
            }

            if (Server && Client)
            {
                Console.WriteLine("-server or -client cannot both be specified");
                return false;
            }

            if (Server && Port <= 0)
            {
                Console.WriteLine("-port is mandatory for -server");
                return false;
            }

            if (Client && string.IsNullOrWhiteSpace(Host))
            {
                Console.WriteLine("-host is mandatory for -client");
                return false;
            }

            return true;
        }
    }
}