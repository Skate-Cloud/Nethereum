using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;

namespace Nethereum.Geth.RPC.Admin
{
    public interface IAdminRemovePeer
    {
        RpcRequest BuildRequest(string enodeUrl, object id = null);
        Task<bool> SendRequestAsync(string enodeUrl, object id = null);
    }
}