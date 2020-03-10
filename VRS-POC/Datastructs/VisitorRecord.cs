using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRS_POC.Datastructs
{
    public class VisitorRecord
    {
        public DateTimeOffset EntryDate { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Nric { get; set; }
        public string PatientName { get; set; }
        public string FloorNumber { get; set; }
        public string ImgBase64 { get; set; }

    }
}
