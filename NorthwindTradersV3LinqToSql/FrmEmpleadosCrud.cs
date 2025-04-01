using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmEmpleadosCrud : Form
    {
        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        bool EventoCargado = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv, ojo no quitar
        OpenFileDialog openFileDialog;

        public FrmEmpleadosCrud()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            DefineMaxLength();
        }

        public void DefineMaxLength()
        {
            txtBIdIni.MaxLength = txtBIdFin.MaxLength = txtId.MaxLength = 10;
            txtBNombres.MaxLength = txtNombres.MaxLength = 10;
            txtBApellidos.MaxLength = txtApellidos.MaxLength = 20;
            txtBTitulo.MaxLength = txtTitulo.MaxLength = 30;
            txtTitCortesia.MaxLength = 25;
            txtBDomicilio.MaxLength = txtDomicilio.MaxLength = 60;
            txtBCiudad.MaxLength = txtCiudad.MaxLength = txtBRegion.MaxLength = txtRegion.MaxLength = 15;
            txtBCodigoP.MaxLength = txtCodigoP.MaxLength = 10;
            txtPais.MaxLength = 15;
            txtBTelefono.MaxLength = txtTelefono.MaxLength = 24;
            txtExtension.MaxLength = 4;
        }

        private void grbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmEmpleadosCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarCboPais();
            LlenarCboReportaA();
            LlenarDgv(null);
        }

        private void DeshabilitarControles()
        {
            txtNombres.ReadOnly = txtApellidos.ReadOnly = txtTitulo.ReadOnly = txtTitCortesia.ReadOnly = true;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = true;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtExtension.ReadOnly = true;
            dtpFNacimiento.Enabled = dtpFContratacion.Enabled = false;
            txtNotas.ReadOnly = false;
            cboReportaA.Enabled = false;
            picFoto.Enabled = false;
            btnCargar.Enabled = false;
        }

        private void HabilitarControles()
        {
            txtNombres.ReadOnly = txtApellidos.ReadOnly = txtTitulo.ReadOnly = false;
            txtTitCortesia.ReadOnly = false;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = false;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtExtension.ReadOnly = false;
            txtNotas.ReadOnly = false;
            dtpFNacimiento.Enabled = dtpFContratacion.Enabled = cboReportaA.Enabled = true;
            picFoto.Enabled = true;
            //btnCargar.Enabled = true;  // no se debe habilitar este control para los registros 1 al nueve
        }

        private void LlenarCboPais()
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                // Realiza la consulta Linq to Sql
                var query = context.SP_EMPLEADOS_PAIS();
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
        }

        private void LlenarCboReportaA()
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                var query = context.SP_EMPLEADOS_NOMBRES_V2();
                cboReportaA.DataSource = query.ToList();
                cboReportaA.DisplayMember = "Nombre";
                cboReportaA.ValueMember = "Id";
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

        private void LlenarDgv(object sender)
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                if (sender == null)
                {
                    var query = from emp in context.SP_EMPLEADOS_LISTAR_V2(false)
                                select new
                                {
                                    emp.Id,
                                    emp.Nombres,
                                    emp.Apellidos,
                                    emp.Título,
                                    emp.Fecha_de_nacimiento,
                                    emp.Ciudad,
                                    emp.País,
                                    Photo = emp.Foto.ToArray(),
                                    emp.Reporta_a
                                };
                    dgv.DataSource = query.ToList();
                }
                else
                {
                    int intBIdIni = 0;
                    int intBIdFin = 0;
                    if (txtBIdIni.Text != "")
                        intBIdIni = int.Parse(txtBIdIni.Text);
                    if (txtBIdFin.Text != "")
                        intBIdFin = int.Parse(txtBIdFin.Text);
                    var query = from emp in context.SP_EMPLEADOS_BUSCAR_V3(intBIdIni, intBIdFin, txtBNombres.Text, txtBApellidos.Text, txtBTitulo.Text, txtBDomicilio.Text, txtBCiudad.Text, txtBRegion.Text, txtBCodigoP.Text, cboBPais.SelectedValue.ToString(), txtBTelefono.Text)
                                select new
                                {
                                    emp.Id,
                                    emp.Nombres,
                                    emp.Apellidos,
                                    emp.Título,
                                    emp.Fecha_de_nacimiento,
                                    emp.Ciudad,
                                    emp.País,
                                    Photo = emp.Foto.ToArray(),
                                    emp.Reporta_a
                                };
                    dgv.DataSource = query.ToList();
                }
                Utils.ConfDgv(dgv);
                ConfDgvEmpleados(dgv);
                if (sender == null)
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran los últimos {dgv.RowCount} empleados registrados");
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

        private void ConfDgvEmpleados(DataGridView dgv)
        {
            dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Photo"].Width = 20;
            dgv.Columns["Photo"].DefaultCellStyle.Padding = new Padding(2, 2, 2, 2);
            ((DataGridViewImageColumn)dgv.Columns["Photo"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
            dgv.Columns["Título"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Fecha_de_nacimiento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Ciudad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["País"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Reporta_a"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Título"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Fecha_de_nacimiento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Ciudad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["País"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Reporta_a"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Fecha_de_nacimiento"].DefaultCellStyle.Format = "dd \" de \"MMM\" de \"yyyy";
            dgv.Columns["Fecha_de_nacimiento"].HeaderText = "Fecha de nacimiento";
            dgv.Columns["Reporta_a"].HeaderText = "Reporta a";
            dgv.Columns["Photo"].HeaderText = "Foto";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosBusqueda();
            BorrarDatosEmpleado();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(null);
        }

        private void BorrarMensajesError()
        {
            errorProvider1.SetError(txtId, "");
            errorProvider1.SetError(txtNombres, "");
            errorProvider1.SetError(txtApellidos, "");
            errorProvider1.SetError(txtTitulo, "");
            errorProvider1.SetError(txtTitCortesia, "");
            errorProvider1.SetError(txtDomicilio, "");
            errorProvider1.SetError(txtCiudad, "");
            errorProvider1.SetError(txtPais, "");
            errorProvider1.SetError(txtTelefono, "");
            errorProvider1.SetError(btnCargar, "");
            errorProvider1.SetError(dtpFNacimiento, "");
            errorProvider1.SetError(dtpFContratacion, "");
            errorProvider1.SetError(cboReportaA, "");
        }

        private void BorrarDatosBusqueda()
        {
            txtBIdIni.Text = txtBIdFin.Text = txtBNombres.Text = txtBApellidos.Text = string.Empty;
            txtBTitulo.Text = txtBDomicilio.Text = txtBCiudad.Text = string.Empty;
            txtBRegion.Text = txtBCodigoP.Text = txtBTelefono.Text = string.Empty;
            cboBPais.SelectedIndex = 0;
        }

        private void BorrarDatosEmpleado()
        {
            txtId.Text = txtNombres.Text = txtApellidos.Text = txtTitulo.Text = string.Empty;
            txtTitCortesia.Text = txtDomicilio.Text = txtCiudad.Text = string.Empty;
            txtRegion.Text = txtCodigoP.Text = txtTelefono.Text = txtPais.Text = string.Empty;
            txtExtension.Text = txtNotas.Text = string.Empty;
            cboReportaA.SelectedIndex = -1;
            picFoto.Image = Properties.Resources.FotoPerfil;
            dtpFNacimiento.Value = dtpFNacimiento.MinDate;
            dtpFContratacion.Value = dtpFContratacion.MinDate;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosEmpleado();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(sender);
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

        private bool ValidarControles()
        {
            bool valida = true;
            if (txtNombres.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtNombres, "Ingrese el nombre");
            }
            if (txtApellidos.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtApellidos, "Ingrese el apellido");
            }
            if (txtTitulo.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtTitulo, "Ingrese el título");
            }
            if (txtTitCortesia.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtTitCortesia, "Ingrese el título de cortesia");
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
            if (picFoto.Image == null)
            {
                valida = false;
                errorProvider1.SetError(btnCargar, "Ingrese la foto");
            }
            if (dtpFNacimiento.Value == new DateTime(1753, 1, 1))
            {
                valida = false;
                errorProvider1.SetError(dtpFNacimiento, "Ingrese la fecha de nacimiento");
            }
            if (dtpFContratacion.Value == new DateTime(1753, 1, 1))
            {
                valida = false;
                errorProvider1.SetError(dtpFContratacion, "Ingrese la fecha de contratación");
            }
            if (cboReportaA.SelectedValue.ToString() == "-1")
            {
                valida = false;
                errorProvider1.SetError(cboReportaA, "Seleccione a quien reporta el empleado");
            }
            return valida;
        }

        private void FrmEmpleadosCrud_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this.Owner);

        private void FrmEmpleadosCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tbpListar)
                if (txtId.Text != "" || txtNombres.Text != "" || txtApellidos.Text != "" || txtTitulo.Text != "" || txtTitCortesia.Text != "" || txtDomicilio.Text != "" || txtCiudad.Text != "" || txtRegion.Text != "" || txtCodigoP.Text != "" || txtPais.Text != "" || txtTelefono.Text != "" || txtExtension.Text != "" || dtpFNacimiento.Value != dtpFNacimiento.MinDate || dtpFContratacion.Value != dtpFContratacion.MinDate || txtNotas.Text.Trim() != "" || cboReportaA.SelectedIndex > 0)
                {
                    DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (respuesta == DialogResult.No)
                        e.Cancel = true;
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
                        int employeeId = int.Parse(txtId.Text);
                        var empleado = (from emp in context.Employees
                                        join jefe in context.Employees on emp.ReportsTo equals jefe.EmployeeID into jefes
                                        from jefe in jefes.DefaultIfEmpty()
                                        where emp.EmployeeID == employeeId
                                        select new
                                        {
                                            emp.EmployeeID,
                                            Nombres = emp.FirstName,
                                            Apellidos = emp.LastName,
                                            Título = emp.Title,
                                            TítuloCortesía = emp.TitleOfCourtesy,
                                            FechaNacimiento = emp.BirthDate,
                                            FechaContratación = emp.HireDate,
                                            emp.Address,
                                            emp.City,
                                            emp.Region,
                                            CódigoPostal = emp.PostalCode,
                                            emp.Country,
                                            emp.HomePhone,
                                            emp.Extension,
                                            emp.Photo,
                                            emp.Notes,
                                            emp.ReportsTo,
                                            ReportaA = jefe.LastName + ", " + jefe.FirstName,
                                            emp.RowVersion
                                        }).FirstOrDefault();
                        if (empleado != null)
                        {
                            txtId.Tag = empleado.RowVersion;
                            txtNombres.Text = empleado.Nombres;
                            txtApellidos.Text = empleado.Apellidos;
                            txtTitulo.Text = empleado.Título;
                            txtTitCortesia.Text = empleado.TítuloCortesía;
                            txtDomicilio.Text = empleado.Address;
                            txtCiudad.Text = empleado.City;
                            txtRegion.Text = empleado.Region;
                            txtCodigoP.Text = empleado.CódigoPostal;
                            txtPais.Text = empleado.Country;
                            txtTelefono.Text = empleado.HomePhone;
                            txtExtension.Text = empleado.Extension;
                            dtpFNacimiento.Value = empleado.FechaNacimiento ?? dtpFNacimiento.MinDate;
                            dtpFContratacion.Value = empleado.FechaContratación ?? dtpFContratacion.MinDate;
                            if (empleado.Photo != null)
                            {
                                byte[] foto = empleado.Photo.ToArray();
                                MemoryStream ms;
                                if (employeeId <= 9)
                                {
                                    ms = new MemoryStream(foto, 78, foto.Length - 78);
                                    btnCargar.Enabled = false; // no se permite modificar porque desconozco el formato de la imagen.
                                }
                                else
                                {
                                    ms = new MemoryStream(foto);
                                    btnCargar.Enabled = true;
                                }
                                picFoto.Image = Image.FromStream(ms);
                            }
                            else
                                picFoto.Image = null;
                            txtNotas.Text = empleado.Notes;
                            cboReportaA.SelectedValue = empleado.ReportsTo ?? 0;
                        }
                        else
                        {
                            MessageBox.Show($"No se encontró el empleado con Id: {txtId.Text}, es posible que otro usuario lo haya eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (tabcOperacion.SelectedTab == tbpListar)
                {
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = true;
                    btnCargar.Visible = false;
                }
                if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    HabilitarControles();
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = true;
                    btnCargar.Visible = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Enabled = true;
                    btnOperacion.Visible = true;
                    btnCargar.Visible = false;
                }
            }
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarDatosEmpleado();
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
                btnOperacion.Text = "Registrar empleado";
                btnOperacion.Visible = true;
                btnOperacion.Enabled = true;
                btnCargar.Enabled = true;
                btnCargar.Visible = true;
                cboReportaA.SelectedValue = -1;
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
                btnCargar.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpListar)
                {
                    btnOperacion.Text = "Imprimir empleado";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = true;
                    btnCargar.Visible = false;
                    btnCargar.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar empleado";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                    btnCargar.Visible = true;
                    btnCargar.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Text = "Eliminar empleado";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                    btnCargar.Visible = false;
                    btnCargar.Enabled = false;
                }
            }
        }

        private void btnOperacion_Click(object sender, EventArgs e)
        {
            int? numRegs = 0;
            int? numId = 0;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpListar)
            {
                FrmRptEmpleado frmRptEmpleado = new FrmRptEmpleado();
                frmRptEmpleado.Owner = this;
                frmRptEmpleado.Id = int.Parse(txtId.Text);
                frmRptEmpleado.ShowDialog();
            }
            else if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (ValidarControles())
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.insertandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    byte[] byteFoto = null;
                    Image image = picFoto.Image;
                    ImageConverter converter = new ImageConverter();
                    byteFoto = (byte[])converter.ConvertTo(image, typeof(byte[]));
                    try
                    {
                        using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                        {
                            string strRegion = "";
                            string strCodigoP = "";
                            string strExtension = "";
                            string strNotas = "";
                            int? intReportaa = 0;
                            if (txtRegion.Text == "") strRegion = null;
                            else strRegion = txtRegion.Text;
                            if (txtCodigoP.Text == "") strCodigoP = null;
                            else strCodigoP = txtCodigoP.Text;
                            if (txtExtension.Text == "") strExtension = null;
                            else strExtension = txtExtension.Text;
                            if (txtNotas.Text == "") strNotas = null;
                            else strNotas = txtNotas.Text;
                            if (int.Parse(cboReportaA.SelectedValue.ToString()) == 0) intReportaa = null;
                            else intReportaa = int.Parse(cboReportaA.SelectedValue.ToString());
                            context.SP_EMPLEADOS_INSERTAR_V2(txtNombres.Text, txtApellidos.Text, txtTitulo.Text, txtTitCortesia.Text, dtpFNacimiento.Value, dtpFContratacion.Value, txtDomicilio.Text, txtCiudad.Text, strRegion, strCodigoP, txtPais.Text, txtTelefono.Text, strExtension, strNotas, intReportaa, byteFoto, ref numId, ref numRegs);
                        }
                        if (numRegs > 0)
                        {
                            txtId.Text = numId.ToString();
                            MessageBox.Show($"El empleado con Id: {txtId.Text} y Nombre: {txtNombres.Text} {txtApellidos.Text} se registró satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            MessageBox.Show($"El empleado con Id: {txtId.Text} y Nombre: {txtNombres.Text} {txtApellidos.Text} NO fue registrado en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(this, ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(this, ex);
                    }
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    LlenarCombos();
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
                    byte[] byteFoto = null;
                    Image image = picFoto.Image;
                    ImageConverter converter = new ImageConverter();
                    byteFoto = (byte[])converter.ConvertTo(image, typeof(byte[]));
                    try
                    {
                        using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                        {
                            string strRegion = "";
                            string strCodigoP = "";
                            string strExtension = "";
                            string strNotas = "";
                            int? intReportaa = 0;
                            if (txtRegion.Text == "") strRegion = null;
                            else strRegion = txtRegion.Text;
                            if (txtCodigoP.Text == "") strCodigoP = null;
                            else strCodigoP = txtCodigoP.Text;
                            if (txtExtension.Text == "") strExtension = null;
                            else strExtension = txtExtension.Text;
                            if (txtNotas.Text == "") strNotas = null;
                            else strNotas = txtNotas.Text;
                            if (int.Parse(cboReportaA.SelectedValue.ToString()) == 0) intReportaa = null;
                            else intReportaa = int.Parse(cboReportaA.SelectedValue.ToString());
                            context.SP_EMPLEADOS_ACTUALIZAR_V4(int.Parse(txtId.Text), txtNombres.Text, txtApellidos.Text, txtTitulo.Text, txtTitCortesia.Text, dtpFNacimiento.Value, dtpFContratacion.Value, txtDomicilio.Text, txtCiudad.Text, strRegion, strCodigoP, txtPais.Text, txtTelefono.Text, strExtension, strNotas, intReportaa, byteFoto, (System.Data.Linq.Binary)txtId.Tag, ref numRegs);
                        }
                        if (numRegs > 0)
                            MessageBox.Show($"El empleado con Id: {txtId.Text} y Nombre: {txtNombres.Text} {txtApellidos.Text} se modificó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El empleado con Id: {txtId.Text} y Nombre: {txtNombres.Text} {txtApellidos.Text} NO fue modificado en la base de datos, es posible que otro usuario lo haya modificado o eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(this, ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(this, ex);
                    }
                    LlenarCombos();
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
            {
                if (txtId.Text == "")
                {
                    MessageBox.Show("Seleccione el empleado a eliminar", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult respuesta = MessageBox.Show($"¿Esta seguro de eliminar el empleado con Id: {txtId.Text} y Nombre: {txtNombres.Text} {txtApellidos.Text}?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    btnOperacion.Enabled = false;
                    Utils.ActualizarBarraDeEstado(this, Utils.eliminandoRegistro);
                    try
                    {
                        using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                        {
                            context.SP_EMPLEADOS_ELIMINAR_V4(int.Parse(txtId.Text), (System.Data.Linq.Binary)txtId.Tag, ref numRegs);
                        }
                        if (numRegs > 0)
                            MessageBox.Show($"El empleado con Id: {txtId.Text} y Nombre: {txtNombres.Text} {txtApellidos.Text} se eliminó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El empleado con Id: {txtId.Text} y Nombre: {txtNombres.Text} {txtApellidos.Text} NO se eliminó en la base de datos, es posible que otro usuario lo haya modificado o eliminado previamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    LlenarCombos();
                    ActualizaDgv();
                }
            }
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            // Mostrar el cuadro de diálogo OpenFileDialog
            //La instrucción siguiente es para que nos muestre todos los tipos juntos
            openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Archivos de imagen (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.InitialDirectory = "c:\\Imágenes\\";
            //La instrucción siguiente es para que nos muestre varias filas en el openfiledialog que nos permita abrir por un tipo especifico
            openFileDialog.Filter = "Archivos jpg (*.jpg)|*.jpg|Archivos jpeg (*.jpeg)|*.jpeg|Archivos png (*.png)|*.png|Archivos bmp (*.bmp)|*.bmp";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Cargar la imagen seleccionada en un objeto Image
                Image image = Image.FromFile(openFileDialog.FileName);

                // Mostrar la imagen en un control PictureBox
                picFoto.Image = image;
                errorProvider1.SetError(btnCargar, "");
            }
        }

        private void ActualizaDgv()
        {
            LlenarDgv(null);
            btnLimpiar.PerformClick();
        }

        private void LlenarCombos()
        {
            LlenarCboPais();
            LlenarCboReportaA();
        }
    }
}
