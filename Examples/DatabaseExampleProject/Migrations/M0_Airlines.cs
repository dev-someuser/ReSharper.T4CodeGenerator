using System;

using FluentMigrator;

namespace DatabaseExampleProject.Migrations
{
    [Migration(0)]
    public class M0_Airlines : Migration
    {
        public override void Up()
        {
            this.Create.Table("Airlines")
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