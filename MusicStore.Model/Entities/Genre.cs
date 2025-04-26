using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Model.Entities
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }

    }
}
