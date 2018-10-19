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
            using (var conn = DbService.GetOpenConnection())
            {
                return new JsonResult(GeoJsonModel.FromLatLons(
                    conn.Query("select pinLat, pinLon from contacts").Select(d => new LatLonPair(GP, new LatLon(d.pinLat, d.pinLon))
                )));
            }
        }
    }
}