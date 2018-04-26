using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex
{
    class ApexAPI
    {
        const string ApiBaseUrl = "https://apexecs.com/api";
        readonly string _serverKey;

        public ApexAPI(string serverKey)
        {
            _serverKey = serverKey;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = new System.Uri(ApiBaseUrl);

            request.AddParameter("server_token", _serverKey, ParameterType.HttpHeader);
            var response = client.Execute<T>(request);

            if(response.ErrorException != null)
            {
                const string message = "Error retrieving response from Apex API. Please ensure you have the correct server API token in your configuration file.";
                var apexException = new ApplicationException(message, response.ErrorException);
                throw apexException;
            }

            return response.Data;
        }

        public void SaveInventory()
    }
}
