using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmCategoriasProductos : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmCategoriasProductos()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            LoadData();
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void LoadData()
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            //var categorias = from cat in context.SP_CATEGORIAS_LISTAR(true)
            //                 select new
            //                 {
            //                     cat.Id,
            //                     cat.Categoría,
            //                     cat.Descripción,
            //                     Photo = cat.Foto.ToArray()
            //                 };
            var query = from cat in context.Categories
                        orderby cat.CategoryID descending
                        select new
                        {
                            Id = cat.CategoryID,
                            Categoría = cat.CategoryName,
                            Descripción = cat.Description,
                            Foto = cat.Picture.ToArray()
                        };
            dgvCategorias.DataSource = query.ToList();
            //dgvCategorias.DataSource = categorias.ToList();
            Utils.ConfDgv(dgvCategorias);
            ConfDgvCategorias();
        }

        private void ConfDgvCategorias()
        {
            //dgvCategorias.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvCategorias.Columns["Categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //((DataGridViewImageColumn)dgvCategorias.Columns["Photo"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
            //dgvCategorias.Columns["Photo"].DefaultCellStyle.Padding = new Padding(4, 4, 4, 4);
            //// la versión linq to sql marca un error en la siguiente línea, ya le intente pero no se deja
            //dgvCategorias.Columns["Photo"].Width = 50;
            dgvCategorias.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvCategorias.Columns["Categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvCategorias.Columns["Descripción"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dgvCategorias.Columns["Foto"].Width = 50;
            dgvCategorias.Columns["Foto"].DefaultCellStyle.Padding = new Padding(4, 4, 4, 4);
            ((DataGridViewImageColumn)dgvCategorias.Columns["Foto"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
        }

        private void dgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow != null)
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                int categoriaId = (int)dgvCategorias.CurrentRow.Cells["Id"].Value;

                //var productos = context.Products.Where(p => p.CategoryID == categoriaId).ToList();
                var productos = (from p in context.Products
                                 join c in context.Categories on p.CategoryID equals c.CategoryID
                                 join prov in context.Suppliers on p.SupplierID equals prov.SupplierID
                                 where p.CategoryID == categoriaId
                                 select new
                                 {
                                     Id = p.ProductID,
                                     Categoría = c.CategoryName,
                                     Producto = p.ProductName,
                                     Cantidad_por_unidad = p.QuantityPerUnit,
                                     Precio = p.UnitPrice,
                                     Unidades_en_inventario = p.UnitsInStock,
                                     Unidades_en_pedido = p.UnitsOnOrder,
                                     Punto_de_pedido = p.ReorderLevel,
                                     Descontinuado = p.Discontinued,
                                     Descripción_de_categoría = c.Description,
                                     Proveedor = prov.CompanyName
                                 }).ToList();
                dgvProductos.DataSource = productos;
                Utils.ConfDgv(dgvProductos);
                ConfDgvProductos();
                Utils.ActualizarBarraDeEstado(this, $"Se encontraron {dgvCategorias.RowCount} registros en categorías y {dgvProductos.RowCount} registros de productos, en la categoría {dgvCategorias.CurrentRow.Cells["Categoría"].Value}");
            }
        }

        private void ConfDgvProductos()
        {
            dgvProductos.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Cantidad_por_unidad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Unidades_en_inventario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Unidades_en_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Punto_de_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Descontinuado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvProductos.Columns["Proveedor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvProductos.Columns["Cantidad_por_unidad"].HeaderText = "Cantidad por unidad";
            dgvProductos.Columns["Unidades_en_inventario"].HeaderText = "Unidades en inventario";
            dgvProductos.Columns["Unidades_en_pedido"].HeaderText = "Unidades en pedido";
            dgvProductos.Columns["Punto_de_pedido"].HeaderText = "Punto de pedido";
            dgvProductos.Columns["Descripción_de_categoría"].HeaderText = "Descripción de categoría";

            dgvProductos.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "c";

            dgvProductos.Columns["Unidades_en_inventario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProductos.Columns["Unidades_en_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProductos.Columns["Punto_de_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void FrmCategoriasProductos_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }
    }
}
