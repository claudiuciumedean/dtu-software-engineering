using LiRACore.Models.RawData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LiRACore.Services
{
    public interface IMapMatchingService
    {
        Osrm.Models.Responses.MatchResponse Match(LiRACore.Osrm.Location[] locations);
    }
}
