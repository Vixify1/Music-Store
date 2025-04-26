using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Entities;

namespace MusicStore.Model.Mappings
{
    public class ArtistMapping : EntityTypeConfiguration<ArtistEntities>
    {
        public override void Configure(EntityTypeBuilder<ArtistEntities> builder)
        {
            base.Configure(builder);
        }
    }


}
