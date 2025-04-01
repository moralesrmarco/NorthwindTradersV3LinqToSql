CREATE OR ALTER PROCEDURE [dbo].[SP_PROVEEDORES_ELIMINAR_V4]
	@Id int,
	@RowVersion timestamp,
	@NumRegs int output
as
	Delete Suppliers
	where SupplierID = @Id AND RowVersion = @RowVersion;
	SET @NumRegs = @@ROWCOUNT;
