using FootballTeams.Data;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballTeams
{
    public static class HelpersExtension
    {
        public static string DefineInputAttributes(this IHtmlHelper helper, string FieldName)
        {
            string res = "class=\"TextBoxBottomLine\"";

            res = res + $" type=\"{(FieldName.ToLower().Contains("date") ? "date" : "text")}\" ";
            if (FieldName.ToLower().Contains("date"))
            {
                res = res + "min = '1960-01-01' max = '2003-01-01'";
            }
            res = res + "v-model=\"" + FieldName + "\"";

            return res;
        }

        public static string CountrySelect(this IHtmlHelper helper, string[] Options = null)
        {
            if (Options == null) { Options = new string[] { "Russia", "USA", "Italy" }; }
            string OptionsHypertext = "";
            foreach (var option in Options)
            {
                OptionsHypertext = OptionsHypertext + $"<option>{option}</option>";
            }
            string res = $"<select v-model='Country' class=\"TextBoxBottomLine\">{OptionsHypertext}</select>";
            return res;
        }

        public static string TextBox(this IHtmlHelper helper, string FieldName)
        {
            return $"<input class=\"TextBoxBottomLine\" {helper.Raw(helper.DefineInputAttributes(FieldName))}/>";
        }

        public static IHtmlContent ProperInput(this IHtmlHelper helper, string FieldName)
        {
            string res = "";
            if (FieldName.Contains("Team"))
            {
                string OptionsHypertext = "";
                foreach (var option in DataManager.GetAllTeams())
                {
                    OptionsHypertext = OptionsHypertext + $"<option value='{option["Id"]}'>{option["Name"]}</option>";
                }
                res = $"<select oninput='PlayerInfoVue.TeamId=this.options[this.selectedIndex].value' class=\"TextBoxBottomLine\" id='Edit_Team'>{OptionsHypertext}</select>"
                    + "<input id='NewTeamNameTextBox' class='TextBoxBottomLine' type='text' hidden preset='Enter team name here...'>" +
                    "<button id='StartEnteringTeamNameButton' onclick='StartAddingTeam()'>+</button>" +
                    "<button id='ConfirmEnteredTeamNameButton' onclick='ConfirmNewTeam()' hidden disabled=\'true\'>Confirm</button>" +
                    "<button id='CancelEnteredTeamNameButton' onclick='CancelNewTeamCreation()' hidden>Cancel</button>";
            }
            else if (FieldName.Contains("Country"))
            {
                res = helper.CountrySelect();
            }
            else if (FieldName.Contains("Gender"))
            {
                string OptionsHypertext = "";
                OptionsHypertext = OptionsHypertext + "<option value='0'>M</option><option value='1'>F</option>";
                res = $"<select v-model='Gender' class=\"TextBoxBottomLine\" id='Edit_Gender'>{OptionsHypertext}</select>";
            }
            else
            {
                res = helper.TextBox(FieldName);
            }
            return helper.Raw(res);
        }
    }
}
