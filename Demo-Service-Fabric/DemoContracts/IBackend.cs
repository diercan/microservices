using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace DemoContracts
{
    public interface IBackend:IService
    {
        Task<long> GetCount();
    }
}
