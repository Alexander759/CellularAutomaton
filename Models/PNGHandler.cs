using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1;
using FirePreventionStructs;
using SkiaSharp;

namespace Models
{
	static class PNGHandler
	{
		public static int width, height, tileSize = 1;
		public static Tile[,] Read(string filePath)
		{
			using SKBitmap bitmap = SKBitmap.Decode(filePath);
			width = bitmap.Width;
			height = bitmap.Height;
			Console.WriteLine($"Image loaded: {width}x{height}");
			Tile[,] tiles = new Tile[width, height];

			for (int i = 0; i < tiles.GetLength(0); i += tileSize)
			{
				for (int j = 0; j < tiles.GetLength(1); j += tileSize)
				{
					var tuple = Tile.fromColor[bitmap.GetPixel(i, j)];
					tiles[i, j] = new Tile(tuple.Item1, tuple.Item2, BurnStateType.Fuel);
				}
			}
			return tiles;
		}
		public static void Write(Tile[,] tiles, string filePath)
		{
			SKBitmap bitmap = new SKBitmap(width, height);
			IntPtr pixelsPtr = bitmap.GetPixels();

			if (pixelsPtr == IntPtr.Zero)
				throw new Exception("Failed to get pixel buffer!");

			// Fill the pixel array (Set each pixel manually)
			for (int y = 0; y < height; y += tileSize)
			{
				for (int x = 0; x < width; x += tileSize)
				{
					bitmap.SetPixel(x, y, Tile.toColors[
						((VegetationType)(tiles[x, y].Vegetation * 10),
						(DensityType)(tiles[x, y].Density * 10))]);
				}
			}

			// Encode as PNG
			using var image = SKImage.FromBitmap(bitmap);
			using var data = image.Encode(SKEncodedImageFormat.Png, 100);

			// Save the file
			File.WriteAllBytes(filePath + "\\output.png", data.ToArray());

			Console.WriteLine("PNG image saved as output.png");
		}
	}
}
