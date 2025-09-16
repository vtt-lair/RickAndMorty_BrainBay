using FluentMigrator;

namespace RickAndMorty.Storage.Migrator.Migrations
{
    [Migration(1)]
    public class CreateCharacterTable : Migration
    {
        public override void Up()
        {
            Create.Table("Characters")
                .WithColumn("Id").AsInt32().PrimaryKey()
                .WithColumn("DateModified").AsDateTime().NotNullable()
                .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Species").AsString().Nullable()
                .WithColumn("Type").AsString().Nullable()
                .WithColumn("Gender").AsString().Nullable()
                .WithColumn("OriginId").AsInt32().Nullable()
                .WithColumn("LocationId").AsInt32().Nullable()
                .WithColumn("Image").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Character");
        }
    }
}