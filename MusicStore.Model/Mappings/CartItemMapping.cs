using MusicStore.Model.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Model.Mappings
{
    public class CartItemMapping : EntityTypeConfiguration<CartItem>
    {
        public override void Configure(EntityTypeBuilder<CartItem> builder)
        {
            base.Configure(builder);
        }
    }
}
