using System.ComponentModel.DataAnnotations;

namespace Glicko_Smash_Implementation_Web_Frontend.Models
{
    public class CreateLeagueModel
    {
        [Required]
        public string LeagueName { get; set; }

        [Required]
        public string LeagueGame { get; set; }
    }
}
