namespace GameDbManagerMega
{
	partial class Hashes
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
			this.checksumDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.gameCkBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.gameDB = new GameDbManagerMega.GameDB();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gameCkBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gameDB)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checksumDataGridViewTextBoxColumn});
			this.dataGridView1.DataSource = this.gameCkBindingSource;
			this.dataGridView1.Location = new System.Drawing.Point(13, 27);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(166, 276);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			// 
			// checksumDataGridViewTextBoxColumn
			// 
			this.checksumDataGridViewTextBoxColumn.DataPropertyName = "Checksum";
			this.checksumDataGridViewTextBoxColumn.HeaderText = "Checksum";
			this.checksumDataGridViewTextBoxColumn.Name = "checksumDataGridViewTextBoxColumn";
			this.checksumDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// gameCkBindingSource
			// 
			this.gameCkBindingSource.DataMember = "GameCk";
			this.gameCkBindingSource.DataSource = this.gameDB;
			this.gameCkBindingSource.CurrentChanged += new System.EventHandler(this.gameCkBindingSource_CurrentChanged);
			// 
			// gameDB
			// 
			this.gameDB.DataSetName = "GameDB";
			this.gameDB.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(13, 309);
			this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(79, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Add hash";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(100, 309);
			this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(79, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "Remove hash";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// Hashes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(192, 346);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.dataGridView1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Hashes";
			this.Text = "Hashes";
			this.Shown += new System.EventHandler(this.Hashes_Shown);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gameCkBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gameDB)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.BindingSource gameCkBindingSource;
		private GameDB gameDB;
		private System.Windows.Forms.DataGridViewTextBoxColumn checksumDataGridViewTextBoxColumn;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}