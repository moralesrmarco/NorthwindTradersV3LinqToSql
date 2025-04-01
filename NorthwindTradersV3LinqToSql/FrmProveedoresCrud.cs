using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmProveedoresCrud : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        bool EventoCargado = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv, ojo no quitar

        public FrmProveedoresCrud()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmProveedoresCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarCboPais();
            LlenarDgv(null);
        }

        private void DeshabilitarControles()
        {
            txtCompañia.ReadOnly = txtContacto.ReadOnly = txtTitulo.ReadOnly = true;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = true;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtFax.ReadOnly = true;
        }

        private void HabilitarControles()
        {
            txtCompañia.ReadOnly = txtContacto.ReadOnly = txtTitulo.ReadOnly = false;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = false;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtFax.ReadOnly = false;
        }

        private void LlenarCboPais()
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                var query = context.SP_PROVEEDORES_PAIS();
                cboBPais.DataSource = query;
                cboBPais.DisplayMember = "País";
                cboBPais.ValueMember = "Id";
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

        private void LlenarDgv(object sender)
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                if (sender == null)
                {
                    var query = from prov in context.SP_PROVEEDORES_LISTAR(false)
                                select prov;
                    Dgv.DataSource = query.ToList();
                }
                else
                {
                    int intBIdIni = 0, intBIdFin = 0;
                    if (txtBIdIni.Text != "") intBIdIni = int.Parse(txtBIdIni.Text);
                    if (txtBIdFin.Text != "") intBIdFin = int.Parse(txtBIdFin.Text);
                    var query = context.SP_PROVEEDORES_BUSCAR_V2(intBIdIni, intBIdFin, txtBCompañia.Text, txtBContacto.Text, txtBDomicilio.Text, txtBCiudad.Text, txtBRegion.Text, txtBCodigoP.Text, cboBPais.SelectedValue.ToString(), txtBTelefono.Text, txtBFax.Text);
                    Dgv.DataSource = query.ToList();
                }
                Utils.ConfDgv(Dgv);
                ConfDgv();
                if (sender == null)
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran los últimos {Dgv.RowCount} proveedores registrados");
                else
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
            Dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Título_de_contacto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Ciudad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Región"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Código_postal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["País"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Teléfono"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Ciudad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Región"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Código_postal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["País"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Nombre_de_compañía"].HeaderText = "Nombre de compañía";
            Dgv.Columns["Nombre_de_contacto"].HeaderText = "Nombre de contacto";
            Dgv.Columns["Título_de_contacto"].HeaderText = "Título de contacto";
            Dgv.Columns["Código_postal"].HeaderText = "Código postal";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosBusqueda();
            BorrarDatosProveedor();
            if (tabcOperacion.SelectedTab != tbpRegistrar) DeshabilitarControles();
            LlenarDgv(null);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosProveedor();
            if (tabcOperacion.SelectedTab != tbpRegistrar) DeshabilitarControles();
            LlenarDgv(sender);
        }

        private void BorrarMensajesError()
        {
            errorProvider1.SetError(txtCompañia, "");
            errorProvider1.SetError(txtContacto, "");
            errorProvider1.SetError(txtDomicilio, "");
            errorProvider1.SetError(txtCiudad, "");
            errorProvider1.SetError(txtPais, "");
            errorProvider1.SetError(txtTelefono, "");
        }

        private void BorrarDatosBusqueda()
        {
            txtBIdIni.Text = txtBIdFin.Text = txtBCompañia.Text = txtBContacto.Text = "";
            txtBDomicilio.Text = txtBCiudad.Text = txtBRegion.Text = txtBCodigoP.Text = "";
            cboBPais.SelectedIndex = 0;
            txtBTelefono.Text = txtBFax.Text = "";
        }

        private void BorrarDatosProveedor()
        {
            txtId.Text = txtCompañia.Text = txtContacto.Text = txtTitulo.Text = txtDomicilio.Text = "";
            txtCiudad.Text = txtRegion.Text = txtCodigoP.Text = txtPais.Text = "";
            txtTelefono.Text = txtFax.Text = "";
        }

        private bool ValidarControles()
        {
            bool valida = true;
            if (txtCompañia.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtCompañia, "Ingrese el nombre de la compañía");
            }
            if (txtContacto.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtContacto, "Ingrese el nombre de contacto");
            }
            if (txtTitulo.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtTitulo, "Ingrese el título del contacto");
            }
            if (txtDomicilio.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtDomicilio, "Ingrese el domicilio");
            }
            if (txtCiudad.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtCiudad, "Ingrese la ciudad");
            }
            if (txtPais.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtPais, "Ingrese el país");
            }
            if (txtTelefono.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtTelefono, "Ingrese el teléfono");
            }
            return valida;
        }

        private void txtBIdIni_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtBIdFin_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtBIdIni_Leave(object sender, EventArgs e)
        {
            Utils.ValidaTxtBIdIni(txtBIdIni, txtBIdFin);
        }

        private void txtBIdFin_Leave(object sender, EventArgs e)
        {
            Utils.ValidaTxtBIdFin(txtBIdIni, txtBIdFin);
        }

        private void FrmProveedoresCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tbpConsultar)
            {
                if (txtId.Text != "" || txtCompañia.Text != "" || txtContacto.Text != "" || txtTitulo.Text != "" || txtDomicilio.Text != "" || txtCiudad.Text != "" || txtRegion.Text != "" || txtCodigoP.Text != "" || txtPais.Text != "" || txtTelefono.Text != "" || txtFax.Text != "")
                {
                    DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (respuesta == DialogResult.No) e.Cancel = true;
                }
            }
        }

        private void FrmProveedoresCrud_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tbpRegistrar)
            {
                DeshabilitarControles();
                DataGridViewRow dgvr = Dgv.CurrentRow;
                txtId.Text = dgvr.Cells["Id"].Value.ToString();
                try
                {
                    using (var context = new NorthwindTradersDataContext())
                    {
                        int id = int.Parse(txtId.Text);
                        var supplier = context.Suppliers.FirstOrDefault(s => s.SupplierID == id);
                        if (supplier != null)
                        {
                            txtId.Tag = supplier.RowVersion;
                            txtCompañia.Text = supplier.CompanyName;
                            txtContacto.Text = supplier.ContactName;
                            txtTitulo.Text = supplier.ContactTitle;
                            txtDomicilio.Text = supplier.Address;
                            txtCiudad.Text = supplier.City;
                            if (supplier.Region != null) txtRegion.Text = supplier.Region;
                            else txtRegion.Text = "";
                            if (supplier.PostalCode != null) txtCodigoP.Text = supplier.PostalCode;
                            else txtCodigoP.Text = "";
                            txtPais.Text = supplier.Country;
                            txtTelefono.Text = supplier.Phone;
                            if (supplier.Fax != null) txtFax.Text = supplier.Fax;
                            else txtFax.Text = "";
                        }
                        else
                        {
                            MessageBox.Show($"No se encontro el proveedor con Id: {txtId.Text}, es posible que otro usuario lo haya eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ActualizaDgv();
                            return;
                        }
                    }
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
            if (tabcOperacion.SelectedTab == tbpModificar)
            {
                HabilitarControles();
                btnOperacion.Enabled = true;
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
                btnOperacion.Enabled = true;
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarDatosProveedor();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (EventoCargado)
                {
                    Dgv.CellClick -= new DataGridViewCellEventHandler(Dgv_CellClick);
                    EventoCargado = false;
                }
                BorrarDatosBusqueda();
                HabilitarControles();
                btnOperacion.Text = "Registrar empleado";
                btnOperacion.Visible = true;
                btnOperacion.Enabled = true;
            }
            else
            {
                if (!EventoCargado)
                {
                    Dgv.CellClick += new DataGridViewCellEventHandler(Dgv_CellClick);
                    EventoCargado = true;
                }
                DeshabilitarControles();
                btnOperacion.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpConsultar)
                    btnOperacion.Visible = false;
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar proveedor";
                    btnOperacion.Visible = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Text = "Eliminar proveedor";
                    btnOperacion.Visible = true;
                }
            }
        }

        private void btnOperacion_Click(object sender, EventArgs e)
        {
            int? numRegs = 0;
            int? numId = 0;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (ValidarControles())
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.insertandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    try
                    {
                        string strRegion = "", strCodigoP = "", strFax = "";
                        if (txtRegion.Text == "") strRegion = null;
                        else strRegion = txtRegion.Text;
                        if (txtCodigoP.Text == "") strCodigoP = null;
                        else strCodigoP = txtCodigoP.Text;
                        if (txtFax.Text == "") strFax = null;
                        else strFax = txtFax.Text;
                        context.SP_PROVEEDORES_INSERTAR_V2(txtCompañia.Text, txtContacto.Text, txtTitulo.Text, txtDomicilio.Text, txtCiudad.Text, txtRegion.Text, txtCodigoP.Text, txtPais.Text, txtTelefono.Text, txtFax.Text, ref numId, ref numRegs);
                        if (numRegs > 0)
                        {
                            txtId.Text = numId.ToString();
                            MessageBox.Show($"El proveedor con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} se registró satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            MessageBox.Show($"El proveedor con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} NO fue registrado en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(this, ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(this, ex);
                    }
                    LlenarCboPais();
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpModificar)
            {
                if (ValidarControles())
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.modificandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    try
                    {
                        string strRegion = "", strCodigoP = "", strFax = "";
                        if (txtRegion.Text == "") strRegion = null;
                        else strRegion = txtRegion.Text;
                        if (txtCodigoP.Text == "") strCodigoP = null;
                        else strCodigoP = txtCodigoP.Text;
                        if (txtFax.Text == "") strFax = null;
                        else strFax = txtFax.Text;
                        context.SP_PROVEEDORES_ACTUALIZAR_V4(int.Parse(txtId.Text), txtCompañia.Text, txtContacto.Text, txtTitulo.Text, txtDomicilio.Text, txtCiudad.Text, strRegion, strCodigoP, txtPais.Text, txtTelefono.Text, strFax, (System.Data.Linq.Binary)txtId.Tag, ref numRegs);
                        if (numRegs > 0)
                            MessageBox.Show($"El proveedor con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} se modificó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El proveedor con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} NO fue modificado en la base de datos, es posible que otro usuario lo haya modificado o eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(this, ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(this, ex);
                    }
                    LlenarCboPais();
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
            {
                if (txtId.Text == "")
                {
                    MessageBox.Show("Seleccione el proveedor a eliminar", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult respuesta = MessageBox.Show($"¿Esta seguro de eliminar el proveedor con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text}?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.eliminandoRegistro);
                    btnOperacion.Enabled = false;
                    try
                    {
                        context.SP_PROVEEDORES_ELIMINAR_V4(int.Parse(txtId.Text), (System.Data.Linq.Binary)txtId.Tag, ref numRegs);
                        if (numRegs > 0)
                            MessageBox.Show($"El proveedor con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} se eliminó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El proveedor con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} NO se eliminó en la base de datos, es posible que otro usuario lo haya modificado o eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SqlException ex) when (ex.Number == 547)
                    {
                        Utils.MsgCatchErrorRestriccionCF(this);
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(this, ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(this, ex);
                    }
                    LlenarCboPais();
                    ActualizaDgv();
                }
            }
        }

        private void ActualizaDgv()
        {
            btnLimpiar.PerformClick();
        }
    }
}
