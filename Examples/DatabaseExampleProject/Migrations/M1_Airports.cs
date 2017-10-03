using System;

using FluentMigrator;

namespace DatabaseExampleProject.Migrations
{
    [Migration(1)]
    public class M1_Airports : Migration
    {
        public override void Up()
        {
            this.Create.Table("Airports")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("Code").AsString().Nullable()
                .WithColumn("Name").AsString().Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}