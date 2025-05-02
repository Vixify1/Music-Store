using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Entities;

namespace MusicStore.Model.Mappings
{
    public class ReviewMapping : EntityTypeConfiguration<Reviews>
    {
        public override void Configure(EntityTypeBuilder<Reviews> builder)
        {
            base.Configure(builder);
        }
    }


}
