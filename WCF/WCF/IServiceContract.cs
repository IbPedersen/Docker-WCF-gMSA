using System.ServiceModel;

namespace WCF
{
    [ServiceContract]
    public interface IServiceContract
    {
        [OperationContract]
        string Connect();
    }
}