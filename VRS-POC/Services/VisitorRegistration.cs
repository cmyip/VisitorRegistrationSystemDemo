using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRS_POC.Datastructs;

namespace VRS_POC.Services
{
    public class VisitorRegistration
    {
        public List<VisitorRecord> VisitorRecords;
        private VisitorRecord currentVisitor;
        

        public VisitorRegistration()
        {
            VisitorRecords = new List<VisitorRecord>();
        }

        public bool UploadImage(string imgBase64)
        {
            return false;
        }

        public bool SetCurrentVisitor(VisitorRecord visitorRecord)
        {
            currentVisitor = visitorRecord;
            VisitorRecords.Add(visitorRecord);
            return true;
        }

        public VisitorRecord GetCurrentRecord()
        {
            return currentVisitor;
        }

    }
}
