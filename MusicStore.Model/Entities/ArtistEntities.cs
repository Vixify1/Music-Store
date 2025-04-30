using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Model.Entities
{
    public class ArtistEntities
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    
        [Required]
        [MaxLength(50)]
        public string Genre { get; set; }
        public List<SelectListItem>? Generes; 
        [MaxLength(50)]
        public string Country { get; set; }
    }
}

