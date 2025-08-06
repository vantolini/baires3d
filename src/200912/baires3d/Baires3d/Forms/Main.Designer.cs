using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace b3d
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Barrios");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Puntos de interés");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Calles");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opcionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataBuilderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLugares = new System.Windows.Forms.TabPage();
            this.tvResultados = new System.Windows.Forms.TreeView();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabRecorridos = new System.Windows.Forms.TabPage();
            this.tvRecorridos = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.cmbColectivos = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.cmbCalleDestino = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbEsquinaDestino = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbCalleInicial = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbEsquinaInicial = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.tvCapas = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.wbDestacados = new System.Windows.Forms.WebBrowser();
            this.button2 = new System.Windows.Forms.Button();
            this.nodeAgenda = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.nmrFar = new System.Windows.Forms.NumericUpDown();
            this.nmrNear = new System.Windows.Forms.NumericUpDown();
            this.chkBoundingSpheres = new System.Windows.Forms.CheckBox();
            this.chkBoundingBoxes = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.nmrSpeed = new System.Windows.Forms.NumericUpDown();
            this.nmrVelocity = new System.Windows.Forms.NumericUpDown();
            this.chkWireframe = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.txtFar = new System.Windows.Forms.TextBox();
            this.txtNear = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.chkFog = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSpecularPower = new System.Windows.Forms.TextBox();
            this.chkPerPixelLighting = new System.Windows.Forms.CheckBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.chkLuces = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCull = new System.Windows.Forms.ComboBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cmsLayers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuReload = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLugares.SuspendLayout();
            this.tabRecorridos.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmrFar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrNear)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmrSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrVelocity)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.cmsLayers.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1114, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.fileToolStripMenuItem.Text = "Archivo";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(97, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opcionesToolStripMenuItem,
            this.dataBuilderToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.optionsToolStripMenuItem.Text = "Ver";
            // 
            // opcionesToolStripMenuItem
            // 
            this.opcionesToolStripMenuItem.Name = "opcionesToolStripMenuItem";
            this.opcionesToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.opcionesToolStripMenuItem.Text = "Opciones";
            this.opcionesToolStripMenuItem.Click += new System.EventHandler(this.opcionesToolStripMenuItem_Click);
            // 
            // dataBuilderToolStripMenuItem
            // 
            this.dataBuilderToolStripMenuItem.Name = "dataBuilderToolStripMenuItem";
            this.dataBuilderToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.dataBuilderToolStripMenuItem.Text = "Data Builder";
            this.dataBuilderToolStripMenuItem.Click += new System.EventHandler(this.dataBuilderToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.acercaDeToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.helpToolStripMenuItem.Text = "Ayuda";
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.acercaDeToolStripMenuItem.Text = "Acerca de...";
            this.acercaDeToolStripMenuItem.Click += new System.EventHandler(this.acercaDeToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel1MinSize = 20;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1114, 835);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer3.Panel1MinSize = 75;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.label21);
            this.splitContainer3.Panel2.Controls.Add(this.tvCapas);
            this.splitContainer3.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer3_Panel2_Paint);
            this.splitContainer3.Size = new System.Drawing.Size(215, 835);
            this.splitContainer3.SplitterDistance = 462;
            this.splitContainer3.TabIndex = 39;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLugares);
            this.tabControl1.Controls.Add(this.tabRecorridos);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(215, 462);
            this.tabControl1.TabIndex = 39;
            // 
            // tabLugares
            // 
            this.tabLugares.Controls.Add(this.tvResultados);
            this.tabLugares.Controls.Add(this.button3);
            this.tabLugares.Controls.Add(this.textBox1);
            this.tabLugares.Location = new System.Drawing.Point(4, 22);
            this.tabLugares.Name = "tabLugares";
            this.tabLugares.Padding = new System.Windows.Forms.Padding(3);
            this.tabLugares.Size = new System.Drawing.Size(207, 436);
            this.tabLugares.TabIndex = 0;
            this.tabLugares.Text = "Inicio";
            this.tabLugares.UseVisualStyleBackColor = true;
            // 
            // tvResultados
            // 
            this.tvResultados.CheckBoxes = true;
            this.tvResultados.Location = new System.Drawing.Point(6, 38);
            this.tvResultados.Name = "tvResultados";
            this.tvResultados.Size = new System.Drawing.Size(195, 392);
            this.tvResultados.TabIndex = 41;
            this.tvResultados.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvResultados_AfterSelect);
            // 
            // button3
            // 
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.Location = new System.Drawing.Point(138, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(63, 26);
            this.button3.TabIndex = 40;
            this.button3.Text = "Buscar";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 26);
            this.textBox1.TabIndex = 39;
            // 
            // tabRecorridos
            // 
            this.tabRecorridos.Controls.Add(this.tvRecorridos);
            this.tabRecorridos.Controls.Add(this.label2);
            this.tabRecorridos.Controls.Add(this.button4);
            this.tabRecorridos.Controls.Add(this.cmbColectivos);
            this.tabRecorridos.Controls.Add(this.label1);
            this.tabRecorridos.Controls.Add(this.checkBox1);
            this.tabRecorridos.Controls.Add(this.btnBuscar);
            this.tabRecorridos.Controls.Add(this.cmbCalleDestino);
            this.tabRecorridos.Controls.Add(this.label8);
            this.tabRecorridos.Controls.Add(this.cmbEsquinaDestino);
            this.tabRecorridos.Controls.Add(this.label9);
            this.tabRecorridos.Controls.Add(this.textBox2);
            this.tabRecorridos.Controls.Add(this.label10);
            this.tabRecorridos.Controls.Add(this.label11);
            this.tabRecorridos.Controls.Add(this.cmbCalleInicial);
            this.tabRecorridos.Controls.Add(this.label7);
            this.tabRecorridos.Controls.Add(this.cmbEsquinaInicial);
            this.tabRecorridos.Controls.Add(this.label6);
            this.tabRecorridos.Controls.Add(this.textBox3);
            this.tabRecorridos.Controls.Add(this.label5);
            this.tabRecorridos.Controls.Add(this.label4);
            this.tabRecorridos.Location = new System.Drawing.Point(4, 22);
            this.tabRecorridos.Name = "tabRecorridos";
            this.tabRecorridos.Size = new System.Drawing.Size(207, 436);
            this.tabRecorridos.TabIndex = 2;
            this.tabRecorridos.Text = "Recorridos";
            this.tabRecorridos.UseVisualStyleBackColor = true;
            // 
            // tvRecorridos
            // 
            this.tvRecorridos.CheckBoxes = true;
            this.tvRecorridos.Location = new System.Drawing.Point(6, 254);
            this.tvRecorridos.Name = "tvRecorridos";
            this.tvRecorridos.Size = new System.Drawing.Size(198, 145);
            this.tvRecorridos.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 408);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Línea";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button4
            // 
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button4.Location = new System.Drawing.Point(181, 404);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(23, 21);
            this.button4.TabIndex = 23;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // cmbColectivos
            // 
            this.cmbColectivos.FormattingEnabled = true;
            this.cmbColectivos.Location = new System.Drawing.Point(42, 405);
            this.cmbColectivos.Name = "cmbColectivos";
            this.cmbColectivos.Size = new System.Drawing.Size(133, 21);
            this.cmbColectivos.TabIndex = 22;
            this.cmbColectivos.SelectedIndexChanged += new System.EventHandler(this.cmbColectivos_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 238);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Resultados:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 218);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(65, 17);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Text = "Guardar";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(134, 215);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(70, 21);
            this.btnBuscar.TabIndex = 18;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // cmbCalleDestino
            // 
            this.cmbCalleDestino.FormattingEnabled = true;
            this.cmbCalleDestino.Location = new System.Drawing.Point(42, 130);
            this.cmbCalleDestino.Name = "cmbCalleDestino";
            this.cmbCalleDestino.Size = new System.Drawing.Size(162, 21);
            this.cmbCalleDestino.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 187);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Esq.:";
            // 
            // cmbEsquinaDestino
            // 
            this.cmbEsquinaDestino.FormattingEnabled = true;
            this.cmbEsquinaDestino.Location = new System.Drawing.Point(42, 184);
            this.cmbEsquinaDestino.Name = "cmbEsquinaDestino";
            this.cmbEsquinaDestino.Size = new System.Drawing.Size(162, 21);
            this.cmbEsquinaDestino.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(2, 160);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Altura:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(42, 157);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(82, 21);
            this.textBox2.TabIndex = 13;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 133);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Calle:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(3, 113);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Dirección destino:";
            // 
            // cmbCalleInicial
            // 
            this.cmbCalleInicial.FormattingEnabled = true;
            this.cmbCalleInicial.Location = new System.Drawing.Point(42, 23);
            this.cmbCalleInicial.Name = "cmbCalleInicial";
            this.cmbCalleInicial.Size = new System.Drawing.Size(162, 21);
            this.cmbCalleInicial.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Esq.:";
            // 
            // cmbEsquinaInicial
            // 
            this.cmbEsquinaInicial.FormattingEnabled = true;
            this.cmbEsquinaInicial.Location = new System.Drawing.Point(42, 77);
            this.cmbEsquinaInicial.Name = "cmbEsquinaInicial";
            this.cmbEsquinaInicial.Size = new System.Drawing.Size(162, 21);
            this.cmbEsquinaInicial.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(-1, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Altura:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(42, 50);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(82, 21);
            this.textBox3.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Calle:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(3, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Dirección inicial:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 4);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 13);
            this.label21.TabIndex = 4;
            this.label21.Text = "Capas";
            // 
            // tvCapas
            // 
            this.tvCapas.CheckBoxes = true;
            this.tvCapas.Location = new System.Drawing.Point(6, 20);
            this.tvCapas.Name = "tvCapas";
            treeNode1.Name = "NodeBarrios";
            treeNode1.Text = "Barrios";
            treeNode2.Name = "NodePuntos";
            treeNode2.Text = "Puntos de interés";
            treeNode3.Name = "NodeCalles";
            treeNode3.Text = "Calles";
            this.tvCapas.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.tvCapas.Size = new System.Drawing.Size(205, 337);
            this.tvCapas.TabIndex = 3;
            this.tvCapas.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvCapas_NodeMouseDoubleClick);
            this.tvCapas.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvCapas_AfterCheck);
            this.tvCapas.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvCapas_AfterSelect);
            this.tvCapas.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvCapas_NodeMouseClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Panel1MinSize = 385;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl3);
            this.splitContainer2.Size = new System.Drawing.Size(892, 835);
            this.splitContainer2.SplitterDistance = 699;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage1);
            this.tabControl3.Controls.Add(this.nodeAgenda);
            this.tabControl3.Controls.Add(this.tabPage2);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(892, 132);
            this.tabControl3.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.wbDestacados);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(884, 106);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Destacados";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // wbDestacados
            // 
            this.wbDestacados.Location = new System.Drawing.Point(6, 6);
            this.wbDestacados.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbDestacados.Name = "wbDestacados";
            this.wbDestacados.Size = new System.Drawing.Size(869, 92);
            this.wbDestacados.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(853, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 14);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // nodeAgenda
            // 
            this.nodeAgenda.Location = new System.Drawing.Point(4, 22);
            this.nodeAgenda.Name = "nodeAgenda";
            this.nodeAgenda.Size = new System.Drawing.Size(884, 106);
            this.nodeAgenda.TabIndex = 2;
            this.nodeAgenda.Text = "Agenda";
            this.nodeAgenda.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.nmrFar);
            this.tabPage2.Controls.Add(this.nmrNear);
            this.tabPage2.Controls.Add(this.chkBoundingSpheres);
            this.tabPage2.Controls.Add(this.chkBoundingBoxes);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.chkWireframe);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.cmbCull);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(884, 106);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Opciones";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // nmrFar
            // 
            this.nmrFar.Location = new System.Drawing.Point(394, 63);
            this.nmrFar.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.nmrFar.Name = "nmrFar";
            this.nmrFar.Size = new System.Drawing.Size(76, 21);
            this.nmrFar.TabIndex = 26;
            this.nmrFar.Value = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            // 
            // nmrNear
            // 
            this.nmrNear.DecimalPlaces = 3;
            this.nmrNear.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nmrNear.Location = new System.Drawing.Point(312, 63);
            this.nmrNear.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmrNear.Name = "nmrNear";
            this.nmrNear.Size = new System.Drawing.Size(76, 21);
            this.nmrNear.TabIndex = 25;
            this.nmrNear.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkBoundingSpheres
            // 
            this.chkBoundingSpheres.AutoSize = true;
            this.chkBoundingSpheres.Location = new System.Drawing.Point(6, 81);
            this.chkBoundingSpheres.Name = "chkBoundingSpheres";
            this.chkBoundingSpheres.Size = new System.Drawing.Size(109, 17);
            this.chkBoundingSpheres.TabIndex = 24;
            this.chkBoundingSpheres.Text = "BoundingSpheres";
            this.chkBoundingSpheres.UseVisualStyleBackColor = true;
            // 
            // chkBoundingBoxes
            // 
            this.chkBoundingBoxes.AutoSize = true;
            this.chkBoundingBoxes.Location = new System.Drawing.Point(6, 58);
            this.chkBoundingBoxes.Name = "chkBoundingBoxes";
            this.chkBoundingBoxes.Size = new System.Drawing.Size(99, 17);
            this.chkBoundingBoxes.TabIndex = 23;
            this.chkBoundingBoxes.Text = "BoundingBoxes";
            this.chkBoundingBoxes.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.nmrSpeed);
            this.groupBox3.Controls.Add(this.nmrVelocity);
            this.groupBox3.Location = new System.Drawing.Point(159, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(147, 81);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Camara";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 50);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(44, 13);
            this.label20.TabIndex = 25;
            this.label20.Text = "Velocity";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(13, 23);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(37, 13);
            this.label19.TabIndex = 24;
            this.label19.Text = "Speed";
            // 
            // nmrSpeed
            // 
            this.nmrSpeed.DecimalPlaces = 1;
            this.nmrSpeed.Location = new System.Drawing.Point(56, 21);
            this.nmrSpeed.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nmrSpeed.Name = "nmrSpeed";
            this.nmrSpeed.Size = new System.Drawing.Size(76, 21);
            this.nmrSpeed.TabIndex = 23;
            this.nmrSpeed.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // nmrVelocity
            // 
            this.nmrVelocity.DecimalPlaces = 1;
            this.nmrVelocity.Location = new System.Drawing.Point(56, 48);
            this.nmrVelocity.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nmrVelocity.Name = "nmrVelocity";
            this.nmrVelocity.Size = new System.Drawing.Size(76, 21);
            this.nmrVelocity.TabIndex = 22;
            this.nmrVelocity.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // chkWireframe
            // 
            this.chkWireframe.AutoSize = true;
            this.chkWireframe.Location = new System.Drawing.Point(6, 35);
            this.chkWireframe.Name = "chkWireframe";
            this.chkWireframe.Size = new System.Drawing.Size(76, 17);
            this.chkWireframe.TabIndex = 12;
            this.chkWireframe.Text = "Wireframe";
            this.chkWireframe.UseVisualStyleBackColor = true;
            this.chkWireframe.CheckedChanged += new System.EventHandler(this.chkWireframe_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.txtFar);
            this.groupBox2.Controls.Add(this.txtNear);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.chkFog);
            this.groupBox2.Location = new System.Drawing.Point(312, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 46);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(44, 21);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(21, 18);
            this.button6.TabIndex = 26;
            this.button6.UseVisualStyleBackColor = true;
            // 
            // txtFar
            // 
            this.txtFar.Location = new System.Drawing.Point(195, 18);
            this.txtFar.Name = "txtFar";
            this.txtFar.Size = new System.Drawing.Size(34, 21);
            this.txtFar.TabIndex = 25;
            // 
            // txtNear
            // 
            this.txtNear.Location = new System.Drawing.Point(117, 18);
            this.txtNear.Name = "txtNear";
            this.txtNear.Size = new System.Drawing.Size(34, 21);
            this.txtNear.TabIndex = 24;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(166, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(23, 13);
            this.label16.TabIndex = 23;
            this.label16.Text = "Far";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(81, 24);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(30, 13);
            this.label17.TabIndex = 22;
            this.label17.Text = "Near";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 24);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 13);
            this.label18.TabIndex = 21;
            this.label18.Text = "Color";
            // 
            // chkFog
            // 
            this.chkFog.AutoSize = true;
            this.chkFog.Location = new System.Drawing.Point(5, 0);
            this.chkFog.Name = "chkFog";
            this.chkFog.Size = new System.Drawing.Size(44, 17);
            this.chkFog.TabIndex = 17;
            this.chkFog.Text = "Fog";
            this.chkFog.UseVisualStyleBackColor = true;
            this.chkFog.CheckedChanged += new System.EventHandler(this.chkFog_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSpecularPower);
            this.groupBox1.Controls.Add(this.chkPerPixelLighting);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.chkLuces);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Location = new System.Drawing.Point(553, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 78);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // txtSpecularPower
            // 
            this.txtSpecularPower.Location = new System.Drawing.Point(53, 47);
            this.txtSpecularPower.Name = "txtSpecularPower";
            this.txtSpecularPower.Size = new System.Drawing.Size(32, 21);
            this.txtSpecularPower.TabIndex = 31;
            // 
            // chkPerPixelLighting
            // 
            this.chkPerPixelLighting.AutoSize = true;
            this.chkPerPixelLighting.Location = new System.Drawing.Point(119, 49);
            this.chkPerPixelLighting.Name = "chkPerPixelLighting";
            this.chkPerPixelLighting.Size = new System.Drawing.Size(101, 17);
            this.chkPerPixelLighting.TabIndex = 30;
            this.chkPerPixelLighting.Text = "PerPixelLighting";
            this.chkPerPixelLighting.UseVisualStyleBackColor = true;
            this.chkPerPixelLighting.CheckedChanged += new System.EventHandler(this.chkPerPixelLighting_CheckedChanged);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(199, 20);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(21, 18);
            this.button9.TabIndex = 29;
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(121, 20);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(21, 18);
            this.button8.TabIndex = 28;
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(51, 20);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(21, 18);
            this.button7.TabIndex = 27;
            this.button7.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(78, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "Diffuse";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(150, 23);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 13);
            this.label13.TabIndex = 21;
            this.label13.Text = "Specular";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 23);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "Ambient";
            // 
            // chkLuces
            // 
            this.chkLuces.AutoSize = true;
            this.chkLuces.Location = new System.Drawing.Point(9, 0);
            this.chkLuces.Name = "chkLuces";
            this.chkLuces.Size = new System.Drawing.Size(53, 17);
            this.chkLuces.TabIndex = 16;
            this.chkLuces.Text = "Luces";
            this.chkLuces.UseVisualStyleBackColor = true;
            this.chkLuces.CheckedChanged += new System.EventHandler(this.chkLuces_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(39, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "Powah";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Cull";
            // 
            // cmbCull
            // 
            this.cmbCull.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCull.FormattingEnabled = true;
            this.cmbCull.Items.AddRange(new object[] {
            "None",
            "Clockwise",
            "CounterClockwise"});
            this.cmbCull.Location = new System.Drawing.Point(33, 6);
            this.cmbCull.Name = "cmbCull";
            this.cmbCull.Size = new System.Drawing.Size(120, 21);
            this.cmbCull.TabIndex = 0;
            this.cmbCull.SelectedIndexChanged += new System.EventHandler(this.cmbCull_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "mesh.jpg");
            this.imageList1.Images.SetKeyName(1, "street.png");
            // 
            // cmsLayers
            // 
            this.cmsLayers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuReload});
            this.cmsLayers.Name = "cmsLayers";
            this.cmsLayers.Size = new System.Drawing.Size(108, 26);
            this.cmsLayers.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsLayers_ItemClicked);
            // 
            // menuReload
            // 
            this.menuReload.Name = "menuReload";
            this.menuReload.Size = new System.Drawing.Size(107, 22);
            this.menuReload.Text = "Reload";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1114, 859);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Baires 3d";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabLugares.ResumeLayout(false);
            this.tabLugares.PerformLayout();
            this.tabRecorridos.ResumeLayout(false);
            this.tabRecorridos.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmrFar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrNear)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmrSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrVelocity)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.cmsLayers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private void tvCapas_AfterSelect(object sender, TreeViewEventArgs e) {
            if (e.Node.Parent == null || Constants.ar3d.ComponentManager.Capital.StreetManager == null) {
                return;
            }

            if (e.Node.Parent.Name == "NodeCalles") {

                Constants.ar3d.ComponentManager.Capital.StreetManager.StreetToDraw = e.Node.Text;
            }
        }

        void tvCapas_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Node.Parent == null || Constants.ar3d.ComponentManager.Capital.StreetManager == null) {
                return;
            }

            if (e.Node.Parent.Name == "NodeCalles") {

                Constants.ar3d.ComponentManager.Capital.StreetManager.StreetToDraw = e.Node.Text;
            }
        }

        void tvCapas_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {

            
            if (e.Node.Parent == null) {
                if (e.Node.Name == "NodeBarrios"){
                    Vector3 bb3 = Constants.ar3d.ComponentManager.Capital.Barrios[0].BoundingSphere.Center;
                    bb3.Y = Constants.ar3d.ComponentManager.Capital.Barrios[0].BoundingSphere.Radius * 1.7f;
                    //bb3.Y = height;
                    Constants.Camera.Position = new Vector3(bb3.X, bb3.Y, bb3.Z);
                }
                return;
            }
            if (e.Node.Parent.Name == "NodeCalles") {
                Vector3 bb2 = Constants.ar3d.ComponentManager.Capital.Calles[e.Node.Text].BoundingSphere.Center;
                Constants.ar3d.ComponentManager.Capital.StreetManager.StreetToDraw = e.Node.Text;
                bb2.Y = Constants.ar3d.ComponentManager.Capital.Calles[e.Node.Text].BoundingSphere.Radius * 1.7f;
                //bb2.Y = height;
                Constants.Camera.Position = new Vector3(bb2.X, bb2.Y, bb2.Z);
            }
            if (e.Node.Parent.Name == "NodeBarrios") {
                Vector3 bb2 = Constants.ar3d.ComponentManager.Capital.Barrios[e.Node.Text].BoundingSphere.Center;
                bb2.Y = Constants.ar3d.ComponentManager.Capital.Barrios[e.Node.Text].BoundingSphere.Radius * 1.7f;
                //bb2.Y = height;
                Constants.Camera.Position = new Vector3(bb2.X, bb2.Y, bb2.Z);
            }
        }

        private float height = 0.0025f;
        private void tvCapas_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e) {
            if(e.Node.Parent == null){
                switch (e.Node.Name){
                    case "NodeBarrios":
                        Constants.ar3d.ComponentManager.Capital.Barrios.Enabled = e.Node.Checked;
                        break;
                    case "NodeCalles":
                        Constants.ar3d.ComponentManager.Capital.Calles.Enabled = e.Node.Checked;
                        break;
                    case "NodePuntos":
                        Constants.ar3d.ComponentManager.Capital.Puntos.Enabled = e.Node.Checked;
                        break;
                }   
                return;
            }
            switch (e.Node.Parent.Name){
                case "NodeBarrios":
                    Constants.ar3d.ComponentManager.Capital.Barrios[e.Node.Text].Enabled = e.Node.Checked;
                    break;
                case "NodeCalles":
                    Constants.ar3d.ComponentManager.Capital.Calles[e.Node.Text].Enabled = e.Node.Checked;
                    break;
                case "NodePuntos":
                    Constants.ar3d.ComponentManager.Capital.Puntos[e.Node.Text].Enabled = e.Node.Checked;
                    break;
            }
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        public System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opcionesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataBuilderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsLayers;
        private System.Windows.Forms.ToolStripMenuItem menuReload;
        public SplitContainer splitContainer2;
        private TabControl tabControl3;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label3;
        public ComboBox cmbCull;
        private GroupBox groupBox1;
        private Label label12;
        private Label label13;
        private Label label14;
        public CheckBox chkLuces;
        private Label label15;
        private GroupBox groupBox2;
        public TextBox txtFar;
        public TextBox txtNear;
        private Label label16;
        private Label label17;
        private Label label18;
        public CheckBox chkFog;
        private Button button6;
        private Button button9;
        private Button button8;
        private Button button7;
        public CheckBox chkWireframe;
        public CheckBox chkPerPixelLighting;
        private GroupBox groupBox3;
        private Label label20;
        private Label label19;
        public NumericUpDown nmrSpeed;
        public NumericUpDown nmrVelocity;
        private TextBox txtSpecularPower;
        public CheckBox chkBoundingSpheres;
        public CheckBox chkBoundingBoxes;
        public NumericUpDown nmrFar;
        public NumericUpDown nmrNear;
        private SplitContainer splitContainer3;
        public TreeView tvCapas;
        private WebBrowser wbDestacados;
        private Button button2;
        private TabPage nodeAgenda;
        private TabControl tabControl1;
        private TabPage tabLugares;
        public TreeView tvResultados;
        private Button button3;
        private TextBox textBox1;
        private TabPage tabRecorridos;
        private Label label2;
        private Button button4;
        public ComboBox cmbColectivos;
        private Label label1;
        private CheckBox checkBox1;
        private Button btnBuscar;
        public ComboBox cmbCalleDestino;
        private Label label8;
        public ComboBox cmbEsquinaDestino;
        private Label label9;
        private TextBox textBox2;
        private Label label10;
        private Label label11;
        public ComboBox cmbCalleInicial;
        private Label label7;
        public ComboBox cmbEsquinaInicial;
        private Label label6;
        private TextBox textBox3;
        private Label label5;
        private Label label4;
        public TreeView tvRecorridos;
        private Label label21;
    }
}