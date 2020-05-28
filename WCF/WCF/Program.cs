using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using System.Security.Principal;

namespace WCF
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string upn = GetUserPrincipalName();

            var commandLine = new CommandLine();
            foreach (string arg in args)
            {
                if (arg.StartsWith("-server=", StringComparison.OrdinalIgnoreCase))
                {
                    commandLine.Server = true;
                    if (int.TryParse(arg.Remove(0, 8), out int port) && port > 0)
                    {
                        commandLine.Port = port;
                    }
                }

                if (arg.StartsWith("-client=", StringComparison.OrdinalIgnoreCase))
                {
                    commandLine.Client = true;
                    commandLine.Host = arg.Remove(0, 8);
                }

                if (arg.StartsWith("-upn=", StringComparison.OrdinalIgnoreCase))
                {
                    commandLine.Upn = arg.Remove(0, 5);
                }
            }

            if (!commandLine.IsValid())
            {
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("  -server=<port>");
                Console.WriteLine("    Optional: -upn=<endpoint identity>, if not using the default calculated");
                Console.WriteLine();
                Console.WriteLine("  -client=<host>:<port>");
                Console.WriteLine("    Optional: -upn=<service endpoint identity>, if not using the default calculated");
                return;
            }

            commandLine.Upn = commandLine.Upn ?? upn;

            if (commandLine.Server)
            {
                var server = new Server(commandLine);
                try
                {
                    Console.ReadLine();
                }
                finally
                {
                    server.Close();
                }

                return;
            }

            if (commandLine.Client)
            {
                var client = new Client(commandLine);
                client.RunPingTest();
            }
        }

        private static string GetUserPrincipalName()
        {
            if (IsWellKnownUser())
            {
                // It is one of the windows service accounts, so UPN is not used
                return "";
            }

            string upn1 = null;
            string upn2 = null;
            string upn3 = null;
            try
            {
                upn1 = UserPrincipal.Current.UserPrincipalName;
                Console.WriteLine($"UserPrincipal.Current.UserPrincipalName: {upn1}");
            }
            catch
            {
                // ignored
            }

            try
            {
                string distinguishedName = UserPrincipal.Current.DistinguishedName;
                if (string.IsNullOrWhiteSpace(distinguishedName))
                {
                    using (var directoryEntry = new DirectoryEntry())
                    {
                        distinguishedName = directoryEntry.Properties["distinguishedName"].Value as string;
                    }
                }

                if (!string.IsNullOrWhiteSpace(distinguishedName))
                {
                    string dnsDomain = string.Join(".", distinguishedName.ToLower().Split(',').Where(n => n.StartsWith("dc=")).Select(n => n.Substring(3)));
                    upn2 = string.Format(CultureInfo.InvariantCulture, "{0}@{1}", Environment.UserName, dnsDomain);
                    Console.WriteLine($"UserPrincipal.Current.DistinguishedName: {upn2}");
                }
            }
            catch
            {
                // ignored
            }

            try
            {
                upn3 = WindowsIdentity.GetCurrent().Name;
                Console.WriteLine($"WindowsIdentity.GetCurrent().Name: {upn3}");
            }
            catch
            {
                // ignored
            }

            Console.WriteLine();
            return NullOrValue(upn1) ?? NullOrValue(upn2) ?? NullOrValue(upn3) ?? "";
        }

        private static string NullOrValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static bool IsWellKnownUser()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                if (identity.User == null)
                {
                    return false;
                }

                SecurityIdentifier securityIdentifier = identity.User;

                // Is it one of the windows service accounts?
                return securityIdentifier.IsWellKnown(WellKnownSidType.LocalSystemSid) ||
                       securityIdentifier.IsWellKnown(WellKnownSidType.LocalServiceSid) ||
                       securityIdentifier.IsWellKnown(WellKnownSidType.NetworkServiceSid);
            }
        }
    }
}