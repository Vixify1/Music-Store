using MusicStore.Model.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicStore.Model.Mappings
{
    public class GenreMapping : EntityTypeConfiguration<Genre>
    {
        public override void Configure(EntityTypeBuilder<Genre> builder)
        {
            base.Configure(builder);
        }
    }
}
