using FluentMigrator;

namespace RickAndMorty.Storage.Migrator.Migrations
{
    [Migration(2)]
    public class CreatePlanetTable : Migration
    {
        public override void Up()
        {
            Create.Table("Planets")
                .WithColumn("Id").AsInt32().PrimaryKey()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Type").AsString().Nullable()
                .WithColumn("Dimension").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Planet");
        }
    }
}
