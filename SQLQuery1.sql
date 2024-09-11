Create type ParamExpType as Table
			(
				Title varchar(30),
				Duration int
			);
			GO 
          

CREATE proc InsertEmployeeSP
    @Name varchar(30),
    @IsActive BIT,
    @JoinDate DATE,
    @ImageName varchar(30),
    @ImageUrl varchar(30),
    @Salary INT,
    @Exp ParamExpType READONLY
AS
BEGIN
    SET NOCOUNT ON;

   
    INSERT INTO Employees (Name, IsActive, JoinDate, ImageName, ImageUrl, Salary)
    VALUES (@Name, @IsActive, @JoinDate, @ImageName, @ImageUrl, @Salary);

   
    DECLARE @EmployeeId INT = SCOPE_IDENTITY();

   
    INSERT INTO Experiences (EmployeeId, Title, Duration)
    SELECT @EmployeeId, Title, Duration
    FROM @Exp;
END;

