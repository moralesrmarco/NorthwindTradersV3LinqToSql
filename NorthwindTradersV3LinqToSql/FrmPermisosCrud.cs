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
    public partial class FrmPermisosCrud : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmPermisosCrud()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void GrbPaint2(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmPermisosCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmPermisosCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarListBoxCatalogo();
            LlenarDgv(null);
        }

        private void DeshabilitarControles()
        {
            listBoxCatalogo.Enabled = false;
            listBoxConcedidos.Enabled = false;
            listBoxCatalogo.Visible = false;
            listBoxConcedidos.Visible = false;
            txtUsuario.Visible = false;
            txtId.Visible = false;
            txtNombre.Visible = false;
            BtnAgregar.Enabled = BtnQuitar.Enabled = BtnAgregarTodos.Enabled = BtnQuitarTodos.Enabled = false;
        }

        private void HabilitarControles()
        {
            listBoxCatalogo.Enabled = true;
            listBoxConcedidos.Enabled = true;
            listBoxCatalogo.Visible = true;
            listBoxConcedidos.Visible = true;
            txtUsuario.Visible = true;
            txtId.Visible = true;
            txtNombre.Visible = true;
            BtnAgregar.Enabled = BtnQuitar.Enabled = BtnAgregarTodos.Enabled = BtnQuitarTodos.Enabled = true;
        }

        private void LlenarListBoxCatalogo()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var query = context.CatalogoPermisos
                    .Where(cp => cp.Estatus == true)
                    .OrderBy(cp => cp.PermisoId)
                    .Select(cp => new
                    {
                        cp.PermisoId,
                        cp.Descripción
                    });
                listBoxCatalogo.DataSource = query.ToList();
                listBoxCatalogo.DisplayMember = "Descripción";
                listBoxCatalogo.ValueMember = "PermisoId";
                listBoxCatalogo.SelectedIndex = -1;
                MDIPrincipal.ActualizarBarraDeEstado();
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

        private void LlenarDgv(object sender)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                // 1) Base de la consulta y filtro por usuarios activos
                var query = context.Usuarios
                    .Where(u => u.Estatus)
                    .AsQueryable();
                if (sender == null)
                {
                    // 2a) Últimos 20 registros
                    query = query
                        .OrderByDescending(u => u.Id)
                        .Take(20);
                }
                else
                {
                    // 2b) Filtros dinámicos
                    if (int.TryParse(txtBIdIni.Text.Trim(), out int idIni) && int.TryParse(txtBIdFin.Text.Trim(), out int idFin) && idIni > 0 && idFin >= idIni)
                    {
                        query = query.Where(u => u.Id >= idIni && u.Id <= idFin);
                    }
                    if (!string.IsNullOrWhiteSpace(txtBPaterno.Text.Trim()))
                    {
                        string paterno = txtBPaterno.Text.Trim();
                        query = query.Where(u => u.Paterno.Contains(paterno));
                    }
                    if (!string.IsNullOrWhiteSpace(txtBMaterno.Text.Trim()))
                    {
                        string materno = txtBMaterno.Text.Trim();
                        query = query.Where(u => u.Materno.Contains(materno));
                    }
                    if (!string.IsNullOrWhiteSpace(txtBNombres.Text.Trim()))
                    {
                        string nombres = txtBNombres.Text.Trim();
                        query = query.Where(u => u.Nombres.Contains(nombres));
                    }
                    if (!string.IsNullOrWhiteSpace(txtBUsuario.Text.Trim()))
                    {
                        string usuario = txtBUsuario.Text.Trim();
                        query = query.Where(u => u.Usuario.Contains(usuario));
                    }
                    query = query
                        .OrderBy(u => u.Paterno)
                        .ThenBy(u => u.Materno)
                        .ThenBy(u => u.Nombres)
                        .ThenBy(u => u.Usuario);
                }
                // 3) Proyección incluyendo columna de texto para Estatus
                var lista = query
                    .Select(u => new
                    {
                        u.Id,
                        u.Paterno,
                        u.Materno,
                        u.Nombres,
                        u.Usuario,
                        u.FechaCaptura,
                        u.FechaModificacion,
                        Estatus = u.Estatus ? "Activo" : "Inactivo"
                    })
                    .ToList();
                // 4) Bind al DataGridView y configuración
                Dgv.DataSource = lista;
                Utils.ConfDgv(Dgv);
                ConfDgv();
                // 5) Mensaje en la barra de estado
                string msg = sender == null
                    ? $"Se muestran los últimos {Dgv.RowCount} usuarios registrados"
                    : $"Se encontraron {Dgv.RowCount} registros";
                MDIPrincipal.ActualizarBarraDeEstado(msg);
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
            Dgv.Columns["FechaCaptura"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["FechaModificacion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Estatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Usuario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["FechaCaptura"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["FechaModificacion"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Estatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Paterno"].HeaderText = "Apellido Paterno";
            Dgv.Columns["Materno"].HeaderText = "Apellido Materno";
            Dgv.Columns["FechaCaptura"].HeaderText = "Fecha de creación";
            Dgv.Columns["FechaModificacion"].HeaderText = "Fecha de modificación";

            Dgv.Columns["FechaCaptura"].DefaultCellStyle.Format = "dd/MMMM/yyyy\nhh:mm:ss tt";
            Dgv.Columns["FechaModificacion"].DefaultCellStyle.Format = "dd/MMMM/yyyy\nhh:mm:ss tt";
        }

        private void txtBIdIni_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdFin_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdIni_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdIni(txtBIdIni, txtBIdFin);

        private void txtBIdFin_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdFin(txtBIdIni, txtBIdFin);

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosPermisos();
            BorrarDatosBusqueda();
            DeshabilitarControles();
            LlenarDgv(null);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosPermisos();
            DeshabilitarControles();
            LlenarDgv(sender);
        }

        private void BorrarDatosPermisos()
        {
            listBoxConcedidos.DataSource = null;
            listBoxConcedidos.Items.Clear();
        }

        private void BorrarDatosBusqueda()
        {
            txtBIdIni.Text = string.Empty;
            txtBIdFin.Text = string.Empty;
            txtBPaterno.Text = string.Empty;
            txtBMaterno.Text = string.Empty;
            txtBNombres.Text = string.Empty;
            txtBUsuario.Text = string.Empty;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            HabilitarControles();
            if (e.RowIndex >= 0 && e.RowIndex < Dgv.RowCount)
            {
                DataGridViewRow row = Dgv.Rows[e.RowIndex];
                txtId.Text = row.Cells["Id"].Value.ToString();
                txtUsuario.Text = row.Cells["Usuario"].Value.ToString();
                txtNombre.Text = $"{row.Cells["Paterno"].Value} {row.Cells["Materno"].Value} {row.Cells["Nombres"].Value}";
                LlenarListBoxConcedidos();
            }
        }

        private void LlenarListBoxConcedidos()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                int id = int.Parse(txtId.Text.Trim());
                // 1) Consulta LINQ to SQL
                var query = context.Permisos
                    .Where(p => p.UsuarioId == id && p.CatalogoPermisos.Estatus)
                    .OrderBy(p => p.PermisoId)
                    .Select(p => new
                    {
                        p.PermisoId,
                        p.CatalogoPermisos.Descripción
                    }).ToList();
                listBoxConcedidos.DataSource = query;
                listBoxConcedidos.DisplayMember = "Descripción";
                listBoxConcedidos.ValueMember = "PermisoId";
                listBoxConcedidos.SelectedIndex = -1;
                MDIPrincipal.ActualizarBarraDeEstado();
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

        private void BtnAgregarTodos_Click(object sender, EventArgs e)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
            // 1) Identificador de usuario
            int usuarioId = int.Parse(txtId.Text.Trim());

            // 2) IDs de los permisos ya asignados (aún sin traer a memoria)
            //   select PermisoId from permisos where UsuarioId = 2 
            var permisosViejosQuery = context.Permisos
                .Where(p => p.UsuarioId == usuarioId)
                .Select(p => p.PermisoId);

            // 3) IDs nuevos: LINQ to SQL traduce !Contains(...) a NOT IN(...)
            /*
            * SELECT PermisoId
            FROM CatalogoPermisos
            WHERE Estatus = 1
              AND PermisoId NOT IN (
                  SELECT PermisoId
                  FROM Permisos
                  WHERE UsuarioId = 2
              )
             * */
            var nuevosIds = context.CatalogoPermisos
                .Where(cp => cp.Estatus && !permisosViejosQuery.Contains(cp.PermisoId))
                .Select(cp => cp.PermisoId)
                .ToList();
            // 3b) Ahora, en memoria, proyectas a la entidad Permisos
            /*
             * Select 2 As usuarioId, PermisoId From (
                SELECT PermisoId
                FROM CatalogoPermisos
                WHERE Estatus = 1
                  AND PermisoId NOT IN (
                      SELECT PermisoId
                      FROM Permisos
                      WHERE UsuarioId = 2
                  )
                  ) As X
             * */
            var nuevosPermisos = nuevosIds
                .Select(id => new Permisos
                {
                    UsuarioId = usuarioId,
                    PermisoId = id,
                }).ToList();
            // 4) Si no hay nada nuevo, salir
            if (!nuevosPermisos.Any())
            {
                MDIPrincipal.ActualizarBarraDeEstado();
                return;
            }
            // 5) Asegurar conexión abierta
            if (context.Connection.State != ConnectionState.Open)
                context.Connection.Open();
            // 6) Iniciar transacción ligada al DataContext
            using (var tx = context.Connection.BeginTransaction())
            {
                context.Transaction = tx;
                try
                {
                    // 7) Insertar los nuevos permisos
                    context.Permisos.InsertAllOnSubmit(nuevosPermisos);
                    context.SubmitChanges();
                    // 8) Confirmar transacción
                    tx.Commit();
                    // 9) Actualizar UI y notificar
                    LlenarListBoxConcedidos();
                    MessageBox.Show($"Se concedieron todos los permisos al usuario {txtUsuario.Text}", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (SqlException ex)
                {
                    // 10) Revertir en caso de error
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
                    // 11) Cerrar conexión
                    if (context.Connection.State == ConnectionState.Open)
                        context.Connection.Close();
                }
                MDIPrincipal.ActualizarBarraDeEstado();
            }
        }

        private void BtnQuitarTodos_Click(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                int usuarioId = int.Parse(txtId.Text.Trim());
                if (context.Connection.State != ConnectionState.Open)
                    context.Connection.Open();
                // Obtén la lista de permisos a eliminar
                var listaPermisos = context.Permisos
                    .Where(p => p.UsuarioId == usuarioId)
                    .ToList();
                // Cuenta cuántos registros vamos a borrar
                int filasAfectadas = listaPermisos.Count;
                if (filasAfectadas > 0)
                {
                    // Marca todos para eliminación
                    context.Permisos.DeleteAllOnSubmit(listaPermisos);
                    // Envía los cambios a la base de datos
                    context.SubmitChanges();
                    // Actualiza la lista de permisos concedidos
                    LlenarListBoxConcedidos();
                    // Muestra un mensaje de éxito
                    MessageBox.Show($"Se eliminaron {filasAfectadas} permisos concedidos al usuario {txtUsuario.Text}", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    // No había permisos para ese usuario
                    MessageBox.Show($"No se encontraron permisos concedidos al usuario {txtUsuario.Text}", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                MDIPrincipal.ActualizarBarraDeEstado();
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
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (listBoxCatalogo.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar un permiso del catálogo para agregarlo.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
            try
            {
                int usuarioId = int.Parse(txtId.Text.Trim());
                int permisoId = (int)listBoxCatalogo.SelectedValue;
                if (context.Connection.State != ConnectionState.Open)
                    context.Connection.Open();
                // Verificar si ya existe (para evitar excepción de clave única)
                bool yaExiste = context.Permisos
                    .Any(p => p.UsuarioId == usuarioId && p.PermisoId == permisoId);
                if (!yaExiste) 
                {
                    // Insertar nuevo permiso
                    var nuevoPermiso = new Permisos
                    {
                        UsuarioId = usuarioId,
                        PermisoId = permisoId
                    };
                    context.Permisos.InsertOnSubmit(nuevoPermiso);
                    // Guardar cambios (LINQ to SQL abre/ciierra transacción automáticamente)
                    context.SubmitChanges();
                }
                LlenarListBoxConcedidos();
            }
            catch (SqlException ex) when (ex.Number == 2627) 
            {
                // Violación de clave única: permiso ya existe, no hacemos nada
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
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            if (listBoxConcedidos.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar un permiso concedido para eliminarlo.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
            try
            {
                int usuarioId = int.Parse(txtId.Text.Trim());
                int permisoId = (int)listBoxConcedidos.SelectedValue;
                if (context.Connection.State != ConnectionState.Open)
                    context.Connection.Open();
                // Buscar el permiso a eliminar
                var permisoAEliminar = context.Permisos
                    .FirstOrDefault(p => p.UsuarioId == usuarioId && p.PermisoId == permisoId);
                if (permisoAEliminar != null)
                {                     
                    // Eliminar el permiso
                    context.Permisos.DeleteOnSubmit(permisoAEliminar);
                    // Guardar cambios (LINQ to SQL abre/ciierra transacción automáticamente)
                    context.SubmitChanges();
                    // Actualizar la lista de permisos concedidos
                    LlenarListBoxConcedidos();
                }
                else
                {
                    MessageBox.Show("El permiso seleccionado no existe o ya ha sido eliminado.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string estado = e.Value.ToString();
            if (estado == "Activo")
            {
                e.CellStyle.BackColor = Color.LightGreen;
                e.CellStyle.ForeColor = Color.Black;
            }
            else if (estado == "Inactivo")
            {
                e.CellStyle.BackColor = Color.Red;
                e.CellStyle.ForeColor = Color.White;
            }
        }
    }
}
