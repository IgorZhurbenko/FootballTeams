using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FootballTeams.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebForms.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public int Number = 10;

        public List<Dictionary<string, object>> Contacts;        

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            this.Contacts = DataManager.AllPlayers().ToList();
        }
    }
}
