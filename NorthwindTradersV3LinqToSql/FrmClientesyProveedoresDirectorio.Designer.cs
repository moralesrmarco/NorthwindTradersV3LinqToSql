﻿namespace NorthwindTradersV3LinqToSql
{
    partial class FrmClientesyProveedoresDirectorio
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Grb = new System.Windows.Forms.GroupBox();
            this.Dgv = new System.Windows.Forms.DataGridView();
            this.checkBoxProveedores = new System.Windows.Forms.CheckBox();
            this.checkBoxClientes = new System.Windows.Forms.CheckBox();
            this.BtnBuscar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.Grb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(10, 10);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxProveedores);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxClientes);
            this.splitContainer1.Panel1.Controls.Add(this.BtnBuscar);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Grb);
            this.splitContainer1.Size = new System.Drawing.Size(780, 430);
            this.splitContainer1.SplitterDistance = 36;
            this.splitContainer1.TabIndex = 0;
            // 
            // Grb
            // 
            this.Grb.Controls.Add(this.Dgv);
            this.Grb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Grb.Location = new System.Drawing.Point(0, 0);
            this.Grb.Name = "Grb";
            this.Grb.Padding = new System.Windows.Forms.Padding(10);
            this.Grb.Size = new System.Drawing.Size(780, 390);
            this.Grb.TabIndex = 0;
            this.Grb.TabStop = false;
            this.Grb.Text = "» Directorio de clientes y proveedores «";
            this.Grb.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // Dgv
            // 
            this.Dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Dgv.Location = new System.Drawing.Point(10, 25);
            this.Dgv.Name = "Dgv";
            this.Dgv.Size = new System.Drawing.Size(760, 355);
            this.Dgv.TabIndex = 0;
            // 
            // checkBoxProveedores
            // 
            this.checkBoxProveedores.AutoSize = true;
            this.checkBoxProveedores.Checked = true;
            this.checkBoxProveedores.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProveedores.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxProveedores.Location = new System.Drawing.Point(319, 13);
            this.checkBoxProveedores.Name = "checkBoxProveedores";
            this.checkBoxProveedores.Size = new System.Drawing.Size(116, 20);
            this.checkBoxProveedores.TabIndex = 21;
            this.checkBoxProveedores.Text = "Proveedores";
            this.checkBoxProveedores.UseVisualStyleBackColor = true;
            // 
            // checkBoxClientes
            // 
            this.checkBoxClientes.AutoSize = true;
            this.checkBoxClientes.Checked = true;
            this.checkBoxClientes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxClientes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxClientes.Location = new System.Drawing.Point(203, 13);
            this.checkBoxClientes.Name = "checkBoxClientes";
            this.checkBoxClientes.Size = new System.Drawing.Size(82, 20);
            this.checkBoxClientes.TabIndex = 20;
            this.checkBoxClientes.Text = "Clientes";
            this.checkBoxClientes.UseVisualStyleBackColor = true;
            // 
            // BtnBuscar
            // 
            this.BtnBuscar.Location = new System.Drawing.Point(463, 10);
            this.BtnBuscar.Name = "BtnBuscar";
            this.BtnBuscar.Size = new System.Drawing.Size(75, 24);
            this.BtnBuscar.TabIndex = 19;
            this.BtnBuscar.Text = "Buscar";
            this.BtnBuscar.UseVisualStyleBackColor = true;
            this.BtnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(83, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Buscar por:";
            // 
            // FrmClientesyProveedoresDirectorio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmClientesyProveedoresDirectorio";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "» Directorio de clientes y proveedores «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmClientesyProveedoresDirectorio_FormClosed);
            this.Load += new System.EventHandler(this.FrmClientesyProveedoresDirectorio_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.Grb.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox checkBoxProveedores;
        private System.Windows.Forms.CheckBox checkBoxClientes;
        private System.Windows.Forms.Button BtnBuscar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox Grb;
        private System.Windows.Forms.DataGridView Dgv;
    }
}