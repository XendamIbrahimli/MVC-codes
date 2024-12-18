using System.ComponentModel.DataAnnotations;

namespace UniqloMVC.ViewModels .Slider
{
    public class SliderCreateVM
    {
        [MaxLength(32, ErrorMessage ="Title lenght must be less tahn 32."), Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }                          
        [MaxLength(64),Required]
        public string Subtitle { get; set; }
        public string? Link { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
