using System;
using System.ServiceModel;

namespace WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceImplementation : IServiceContract
    {
        private static string Timestamp
        {
            get => DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.fff");
        }

        public string Connect()
        {
            OperationContext operationContext = OperationContext.Current;
            Console.WriteLine();
            Console.WriteLine($@"{Timestamp} - Connection established:");
            Console.WriteLine(@"- Incoming Windows Identity:");
            Console.WriteLine($@"    Name = {operationContext.ServiceSecurityContext.WindowsIdentity.Name}");
            Console.WriteLine($@"    Authenticated = {operationContext.ServiceSecurityContext.WindowsIdentity.IsAuthenticated}");
            Console.WriteLine($@"    AuthenticationType = {operationContext.ServiceSecurityContext.WindowsIdentity.AuthenticationType}");
            Console.WriteLine($@"    User = {operationContext.ServiceSecurityContext.WindowsIdentity.User?.Value ?? "<Empty>"}");
            Console.WriteLine(@"- Incoming Primary Identity:");
            Console.WriteLine($@"    Name = {operationContext.ServiceSecurityContext.PrimaryIdentity.Name}");
            Console.WriteLine($@"    Authenticated = {operationContext.ServiceSecurityContext.PrimaryIdentity.IsAuthenticated}");
            Console.WriteLine($@"    AuthenticationType = {operationContext.ServiceSecurityContext.PrimaryIdentity.AuthenticationType}");
            return "OK";
        }
    }
}