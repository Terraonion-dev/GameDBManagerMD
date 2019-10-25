using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;

namespace Quantizer
{
	public class Program
	{
		static ushort[] bitmap;
		static uint[,] palettes;
		static byte[,,] tiles;
		static byte[] palcodes;
		static byte[] palfrees;
		static ushort[,] finalpalettes;
		static int[] colusage;


		static void WriteMessageLine(String message = "")
		{
			//Console.WriteLine(message);
		}

		static void WriteMessage(String message = "")
		{
			//Console.Write(message);
		}

		static int CountPaletteColors(int pal)
		{
			int n = 0;
			for (int k = 1; k < 256; ++k)
			{
				//if (palettes[pal, k] == 0x10000)
				//	break;
				if (palettes[pal, k] != 0x10000)
					++n;
			}

			return n;
		}

		static void SwapColor(int tile, byte c1, byte c2)
		{
			for (int y = 0; y < 8; ++y)
			{
				for (int x = 0; x < 8; ++x)
				{
					if (tiles[tile, x, y] == c1)
						tiles[tile, x, y] = c2;
				}
			}
		}

		static int NumSameColors(int tile, int palfinal)
		{
			int nfound = 0;
			for (int i = 1; i < CountPaletteColors(tile) + 1; ++i)
			{
				for (int j = 1; j < 16 - palfrees[palfinal]; ++j)
				{
					if ((palettes[tile, i] & 0x7FFF) == (finalpalettes[palfinal, j] & 0x7FFF))
					{
						++nfound;
						break;
					}
				}
			}

			return nfound;
		}

		static void AddMissingColors(int tile, int palfinal)
		{

			for (int i = 1; i < CountPaletteColors(tile) + 1; ++i)
			{
				bool found = false;
				for (int j = 1; j < 16 - palfrees[palfinal]; ++j)
				{
					if ((palettes[tile, i] & 0x7FFF) == (finalpalettes[palfinal, j] & 0x7FFF))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					finalpalettes[palfinal, 16 - palfrees[palfinal]] = (ushort)(palettes[tile, i] & 0x7FFF);
					--palfrees[palfinal];
				}
			}
		}

		public static bool Quantize(String src,String dst)
		{
			if (!File.Exists(src))
			{
				Console.WriteLine("File "+src+" not found");
				return false;
			}

			//StreamWriter log = new StreamWriter(@"log.txt", false);

			//String f = "0000.png";
			//String f = @"F:\Firmware\GameDbManager\bin\Debug\Images\reduced\yamamura.png";
			//foreach (var f in Directory.EnumerateFiles(@"F:\Firmware\GameDbManager\bin\Debug\Images\reduced","*.png"))
			String f = src;
			using(Bitmap bm = (Bitmap)Image.FromFile(f))
			{
				string name = f.Substring(f.LastIndexOf("\\") + 1);
				name = name.Substring(0, name.LastIndexOf("."));


				if (bm.Width != 64 || bm.Height != 40)
				{
					Console.WriteLine("Screenshot for " + name + " has wrong size. It must be 64x40 pixels");
					return false;
				}


				//if (name != "aof3")
				//	continue;

				int tilesx = bm.Width / 8;
				int tilesy = bm.Height / 8;
				int totaltiles = tilesx * tilesy;

				bitmap = new ushort[bm.Width * bm.Height];
				palettes = new uint[totaltiles, 256];
				tiles = new byte[totaltiles, 8, 8];
				palcodes = new byte[totaltiles];
				palfrees = new byte[totaltiles];
				finalpalettes = new ushort[totaltiles, 16];
				colusage = new int[0x10000];

				for (int y = 0; y < bm.Height; ++y)
				{
					for (int x = 0; x < bm.Width; ++x)
					{
						Color c = bm.GetPixel(x, y);

						ushort r = (ushort)(c.R >> 5);
						ushort g = (ushort)(c.G >> 5);
						ushort b = (ushort)(c.B >> 5);

						bitmap[y * bm.Width + x] = (ushort)((r << 6) | (g << 3) | b);
					}
				}
/*
				for (int it = 0; it < 10; ++it)
				{
					for (int i = 0; i < bm.Width * bm.Height; ++i)
					{
						ushort c = bitmap[i];

						for (int j = i + 1; j < bm.Width * bm.Height; ++j)
						{
							ushort c2 = bitmap[j];

							ushort b1 = (ushort)(c & 0x1f);
							ushort g1 = (ushort)((c >> 5) & 0x1F);
							ushort r1 = (ushort)((c >> 10) & 0x1F);

							ushort b2 = (ushort)(c2 & 0x1f);
							ushort g2 = (ushort)((c2 >> 5) & 0x1F);
							ushort r2 = (ushort)((c2 >> 10) & 0x1F);

							if (i == 0x4e5)
							{
								int b = 1;
							}

							if (Math.Abs(r1 - r2) <= 2)
								r2 = r1 = (ushort)((r1 + r2) >> 1);
							if (Math.Abs(g1 - g2) <= 2)
								g2 = g1 = (ushort)((g1 + g2) >> 1);
							if (Math.Abs(b1 - b2) <= 2)
								b2 = b1 = (ushort)((b1 + b2) >> 1);

							bitmap[i] = (ushort)((r1 << 10) | (g1 << 5) | b1);
							bitmap[j] = (ushort)((r2 << 10) | (g2 << 5) | b2);
						}
					}
				}
				*/
				for (int ty = 0; ty < tilesy; ++ty)
				{
					for (int tx = 0; tx < tilesx; ++tx)
					{
						int tile = ty * tilesx + tx;

						if (tile == 16)
						{
							int a = 1;
						}

						for (int p = 0; p < 256; ++p)
						{
							palettes[tile, p] = 0x10000;
						}

						for (int y = 0; y < 8; ++y)
						{
							for (int x = 0; x < 8; ++x)
							{
								ushort c = bitmap[(ty * 8 + y) * bm.Width + tx * 8 + x];
								bool found = false;

								for (int i = 1; i < 256; ++i)
								{
									if ((palettes[tile, i] & 0x1FFFF) == c)
									{
										tiles[tile, x, y] = (byte)i;
										found = true;
										break;
									}
								}

								if (!found)
								{
									found = false;

									for (int i = 1; i < 256; ++i)
									{
										if (palettes[tile, i] == 0x10000)
										{
											palettes[tile, i] = (uint)(0x800000 | c);
											tiles[tile, x, y] = (byte)i;
											found = true;
											break;
										}
									}
								}

								if (!found)
								{
									int a = 1;
								}
							}
						}
					}
				}
				/*
							for (int ty = 0; ty < 7 * 8; ++ty)
							{
								for (int tx = 0; tx < 10 * 8; ++tx)
								{
									int tile = (ty / 8) * 10 + (tx / 8);
									int y = ty % 8;
									int x = tx % 8;

									WriteMessage(tiles[tile, x, y].ToString("X2"));
								}

								WriteMessageLine();
							}
				*/
				for (int i = 0; i < totaltiles; ++i)
				{
					int n = 0;
					for (int k = 1; k < 256; ++k, ++n)
					{
						if (palettes[i, k] == 0x10000)
							break;
					}
					WriteMessageLine("Pal " + i + " Colors: " + n);

					for (int k = 1; k < 256; ++k, ++n)
					{
						if (palettes[i, k] == 0x10000)
							break;
						colusage[palettes[i, k] & 0x7FFF]++;
						WriteMessage("0x" + (palettes[i, k] & 0x7FFF).ToString("X4") + ", ");
					}

					WriteMessageLine();
				}

				for (int i = 0; i < totaltiles; ++i)
				{
					palfrees[i] = 15;
				}

				int curpal = 0;


				//first reduce palettes with > 15 colors to 15
				WriteMessageLine("Reducing palette colors");
				for (int i = 0; i < totaltiles; ++i)
				{
					if (CountPaletteColors(i) > 15)
					{
						WriteMessageLine("Pal " + i + " needs reduction");

						int nit = 1;

						if (i == 0x35)
						{
							int a = 1;
						}

						while (CountPaletteColors(i) > 15)
						{
							int colors = CountPaletteColors(i);
							for (int c1 = 1; c1 < 256; ++c1)
							{
								for (int c2 = c1 + 1; c2 < 256; ++c2)
								{
									uint col1 = palettes[i, c1];
									uint col2 = palettes[i, c2];

									if (col1 == 0x10000 || col2 == 0x10000)
										continue;

									ushort b1 = (ushort)(col1 & 0x7);
									ushort g1 = (ushort)((col1 >> 3) & 0x7);
									ushort r1 = (ushort)((col1 >> 6) & 0x7);

									ushort b2 = (ushort)(col2 & 0x7);
									ushort g2 = (ushort)((col2 >> 3) & 0x7);
									ushort r2 = (ushort)((col2 >> 6) & 0x7);


									double diff = 4.0f * Math.Sqrt((b1 - b2) * (b1 - b2) + (g1 - g2) * (g1 - g2) + (r1 - r2) * (r1 - r2));

									if (nit > 2)
									{
										int a = 1;
									}
									if (diff < nit)
									{
										if (i == 0xb && (c1 == 0xc || c2 == 0xc))
										{
											int a = 1;
										}
										if (colusage[col1 & 0x7FFF] > colusage[col2 & 0x7FFF])
										{
											SwapColor(i, (byte)c2, (byte)c1);
											palettes[i, c2] = 0x10000;
										}
										else
										{
											SwapColor(i, (byte)c1, (byte)c2);
											palettes[i, c1] = 0x10000;
										}
										int b = 1;

									}

								}
							}

							++nit;
						}

						//now pack the palette
						uint[] p2 = new uint[256];
						p2[0] = 0x10000;
						int n = 1;
						for (int c1 = 1; c1 < 256; ++c1)
						{
							if (palettes[i, c1] != 0x10000)
							{
								p2[n] = palettes[i, c1];
								SwapColor(i, (byte)c1, (byte)n);
								++n;
							}
						}
						for (int c1 = 0; c1 < 256; ++c1)
						{
							if (c1 < n)
								palettes[i, c1] = p2[c1];
							else
								palettes[i, c1] = 0x10000;
						}
						/*
						for (int c1 = 1; c1 < 256; ++c1)
						{
							if (palettes[i, c1] == 0x10000)
							{
								for (int c2 = 255; c2 >= 1; --c2)
								{
									if (palettes[i, c2] != 0x10000)
									{
										if (c1 == 0xFF || c2 == 0xFF)
										{
											int a = 1;
										}
										palettes[i, c1] = palettes[i, c2];
										palettes[i, c2] = 0x10000;
										SwapColor(i, (byte)c2, (byte)c1);
										break;
									}
								}
							}

						}
						*/

					}
				}

				//Recount colors
				WriteMessageLine("Re-counting colors");
				for (int i = 0; i < totaltiles; ++i)
				{
					int n = 0;
					for (int k = 1; k < 256; ++k, ++n)
					{
						if (palettes[i, k] == 0x10000)
							break;
					}
					WriteMessageLine("Pal " + i + " Colors: " + n);

					for (int k = 1; k < 256; ++k, ++n)
					{
						if (palettes[i, k] == 0x10000)
							break;
						colusage[palettes[i, k] & 0x7FFF]++;
						WriteMessage("0x" + (palettes[i, k] & 0x7FFF).ToString("X4") + ", ");
					}

					WriteMessageLine();
				}


				//Run through all palettes to try to extract the common color sets

				for (int ncols = 15; ncols >= 0; --ncols)
				{
					for (int tile = 0; tile < totaltiles; ++tile)
					{
						if (CountPaletteColors(tile) == ncols)
						{
							//first find a palette containing all colors
							bool found = false;

							if (tile == 17)
							{
								int ww = 1;
							}

							for (int p = 0; p < totaltiles; ++p)
							{
								if (NumSameColors(tile, p) == CountPaletteColors(tile))
								{
									palcodes[tile] = (byte)p;
									found = true;
									break;
								}
							}

							if (!found)
							{
								//find a palette with a subset of colors and enough free colors
								for (int p = 0; p < totaltiles; ++p)
								{
									if (palfrees[p] >= CountPaletteColors(tile) - NumSameColors(tile, p))
									{
										//add the missing colors to this palette
										palcodes[tile] = (byte)p;
										AddMissingColors(tile, p);
										found = true;
										break;
									}
								}
							}
						}
					}
				}

				//Recolorize the tilemap
				for (int i = 0; i < totaltiles; ++i)
				{
					for (int y = 0; y < 8; ++y)
					{
						for (int x = 0; x < 8; ++x)
						{
							uint c = palettes[i, tiles[i, x, y]] & 0x7FFF;
							byte tilepal = palcodes[i];

							bool found = false;
							for (int cc = 0; cc < 16 - palfrees[tilepal]; ++cc)
							{
								if ((finalpalettes[tilepal, cc] & 0x7FFF) == (c & 0x7FFF))
								{
									tiles[i, x, y] = (byte)cc;
									found = true;
									break;
								}
							}

							if (!found)
							{
								int a = 1;
							}
							Debug.Assert(found);
						}
					}
				}

				WriteMessageLine();
				WriteMessageLine();
				WriteMessageLine("Final Palettes");

				int npals = 0;

				for (int i = 0; i < totaltiles; ++i)
				{
					if (palfrees[i] == 15)
						continue;
					WriteMessageLine("Palette " + i + " Free " + palfrees[i]);

					for (int c = 0; c < 15 - palfrees[i]; ++c)
					{
						WriteMessage("0x" + (finalpalettes[i, c] & 0x7FFF).ToString("X4") + ", ");
					}

					WriteMessageLine();

					++npals;
				}

				if (npals > 3)
				{
					//log.WriteLine("Game " + name + " has too many palettes: " + npals);
					Console.WriteLine("Screenshot for " + name + " has too many palettes: " + npals);
					//log.Close();
					return false;
				}

				WriteMessageLine();
				WriteMessageLine("Tilemap:");
				for (int ty = 0; ty < tilesy * 8; ++ty)
				{
					for (int tx = 0; tx < tilesx * 8; ++tx)
					{
						int tile = (ty / 8) * tilesx + (tx / 8);
						int y = ty % 8;
						int x = tx % 8;

						WriteMessage(tiles[tile, x, y].ToString("X"));
						if (x == 7)
							WriteMessage(" ");
					}

					WriteMessageLine();
					if((ty % 8) == 7)
						WriteMessageLine();
				}


				//FileStream fsw = new FileStream(name + ".tile", FileMode.Create);
				FileStream fsw = new FileStream(dst, FileMode.Create);


				for (int yy = 0; yy < tilesy; ++yy)
				{
					for (int xx = 0; xx < tilesx; ++xx)
					{
						int tile = yy * tilesx + xx;
						byte[] tilepce = new byte[32];
						for (int y = 0; y < 8; ++y)
						{
							for (int x = 0; x < 4; ++x)
							{
								byte b1 = tiles[tile, 2*x, y];
								byte b2 = tiles[tile, 2*x+1, y];

								tilepce[4 * y + x] = (byte)((b1 << 4)|b2);
							}
						}

						fsw.Write(tilepce, 0, 32);
					}
				}

				//Tiles
				BinaryWriter bw = new BinaryWriter(fsw);
				{
					WriteMessageLine();
					WriteMessageLine("Palette Map:");
					for (int ty = 0; ty < tilesy; ++ty)
					{
						for (int tx = 0; tx < tilesx; ++tx)
						{
							WriteMessage(palcodes[ty * tilesx + tx].ToString("X") + ",");
							bw.Write(palcodes[ty * tilesx + tx]);
						}
						WriteMessageLine();
					}
				}
				//Palettes
				{
					WriteMessageLine();
					WriteMessageLine("Palettes:");
					for (int i = 0; i < totaltiles; ++i)
					{
						if (palfrees[i] == 15)
							continue;

						WriteMessage("Palette " + i.ToString("D2") + ": ");
						ushort[] palette = new ushort[16];

						for (int c = 0; c < 15 - palfrees[i] + 1; ++c)
						{
							ushort col = (ushort)(finalpalettes[i, c] & 0x7FFF);
							palette[c] = col;
							WriteMessage(col.ToString("X4") + ",");
						}

						for (int j = 0; j < 16; ++j)
						{
							ushort pal = palette[j];

							ushort b1 = (ushort)(pal & 0x7);
							ushort g1 = (ushort)((pal >> 3) & 0x7);
							ushort r1 = (ushort)((pal >> 6) & 0x7);

							//pal = (ushort)((r1 << 1) | (g1 << 5) | (b1 << 9));

							pal = (ushort)((r1 << 9) | (g1 << 13) | (b1 << 1));

							bw.Write(pal);
						}

						WriteMessageLine();
					}

				}

				fsw.Close();
			}

			//log.Close();

			return true;
		}
	}
}
