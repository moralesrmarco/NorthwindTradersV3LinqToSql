CREATE   PROCEDURE SP_PEDIDOSDETALLE_ACTUALIZAR_V3
	@OrderId int,
	@ProductId int,
	@Quantity smallint,
	@Discount real,
	@QuantityOld smallint,
	@DiscountOld real,
	@NumRegs int output
AS
BEGIN
	Declare @Difference smallint;
	Set @Difference = @Quantity - @QuantityOld;
	Begin Try
		Begin Transaction;
			-- Verificar si el detalle del pedido existe
			If Exists (Select 1 From [Order Details] Where OrderID = @OrderId And ProductID = @ProductId)
			Begin
				-- Si @Difference es mayor que cero, significa que la nueva cantidad (@Quantity) es mayor que la 
				-- cantidad anterior (@QuantityOld), entonces debemos restar la diferencia a UnitsInStock porque 
				-- más productos se han vendido.
				If @Difference > 0
				Begin
					-- Reduciendo el inventario
					Update Products 
					Set UnitsInStock = UnitsInStock - @Difference
					Where ProductID = @ProductId;
				End
				-- Si @Difference es menor que cero, significa que la nueva cantidad es menor que la cantidad anterior, 
				-- entonces debemos sumar la diferencia (en términos absolutos) a UnitsInStock porque menos productos 
				-- se han vendido o se han devuelto productos.
				Else If @Difference < 0
				Begin
					-- Incrementando el inventario
					Update Products
					Set UnitsInStock = UnitsInStock + ABS(@Difference)
					Where ProductID = @ProductId;
				End
				-- Actualizar el detalle del pedido
				Update [Order Details] 
				Set Quantity = @Quantity,
					Discount = @Discount
				Where OrderID = @OrderId And ProductID = @ProductId;
				--devuelve el número de filas afectadas por la última instrucción ejecutada y la asigna a NumRegs
				Set @NumRegs = @@ROWCOUNT; 
				-- Commit de la transacción si todo es exitoso
				Commit Transaction;
			End
			Else
			Begin
				-- Si el detalle del pedido no existe, lanzar un error 
				RAISERROR ('El detalle del pedido no existe. El registro fue eliminado previamente por otro usuario de la red', 16, 1); 
				ROLLBACK TRANSACTION;
			End
	End Try
	Begin Catch
		Rollback Transaction;
		Throw;
	End Catch
END