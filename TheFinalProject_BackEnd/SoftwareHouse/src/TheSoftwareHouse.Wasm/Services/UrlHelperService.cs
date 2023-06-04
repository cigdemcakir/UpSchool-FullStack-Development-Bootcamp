using Domain.Services;

namespace Wasm.Services
{
    public class UrlHelperService:IUrlHelperService
    {
        //public string ApiUrl { get; }
        public string SignalRUrl { get; }

        public UrlHelperService(string signalRUrl /*string apiUrl*/)
        {
            //ApiUrl = apiUrl;

            SignalRUrl = signalRUrl;
        }
    }
}
