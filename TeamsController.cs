using FootballTeams.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballTeams
{
    [Route("api/[controller]")]
    public class TeamsController : Controller
    {
        [HttpPost]
        public string Add([FromBody]object value)
        {
            Dictionary<string, object> Data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(value.ToString());
            var NewId = DataManager.EnlistNewTeam(Data);
            return NewId == null ? "Failure" : NewId.ToString();
        }

    }

}
