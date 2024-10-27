/*
ALTER PROCEDURE [dbo].[SP_CATEGORIAS_LISTAR]
	@top100 Bit = 1 
AS
BEGIN
	If @top100 = 1
	BEGIN
		SELECT Categories.CategoryID as Id, Categories.CategoryName as Categoría, Categories.Description as [Descripción], Categories.Picture as Foto
		FROM Categories
		ORDER BY CategoryID DESC
	END
	ELSE
	BEGIN 
		SELECT TOP 20 Categories.CategoryID as Id, Categories.CategoryName as Categoría, Categories.Description as [Descripción], Categories.Picture as Foto
		FROM Categories
		ORDER BY CategoryID DESC
	END
END

ALTER PROCEDURE [dbo].[SP_CATEGORIAS_BUSCAR_V2]
	@IdIni int,
	@IdFin int,
	@Categoria nvarchar(15)
AS
BEGIN
	SELECT Categories.CategoryID as Id, Categories.CategoryName as Categoría, Categories.Description as Descripción, 
	Categories.Picture as Foto
	FROM Categories
	WHERE
	(@IdIni = 0 OR Categories.CategoryID BETWEEN @IdIni AND @IdFin) AND 
	(@Categoria = '' OR Categories.CategoryName LIKE '%' + @Categoria + '%')
	ORDER BY Categories.CategoryID DESC
END

ALTER PROCEDURE [dbo].[SP_CATEGORIAS_INSERTAR_V2]
    @Categoria NVARCHAR(15),
    @Descripcion NVARCHAR(max),
	@Foto image,
    @Id INT OUTPUT,
	@NumRegs int output
as
	INSERT INTO Categories
	(CategoryName, Description, Picture)
	VALUES(@Categoria, @Descripcion, @Foto);
	SET @Id = SCOPE_IDENTITY();
	SET @NumRegs = @@ROWCOUNT;

ALTER PROCEDURE [dbo].[SP_CATEGORIAS_ACTUALIZAR_V2]
	@Id Int,
    @Categoria NVARCHAR(15),
    @Descripcion NVARCHAR(max),
	@Foto image,
	@NumRegs int output
AS
	UPDATE Categories
	SET CategoryName = @Categoria,
	Description = @Descripcion,
	Picture = @Foto
	where CategoryID = @Id;
	SET @NumRegs = @@ROWCOUNT;

ALTER PROCEDURE [dbo].[SP_CATEGORIAS_ELIMINAR_V2]
	@Id int,
	@NumRegs int output
as
	Delete Categories
	where CategoryID = @Id;
	SET @NumRegs = @@ROWCOUNT;

ALTER PROCEDURE [dbo].[SP_CATEGORIAS_SELECCIONAR_V2]
AS
	SELECT 0 AS Id, '«--- Seleccione ---»' AS Categoria
	UNION ALL
	Select CategoryId As Id, CategoryName As Categoria From Categories


*/