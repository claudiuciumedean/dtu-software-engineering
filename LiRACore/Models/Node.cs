using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models
{
    public class Node
    {
        public Guid NodeId { get; set; }

        public float lat { get; set; }
        public float lon { get; set; }

        public Guid FK_Section { get; set;}
        public Section Section { get; set; }

    }
}
