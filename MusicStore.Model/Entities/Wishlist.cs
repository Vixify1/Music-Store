using MusicStore.Model.Mappings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Model.Entities
{
    public class Wishlist
    {
        public int WishlistId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public int AlbumId { get; set; }

        [ForeignKey("AlbumId")]
        public Album Album { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
