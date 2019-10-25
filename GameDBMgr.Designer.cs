namespace GameDbManagerMega
{
	partial class GameDBMgr
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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.yearDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.genreDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.gameDB = new GameDbManagerMega.GameDB();
			this.screenshotDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewButtonColumn();
			this.Hashes = new System.Windows.Forms.DataGridViewButtonColumn();
			this.gameBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			this.genreBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gameDB)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gameBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.genreBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.yearDataGridViewTextBoxColumn,
            this.genreDataGridViewTextBoxColumn,
            this.screenshotDataGridViewTextBoxColumn,
            this.Hashes});
			this.dataGridView1.DataSource = this.gameBindingSource;
			this.dataGridView1.Location = new System.Drawing.Point(12, 12);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(873, 398);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			// 
			// nameDataGridViewTextBoxColumn
			// 
			this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
			this.nameDataGridViewTextBoxColumn.FillWeight = 300F;
			this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
			this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
			this.nameDataGridViewTextBoxColumn.Width = 300;
			// 
			// yearDataGridViewTextBoxColumn
			// 
			this.yearDataGridViewTextBoxColumn.DataPropertyName = "Year";
			this.yearDataGridViewTextBoxColumn.HeaderText = "Year";
			this.yearDataGridViewTextBoxColumn.Name = "yearDataGridViewTextBoxColumn";
			// 
			// genreDataGridViewTextBoxColumn
			// 
			this.genreDataGridViewTextBoxColumn.DataPropertyName = "Genre";
			this.genreDataGridViewTextBoxColumn.DataSource = this.gameDB;
			this.genreDataGridViewTextBoxColumn.DisplayMember = "Genre.Name";
			this.genreDataGridViewTextBoxColumn.HeaderText = "Genre";
			this.genreDataGridViewTextBoxColumn.Name = "genreDataGridViewTextBoxColumn";
			this.genreDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.genreDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.genreDataGridViewTextBoxColumn.ValueMember = "Genre.Genre";
			// 
			// gameDB
			// 
			this.gameDB.DataSetName = "GameDB";
			this.gameDB.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// screenshotDataGridViewTextBoxColumn
			// 
			this.screenshotDataGridViewTextBoxColumn.DataPropertyName = "Screenshot";
			this.screenshotDataGridViewTextBoxColumn.HeaderText = "Screenshot";
			this.screenshotDataGridViewTextBoxColumn.Name = "screenshotDataGridViewTextBoxColumn";
			this.screenshotDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.screenshotDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.screenshotDataGridViewTextBoxColumn.Width = 200;
			// 
			// Hashes
			// 
			this.Hashes.HeaderText = "Hashes";
			this.Hashes.Name = "Hashes";
			this.Hashes.Text = "...";
			// 
			// gameBindingSource
			// 
			this.gameBindingSource.DataMember = "Game";
			this.gameBindingSource.DataSource = this.bindingSource1;
			// 
			// bindingSource1
			// 
			this.bindingSource1.DataSource = this.gameDB;
			this.bindingSource1.Position = 0;
			this.bindingSource1.CurrentChanged += new System.EventHandler(this.bindingSource1_CurrentChanged);
			// 
			// genreBindingSource
			// 
			//this.genreBindingSource.DataSource = typeof(GenreTable);
			this.genreBindingSource.CurrentChanged += new System.EventHandler(this.genreBindingSource_CurrentChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(806, 423);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(79, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Convert Imgs";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(12, 423);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "Scan Roms";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(102, 426);
			this.progressBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(168, 17);
			this.progressBar1.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(290, 426);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 13);
			this.label1.TabIndex = 4;
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// GameDBMgr
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(896, 460);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.dataGridView1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GameDBMgr";
			this.Text = "Game DB Manager for MegaSD";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Shown += new System.EventHandler(this.GameDBMgr_Shown);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gameDB)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gameBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.genreBindingSource)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.BindingSource bindingSource1;
		private GameDB gameDB;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.BindingSource gameBindingSource;
		private System.Windows.Forms.BindingSource genreBindingSource;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn yearDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn genreDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewButtonColumn screenshotDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewButtonColumn Hashes;
	}
}

