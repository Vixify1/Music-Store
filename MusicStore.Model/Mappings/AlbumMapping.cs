using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Entities;

namespace MusicStore.Model.Mappings
{
    public class AlbunMapping : EntityTypeConfiguration<Album>
    {
        public override void Configure(EntityTypeBuilder<Album> builder)
        {
            base.Configure(builder);
        }
    }


}
