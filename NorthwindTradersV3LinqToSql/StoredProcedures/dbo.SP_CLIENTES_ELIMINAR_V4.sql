CREATE OR ALTER PROCEDURE [dbo].[SP_CLIENTES_ELIMINAR_V4]
	@Id nchar(5),
	@RowVersion timestamp,
	@NumRegs int output
as
	Delete Customers
	where CustomerID = @Id AND RowVersion = @RowVersion;
	SET @NumRegs = @@ROWCOUNT;
