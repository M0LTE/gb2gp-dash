using Dapper;
using gp2gp_dash.Models;
using gp2gp_dash.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace gp2gp_dash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapDataController : ControllerBase
    {
        public MapDataController(IDbService dbService)
        {
            DbService = dbService;
        }

        readonly static LatLon GP = new LatLon(51.648126, 0.002209);

        private IDbService DbService;

        [HttpGet]
        public IActionResult GetData()
        {
            var conn = DbService.GetOpenConnection();

            var data = conn.Query("select pinLat, pinLon, theircall from contacts where pinlat is not null and pinlon is not null and theircall not in (select call from badcall);")
                    .Select(d => new LatLonPair(GP, new LatLon((double)d.pinLat, (double)d.pinLon)))
                    .ToArray();

            return new JsonResult(GeoJsonModel.FromLatLons(
                data
            ));
        }
    }
}