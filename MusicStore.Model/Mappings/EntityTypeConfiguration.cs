﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MusicStore.Model.Mappings
{
    /// <summary>
    /// Represents entity mapping configuration.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public abstract class EntityTypeConfiguration<TEntity> : IMappingConfiguration,
        IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        public virtual int Order { get; set; } = 1;

        #region Utilities

        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom configuration code.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity.</param>
        protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configures the entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity.</param>
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            //add custom configuration
            PostConfigure(builder);
        }

        /// <summary>
        /// Apply this mapping configuration.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context.</param>
        public virtual void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }

        #endregion
    }
}
