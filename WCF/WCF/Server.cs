using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WCF
{
    public class Server
    {
        private readonly ServiceHost _host;

        public Server(CommandLine commandLine)
        {
            string domainName = GetLocalhostAsFullyQualifiedDomainName();
            _host = new ServiceHost(new ServiceImplementation());
            var endpoints = new List<ServiceEndpoint>
            {
                CreateEndpoint(commandLine.Upn, new Uri($"net.tcp://{domainName}:{commandLine.Port}/receive"))
            };
            foreach (ServiceEndpoint endpoint in endpoints)
            {
                _host.AddServiceEndpoint(endpoint);
            }

            _host.Open();

            Console.WriteLine();
            Console.WriteLine($"The server is ready: {$@"{domainName}:{commandLine.Port}"}. Upn: {commandLine.Upn}");
            Console.WriteLine("Press to stop server");
            Console.WriteLine();
        }

        private static ServiceEndpoint CreateEndpoint(string upn, Uri uri)
        {
            var address = new EndpointAddress(uri, string.IsNullOrEmpty(upn) ? null : EndpointIdentity.CreateUpnIdentity(upn));
            var binding = new NetTcpBinding(SecurityMode.Transport)
            {
                PortSharingEnabled = false
            };
            ContractDescription contract = ContractDescription.GetContract(typeof(IServiceContract));
            return new ServiceEndpoint(contract, binding, address);
        }

        public void Close()
        {
            _host.Close();
        }

        private static string GetLocalhostAsFullyQualifiedDomainName()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return $"{ipProperties.HostName}.{ipProperties.DomainName}";
        }
    }
}