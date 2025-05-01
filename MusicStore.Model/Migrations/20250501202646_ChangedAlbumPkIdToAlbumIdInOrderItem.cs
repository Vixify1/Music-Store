using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.Model.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderItemAlbumIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, let's identify and drop any foreign key constraints that reference the Id column
            migrationBuilder.Sql(@"
                DECLARE @ConstraintName NVARCHAR(256)
                
                -- Find any foreign key constraints on the Id column
                SELECT @ConstraintName = name
                FROM sys.foreign_keys
                WHERE parent_object_id = OBJECT_ID('OrderItems')
                AND OBJECT_NAME(referenced_object_id) = 'Album'
                
                -- If a constraint exists, drop it
                IF @ConstraintName IS NOT NULL
                BEGIN
                    DECLARE @sql NVARCHAR(500) = 'ALTER TABLE [OrderItems] DROP CONSTRAINT [' + @ConstraintName + ']'
                    EXEC sp_executesql @sql
                END
            ");

            // Next, let's handle any indexes on the Id column
            migrationBuilder.Sql(@"
                DECLARE @IndexName NVARCHAR(256)
                
                -- Find any indexes on the Id column
                SELECT @IndexName = name
                FROM sys.indexes
                WHERE object_id = OBJECT_ID('OrderItems')
                AND EXISTS (
                    SELECT 1 FROM sys.index_columns ic
                    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    WHERE ic.object_id = OBJECT_ID('OrderItems')
                    AND c.name = 'Id'
                    AND ic.index_id = sys.indexes.index_id
                )
                
                -- If an index exists, drop it
                IF @IndexName IS NOT NULL
                BEGIN
                    DECLARE @sql NVARCHAR(500) = 'DROP INDEX [' + @IndexName + '] ON [OrderItems]'
                    EXEC sp_executesql @sql
                END
            ");

            // Now, safely rename the column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Id' AND Object_ID = Object_ID(N'OrderItems'))
                BEGIN
                    -- Rename the column
                    EXEC sp_rename 'OrderItems.Id', 'AlbumId', 'COLUMN'
                END
            ");

            // Create the index and constraint with new names
            migrationBuilder.Sql(@"
                -- Create a new index if the AlbumId column exists
                IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'AlbumId' AND Object_ID = Object_ID(N'OrderItems'))
                BEGIN
                    -- Create index if it doesn't already exist
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.indexes 
                        WHERE name = 'IX_OrderItems_AlbumId' 
                        AND object_id = OBJECT_ID('OrderItems')
                    )
                    BEGIN
                        CREATE INDEX [IX_OrderItems_AlbumId] ON [OrderItems]([AlbumId])
                    END
                    
                    -- Add foreign key constraint if it doesn't already exist
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.foreign_keys
                        WHERE name = 'FK_OrderItems_Album_AlbumId'
                        AND parent_object_id = OBJECT_ID('OrderItems')
                    )
                    BEGIN
                        -- Make sure the Album table's primary key is Id before adding the constraint
                        IF EXISTS (
                            SELECT 1 FROM sys.indexes 
                            WHERE name = 'PK_Album' 
                            AND object_id = OBJECT_ID('Album')
                        )
                        BEGIN
                            ALTER TABLE [OrderItems] ADD CONSTRAINT [FK_OrderItems_Album_AlbumId] 
                            FOREIGN KEY ([AlbumId]) REFERENCES [Album] ([Id]) ON DELETE CASCADE
                        END
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // We'll use the same dynamic approach for the Down migration
            // First, drop any constraints on AlbumId
            migrationBuilder.Sql(@"
                DECLARE @ConstraintName NVARCHAR(256)
                
                SELECT @ConstraintName = name
                FROM sys.foreign_keys
                WHERE parent_object_id = OBJECT_ID('OrderItems')
                AND OBJECT_NAME(referenced_object_id) = 'Album'
                
                IF @ConstraintName IS NOT NULL
                BEGIN
                    DECLARE @sql NVARCHAR(500) = 'ALTER TABLE [OrderItems] DROP CONSTRAINT [' + @ConstraintName + ']'
                    EXEC sp_executesql @sql
                END
            ");

            // Drop any indexes on AlbumId
            migrationBuilder.Sql(@"
                DECLARE @IndexName NVARCHAR(256)
                
                SELECT @IndexName = name
                FROM sys.indexes
                WHERE object_id = OBJECT_ID('OrderItems')
                AND EXISTS (
                    SELECT 1 FROM sys.index_columns ic
                    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    WHERE ic.object_id = OBJECT_ID('OrderItems')
                    AND c.name = 'AlbumId'
                    AND ic.index_id = sys.indexes.index_id
                )
                
                IF @IndexName IS NOT NULL
                BEGIN
                    DECLARE @sql NVARCHAR(500) = 'DROP INDEX [' + @IndexName + '] ON [OrderItems]'
                    EXEC sp_executesql @sql
                END
            ");

            // Rename column back to Id
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'AlbumId' AND Object_ID = Object_ID(N'OrderItems'))
                BEGIN
                    EXEC sp_rename 'OrderItems.AlbumId', 'Id', 'COLUMN'
                END
            ");

            // Recreate original index and constraint
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Id' AND Object_ID = Object_ID(N'OrderItems'))
                BEGIN
                    -- Create index
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.indexes 
                        WHERE name = 'IX_OrderItems_Id' 
                        AND object_id = OBJECT_ID('OrderItems')
                    )
                    BEGIN
                        CREATE INDEX [IX_OrderItems_Id] ON [OrderItems]([Id])
                    END
                    
                    -- Add foreign key constraint
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.foreign_keys
                        WHERE name = 'FK_OrderItems_Album_Id'
                        AND parent_object_id = OBJECT_ID('OrderItems')
                    )
                    BEGIN
                        IF EXISTS (
                            SELECT 1 FROM sys.indexes 
                            WHERE name = 'PK_Album' 
                            AND object_id = OBJECT_ID('Album')
                        )
                        BEGIN
                            ALTER TABLE [OrderItems] ADD CONSTRAINT [FK_OrderItems_Album_Id] 
                            FOREIGN KEY ([Id]) REFERENCES [Album] ([Id]) ON DELETE CASCADE
                        END
                    END
                END
            ");
        }
    }
}