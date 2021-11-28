using System.ComponentModel.DataAnnotations;

namespace Smash_Glicko_Frontend.Models
{
    public class CreateLeagueModel
    {
        [Required]
        public string LeagueName { get; set; }

        [Required]
        public string LeagueGame { get; set; }

        public bool IsPublic { get; set; }
    }
}
