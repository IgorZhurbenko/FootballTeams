using FootballTeams.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballTeams
{
    public class PlayersController : Controller
    {
        public IActionResult Index()
        {
            return this.View("Index", new Index_DTO());
        }
    }
    [Route("api/[controller]")]
    public class PlayersAPIController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var PlayerInfo = DataManager.PlayerInfo(id);
            if (PlayerInfo == null)
            {
                return "Contact not found";
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(PlayerInfo);
            }
        }

        // POST api/<controller>
        [HttpPost]
        public void Save([FromBody]object value)
        {
            Dictionary<string, object> Data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(value.ToString());
            
            if (Data["id"].ToBool())
            {
                DataManager.UpdatePlayerInfo(Data);
            }
            else
            {
                DataManager.EnlistNewPlayer(Data);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DataManager.DeletePlayer(id);
        }
    }

}
