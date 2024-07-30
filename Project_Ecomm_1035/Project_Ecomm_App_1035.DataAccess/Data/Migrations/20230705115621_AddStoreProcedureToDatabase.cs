using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Ecomm_App_1035.DataAccess.Migrations
{
    public partial class AddStoreProcedureToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE CreateCoverType
	                                @name varchar(50)
                                    AS
	                                insert CoverTypes values (@name)");
            migrationBuilder.Sql(@"CREATE PROCEDURE UpdateCoverType
                                       @id int ,
	                                   @name varchar(50)
                                       AS
	                                   update CoverTypes set name = @name where id=@id");
            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteCoverType
                                       @id int 
                                       AS
	                                   delete from CoverTypes where id=@id");
            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverTypes
                                       AS
                                       select * from CoverTypes");
            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverType
                                       @id int 
                                       AS
                                        Select * from CoverTypes where id=@id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
