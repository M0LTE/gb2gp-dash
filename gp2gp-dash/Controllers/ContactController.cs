using Dapper;
using gp2gp_dash.Models;
using gp2gp_dash.Services;
using Microsoft.AspNetCore.Mvc;

namespace gp2gp_dash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        IDbService dbService;
        public ContactController(IDbService dbService)
        {
            this.dbService = dbService;
        }

        [HttpPost]
        public void AddContact(ContactRow contactRow)
        {
            using (var conn = dbService.GetOpenConnection())
            {
                conn.Execute("insert into contacts (id, utctime, theircall, ourstation, sentreport, receivedreport, pinlat, pinlon, theiroperator, theirgroup, theirlocation) values (@id, @utctime, @theircall, @ourstation, @sentreport, @receivedreport, @pinlat, @pinlon, @theiroperator, @theirgroup, @theirlocation);", contactRow);
            }
        }
    }
}