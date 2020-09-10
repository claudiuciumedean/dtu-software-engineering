using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiRAMap.Models;
using LiRAMap.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LiRAMap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IWayProvider WayProvider;
        private readonly IConditionProvider ConditionProvider;

        public MapController(IWayProvider w, IConditionProvider c)
        {
            WayProvider = w;
            ConditionProvider = c;
        }

        private static object CreateResponse(IEnumerable<Way> Ways, IEnumerable<Node> Nodes, ICollection<Condition> Conditions)
        {
            return new
            {
                Ways,
                Nodes = Nodes.ToDictionary(n => "" + n.Id, n => n.Coords),
                Conditions
            };
        }

        [HttpGet]
        [HttpPost]
        public async Task<ActionResult<object>> GetRoads(string bounds, [FromForm] string AlreadySearched)
        {
            List<double> coords = bounds.Split(",").Select(double.Parse).ToList();
            if (coords.Count != 4) return BadRequest();

            List<double[]> known = null;
            if (AlreadySearched != null)
            {
                known = new List<double[]>();
                foreach(string s in AlreadySearched.Split(";"))
                {
                    known.Add(s.Split(",").Select(double.Parse).ToArray());
                }
            }

            Tuple<IEnumerable<Way>, IEnumerable<Node>> Map = await WayProvider.GetWaysInBounds(coords[0], coords[1], coords[2], coords[3], known);
            if (Map.Item1.Count() == 0) return NotFound();
            ICollection<Condition> Conditions = await ConditionProvider.GetConditions(Map.Item1);

            return Ok(CreateResponse(Map.Item1, Map.Item2, Conditions));
        }
    }
}