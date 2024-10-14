﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class MDIPrincipal : Form
    {
        private int childFormNumber = 0;

        public ToolStripStatusLabel ToolStripEstado
        {
            get { return tsslEstado; }
            set { tsslEstado = value; }
        }

        public MDIPrincipal()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Ventana " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
        }

        private void tsmiMantenimientoDeEmpleados_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmEmpleadosCrud frmEmpleadosCrud = new FrmEmpleadosCrud
            {
                MdiParent = this
            };
            frmEmpleadosCrud.Show();
        }

        private void tsmiMantenimientoDeClientes_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmClientesCrud frmClientesCrud = new FrmClientesCrud
            {
                MdiParent = this
            };
            frmClientesCrud.Show();
        }

        private void tsmiDirectorioDeClientesYProveedoresPorCiudad_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmClientesyProveedoresDirectorioxCiudad frmClientesyProveedoresDirectorioxCiudad = new FrmClientesyProveedoresDirectorioxCiudad
            {
                MdiParent = this
            };
            frmClientesyProveedoresDirectorioxCiudad.Show();
        }

        private void tsmiDirectorioDeClientesYProveedoresPorPais_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmClientesyProveedoresDirectorioxPais frmClientesyProveedoresDirectorioxPais = new FrmClientesyProveedoresDirectorioxPais
            {
                MdiParent = this
            };
            frmClientesyProveedoresDirectorioxPais.Show();
        }

        private void tsmiMantenimientoDeProveedores_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmProveedoresCrud frmProveedoresCrud = new FrmProveedoresCrud
            {
                MdiParent = this
            };
            frmProveedoresCrud.Show();
        }

        private void tsmiProveedoresProductos_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmProveedoresProductos frmProveedoresProductos = new FrmProveedoresProductos
            {
                MdiParent = this
            };
            frmProveedoresProductos.Show();
        }

        private void tsmi2DirectorioDeClientesYProveedoresPorCiudad_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmClientesyProveedoresDirectorioxCiudad frmClientesyProveedoresDirectorioxCiudad = new FrmClientesyProveedoresDirectorioxCiudad
            {
                MdiParent = this
            };
            frmClientesyProveedoresDirectorioxCiudad.Show();
        }

        private void tsmi2DirectorioDeClientesYProveedoresPorPais_Click(object sender, EventArgs e)
        {
            Utils.CerrarFormularios();
            FrmClientesyProveedoresDirectorioxPais frmClientesyProveedoresDirectorioxPais = new FrmClientesyProveedoresDirectorioxPais
            {
                MdiParent = this
            };
            frmClientesyProveedoresDirectorioxPais.Show();
        }
    }
}
