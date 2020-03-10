using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VRS_POC.Datastructs;

namespace VRS_POC.Services
{
    public class SenseTimeService
    {
        IOptions<SensetimeConfig> _sensetimeConfig;
        public string _authToken;
        public SenseTimeService(IOptions<SensetimeConfig> sensetimeConfig)
        {
            _sensetimeConfig = sensetimeConfig;
        }

        public void DoLogin()
        {
            SensetimeLogin(_sensetimeConfig.Value.Username, _sensetimeConfig.Value.Password);
        }

        public void SensetimeLogin(string username, string password)
        {
            // Redacted to honor non-disclosure agreement
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("<redacted>", Method.POST);
            var requestBody = new Dictionary<string, string>()
            {
                
            };
            var jsonString = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json; charset=utf-8", jsonString, ParameterType.RequestBody);
            try
            {
                var result = client.Execute(request);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Content);
                    string authToken;
                    responseBody.TryGetValue("data", out authToken);
                    _authToken = authToken;
                }
            } catch (Exception e)
            {
                throw (e);
            }
        }

        public string AddPerson(VisitorRecord record, string imageUri)
        {
            // Redacted to honor non-disclosure agreement
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("<redacted>", Method.POST);
            request.AddHeader("Content-Type", "application/json;charset=UTF-8");
            request.AddHeader("Authorization", "Basic " + _authToken);
            var requestBody = new Dictionary<string, object>()
            {
                
            };
            var jsonString = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json; charset=utf-8", jsonString, ParameterType.RequestBody);
            try
            {
                var result = client.Execute(request);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Content);
                    string personId;
                    responseBody.TryGetValue("data", out personId);
                    return personId;
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            return "";
        }

        public bool AddPersonToGroup(string personId)
        {
            // Redacted to honor non-disclosure agreement
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("<redacted>", Method.POST);
            request.AddHeader("Content-Type", "application/json;charset=UTF-8");
            request.AddHeader("Authorization", "Basic " + _authToken);
            object requestBody = new
            {

            };
            var jsonString = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json; charset=utf-8", jsonString, ParameterType.RequestBody);
            try
            {
                var result = client.Execute(request);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            return false;
        }

        public string UploadPhoto(byte[] image)
        {
            // Redacted to honor non-disclosure agreement
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("<redacted>", Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Basic " + _authToken);
            Guid g = Guid.NewGuid();
            request.AddFile("image", image, g.ToString() + ".png");
            try
            {
                var result = client.Execute(request);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Content);
                    string imgPath;
                    responseBody.TryGetValue("data", out imgPath);
                    return imgPath;
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            return "";
        }
    }
}
