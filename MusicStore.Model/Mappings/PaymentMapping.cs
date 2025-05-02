using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Entities;

namespace MusicStore.Model.Mappings
{
    public class PaymentMapping : EntityTypeConfiguration<Payment>
    {
        public override void Configure(EntityTypeBuilder<Payment> builder)
        {
            base.Configure(builder);
        }
    }


}
