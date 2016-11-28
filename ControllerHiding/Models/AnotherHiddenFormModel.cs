using System;
using System.ComponentModel.DataAnnotations;

namespace ControllerHiding.Models
{
    public class AnotherHiddenFormModel
    {
        [Required]
        [StringLength(10, ErrorMessage = "Your name has to have a length of less than 10.")]
        public string Name { get; set; }

        [Required]
        public string Street { get; set; }
    }
}