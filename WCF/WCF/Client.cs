using System;
using System.Diagnostics;
using System.ServiceModel;

namespace WCF
{
    public class Client
    {
        private readonly ChannelFactory<IServiceContract> _channelFactory;
        private readonly Stopwatch _timer = new Stopwatch();

        public Client(CommandLine commandLine)
        {
            string uri = $"net.tcp://{commandLine.Host}/receive";
            var binding = new NetTcpBinding(SecurityMode.Transport)
            {
                MaxReceivedMessageSize = 100000,
                SendTimeout = new TimeSpan(0, 15, 0),
                ReceiveTimeout = new TimeSpan(0, 15, 0),
                TransferMode = TransferMode.Buffered,
                PortSharingEnabled = false,
                ReaderQuotas =
                {
                    MaxStringContentLength = MaxContentsLength,
                    MaxArrayLength = 100,
                    MaxBytesPerRead = MaxContentsLength
                }
            };
            var endpoint = new EndpointAddress(new Uri(uri), string.IsNullOrEmpty(commandLine.Upn) ? null : EndpointIdentity.CreateUpnIdentity(commandLine.Upn));
            Console.WriteLine($"Connecting to endpoint: {uri}. Upn:{commandLine.Upn}");
            _channelFactory = new ChannelFactory<IServiceContract>(binding, endpoint);
        }

        public static int MaxContentsLength { get; } = 10000;

        public void RunPingTest()
        {
            long initial = Send(f => f.Connect());
            if (initial < 0)
            {
                Console.WriteLine("Run aborted");
                return;
            }

            Console.WriteLine("Connected");
        }

        private long Send(Action<IServiceContract> action)
        {
            long elapsed;
            try
            {
                _timer.Start();
                IServiceContract channel = _channelFactory.CreateChannel();
                ((IClientChannel) channel).Open();
                action(channel);
                ((IClientChannel) channel).Close();
                elapsed = _timer.ElapsedMilliseconds;
                _timer.Stop();
                _timer.Reset();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught {ex.Message}: {ex.StackTrace}");
                while (ex != null)
                {
                    ex = ex.InnerException;
                    if (ex != null)
                    {
                        Console.WriteLine($@" ==> {ex.Message}: {ex.StackTrace}");
                    }
                }

                return -1;
            }

            return elapsed;
        }
    }
}