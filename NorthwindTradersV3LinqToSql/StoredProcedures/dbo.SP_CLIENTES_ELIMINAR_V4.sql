CREATE OR ALTER PROCEDURE [dbo].[SP_EMPLEADOS_ELIMINAR_V4]
	@Id int,
	@RowVersion timestamp,
	@NumRegs int output
AS
BEGIN
		DELETE Employees 
		WHERE  EmployeeID = @Id AND RowVersion = @RowVersion;
		SET @NumRegs = @@ROWCOUNT; 
END