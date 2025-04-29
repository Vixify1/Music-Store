using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Model.Mappings
{
    public class WishlistMapping : EntityTypeConfiguration<Wishlist>
    {
        public override void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            base.Configure(builder);
        }
    }
}


