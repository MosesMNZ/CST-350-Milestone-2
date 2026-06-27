using System.ComponentModel.DataAnnotations;

namespace CST_350_Milestone.Models
{
    public class GameSettingsModel
    {
        [Required(ErrorMessage = "Board size is required.")]
        [RegularExpression(@"^(8|10|12)$", ErrorMessage = "Board size must be 8, 10, or 12.")]
        public string BoardSize { get; set; }

        [Required(ErrorMessage = "Difficulty level is required.")]
        [RegularExpression(@"^(Easy|Medium|Hard)$", ErrorMessage = "Difficulty must be Easy, Medium, or Hard.")]
        public string Difficulty { get; set; }
    }
}
