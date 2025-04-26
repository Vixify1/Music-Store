using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Entities;
using MusicStore.Model.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Model.Mappings
{
    public class CartMapping : EntityTypeConfiguration<Cart>
    {
        public override void Configure(EntityTypeBuilder<Cart> builder)
        {
            base.Configure(builder);
        }
    }
}
