using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmProductosConsultaAlfabetica : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmProductosConsultaAlfabetica()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void FrmProductosConsultaAlfabetica_Load(object sender, EventArgs e)
        {
            Utils.ConfDgv(Dgv);
            LlenarDgv();
            ConfDgv();
        }

        private void LlenarDgv()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = from prod in context.VW_PRODUCTOSLISTAALFABETICA
                            orderby prod.Producto
                            select prod;

                Dgv.DataSource = query;

                Utils.ActualizarBarraDeEstado(this, $"Se encontraron {Dgv.RowCount} registros");
            }
            catch (SqlException ex)
            {
                Utils.MsgCatchOueclbdd(this, ex);
            }
            catch (Exception ex)
            {
                Utils.MsgCatchOue(this, ex);
            }
        }

        private void ConfDgv()
        {
            Dgv.Columns["IdProveedor"].Visible = false;
            Dgv.Columns["IdCategoria"].Visible = false;

            Dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Unidades_en_inventario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Unidades_en_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Punto_de_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Descontinuado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Unidades_en_inventario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Unidades_en_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Punto_de_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Precio"].DefaultCellStyle.Format = "c";

            Dgv.Columns["Cantidad_por_unidad"].HeaderText = "Cantidad por unidad";
            Dgv.Columns["Unidades_en_inventario"].HeaderText = "Unidades en inventario";
            Dgv.Columns["Unidades_en_pedido"].HeaderText = "Unidades en pedido";
            Dgv.Columns["Punto_de_pedido"].HeaderText = "Punto de pedido";            
        }

        private void FrmProductosConsultaAlfabetica_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }
    }
}
