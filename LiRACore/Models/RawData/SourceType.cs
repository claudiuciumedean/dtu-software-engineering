using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.RawData
{
    public class SourceType
    {
        public Guid SourceTypeId { get; set; }

        // Creation History
        public DateTimeOffset Created_Date { get; set; }

        // Last Update History
        public DateTimeOffset Updated_Date { get; set; }

        public ICollection<Device> Devices { get; set; }

    }
}
