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
    public partial class FrmProveedoresProductos : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        BindingSource bsProveedores = new BindingSource();
        BindingSource bsProductos = new BindingSource();

        public FrmProveedoresProductos()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void FrmProveedoresProductos_Load(object sender, EventArgs e)
        {
            DgvProveedores.DataSource = bsProveedores;
            DgvProductos.DataSource = bsProductos;
            GetData();
            Utils.ConfDgv(DgvProveedores);
            Utils.ConfDgv(DgvProductos);
            ConfDgvProveedores();
            ConfDgvProductos();
        }

        private void GetData()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                DataSet ds = new DataSet();
                ds.Locale = System.Globalization.CultureInfo.InvariantCulture;

                var proveedores = context.SP_PROVEEDORES_LISTAR(true).ToList();
                var productos = context.SP_PRODUCTOS_ALL(true).ToList();

                // Convertir listas en DataTables y agregar al DataSet
                ds.Tables.Add(ToDataTable(proveedores, "Proveedores"));
                ds.Tables.Add(ToDataTable(productos, "Productos"));

                // Crear la relación entre las tablas
                DataRelation dataRelation = new DataRelation(
                    "ProveedoresProductos",
                    ds.Tables["Proveedores"].Columns["Id"],
                    ds.Tables["Productos"].Columns["IdProveedor"]
                );
                ds.Relations.Add(dataRelation);

                bsProveedores.DataSource = ds;
                bsProveedores.DataMember = "Proveedores";
                bsProductos.DataSource = bsProveedores;
                bsProductos.DataMember = "ProveedoresProductos";
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

        private void ConfDgvProveedores()
        {
            DgvProveedores.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Título_de_contacto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Ciudad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Región"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Código_postal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["País"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Teléfono"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DgvProveedores.Columns["Ciudad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProveedores.Columns["Región"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProveedores.Columns["Código_postal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProveedores.Columns["País"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvProveedores.Columns["Nombre_de_compañía"].HeaderText = "Nombre de compañía";
            DgvProveedores.Columns["Nombre_de_contacto"].HeaderText = "Nombre de contacto";
            DgvProveedores.Columns["Título_de_contacto"].HeaderText = "Título de contacto";
            DgvProveedores.Columns["Código_postal"].HeaderText = "Código postal";
        }

        private void ConfDgvProductos()
        {
            DgvProductos.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProductos.Columns["Precio"].DefaultCellStyle.Format = "c";
            DgvProductos.Columns["Unidades_en_inventario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProductos.Columns["Unidades_en_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProductos.Columns["Punto_de_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvProductos.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Unidades_en_inventario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Unidades_en_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Punto_de_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Descontinuado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Proveedor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Cantidad_por_unidad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["Descripción_de_categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DgvProductos.Columns["IdCategoria"].Visible = false;
            DgvProductos.Columns["IdProveedor"].Visible = false;

            DgvProductos.Columns["Cantidad_por_unidad"].HeaderText = "Cantidad por unidad";
            DgvProductos.Columns["Unidades_en_inventario"].HeaderText = "Unidades en inventario";
            DgvProductos.Columns["Unidades_en_pedido"].HeaderText = "Unidades en pedido";
            DgvProductos.Columns["Punto_de_pedido"].HeaderText = "Punto de pedido";
            DgvProductos.Columns["Descripción_de_categoría"].HeaderText = "Descripción de categoría";
        }

        // Método para convertir una lista en DataTable
        public static DataTable ToDataTable<T>(IList<T> data, string tableName)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable(tableName);
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private void FrmProveedoresProductos_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }

        private void DgvProveedores_SelectionChanged(object sender, EventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this, $"Se encontraron {DgvProveedores.RowCount} registros en proveedores y {DgvProductos.RowCount} registros de productos; del proveedor {DgvProveedores.CurrentRow.Cells["Nombre_de_compañía"].Value}");
        }

        private void DgvProveedores_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this, $"Se encontraron {DgvProveedores.RowCount} registros en proveedores");
        }
    }
}
