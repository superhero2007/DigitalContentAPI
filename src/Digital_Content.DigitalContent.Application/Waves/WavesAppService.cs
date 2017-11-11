using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Digital_Content.DigitalContent.Waves
{
    public class WavesAppService: DigitalContentAppServiceBase, IWavesAppService
    {
        private const string _getAddressesUrl = "http://34.208.102.103:6869/addresses"; 

        public async Task<List<string>> GetAddresses()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var urlAddresses = new Uri(_getAddressesUrl);
                    var response = await client.GetAsync(urlAddresses);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var responseModel = JsonConvert.DeserializeObject<List<string>>(result);
                        return responseModel != null ? responseModel : new List<string>();
                    }
                    else
                    {
                        throw new NullReferenceException("Response not success");
                    }
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }

        }

        
        public async Task<object> Redirect(string url, string methodType, string parameters = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var urlAddresses = new Uri(url);
                    var response = new HttpResponseMessage();
                    if (methodType == "get")
                    {
                        response = await client.GetAsync(urlAddresses);
                    }
                    else
                    {
                        var jsonSer = new JavaScriptSerializer();
                        var body = jsonSer.Deserialize<Dictionary<string, string>>(parameters);
                        var content = new FormUrlEncodedContent(body);

                        var request = new HttpRequestMessage()
                        {
                            Method = HttpMethod.Post,
                            RequestUri = urlAddresses,
                            Content = content
                        };
                        response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var responseModel = JsonConvert.DeserializeObject<object>(result);
                        return responseModel != null ? responseModel : new List<string>();
                    }
                    else
                    {
                        throw new NullReferenceException("Response not success");
                    }
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
        }



    }
}
