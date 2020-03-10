using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using VRS_POC.Datastructs;
using VRS_POC.Services;

namespace API_Test
{
    public class Tests
    {
        IOptions<SensetimeConfig> sensetimeConfig;
        SenseTimeService sensetimeService;
        [SetUp]
        public void Setup()
        {
            sensetimeConfig = Options.Create(new SensetimeConfig()
            {
                Username = "VRS-Admin",
                Password = "rN2T18dcFv7/u9JK39NeTw==",
                Hostname = "http://hostserver"
                // Hostname = "http://localhost:8181"
            });
            sensetimeService = new SenseTimeService(sensetimeConfig);
        }

        [Test]
        public void TestLoginApi()
        {
            sensetimeService._authToken = null;
            sensetimeService.DoLogin();
            if (String.IsNullOrEmpty(sensetimeService._authToken))
            {
                Assert.Fail();
            }
            Assert.Pass();
        }

        [Test]
        public void TestUploadApi()
        {

        }
    }
}