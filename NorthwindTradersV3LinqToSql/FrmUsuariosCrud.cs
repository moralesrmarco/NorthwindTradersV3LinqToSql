using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmUsuariosCrud : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        bool EventoCargado = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv ojo no quitar
        bool _imagenMostrada = true;
        string passHasheadaOld = string.Empty;
        string usuarioOld = string.Empty;
        string paternoOld, maternoOld, nombresOld, pwdOld, pwdConfirmarOld; // Variables para almacenar los datos del usuario antes de modificarlo
        bool estatusOld; // Variable para almacenar el estatus del usuario antes de modificarlo

        public FrmUsuariosCrud()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmUsuariosCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmUsuariosCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tbpConsultar & tabcOperacion.SelectedTab != tbpEliminar)
            {
                if (paternoOld != txtPaterno.Text || maternoOld != txtMaterno.Text || nombresOld != txtNombres.Text || usuarioOld != txtUsuario.Text || pwdOld != txtPwd.Text || pwdConfirmarOld != txtConfirmarPwd.Text || estatusOld != chkbEstatus.Checked)
                {
                    DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (respuesta == DialogResult.No)
                        e.Cancel = true; // Cancela el cierre del formulario
                }
            }
        }

        private void FrmUsuariosCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarDgv(null);
        }

        private void DeshabilitarControles()
        {
            txtPaterno.ReadOnly = txtMaterno.ReadOnly = txtNombres.ReadOnly = txtUsuario.ReadOnly = txtPwd.ReadOnly = txtConfirmarPwd.ReadOnly = true;
            lblFechaCaptura.Text = lblFechaModificacion.Text = string.Empty;
            chkbEstatus.Enabled = false;
            btnTogglePwd1.Enabled = false;
        }

        private void HabilitarControles()
        {
            txtPaterno.ReadOnly = txtMaterno.ReadOnly = txtNombres.ReadOnly = txtUsuario.ReadOnly = txtPwd.ReadOnly = txtConfirmarPwd.ReadOnly = false;
            btnTogglePwd1.Enabled = true;
            chkbEstatus.Enabled = true;
        }

        private void LlenarDgv(object sender)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                if (sender == null)
                {
                    var query = context.Usuarios
                    .OrderByDescending(u => u.Id)
                    .Take(20)
                    .Select(u => new
                    {
                        u.Id,
                        u.Paterno,
                        u.Materno,
                        u.Nombres,
                        u.Usuario,
                        u.Password,
                        u.FechaCaptura,
                        u.FechaModificacion,
                        Estatus = u.Estatus ? "Activo" : "Inactivo"
                    })
                    .ToList();
                    Dgv.DataSource = query;
                }
                else
                {
                    int intBIdIni = 0, intBIdFin = 0;
                    if (txtBIdIni.Text != "") intBIdIni = int.Parse(txtBIdIni.Text);
                    if (txtBIdFin.Text != "") intBIdFin = int.Parse(txtBIdFin.Text);
                    var query = context.SP_USUARIOS_BUSCAR(intBIdIni, intBIdFin, txtBPaterno.Text, txtBMaterno.Text, txtBNombres.Text, txtBUsuario.Text).ToList();                    
                    Dgv.DataSource= query;

                }
                Utils.ConfDgv(Dgv);
                ConfDgv();
                if (sender == null)
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran los últimos {Dgv.RowCount} usuarios registrados");
                else
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {Dgv.RowCount} registros");
            }
            catch (SqlException ex)
            {
                Utils.MsgCatchOueclbdd(ex);
            }
            catch (Exception ex)
            {
                Utils.MsgCatchOue(ex);
            }
        }

        private void ConfDgv()
        {
            Dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Paterno"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Materno"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Nombres"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Usuario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Password"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["FechaCaptura"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["FechaModificacion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Estatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Usuario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Password"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["FechaCaptura"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["FechaModificacion"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Estatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Paterno"].HeaderText = "Apellido Paterno";
            Dgv.Columns["Materno"].HeaderText = "Apellido Materno";
            Dgv.Columns["Password"].HeaderText = "Contraseña";
            Dgv.Columns["FechaCaptura"].HeaderText = "Fecha de creación";
            Dgv.Columns["FechaModificacion"].HeaderText = "Fecha de modificación";

            Dgv.Columns["FechaCaptura"].DefaultCellStyle.Format = "dd/MMMM/yyyy\nhh:mm:ss tt";
            Dgv.Columns["FechaModificacion"].DefaultCellStyle.Format = "dd/MMMM/yyyy\nhh:mm:ss tt";
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (Dgv.Columns[e.ColumnIndex].Name == "Password" && e.Value != null)
                e.Value = new string('●', 10);
            if (Dgv.Columns[e.ColumnIndex].Name == "Estatus" && e.Value != null)
            {
                string valor = e.Value.ToString();

                if (valor == "Activo" || valor == "True" )
                {
                    e.Value = "Activo";
                    // Fondo verde para activos
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.ForeColor = Color.Black;
                }
                else if (valor == "Inactivo" || valor == "False")
                {
                    e.Value = "Inactivo";
                    // Fondo rojo para inactivos
                    e.CellStyle.BackColor = Color.Red;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosUsuario();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            PonerNoVisibleBtnTogglePwd1();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(null);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosUsuario();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(sender);
        }

        private void BorrarDatosUsuario()
        {
            txtId.Text = txtPaterno.Text = txtMaterno.Text = txtNombres.Text = txtUsuario.Text = txtPwd.Text = txtConfirmarPwd.Text = string.Empty;
            chkbEstatus.Checked = false;
            lblFechaCaptura.Text = lblFechaModificacion.Text = string.Empty;
        }

        private void BorrarMensajesError() => errorProvider1.Clear();

        private void BorrarDatosBusqueda()
        {
            txtBIdIni.Text = txtBIdFin.Text = txtBPaterno.Text = txtBMaterno.Text = txtBNombres.Text = txtBUsuario.Text = string.Empty;
        }

        private void PonerNoVisibleBtnTogglePwd1()
        {
            txtPwd.UseSystemPasswordChar = txtConfirmarPwd.UseSystemPasswordChar = true;
            btnTogglePwd1.Image = Properties.Resources.mostrarCh;
        }

        private bool ValidarControles()
        {
            bool valida = true;
            if (string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                errorProvider1.SetError(txtNombres, "El nombre es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider1.SetError(txtUsuario, "El usuario es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtPwd.Text)) 
            {
                errorProvider1.SetError(txtPwd, "La contraseña es obligatoria");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtConfirmarPwd.Text))
            {
                errorProvider1.SetError(txtConfirmarPwd, "La confirmación de la contraseña es obligatoria");
                valida = false;
            }
            if (valida)
            {
                // Validar que el usuario no exista en la base de datos
                bool existeUsuario = context.Usuarios.Any(u => u.Usuario == txtUsuario.Text);
                if (existeUsuario)
                {
                    errorProvider1.SetError(txtUsuario, "El usuario ya existe, por favor elige otro");
                    valida = false;
                }
                // Validar que las contraseñas coincidan
                if (txtPwd.Text != txtConfirmarPwd.Text)
                {
                    errorProvider1.SetError(txtPwd, "Las contraseñas no coinciden");
                    errorProvider1.SetError(txtConfirmarPwd, "Las contraseñas no coinciden");
                    valida = false;
                }
            }
            return valida;
        }

        private bool ValidarControles1()
        {
            bool valida = true;
            if (string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                errorProvider1.SetError(txtNombres, "El nombre es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider1.SetError(txtUsuario, "El usuario es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtPwd.Text))
            {
                errorProvider1.SetError(txtPwd, "La contraseña es obligatoria");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtConfirmarPwd.Text))
            {
                errorProvider1.SetError(txtConfirmarPwd, "La confirmación de la contraseña es obligatoria");
                valida = false;
            }
            if (valida)
            {
                // Validar que las contraseñas coincidan
                if (txtPwd.Text != txtConfirmarPwd.Text)
                {
                    errorProvider1.SetError(txtPwd, "Las contraseñas no coinciden");
                    errorProvider1.SetError(txtConfirmarPwd, "Las contraseñas no coinciden");
                    valida = false;
                }
            }
            return valida;
        }

        private void txtBIdIni_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdFin_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdIni_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdIni(txtBIdIni, txtBIdFin);

        private void txtBIdFin_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdFin(txtBIdIni, txtBIdFin);

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tbpRegistrar)
            {
                PonerNoVisibleBtnTogglePwd1();
                DeshabilitarControles();
                DataGridViewRow dgvr = Dgv.CurrentRow;
                if (dgvr != null)
                {
                    txtId.Text = dgvr.Cells["Id"].Value.ToString();
                    paternoOld = txtPaterno.Text = dgvr.Cells["Paterno"].Value.ToString();
                    maternoOld = txtMaterno.Text = dgvr.Cells["Materno"].Value.ToString();
                    nombresOld = txtNombres.Text = dgvr.Cells["Nombres"].Value.ToString();
                    usuarioOld = txtUsuario.Text = dgvr.Cells["Usuario"].Value.ToString();
                    pwdOld = txtPwd.Text = dgvr.Cells["Password"].Value.ToString();
                    pwdConfirmarOld = txtConfirmarPwd.Text = txtPwd.Text;// Para que coincidan al editar
                    if (dgvr.Cells["FechaCaptura"].Value != null)
                        lblFechaCaptura.Text = Convert.ToDateTime(dgvr.Cells["FechaCaptura"].Value).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                    else
                        lblFechaCaptura.Text = "Nulo";
                    if (dgvr.Cells["FechaModificacion"].Value != null)
                        lblFechaModificacion.Text = Convert.ToDateTime(dgvr.Cells["FechaModificacion"].Value).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                    else
                        lblFechaModificacion.Text = "Nulo";
                    estatusOld = chkbEstatus.Checked = dgvr.Cells["Estatus"].Value?.ToString() == "Activo";
                    passHasheadaOld = txtPwd.Text.Trim(); // Almacena la contraseña hasheada antes de modificarla
                }
                if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    btnTogglePwd1.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                    btnOperacion.Enabled = true;
            }
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarDatosUsuario();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (EventoCargado)
                {
                    Dgv.CellClick -= Dgv_CellClick; // Desvincula el evento para evitar que se ejecute al cambiar de pestaña
                    EventoCargado = false;
                }
                paternoOld = maternoOld = nombresOld = usuarioOld = pwdOld = pwdConfirmarOld = string.Empty;
                estatusOld = false;
                BorrarDatosBusqueda();
                HabilitarControles();
                btnOperacion.Text = "Registrar usuario";
                btnOperacion.Visible = true;
                btnOperacion.Enabled = true;
            }
            else
            {
                if (!EventoCargado)
                {
                    Dgv.CellClick += Dgv_CellClick;
                    EventoCargado = true;
                }
                DeshabilitarControles();
                btnOperacion.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpConsultar)
                {
                    btnOperacion.Visible = false;
                    btnOperacion.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar usuario";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar) 
                {
                    btnOperacion.Text = "Eliminar usuario";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
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
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                    DeshabilitarControles();
                    PonerNoVisibleBtnTogglePwd1();
                    btnOperacion.Enabled = false;
                    DateTime fechaCaptura = DateTime.Now;
                    try
                    {
                        string passHasheada = Utils.ComputeSha256Hash(txtPwd.Text.Trim());
                        var nuevoUsuario = new Usuarios()
                        {
                            Paterno = txtPaterno.Text.Trim(),
                            Materno = txtMaterno.Text.Trim(),
                            Nombres = txtNombres.Text.Trim(),
                            Usuario = txtUsuario.Text.Trim(),
                            Password = passHasheada,
                            FechaCaptura = fechaCaptura,
                            FechaModificacion = fechaCaptura,
                            Estatus = chkbEstatus.Checked
                        };
                        context.Usuarios.InsertOnSubmit(nuevoUsuario);
                        context.SubmitChanges();
                        numId = nuevoUsuario.Id;
                        bool insertExitoso = numId > 0;
                        numRegs = insertExitoso ? 1 : 0;
                        if (numRegs > 0)
                        {
                            txtId.Text = numId.ToString();
                            lblFechaCaptura.Text = Convert.ToDateTime(nuevoUsuario.FechaCaptura).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                            lblFechaModificacion.Text = Convert.ToDateTime(nuevoUsuario.FechaModificacion).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                            MessageBox.Show($"El usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text} se registró satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            MessageBox.Show($"El usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text} NO fue registrado en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // Error de clave duplicada
                    {
                        errorProvider1.SetError(txtUsuario, "El usuario ya existe, por favor elige otro");
                        return;
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(ex);
                    }
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    btnLimpiar.PerformClick();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpModificar)
            {
                if (txtUsuario.Text.Trim() == usuarioOld && ValidarControles1())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                    DeshabilitarControles();
                    PonerNoVisibleBtnTogglePwd1();
                    btnOperacion.Enabled = false;
                    try
                    {
                        int id = int.Parse(txtId.Text);
                        var usuarioAModificar = context.Usuarios.FirstOrDefault(u => u.Id == id);
                        if (usuarioAModificar != null)
                        {
                            usuarioAModificar.Paterno = txtPaterno.Text.Trim();
                            usuarioAModificar.Materno = txtMaterno.Text.Trim();
                            usuarioAModificar.Nombres = txtNombres.Text.Trim();
                            usuarioAModificar.Usuario = txtUsuario.Text.Trim();
                            if (txtPwd.Text.Trim() != passHasheadaOld)
                                usuarioAModificar.Password = Utils.ComputeSha256Hash(txtPwd.Text.Trim());
                            else
                                usuarioAModificar.Password = passHasheadaOld;
                            usuarioAModificar.FechaModificacion = DateTime.Now;
                            usuarioAModificar.Estatus = chkbEstatus.Checked;
                            var cambios = context.GetChangeSet();
                            numRegs = cambios.Updates.Count;
                            context.SubmitChanges();
                            if (numRegs > 0)
                            {
                                lblFechaModificacion.Text = Convert.ToDateTime(usuarioAModificar.FechaModificacion).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                                MessageBox.Show($"El usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text} se modificó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show($"El usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text} NO se modificó en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(ex);
                    }
                    btnLimpiar.PerformClick();
                    BorrarVariablesOld();
                }
                else if (txtUsuario.Text.Trim() != usuarioOld && ValidarControles())
                {
                    // Cambia la logica con respecto a la versión ado.net, ya que el linq to sql no nos permite modificar el usuario ya que es una llave primaria, por lo que se debe eliminar primero el registro que ya existe y dar de alta uno nuevo,  "No se puede modificar un miembro que define la identidad del objeto. Agregue un nuevo objeto con una nueva identidad y elimine el existente"
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                    DeshabilitarControles();
                    PonerNoVisibleBtnTogglePwd1();
                    btnOperacion.Enabled = false;
                    try
                    {
                        if (context.Connection.State != ConnectionState.Open)
                            context.Connection.Open();
                        using (var tx = context.Connection.BeginTransaction())
                        {
                            context.Transaction = tx;
                            try
                            {
                                int idViejo = int.Parse(txtId.Text);
                                // Lee usuario y permisos viejos
                                var usuarioViejo = context.Usuarios.SingleOrDefault(u => u.Id == idViejo);
                                if (usuarioViejo == null)
                                {
                                    MessageBox.Show($"El usuario con Id: {idViejo} no existe en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                var permisosViejos = context.Permisos.Where(p => p.UsuarioId == idViejo).ToList();
                                // 4b) Crea la nueva entidad sin asignar Id (auto-incremental)
                                // Construyo nuevo objeto Usuarios con la nueva llave
                                var usuarioNuevo = new Usuarios()
                                {
                                    Paterno = txtPaterno.Text.Trim(),
                                    Materno = txtMaterno.Text.Trim(),
                                    Nombres = txtNombres.Text.Trim(),
                                    Usuario = txtUsuario.Text.Trim(),
                                    Password = (txtPwd.Text.Trim() != passHasheadaOld)
                                    ? Utils.ComputeSha256Hash(txtPwd.Text.Trim())
                                    : passHasheadaOld,
                                    FechaModificacion = DateTime.Now,
                                    Estatus = chkbEstatus.Checked
                                };
                                // 5) Inserta _solo_ el usuario y flushea para obtener Id
                                context.Usuarios.InsertOnSubmit(usuarioNuevo);
                                context.SubmitChanges();
                                int nuevoId = usuarioNuevo.Id;
                                // ←–––– Sincroniza tu UI
                                txtId.Text = nuevoId.ToString();
                                // 6) Clona los permisos apuntando al nuevo Id
                                var permisosNuevos = permisosViejos.Select(p => new Permisos
                                {
                                    UsuarioId = nuevoId, // Asigna el nuevo Id
                                    PermisoId = p.PermisoId, // Copia el permiso
                                }).ToList();
                                // 7) Marca para eliminación los registros viejos
                                context.Permisos.DeleteAllOnSubmit(permisosViejos);
                                context.Usuarios.DeleteOnSubmit(usuarioViejo);
                                // 8) Inserta los permisos clonados
                                context.Permisos.InsertAllOnSubmit(permisosNuevos);
                                // 9) Aplica todos los cambios restantes
                                context.SubmitChanges();
                                // 10) Confirma transacción
                                tx.Commit();
                                // 11) Mensaje al usuario
                                lblFechaModificacion.Text = Convert.ToDateTime(usuarioNuevo.FechaModificacion).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                                MessageBox.Show($"El usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text} se modificó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (SqlException ex)
                            {
                                tx.Rollback();
                                Utils.MsgCatchOueclbdd(ex);
                            }
                            catch (Exception ex)
                            {
                                tx.Rollback();
                                Utils.MsgCatchOue(ex);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Utils.MsgCatchOueclbdd(ex);
                    }
                    catch (Exception ex)
                    {
                        Utils.MsgCatchOue(ex);
                    }
                    finally
                    {
                        if (context.Connection.State == ConnectionState.Open)
                            context.Connection.Close();
                    }
                    btnLimpiar.PerformClick();
                    BorrarVariablesOld();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
            {
                if (txtId.Text == "")
                {
                    MessageBox.Show("Seleccione el usuario a eliminar", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar el usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text}, tenga en cuenta que también se eliminaran los permisos que se le hayan concedido en el sistema?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                    btnOperacion.Enabled = false;
                    if (context.Connection.State != ConnectionState.Open)
                        context.Connection.Open();
                    using (var tx = context.Connection.BeginTransaction())
                    {
                        context.Transaction = tx;
                        try
                        {
                            int id = int.Parse(txtId.Text);
                            // Eliminar los permisos asociados al usuario
                            var permisos = context.Permisos.Where(p => p.UsuarioId == id).ToList();
                            context.Permisos.DeleteAllOnSubmit(permisos);
                            // Eliminar el usuario
                            var usuario = context.Usuarios.SingleOrDefault(u => u.Id == id);
                            if (usuario != null)
                                context.Usuarios.DeleteOnSubmit(usuario);
                            // Guardar los cambios
                            context.SubmitChanges();
                            tx.Commit();
                            if (usuario != null)
                                MessageBox.Show($"El usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text} se eliminó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show($"El usuario con Id: {txtId.Text} y Nombre: {txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text} NO se eliminó en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                        catch (SqlException ex)
                        {
                            tx.Rollback();
                            Utils.MsgCatchOueclbdd(ex);
                        }
                        catch (Exception ex)
                        {
                            tx.Rollback();
                            Utils.MsgCatchOue(ex);
                        }
                        finally
                        {
                            if (context.Connection.State == ConnectionState.Open)
                                context.Connection.Close();
                        }
                        btnLimpiar.PerformClick();
                    }
                }
            }
        }

        private void BorrarVariablesOld()
        {
            paternoOld = maternoOld = nombresOld = usuarioOld = pwdOld = pwdConfirmarOld = string.Empty;
            estatusOld = false;
        }

        private void btnTogglePwd1_Click(object sender, EventArgs e)
        {
            _imagenMostrada = !_imagenMostrada;
            txtPwd.UseSystemPasswordChar = !txtPwd.UseSystemPasswordChar;
            txtConfirmarPwd.UseSystemPasswordChar = !txtConfirmarPwd.UseSystemPasswordChar;
            btnTogglePwd1.Image = _imagenMostrada ? Properties.Resources.mostrarCh : Properties.Resources.ocultarCh;
        }

        private void txtPwd_MouseUp(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void txtConfirmarPwd_MouseUp(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            if (tabcOperacion.SelectedTab == tbpModificar)
                if (txtPwd.Text.Trim() != passHasheadaOld)
                    btnTogglePwd1.Enabled = true;
                else
                    btnTogglePwd1.Enabled = false;
        }

        private void txtConfirmarPwd_TextChanged(object sender, EventArgs e)
        {
            if (tabcOperacion.SelectedTab == tbpModificar)
                if (txtConfirmarPwd.Text.Trim() != passHasheadaOld)
                    btnTogglePwd1.Enabled = true;
                else
                    btnTogglePwd1.Enabled = false;
        }
    }
}
