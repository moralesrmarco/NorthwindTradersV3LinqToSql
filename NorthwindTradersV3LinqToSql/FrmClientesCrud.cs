using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmClientesCrud : Form
    {
        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        bool EventoCargado = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv, ojo no quitar

        public FrmClientesCrud()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            DefineMaxLength();
        }

        public void DefineMaxLength()
        {
            txtBId.MaxLength = txtId.MaxLength = 5;
            txtBCompañia.MaxLength = txtCompañia.MaxLength = 40;
            txtBContacto.MaxLength = txtContacto.MaxLength = 30;
            txtTitulo.MaxLength = 30;
            txtBDomicilio.MaxLength = txtDomicilio.MaxLength = 60;
            txtBCiudad.MaxLength = txtCiudad.MaxLength = txtBRegion.MaxLength = txtRegion.MaxLength = 15;
            txtBCodigoP.MaxLength = txtCodigoP.MaxLength = 10;
            txtPais.MaxLength = 15;
            txtBTelefono.MaxLength = txtTelefono.MaxLength = txtBFax.MaxLength = txtFax.MaxLength = 24;
        }

        private void grbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmClientesCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarCboPais();
            LlenarDgv(null);
        }

        private void DeshabilitarControles()
        {
            txtId.ReadOnly = txtCompañia.ReadOnly = txtContacto.ReadOnly = txtTitulo.ReadOnly = true;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = true;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtFax.ReadOnly = true;
            btnOperacion.Visible = false;
        }

        private void HabilitarControles()
        {
            txtId.ReadOnly = txtCompañia.ReadOnly = txtContacto.ReadOnly = txtTitulo.ReadOnly = false;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = false;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtFax.ReadOnly = false;
            btnOperacion.Visible = true;
        }

        private void LlenarCboPais()
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                // Realiza la consulta Linq to Sql
                var query = context.SP_CLIENTES_PAIS();
                // Asigna los datos al combo box
                cboBPais.DataSource = query.ToList();
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
                    var query = from cli in context.SP_CLIENTES_LISTAR(false)
                                select cli;
                    dgv.DataSource = query.ToList();
                }
                else
                {
                    var query = from cli in context.SP_CLIENTES_BUSCAR(txtBId.Text, txtBCompañia.Text, txtBContacto.Text, txtBDomicilio.Text, txtBCiudad.Text, txtBRegion.Text, txtBCodigoP.Text, cboBPais.SelectedValue.ToString(), txtBTelefono.Text, txtBFax.Text)
                                select cli;
                    dgv.DataSource = query.ToList();
                }
                Utils.ConfDgv(dgv);
                ConfDgvClientes(dgv);
                if (sender == null)
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran los primeros {dgv.RowCount} clientes registrados");
                else
                    Utils.ActualizarBarraDeEstado(this, $"Se encontraron {dgv.RowCount} registros");
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

        private void ConfDgvClientes(DataGridView dgv)
        {
            dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Código_postal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["País"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Teléfono"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Ciudad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Región"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Código_postal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["País"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Nombre_de_compañía"].HeaderText = "Nombre de compañía";
            dgv.Columns["Nombre_de_contacto"].HeaderText = "Nombre de contacto";
            dgv.Columns["Título_de_contacto"].HeaderText = "Título de contacto";
            dgv.Columns["Código_postal"].HeaderText = "Código postal";
        }

        private void BorrarDatosCliente()
        {
            txtId.Text = txtCompañia.Text = txtContacto.Text = txtDomicilio.Text = txtCiudad.Text = "";
            txtRegion.Text = txtCodigoP.Text = txtTelefono.Text = txtFax.Text = txtPais.Text = txtTitulo.Text = "";
        }

        private void BorrarMensajesError()
        {
            errorProvider1.SetError(txtId, "");
            errorProvider1.SetError(txtCompañia, "");
            errorProvider1.SetError(txtContacto, "");
            errorProvider1.SetError(txtTitulo, "");
            errorProvider1.SetError(txtDomicilio, "");
            errorProvider1.SetError(txtCiudad, "");
            errorProvider1.SetError(txtPais, "");
            errorProvider1.SetError(txtTelefono, "");
        }

        private void BorrarDatosBusqueda()
        {
            txtBId.Text = txtBCompañia.Text = txtBContacto.Text = txtBDomicilio.Text = txtBCiudad.Text = "";
            txtBRegion.Text = txtBCodigoP.Text = txtBTelefono.Text = txtBFax.Text = "";
            cboBPais.SelectedIndex = 0;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosBusqueda();
            BorrarDatosCliente();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(null);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosCliente();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(sender);
        }

        private bool ValidarControles()
        {
            bool valida = true;
            if (txtId.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtId, "Ingrese el Id del cliente");
            }
            if (txtCompañia.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtCompañia, "Ingrese el nombre de la compañía");
            }
            if (txtContacto.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtContacto, "Ingrese el nombre del contacto");
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

        private void FrmClientesCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tbpListar)
                if (txtId.Text != "" || txtCompañia.Text != "" || txtContacto.Text != "" || txtTitulo.Text != "" || txtDomicilio.Text != "" || txtCiudad.Text != "" || txtRegion.Text != "" || txtCodigoP.Text != "" || txtPais.Text != "" || txtTelefono.Text != "" || txtFax.Text != "")
                {
                    DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (respuesta == DialogResult.No)
                        e.Cancel = true;
                }
        }

        private void FrmClientesCrud_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarDatosCliente();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (EventoCargado)
                {
                    dgv.CellClick -= new DataGridViewCellEventHandler(dgv_CellClick);
                    EventoCargado = false;
                }
                BorrarDatosBusqueda();
                HabilitarControles();
                txtId.Enabled = true;
                txtId.ReadOnly = false;
                btnOperacion.Text = "Registrar cliente";
                btnOperacion.Enabled = true;
            }
            else
            {
                if (!EventoCargado)
                {
                    dgv.CellClick += new DataGridViewCellEventHandler(dgv_CellClick);
                    EventoCargado = true;
                }
                DeshabilitarControles();
                btnOperacion.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpListar)
                    btnOperacion.Visible = false;
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar cliente";
                    btnOperacion.Visible = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Text = "Eliminar cliente";
                    btnOperacion.Visible = true;
                }
            }
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tbpRegistrar)
            {
                DeshabilitarControles();
                DataGridViewRow dgvr = dgv.CurrentRow;
                txtId.Text = dgvr.Cells["Id"].Value.ToString();
                try
                {
                    using (var context = new NorthwindTradersDataContext())
                    {
                        string customerId = txtId.Text;
                        var customer = (from cli in context.Customers
                                        where cli.CustomerID == customerId
                                        select cli).FirstOrDefault();
                        if (customer != null)
                        {
                            txtId.Tag = customer.RowVersion;
                            txtCompañia.Text = customer.CompanyName;
                            txtContacto.Text = customer.ContactName;
                            txtTitulo.Text = customer.ContactTitle;
                            txtDomicilio.Text = customer.Address;
                            txtCiudad.Text = customer.City;
                            if (customer.Region == null) txtRegion.Text = "";
                            else txtRegion.Text = customer.Region;
                            if (customer.PostalCode == null) txtCodigoP.Text = "";
                            else txtCodigoP.Text = customer.PostalCode;
                            txtPais.Text = customer.Country;
                            txtTelefono.Text = customer.Phone;
                            if (customer.Fax == null) txtFax.Text = "";
                            else txtFax.Text = customer.Fax;
                        }
                        else
                        {
                            MessageBox.Show($"No se encontró el cliente con Id: {txtId.Text}, es posible que otro usuario lo haya eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    HabilitarControles();
                    txtId.Enabled = false;
                    btnOperacion.Enabled = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = true;
                }
            }
        }

        private void btnOperacion_Click(object sender, EventArgs e)
        {
            int? numRegs = 0;
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
                        using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                        {
                            string strRegion = "";
                            string strCodigoP = "";
                            string strFax = "";
                            if (txtRegion.Text == "") strRegion = null;
                            else strRegion = txtRegion.Text;
                            if (txtCodigoP.Text == "") strCodigoP = null;
                            else strCodigoP = txtCodigoP.Text;
                            if (txtFax.Text == "") strFax = null;
                            else strFax = txtFax.Text;
                            context.SP_CLIENTES_INSERTAR_V2(txtId.Text, txtCompañia.Text, txtContacto.Text, txtTitulo.Text, txtDomicilio.Text, txtCiudad.Text, strRegion, strCodigoP, txtPais.Text, txtTelefono.Text, strFax, ref numRegs);
                        }
                        if (numRegs > 0)
                            MessageBox.Show($"El cliente con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} se registró satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El cliente con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} NO fue registrado en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SqlException ex) when (ex.Number == 2627)
                    {
                        Utils.MsgCatchErrorClaveDuplicada(this);
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
                        using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                        {
                            string strRegion = "";
                            string strCodigoP = "";
                            string strFax = "";
                            if (txtRegion.Text == "") strRegion = null;
                            else strRegion = txtRegion.Text;
                            if (txtCodigoP.Text == "") strCodigoP = null;
                            else strCodigoP = txtCodigoP.Text;
                            if (txtFax.Text == "") strFax = null;
                            else strFax = txtFax.Text;
                            context.SP_CLIENTES_ACTUALIZAR_V4(txtId.Text, txtCompañia.Text, txtContacto.Text, txtTitulo.Text, txtDomicilio.Text, txtCiudad.Text, strRegion, strCodigoP, txtPais.Text, txtTelefono.Text, strFax, (System.Data.Linq.Binary)txtId.Tag, ref numRegs);
                        }
                        if (numRegs > 0)
                            MessageBox.Show($"El cliente con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} se modificó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El cliente con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} NO fue modificado en la base de datos, es posible que otro usuario lo haya modificado o eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Seleccione el cliente a eliminar", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult respuesta = MessageBox.Show($"Esta seguro de eliminar el cliente con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text}?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    btnOperacion.Enabled = false;
                    Utils.ActualizarBarraDeEstado(this, Utils.eliminandoRegistro);
                    try
                    {
                        using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                        {
                            context.SP_CLIENTES_ELIMINAR_V4(txtId.Text, (System.Data.Linq.Binary)txtId.Tag, ref numRegs);
                        }
                        if (numRegs > 0)
                            MessageBox.Show($"El cliente con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} se eliminó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El cliente con Id: {txtId.Text} y Nombre de Compañía: {txtCompañia.Text} NO se eliminó en la base de datos, es posible que otro usuario lo haya modificado o eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            LlenarDgv(null);
            btnLimpiar.PerformClick();
        }
    }
}
