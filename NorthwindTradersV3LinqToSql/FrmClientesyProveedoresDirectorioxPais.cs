using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmClientesyProveedoresDirectorioxPais : Form
    {
        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmClientesyProveedoresDirectorioxPais()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void FrmClientesyProveedoresDirectorioxPais_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
        }

        private void LlenarComboBox()
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                var query = context.SP_CLIENTESPROVEEDORES_PAIS();
                comboBox.DataSource = query;
                comboBox.DisplayMember = "País";
                comboBox.ValueMember = "IdPaís";
            }
            catch (SqlException ex)
            {
                Utils.MsgCatchOueclbdd(this, ex);
            }
            catch (Exception ex)
            {
                Utils.MsgCatchOue(this, ex);
            }
            Utils.ActualizarBarraDeEstado(this);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex == 0)
                return;
            LlenarDgv();
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex == 0)
                return;
            LlenarDgv();
        }

        private void LlenarDgv()
        {
            try
            {
                Dgv.Visible = true;
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                if (comboBox.SelectedValue.ToString() == "aaaaa")
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                orderby cliprov.País, cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Dgv.DataSource = query;
                }
                else
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                where cliprov.País == comboBox.SelectedValue.ToString()
                                orderby cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Dgv.DataSource = query;
                }
                Utils.ConfDgv(Dgv);
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
            Dgv.Columns["País"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Ciudad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Relación"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Teléfono"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Región"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Código_postal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Ciudad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Relación"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Teléfono"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Región"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Código_postal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Fax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Nombre_de_compañía"].HeaderText = "Nombre de compañía";
            Dgv.Columns["Nombre_de_contacto"].HeaderText = "Nombre de contacto";
            Dgv.Columns["Código_postal"].HeaderText = "Código postal";
        }
    }
}
