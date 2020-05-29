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
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("/GUNS/mgr/login", Method.POST);
            var requestBody = new Dictionary<string, string>()
            {
                { "username", username },
                { "password", password },
                { "accountType", "2" }
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
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("/PERSON/person/create", Method.POST);
            request.AddHeader("Content-Type", "application/json;charset=UTF-8");
            request.AddHeader("Authorization", "Basic " + _authToken);
            // {"birthday":"","cnName":"Daniel","enName":"","idNumber":"888811111","documentId":"","imageURI":"/images/person/20200226/255/6a656596470320f1e78887eb0c4e0322.jpg",
            // "operatePerson":"admin","sex":"","personType":"","personTag":"","desc":"","idType":"","idCountry":"","idExpiryDate":"","privilege":""}
            var requestBody = new Dictionary<string, object>()
            {
                { "birthday", "" },
                { "cnName", record.Name },
                { "enName", "" },
                { "idNumber", record.Nric },
                { "documentId", "" },
                { "imageURI", imageUri },
                { "operatePerson", "VRS Kiosk" },
                { "sex", "" },
                { "personType", "" },
                { "personTag", "" },
                { "desc", String.Format("Floor Number:{0}\nPatient Name:{1}\nMobile Number:{2}", record.FloorNumber, record.PatientName, record.MobileNumber ) },
                { "idType", "" },
                { "idCountry", "" },
                { "idExpiryDate", "" },
                { "privilege", "" }
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
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("/PERSON/member/addMemberToGroups", Method.POST);
            request.AddHeader("Content-Type", "application/json;charset=UTF-8");
            request.AddHeader("Authorization", "Basic " + _authToken);
            object requestBody = new
            {
                groupIdList = new List<string>()
                {
                    "2"
                },
                operatePerson = "admin",
                uid = personId
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
            var hostname = _sensetimeConfig.Value.Hostname;
            var client = new RestClient(hostname);
            var request = new RestRequest("/UTILITY/files/uploadImage", Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Basic " + _authToken);
            Guid g = Guid.NewGuid();
            request.AddFile("image", image, g.ToString() + ".png");
            request.AddParameter("serviceType", "person");
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

        public void AddPersonSensetime()
        {
            var hostname = _sensetimeConfig.Value.Hostname;
            var endpoint = hostname + "/PERSONS/create";
            var client = new RestClient(hostname);
        }
    }
}
