using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Model.Entities;

namespace MusicStore.Model.Mappings
{
    public class AlbunItemMapping : EntityTypeConfiguration<AlbumItem>
    {
        public override void Configure(EntityTypeBuilder<AlbumItem> builder)
        {
            base.Configure(builder);
        }
    }


}
