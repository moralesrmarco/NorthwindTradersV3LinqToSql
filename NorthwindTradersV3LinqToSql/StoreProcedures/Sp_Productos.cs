/*
ALTER PROCEDURE [dbo].[SP_PRODUCTOS_ALL]
	@top100 Bit = 1 
AS
BEGIN
	If @top100 = 1
	BEGIN
		SELECT Products.ProductID AS Id, Products.ProductName AS Producto, Products.QuantityPerUnit AS [Cantidad por unidad], 
		Products.UnitPrice AS Precio, Products.UnitsInStock AS [Unidades en inventario], Products.UnitsOnOrder AS [Unidades en pedido], 
		Products.ReorderLevel AS [Punto de pedido], Products.Discontinued AS Descontinuado, Categories.CategoryName AS Categoría, Categories.Description As [Descripción de categoría], 
		Suppliers.CompanyName AS Proveedor, Categories.CategoryID As IdCategoria, Suppliers.SupplierID As IdProveedor
		FROM Products LEFT OUTER JOIN Categories ON Products.CategoryID = Categories.CategoryID LEFT OUTER JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID
		ORDER BY Products.ProductID DESC
	END
	ELSE
	BEGIN 
		SELECT TOP 20 Products.ProductID AS Id, Products.ProductName AS Producto, Products.QuantityPerUnit AS [Cantidad por unidad], 
		Products.UnitPrice AS Precio, Products.UnitsInStock AS [Unidades en inventario], Products.UnitsOnOrder AS [Unidades en pedido], 
		Products.ReorderLevel AS [Punto de pedido], Products.Discontinued AS Descontinuado, Categories.CategoryName AS Categoría, Categories.Description As [Descripción de categoría], 
		Suppliers.CompanyName AS Proveedor, Categories.CategoryID As IdCategoria, Suppliers.SupplierID As IdProveedor
		FROM Products LEFT OUTER JOIN Categories ON Products.CategoryID = Categories.CategoryID LEFT OUTER JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID
		ORDER BY Products.ProductID DESC
	END
END

ALTER PROCEDURE [dbo].[SP_PRODUCTOS_BUSCAR_V2]
	@IdIni int,
	@IdFin int,
	@Producto nvarchar(40),
	@Categoria int,
	@Proveedor int
AS
BEGIN
	SELECT Products.ProductID AS Id, Products.ProductName AS Producto, Products.QuantityPerUnit AS [Cantidad por unidad], 
	Products.UnitPrice AS Precio, Products.UnitsInStock AS [Unidades en inventario], Products.UnitsOnOrder AS [Unidades en pedido], 
	Products.ReorderLevel AS [Punto de pedido], Products.Discontinued AS Descontinuado, Categories.CategoryName AS Categoría, Categories.Description As [Descripción de categoría], 
	Suppliers.CompanyName AS Proveedor, Categories.CategoryID As IdCategoria, Suppliers.SupplierID As IdProveedor
	FROM Products LEFT OUTER JOIN Categories ON Products.CategoryID = Categories.CategoryID LEFT OUTER JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID
	WHERE
	(@IdIni = 0 OR Products.ProductID BETWEEN @IdIni AND @IdFin) AND 
	(@Producto = '' OR Products.ProductName LIKE '%' + @Producto + '%') AND
	(@Categoria = 0 OR Products.CategoryID = @Categoria ) AND
	(@Proveedor = 0 OR Products.SupplierID = @Proveedor)
	ORDER BY Products.ProductID DESC
END

ALTER PROCEDURE [dbo].[SP_PRODUCTOS_INSERTAR_V2]

    @Categoria INT,
    @Proveedor INT,
    @Producto NVARCHAR(40),
	@Cantidad NVARCHAR(20),
	@Precio MONEY,
    @UInventario SMALLINT,
    @UPedido SMALLINT,
    @PPedido SMALLINT,
    @Descontinuado BIT,
    @Id INT OUTPUT,
	@NumRegs INT OUTPUT
as
	INSERT INTO Products
	(ProductName, SupplierId, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
	VALUES(@Producto, @Proveedor, @Categoria, @Cantidad, @Precio, @UInventario, @UPedido, @PPedido, @Descontinuado);


    SET @Id = SCOPE_IDENTITY();
	SET @NumRegs = @@ROWCOUNT;

ALTER PROCEDURE [dbo].[SP_PRODUCTOS_ACTUALIZAR_V2]
	@Id Int,
	@Producto Varchar(40),
	@Proveedor Int,
	@Categoria Int,
	@Cantidad VarChar(20),
	@Precio Money,
	@UInventario Smallint,
	@UPedido SmallInt,
	@PPedido SmallInt,
	@Descontinuado Bit,
	@NumRegs int output
AS
	update Products
	set ProductName = @Producto,
	SupplierID = @Proveedor,
	CategoryID = @Categoria,
	QuantityPerUnit = @Cantidad,
	UnitPrice = @Precio,
	UnitsInStock = @UInventario,
	UnitsOnOrder = @UPedido,
	ReorderLevel = @PPedido,
	Discontinued = @Descontinuado
	where ProductID = @Id;
	SET @NumRegs = @@ROWCOUNT;

ALTER PROCEDURE [dbo].[SP_PRODUCTOS_ELIMINAR_V2]
	@Id int, 
	@NumRegs int output
as
	Delete Products
	where ProductID = @Id;
	SET @NumRegs = @@ROWCOUNT;


ALTER   PROCEDURE [dbo].[SP_PRODUCTOS_SELECCIONAR]
	@Categoria int
AS
	SELECT 0 AS Id, '«--- Seleccione ---»' AS Producto
	UNION ALL
	Select ProductId As Id,  ProductName As Producto 
	From Products
	Where CategoryId = @Categoria And Discontinued = 'FALSE'
	Order by Producto


*/