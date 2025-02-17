﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmClientesyProveedoresDirectorioxPais_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
            Utils.ConfDgv(Dgv);
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
            if (comboBox.SelectedIndex == 0 | (!checkBoxClientes.Checked & !checkBoxProveedores.Checked))
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
                if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                orderby cliprov.País, cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = "» Directorio de clientes y proveedores por país [ Todos los países ] «";
                    Dgv.DataSource = query;
                }
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                where cliprov.País == comboBox.SelectedValue.ToString()
                                orderby cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = $"» Directorio de clientes y proveedores por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                    Dgv.DataSource = query;
                }
                else if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                where cliprov.Relación == "Cliente"
                                orderby cliprov.País, cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = "» Directorio de clientes por país [ Todos los países ] «";
                    Dgv.DataSource = query;
                }
                else if (comboBox.SelectedValue.ToString() == "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                where cliprov.Relación == "Proveedor"
                                orderby cliprov.País, cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = "» Directorio de proveedores por país [ Todos los países ] «";
                    Dgv.DataSource = query;
                }
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                where cliprov.País == comboBox.SelectedValue.ToString() & cliprov.Relación == "Cliente"
                                orderby cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = $"» Directorio de clientes por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                    Dgv.DataSource = query;
                }
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                {
                    var query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS
                                where cliprov.País == comboBox.SelectedValue.ToString() & cliprov.Relación == "Proveedor"
                                orderby cliprov.Ciudad, cliprov.Nombre_de_compañía
                                select cliprov;
                    Grb.Text = $"» Directorio de proveedores por país [ País: {comboBox.SelectedValue.ToString()} ] «";
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

        private void FrmClientesyProveedoresDirectorioxPais_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);
    }
}
