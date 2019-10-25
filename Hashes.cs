using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameDbManagerMega
{
	public partial class Hashes : Form
	{
		public int gameID;
		public GameDB gdb;

		public Hashes()
		{
			InitializeComponent();

			gameDB = gdb;
			gameCkBindingSource.Filter = "GameID = " + gameID;
		}

		private void gameCkBindingSource_CurrentChanged(object sender, EventArgs e)
		{

		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void Hashes_Shown(object sender, EventArgs e)
		{
			gameDB = gdb;
			gameCkBindingSource.DataSource = gdb;
			gameCkBindingSource.Filter = "GameID = " + gameID;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofn = new OpenFileDialog();
			ofn.CheckPathExists = true;
			ofn.Multiselect = false;
			ofn.Filter = "ROM Files (*.bin,*.md,*.sms,*.32x,*.sg)|*.bin;*.md;*.sms;*.32x;*.sg|CDRom cuesheets (*.cue)|*.cue";

			if (ofn.ShowDialog() == DialogResult.OK)
			{
				uint crc;
				if (ofn.FileName.EndsWith(".bin", StringComparison.InvariantCultureIgnoreCase) ||
					ofn.FileName.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase) ||
					ofn.FileName.EndsWith(".sms", StringComparison.InvariantCultureIgnoreCase) ||
					ofn.FileName.EndsWith(".32x", StringComparison.InvariantCultureIgnoreCase) ||
					ofn.FileName.EndsWith(".sg", StringComparison.InvariantCultureIgnoreCase)
					)
				{
					var data = File.ReadAllBytes(ofn.FileName);

					if ((data.Length & 0xFFF) != 0)
					{
						uint l = (uint)data.Length & 0xFFFFF000;

						var dat = new byte[l];
						Array.Copy(data, data.Length & 0xFFF, dat, 0, l);

						data = dat;

						crc = Crc32.Compute(data);
					}
					else
					{
						crc = Crc32.Compute(data);
					}
				}
				else if (ofn.FileName.EndsWith(".cue", StringComparison.InvariantCultureIgnoreCase))
				{
					var cue = new CueSheet(ofn.FileName);
					crc = GameDBMgr.ComputeCueCrc(cue);
				}
				else
				{
					MessageBox.Show("Invalid file extension");
					return;
				}

				//Check if the crc already exists
				String crcstr = crc.ToString("X8");
				var ck = gameDB.GameCk.FirstOrDefault(x => x.Checksum == crcstr);
				if (ck != null)
				{
					if (ck.GameID == gameID)
						MessageBox.Show("This checksum already exists for this game");
					else
						MessageBox.Show("This checksum already exists for a different game!!");
				}
				else
				{
					ck = gameDB.GameCk.NewGameCkRow();
					ck.GameID = gameID;
					ck.Checksum = crcstr;

					gameDB.GameCk.AddGameCkRow(ck);

				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewCell r in dataGridView1.SelectedCells)
			{
				String crc = (String)r.Value;

				GameDB.GameCkRow ck = gameDB.GameCk.FirstOrDefault(x => x.Checksum == crc);

				if(ck != null)
					gameDB.GameCk.RemoveGameCkRow(ck);
			}
		}
	}
}
