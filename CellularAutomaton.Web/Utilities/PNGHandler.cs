using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CellularAutomaton.Web.Models;
using CellularAutomaton.Web.Utilities;
using SkiaSharp;

namespace Utilities
{
	static class PNGHandler
	{
        public static int width, height, tileSize = 10;
		public static bool foundFire = false;
        public static IWebHostEnvironment? Environment { get; set; }

		public static List<string> WriteFiles(int numberOfFilesToWrite, string imageInBase64,
			double windDirection, int width, int height, int tileSize)
		{
			PNGHandler.tileSize = tileSize;

			SKBitmap beginning = BitMapBase64Converter.ConvertBase64ToSKBitmap(imageInBase64);
            Tile[,] tiles = PNGHandler.Read(beginning);
            Model model = new Model(width, height, tiles, windDirection);

			List<string> result = new List<string>();

			for (int i = 0; i < numberOfFilesToWrite; i++)
            {
                model.SimulateFireSpread();
                result.Add(PNGHandler.Write(model.Grid, i));
            }

			beginning.Dispose();
			return result;
        }

		public static Tile[,] Read(SKBitmap bitmap)
		{
			width = bitmap.Width;
			height = bitmap.Height;
			Tile[,] tiles = new Tile[width / tileSize, height / tileSize];

			for (int i = 0; i < tiles.GetLength(0); i++)
			{
				for (int j = 0; j < tiles.GetLength(1); j++)
				{
					var color = bitmap.GetPixel(i * tileSize, j * tileSize);
					if (color == 0xffff0000) // Fire color
					{
						tiles[i, j] = new Tile(VegetationType.High, DensityType.Dense, BurnStateType.Burning);
						foundFire = true;
						continue;
					}
					if (color == 0xff1a120d) // Burnt color
					{
						tiles[i, j] = new Tile(VegetationType.High, DensityType.Dense, BurnStateType.Burnt);
						continue;
					}
					if (!Tile.fromColor.TryGetValue(color, out var tuple)) // Unknown color
					{
						if (j == 0)
						{
							if (i == 0) tiles[i, j] = new Tile(VegetationType.Rock, DensityType.None, BurnStateType.None);
							else tiles[i, j] = tiles[i - 1, j];
							continue;
						}
						if (i == 0)
						{
							tiles[i, j] = tiles[i, j - 1];
							continue;
						}
						tiles[i, j] = new Random().Next(1) == 1 ? tiles[i, j - 1] : tiles[i - 1, j];
						continue;
					}
					if (tuple.Item2 == DensityType.None) // Water or rock
					{
						tiles[i, j] = new Tile(tuple.Item1, tuple.Item2, BurnStateType.None);
					}
					// Vegetation
					tiles[i, j] = new Tile(tuple.Item1, tuple.Item2, BurnStateType.Fuel);
				}
			}
			if (!foundFire) ; // End state!
			return tiles;
		}

		public static string Write(Tile[,] tiles, int index)
		{

			using SKBitmap bitmap = new SKBitmap(width, height);
			using SKCanvas canvas = new SKCanvas(bitmap);
			IntPtr pixelsPtr = bitmap.GetPixels();

			if (pixelsPtr == IntPtr.Zero)
				throw new Exception("Failed to get pixel buffer!");

			// Fill the pixel array (Set each pixel manually)
			for (int x = 0; x < width / tileSize; x++)
			{
				for (int y = 0; y < height / tileSize; y++)
				{
					SKPaint paint;
					if (tiles[x, y].BurnState == BurnStateType.Burning)
					{
						paint = new SKPaint { Color = 0xffff0000 };
					}
					else if (tiles[x, y].BurnState == BurnStateType.Burnt)
					{
						paint = new SKPaint { Color = 0xff1a120d };
					}
					else
					{

						paint = new SKPaint
						{
							Color = Tile.toColors[
							((VegetationType)(tiles[x, y].Vegetation * 10),
							(DensityType)(tiles[x, y].Density * 10))]
						};
					}
					canvas.DrawRect(x * tileSize, y * tileSize, tileSize, tileSize, paint);
				}
			}

			return BitMapBase64Converter.ConvertSKBitmapToBase64(bitmap);
		}
	}
}
