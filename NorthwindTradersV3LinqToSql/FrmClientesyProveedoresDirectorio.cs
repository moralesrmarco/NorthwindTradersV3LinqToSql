using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmClientesyProveedoresDirectorio : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmClientesyProveedoresDirectorio()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmClientesyProveedoresDirectorio_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void FrmClientesyProveedoresDirectorio_Load(object sender, EventArgs e) => Utils.ConfDgv(Dgv);

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (!checkBoxClientes.Checked & !checkBoxProveedores.Checked)
            {
                MessageBox.Show(Utils.errorCriterioSelec, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LlenarDgv();
        }

        private void LlenarDgv()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                if (checkBoxClientes.Checked & checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD
                                orderby cliprov.Relación, cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = "» Directorio de clientes y proveedores «";
                    Dgv.DataSource = query;
                }
                else if (checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD
                                where cliprov.Relación == "Cliente"
                                orderby cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = "» Directorio de clientes «";
                    Dgv.DataSource = query;
                }
                else if (!checkBoxClientes.Checked & checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD
                                where cliprov.Relación == "Proveedor"
                                orderby cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = "» Directorio de proveedores «";
                    Dgv.DataSource = query;
                }
                ConfDgv();
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
            Dgv.Columns["Ciudad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["País"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Relación"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Teléfono"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Región"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Código_postal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["País"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Relación"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Teléfono"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Región"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Código_postal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Fax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Nombre_de_compañía"].HeaderText = "Nombre de compañía";
            Dgv.Columns["Nombre_de_contacto"].HeaderText = "Nombre de contacto";
            Dgv.Columns["Código_postal"].HeaderText = "Código postal";

            Dgv.Columns["Nombre_de_compañía"].DisplayIndex = 0;
            Dgv.Columns["Nombre_de_contacto"].DisplayIndex = 1;
            Dgv.Columns["Relación"].DisplayIndex = 2;
            Dgv.Columns["Domicilio"].DisplayIndex = 3;
            Dgv.Columns["Ciudad"].DisplayIndex = 4;
            Dgv.Columns["Región"].DisplayIndex = 5;
            Dgv.Columns["Código_postal"].DisplayIndex = 6;
            Dgv.Columns["País"].DisplayIndex = 7;
            Dgv.Columns["Teléfono"].DisplayIndex = 8;
            Dgv.Columns["Fax"].DisplayIndex = 9;
        }
    }
}
