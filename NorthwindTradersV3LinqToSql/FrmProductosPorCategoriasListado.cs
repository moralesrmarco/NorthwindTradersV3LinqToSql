using System;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmProductosPorCategoriasListado : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmProductosPorCategoriasListado()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void FrmProductosPorCategoriasListado_Load(object sender, EventArgs e)
        {
            Utils.ConfDgv(DgvListado);
            LlenarDgv();
            ConfDgv();
        }

        private void LlenarDgv()
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            var listadoProductosPorCategorias = from cat_prod in context.VW_PRODUCTOSPORCATEGORIALISTADO
                                                orderby cat_prod.Categoría, cat_prod.Producto
                                                select cat_prod;
                                              DgvListado.DataSource = listadoProductosPorCategorias.ToList();
            AddCheckboxColumn();
            Utils.ActualizarBarraDeEstado(this, $"Se encontraron {DgvListado.RowCount} registros");
        }

        private void AddCheckboxColumn()
        {
            // Asegura que la columna "Descontinuado" sea de tipo checkbox
            DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
            chkColumn.HeaderText = "Descontinuado";
            chkColumn.DataPropertyName = "Descontinuado";
            chkColumn.Name = "Descontinuado";
            chkColumn.TrueValue = true;
            chkColumn.FalseValue = false;

            // Añadir la nueva columna de checkbox al DataGridView
            DgvListado.Columns.Insert(DgvListado.Columns.Count - 1, chkColumn);

            // Ocultar la columna original de Descontinuado que no es de tipo CheckBox
            DgvListado.Columns["Descontinuado"].Visible = false;
        }

        private void ConfDgv()
        {
            DgvListado.Columns["Id_Producto"].Visible = false;

            DgvListado.Columns["Categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["Unidades_en_inventario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["Unidades_en_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["Punto_de_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // fue necesario usar el número de la columna porque hay dos columnas con el texto Descontinuado
            DgvListado.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DgvListado.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvListado.Columns["Unidades_en_inventario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvListado.Columns["Unidades_en_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvListado.Columns["Punto_de_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvListado.Columns["Precio"].DefaultCellStyle.Format = "c";

            DgvListado.Columns["Cantidad_por_unidad"].HeaderText = "Cantidad por unidad";
            DgvListado.Columns["Unidades_en_inventario"].HeaderText = "Unidades en inventario";
            DgvListado.Columns["Unidades_en_pedido"].HeaderText = "Unidades en pedido";
            DgvListado.Columns["Punto_de_pedido"].HeaderText = "Punto de pedido";
        }

        private void FrmProductosPorCategoriasListado_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }

    }
}
