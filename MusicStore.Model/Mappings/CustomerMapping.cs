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
    public class CustomerMapping : EntityTypeConfiguration<Customer>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            base.Configure(builder);
        }
    }
}
