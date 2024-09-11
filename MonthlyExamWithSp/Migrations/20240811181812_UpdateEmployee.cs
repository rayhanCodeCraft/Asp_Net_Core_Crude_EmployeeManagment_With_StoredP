using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonthlyExamWithSp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE UpdateEmployeeSP
                @EmployeeId INT,
                @Name NVARCHAR(MAX),
                @IsActive BIT,
                @JoinDate DATE,
                @ImageName NVARCHAR(MAX),
                @ImageUrl NVARCHAR(MAX),
                @Salary INT,
                @Exp ParamExpType READONLY
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Update Employee details
                UPDATE Employees
                SET Name = @Name,
                    IsActive = @IsActive,
                    JoinDate = @JoinDate,
                    ImageName = @ImageName,
                    ImageUrl = @ImageUrl,
                    Salary = @Salary
                WHERE EmployeeId = @EmployeeId;

   
                DELETE FROM Experiences WHERE EmployeeId = @EmployeeId;

   
                INSERT INTO Experiences (EmployeeId, Title, Duration)
                SELECT @EmployeeId, Title, Duration FROM @Exp;
            END
            ");


            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE DeleteEmployeeSP
                @EmployeeId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                
                DELETE FROM Experiences WHERE EmployeeId = @EmployeeId;

                
                DELETE FROM Employees WHERE EmployeeId = @EmployeeId;
            END
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
