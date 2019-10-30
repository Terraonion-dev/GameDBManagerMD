using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GameDbManagerMega
{
	public partial class GameDBMgr : Form
	{
		public class GenreTable
		{
			public int ID;
			public string Name;
		}

		public List<GenreTable> genreTable = new List<GenreTable>();

		public void AddGenre(int id, string name)
		{
			GameDB.GenreRow genre = gameDB.Genre.NewGenreRow();
			genre.Name = name;
			gameDB.Genre.AddGenreRow(genre);
		}

		public GameDBMgr()
		{
			InitializeComponent();

			if (File.Exists("db.xml"))
			{
				gameDB.ReadXml("db.xml");
			}

			Crc32.gen_crc_table();

			//Don't change the ID or order of this table, as it must match the one in the firmware!!
			if (gameDB.Genre.Count == 0)
			{
				AddGenre(1, "Shooter");
				AddGenre(2, "Action");
				AddGenre(3, "Sports");
				AddGenre(4, "Misc");
				AddGenre(5, "Casino");
				AddGenre(6, "Driving");
				AddGenre(7, "Platform");
				AddGenre(8, "Puzzle");
				AddGenre(9, "Boxing");
				AddGenre(10, "Wrestling");
				AddGenre(11, "Strategy");
				AddGenre(12, "Soccer");
				AddGenre(13, "Golf");
				AddGenre(14, "Beat'em-Up");
				AddGenre(15, "Baseball");
				AddGenre(16, "Mahjong");
				AddGenre(17, "Board");
				AddGenre(18, "Tennis");
				AddGenre(19, "Fighter");
				AddGenre(20, "Horse Racing");
				AddGenre(21, "Other");
			}

			genreBindingSource.DataSource = genreTable;
		}

		private void bindingSource1_CurrentChanged(object sender, EventArgs e)
		{

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			gameDB.WriteXml("db.xml");
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			switch (e.ColumnIndex)
			{
				//screenshot
				case 3:
					{
						OpenFileDialog ofd = new OpenFileDialog();
						ofd.Filter = "Images (*.png)|*.png";
						ofd.ValidateNames = true;
						ofd.CheckFileExists = true;
						DialogResult dr = ofd.ShowDialog();
						if (dr == DialogResult.OK)
						{
							string cwd = Directory.GetCurrentDirectory();
							string fname = ofd.FileName;

							if (fname.StartsWith(cwd))
								fname = fname.Substring(cwd.Length + 1);

							dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = fname;
						}

						break;
					}
				//hash
				case 4:
					{
						Hashes hh = new GameDbManagerMega.Hashes();

						GameDB.GameRow gr = (GameDB.GameRow)((DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem).Row;

						hh.gdb = gameDB;
						hh.gameID = gr.ID;

						hh.ShowDialog();
						break;
					}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists("TileCache"))
			{
				Directory.CreateDirectory("TileCache");
			}


			foreach (GameDB.GameRow game in gameDB.Game)
			{
				if (!game.IsScreenshotNull())
				{
					if (!string.IsNullOrEmpty(game.Screenshot))
					{
						//string dst = "TileCache/" + game.Screenshot.Substring(game.Screenshot.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
						string dst = "TileCache/" + game.Screenshot.Replace("\\", "_").Replace("/", "_").Replace(":", "_");

						dst = dst.Replace(".png", ".tile");

						if (!File.Exists(dst) || File.GetLastWriteTime(game.Screenshot) > File.GetLastWriteTime(dst))
						{
							if (!Quantizer.Program.Quantize(game.Screenshot, dst))
							{
							}
						}
					}
				}
			}
		}

		private class Game
		{
			public uint checksum;
			public ushort remap;
			public int GameID;
		};

		private static byte ProcessChar(byte b)
		{
			if (b < 'A' || b > 'z')
			{
				return b;
			}

			return (byte)char.ToUpperInvariant((char)b);
		}

		private static long GetSize(string file)
		{
			long sz;
			using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				sz = fs.Length;
			}

			return sz;
		}

		private static void ScanDir(string dir, GameDB db, GameDBMgr form)
		{
			ushort gamecnt = 0;
			ushort sccnt = 0;
			Game[] games = new Game[1024];
			byte[] scshots = new byte[1024 * 3072];

			if (form != null)
			{
				form.Invoke(new Action(() =>
				{
					form.label1.Text = Path.GetFileName(dir);
				}));
			}
			else
			{
				Console.WriteLine("Processing directory " + Path.GetFileName(dir));
			}


			foreach (string f in Directory.GetFiles(dir, "*.*"))
			{
				if (f.Contains(".wav"))
				{
					continue;
				}

				if (f.Contains("Track ") && f.Contains(".bin"))
				{
					continue;
				}

				if (GetSize(f) > 16 * 1024 * 1024)
				{
					continue;
				}

				byte[] data = File.ReadAllBytes(f);

				uint crc = Crc32.Compute(data);

				GameDB.GameCkRow ck = db.GameCk.FirstOrDefault(w => w.Checksum == crc.ToString("X8"));

				if (ck == null && (data.Length & 0xFFF) != 0)
				{
					uint l = (uint)data.Length & 0xFFFFF000;

					byte[] dat = new byte[l];
					Array.Copy(data, data.Length & 0xFFF, dat, 0, l);

					data = dat;

					crc = Crc32.Compute(data);

					ck = db.GameCk.FirstOrDefault(w => w.Checksum == crc.ToString("X8"));

				}

				if (ck != null)
				{
					//var namecrc = Crc32.Compute(Encoding.ASCII.GetBytes(f.Substring(f.LastIndexOf('\\') + 1)));
					string nn = f.Substring(f.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
					nn = nn.Substring(0, nn.LastIndexOf('.'));

					byte[] name = Encoding.ASCII.GetBytes(nn);
					byte[] namecnv = new byte[56];

					if (name.Length > 56)
					{
						Array.Copy(name, namecnv, 56);
					}
					else
					{
						Array.Copy(name, namecnv, name.Length);
					}

					for (int i = 0; i < namecnv.Length; ++i)
					{
						namecnv[i] = ProcessChar(namecnv[i]);
					}

					uint namecrc = Crc32.update_crc(0xFFFFFFFF, namecnv, 56);

					Game existing = games.Take(gamecnt).FirstOrDefault(x => x.checksum != 0 && x.GameID == ck.GameID);

					if (existing != null)
					{
						Game g = new Game();
						g.checksum = namecrc;
						g.remap = existing.remap;
						g.GameID = existing.GameID;

						games[gamecnt] = g;
						++gamecnt;
					}
					else
					{
						GameDB.GameRow gg = db.Game.FirstOrDefault(x => x.ID == ck.GameID);

						if (gg != null && !gg.IsScreenshotNull())
						{
							//string dst = "TileCache/" + gg.Screenshot.Substring(gg.Screenshot.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
							string dst = "TileCache/" + gg.Screenshot.Replace("\\", "_").Replace("/", "_").Replace(":", "_");

							dst = dst.Replace(".png", ".tile");

							if (File.Exists(dst))
							{
								Game g = new Game();
								g.checksum = namecrc;
								g.remap = sccnt;
								g.GameID = ck.GameID;

								games[gamecnt] = g;
								++gamecnt;

								//copy screenshot to the scshot block
								byte[] scshot = File.ReadAllBytes(dst);

								byte[] scshot2 = new byte[2048];
								Array.Copy(scshot, scshot2, scshot.Length);

								//add year, genre...
								scshot2[0x700] = 0x1;   //version
								if (!gg.IsGenreNull())
								{
									scshot2[0x701] = (byte)gg.Genre;
								}

								if (!gg.IsYearNull())
								{
									scshot2[0x702] = (byte)(gg.Year & 0xFF);
									scshot2[0x703] = (byte)((gg.Year >> 8) & 0xFF);
								}

								Debug.Assert(scshot2.Length == 2048);
								Array.Copy(scshot2, 0, scshots, 2048 * sccnt, scshot2.Length);

								sccnt++;
							}
						}
					}
				}
				else
				{
					//Console.WriteLine("Missing crc " + f);
				}
			}

			//scan dirs for cds
			foreach (string d in Directory.GetDirectories(dir))
			{
				string[] cues = Directory.GetFiles(d, "*.cue");
				if (cues.Length == 1)
				{
					string []MDs = Directory.GetFiles(d, "*.md");

					try
					{
						CueSheet cue = new CueSheet(cues[0]);
						uint crc = 0;

						if (MDs.Length == 1) //It's an MD+ game
						{
							byte[] data = File.ReadAllBytes(MDs[0]);

							crc = Crc32.Compute(data);
						}
						else
						{
							crc = ComputeCueCrc(cue);
						}


						GameDB.GameCkRow ck = db.GameCk.FirstOrDefault(w => w.Checksum == crc.ToString("X8"));

						if (ck != null)
						{
							//var namecrc = Crc32.Compute(Encoding.ASCII.GetBytes(f.Substring(f.LastIndexOf('\\') + 1)));
							string nn = d.Substring(d.LastIndexOfAny(new char[] { '\\', '/' }) + 1);

							byte[] name = Encoding.ASCII.GetBytes(nn);
							byte[] namecnv = new byte[56];

							Array.Copy(name, namecnv, name.Length > 56 ? 56 : name.Length);

							for (int i = 0; i < namecnv.Length; ++i)
							{
								namecnv[i] = ProcessChar(namecnv[i]);
							}

							uint namecrc = Crc32.update_crc(0xFFFFFFFF, namecnv, 56);

							Game existing = games.Take(gamecnt).FirstOrDefault(x => x.checksum != 0 && x.GameID == ck.GameID);

							if (existing != null)
							{
								Game g = new Game();
								g.checksum = namecrc;
								g.remap = existing.remap;
								g.GameID = existing.GameID;

								games[gamecnt] = g;
								++gamecnt;
							}
							else
							{
								GameDB.GameRow gg = db.Game.FirstOrDefault(x => x.ID == ck.GameID);

								if (gg != null && !gg.IsScreenshotNull())
								{
									//string dst = "TileCache/" + gg.Screenshot.Substring(gg.Screenshot.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
									string dst = "TileCache/" + gg.Screenshot.Replace("\\","_").Replace("/","_").Replace(":","_");

									dst = dst.Replace(".png", ".tile");

									if (File.Exists(dst))
									{
										Game g = new Game();
										g.checksum = namecrc;
										g.remap = sccnt;
										g.GameID = ck.GameID;

										games[gamecnt] = g;
										++gamecnt;

										//copy screenshot to the scshot block
										byte[] scshot = File.ReadAllBytes(dst);

										byte[] scshot2 = new byte[2048];
										Array.Copy(scshot, scshot2, scshot.Length);

										//add year, genre...
										scshot2[0x700] = 0x1;   //version
										if (!gg.IsGenreNull())
											scshot2[0x701] = (byte)gg.Genre;

										if (!gg.IsYearNull())
										{
											scshot2[0x702] = (byte)(gg.Year & 0xFF);
											scshot2[0x703] = (byte)((gg.Year >> 8) & 0xFF);
										}

										Debug.Assert(scshot2.Length == 2048);
										Array.Copy(scshot2, 0, scshots, 2048 * sccnt, scshot2.Length);

										sccnt++;
									}
								}
							}
						}
						else
						{
							//Console.WriteLine("Missing crc " + f);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error processing " + cues[0] + " . Skipped");
						Console.WriteLine(ex.Message);
					}

				}
			}

			if (gamecnt != 0)
			{
				games = games.Take(gamecnt).OrderBy(x => x.checksum).ToArray();

				//Console.WriteLine("Writting game.db");

				using (FileStream fs = new FileStream(dir + "/games.dbs", FileMode.Create, FileAccess.Write))
				{
					using (BinaryWriter bw = new BinaryWriter(fs))
					{

						foreach (Game g in games)
						{
							bw.Write(g.checksum);
						}

						for (int i = 0; i < 1024 - games.Length; ++i)
						{
							bw.Write(0xFFFFFFFF);
						}

						foreach (Game g in games)
						{
							bw.Write(g.remap);
						}

						for (int i = 0; i < 1024 - games.Length; ++i)
						{
							bw.Write((ushort)0xFFFF);
						}

						bw.Write(scshots, 0, 2048 * sccnt);
					}
				}
			}

			foreach (string d in Directory.GetDirectories(dir))
			{
				ScanDir(d, db, form);
			}
		}


		private Thread thProcessing = null;
		private void button2_Click(object sender, EventArgs e)
		{
			progressBar1.Value = 0;
			//String dir = @"L:\hucard\hu2";
			//String dir = @"i:\\";
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.ShowNewFolderButton = false;

			if (fbd.ShowDialog() == DialogResult.OK)
			{
				button1.Enabled = false;
				button2.Enabled = false;
				progressBar1.Style = ProgressBarStyle.Marquee;
				thProcessing = new Thread(() =>
				{
					ScanDir(fbd.SelectedPath, gameDB, this);

					this.Invoke(new Action(() =>
					{
						button1.Enabled = true;
						button2.Enabled = true;
						progressBar1.Style = ProgressBarStyle.Continuous;
						label1.Text = "Done";
					}));
				});
				thProcessing.Start();
			}

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		public static uint ComputeCueCrc(CueSheet cue)
		{
			if (!cue.HasDataTrack())
			{
				throw new Exception("Audio only CDs are not supported for hashing");
			}
			CueSheet.Track track = cue.FindFirstDataTrack();
			uint crcAccum = 0xFFFFFFFF;
			using (FileStream fs = new FileStream(track.FileName, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader reader = new BinaryReader(fs))
				{
					fs.Seek(track.FileOffset, SeekOrigin.Begin);

					int numsects = 1;

					for (int i = 0; i < numsects; ++i)
					{
						byte[] data;
						if (track.SectorSize == CueSheet.SectorSize._2352)
						{
							reader.ReadBytes(16);
							data = reader.ReadBytes(2048);
							reader.ReadBytes(288);
						}
						else
						{
							data = reader.ReadBytes(2048);
						}

						if (i == 0) //check signature
						{
							if (data[0x100] != 'S' || data[0x101] != 'E' || data[0x102] != 'G' || data[0x103] != 'A')
							{
								//Debug.Assert(false);
								Console.WriteLine("Err\n");
							}

						}

						crcAccum = Crc32.ComputeStep(crcAccum, data);
					}

					crcAccum = Crc32.Finalize(crcAccum);
				}
			}

			return crcAccum;
		}

		public static void DoScanWithoutUI(string dir)
		{
			GameDB db = new GameDB();
			if (File.Exists("db.xml"))
				db.ReadXml("db.xml");

			Crc32.gen_crc_table();

			ScanDir(dir, db, null);
		}

		public static void ConvertImages()
		{
			Crc32.gen_crc_table();

			if (!Directory.Exists("TileCache"))
			{
				Directory.CreateDirectory("TileCache");
			}

			GameDB db = new GameDB();
			if (File.Exists("db.xml"))
			{
				db.ReadXml("db.xml");
			}

			foreach (GameDB.GameRow game in db.Game)
			{
				if (!game.IsScreenshotNull())
				{
					if (!string.IsNullOrEmpty(game.Screenshot))
					{
						string ss = game.Screenshot.Replace("\\", "/");
						if (!File.Exists(ss))
						{
							Console.WriteLine("File " + ss + " is missing");
							continue;
						}

						string dst = "TileCache/" + game.Screenshot.Substring(game.Screenshot.LastIndexOfAny(new char[] { '\\', '/' }) + 1);

						dst = dst.Replace(".png", ".tile");

						if (!File.Exists(dst) || File.GetLastWriteTime(game.Screenshot) > File.GetLastWriteTime(dst))
						{
							if (!Quantizer.Program.Quantize(ss, dst))
							{
								Console.WriteLine("Error converting " + game.Screenshot + ". Skipped");
							}
						}
					}
				}
			}
		}

		private void GameDBMgr_Shown(object sender, EventArgs e)
		{
			DataGridViewColumn c = dataGridView1.Columns[2];

			dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

			gameBindingSource.ResetBindings(true);
		}

		private void genreBindingSource_CurrentChanged(object sender, EventArgs e)
		{

		}
	}

	public sealed class Crc32 : HashAlgorithm
	{
		public const uint DefaultPolynomial = 0xedb88320u;
		public const uint DefaultSeed = 0xffffffffu;

		private static uint[] defaultTable;

		private readonly uint seed;
		private readonly uint[] table;
		private uint hash;

		public Crc32()
			: this(DefaultPolynomial, DefaultSeed)
		{
		}

		public Crc32(uint polynomial, uint seed)
		{
			table = InitializeTable(polynomial);
			this.seed = hash = seed;
		}

		public override void Initialize()
		{
			hash = seed;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			hash = CalculateHash(table, hash, array, ibStart, cbSize);
		}

		protected override byte[] HashFinal()
		{
			byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
			HashValue = hashBuffer;
			return hashBuffer;
		}

		public override int HashSize => 32;

		public static uint Compute(byte[] buffer)
		{
			return Compute(DefaultSeed, buffer);
		}

		public static uint Compute(uint seed, byte[] buffer)
		{
			return Compute(DefaultPolynomial, seed, buffer);
		}

		public static uint ComputeStep(uint seed, byte[] buffer)
		{
			return CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
		}

		public static uint Finalize(uint seed)
		{
			return ~seed;
		}


		public static uint Compute(uint polynomial, uint seed, byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
		}

		private static uint[] InitializeTable(uint polynomial)
		{
			if (polynomial == DefaultPolynomial && defaultTable != null)
			{
				return defaultTable;
			}

			uint[] createTable = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint entry = (uint)i;
				for (int j = 0; j < 8; j++)
					if ((entry & 1) == 1)
						entry = (entry >> 1) ^ polynomial;
					else
						entry = entry >> 1;
				createTable[i] = entry;
			}

			if (polynomial == DefaultPolynomial)
			{
				defaultTable = createTable;
			}

			return createTable;
		}

		private static uint CalculateHash(uint[] table, uint seed, IList<byte> buffer, int start, int size)
		{
			uint hash = seed;
			for (int i = start; i < start + size; i++)
				hash = (hash >> 8) ^ table[buffer[i] ^ hash & 0xff];
			return hash;
		}

		private static byte[] UInt32ToBigEndianBytes(uint uint32)
		{
			byte[] result = BitConverter.GetBytes(uint32);

			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(result);
			}

			return result;
		}

		//This is the same calculation than the HW does
		private static uint[] crc_table = new uint[256];
		public static void gen_crc_table()
		{
			ushort i, j;
			uint crc_accum;

			for (i = 0; i < 256; i++)
			{
				crc_accum = ((uint)i << 24);
				for (j = 0; j < 8; j++)
				{
					if ((crc_accum & 0x80000000L) != 0)
						crc_accum = (crc_accum << 1) ^ 0x04c11db7;
					else
						crc_accum = (crc_accum << 1);
				}
				crc_table[i] = crc_accum;
			}
		}

		public static uint update_crc(uint crc_accum, byte[] data_blk_ptr, uint data_blk_size)
		{
			uint i, j;

			for (j = 0; j < data_blk_size; j++)
			{
				i = ((uint)(crc_accum >> 24) ^ data_blk_ptr[j ^ 3]) & 0xFF;
				crc_accum = (crc_accum << 8) ^ crc_table[i];
			}
			//crc_accum = ~crc_accum;

			return crc_accum;
		}

	}

	public class CueSheet
	{
		public enum TrackType { AUDIO, DATA, OTHER };
		public enum SectorSize { _2352, _2048, OTHER };
		public struct Track
		{
			public string FileName;
			public uint LBA;
			public uint LBAEnd;
			public uint Pregap;
			public uint FileOffset;
			public TrackType Type;
			public SectorSize SectorSize;
		}

		public Track[] Tracks;

		private uint ParseMSF(string msf)
		{
			string[] tokens = msf.Split(':');

			return (uint.Parse(tokens[0]) * 60 * 75) + uint.Parse(tokens[1]) * 75 + uint.Parse(tokens[2]);
		}

		public CueSheet(string file)
		{
			Tracks = new Track[100];

			string[] lines = File.ReadAllLines(file);

			uint currentLBA = 0;
			uint currentOffset = 0;
			int currentTrack = -1;
			uint pregaps = 0;
			bool hasIndex0 = false;
			string fileName = "";
			FileInfo finfo = null;

			foreach (string line in lines)
			{
				string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				if (tokens.Length < 1)
					continue;

				switch (tokens[0])
				{
					case "FILE":
						{
							if (currentTrack != -1) //terminate current track
							{
								uint secsz = (uint)(Tracks[currentTrack].SectorSize == SectorSize._2048 ? 2048 : 2352);
								uint totalframes = (uint)finfo.Length / secsz;

								currentLBA += totalframes;

								Tracks[currentTrack].LBAEnd = currentLBA;
							}

							//if it starts with ", parse till next "
							if (tokens[1][0] == '\"')
							{
								int start = line.IndexOf('\"');
								int end = line.IndexOf('\"', start + 1);
								fileName = line.Substring(start + 1, end - start - 1);
							}
							else
							{
								fileName = tokens[1];
							}

							fileName = Path.GetDirectoryName(file) + "/" + fileName;

							finfo = new FileInfo(fileName);

							currentOffset = 0;
							pregaps = 0;
							hasIndex0 = false;

							break;
						}
					case "TRACK":
						{
							int prvTrack = currentTrack;


							currentTrack = int.Parse(tokens[1]) - 1;

							Tracks[currentTrack].FileName = fileName;
							Tracks[currentTrack].FileOffset = currentOffset;
							Tracks[currentTrack].LBA = currentLBA;
							Tracks[currentTrack].Pregap = 0;
							Tracks[currentTrack].LBAEnd = 0;

							switch (tokens[2]) //format
							{
								case "AUDIO":
									{
										Tracks[currentTrack].SectorSize = SectorSize._2352;
										Tracks[currentTrack].Type = TrackType.AUDIO;
										break;
									}
								case "MODE1/2352":
									{
										Tracks[currentTrack].SectorSize = SectorSize._2352;
										Tracks[currentTrack].Type = TrackType.DATA;
										break;
									}
								case "MODE1/2048":
									{
										Tracks[currentTrack].SectorSize = SectorSize._2048;
										Tracks[currentTrack].Type = TrackType.DATA;
										break;
									}
								default:
									{
										Tracks[currentTrack].SectorSize = SectorSize.OTHER;
										Tracks[currentTrack].Type = TrackType.OTHER;
										break;
									}
							}

							if (prvTrack != -1)
							{
								Tracks[prvTrack].LBAEnd = Tracks[currentTrack].LBA;
							}

							break;
						}
					case "PREGAP":
						{
							Tracks[currentTrack].Pregap = ParseMSF(tokens[1]);
							currentLBA += Tracks[currentTrack].Pregap;
							pregaps += Tracks[currentTrack].Pregap;
							break;
						}
					case "INDEX":
						{
							switch (int.Parse(tokens[1]))
							{
								case 0:
									{
										//index 0, pregap
										Tracks[currentTrack].Pregap = ParseMSF(tokens[2]);
										if (Tracks[currentTrack].Pregap > 500) //bleh ??
										{
											currentLBA = ParseMSF(tokens[2]);
											hasIndex0 = true;
										}
										else
										{
											currentLBA += Tracks[currentTrack].Pregap;
										}


										break;
									}
								case 1:
									{
										//index 1, data
										uint offset = ParseMSF(tokens[2]);
										uint secsz = (uint)(Tracks[currentTrack].SectorSize == SectorSize._2048 ? 2048 : 2352);

										if (hasIndex0)
										{
											Tracks[currentTrack].LBA = offset + pregaps;
											Tracks[currentTrack].Pregap = offset - currentLBA;
											Tracks[currentTrack].FileOffset = offset * secsz;
											currentLBA = offset;
										}
										else
										{
											Tracks[currentTrack].LBA = currentLBA + offset;
											Tracks[currentTrack].FileOffset = offset * secsz;
										}

										if (currentTrack != 0)
										{
											Tracks[currentTrack - 1].LBAEnd = Tracks[currentTrack].LBA;
										}

										break;
									}
							}


							break;
						}
				}
			}

			if (currentTrack != -1) //terminate last track
			{
				uint secsz = (uint)(Tracks[currentTrack].SectorSize == SectorSize._2048 ? 2048 : 2352);
				uint totalframes = (uint)finfo?.Length / secsz;

				currentLBA += totalframes;

				Tracks[currentTrack].LBAEnd = currentLBA;
			}


			int numTracks = currentTrack + 1;

			Track[] finalTracks = new Track[numTracks];

			Array.Copy(Tracks, finalTracks, numTracks);

			Tracks = finalTracks;

		}

		public bool HasDataTrack()
		{
			return Tracks.Count(x => x.Type == TrackType.DATA) != 0;
		}
		public Track FindFirstDataTrack()
		{
			return Tracks.FirstOrDefault(x => x.Type == TrackType.DATA);
		}
	}
}
