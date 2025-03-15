using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public enum VegetationType : sbyte
	{
		Water = -20,
		Rock = -10,
		Low = -3,
		Medium = 0,
		High = 4
	}

	public enum DensityType : sbyte
	{
		None = -10,
		Sparse = -4,
		Medium = 0,
		Dense = 3
	}

	public enum BurnStateType : byte
	{
		None,
		Fuel,
		Burning,
		Burnt
	}

	public struct Tile
	{
		public Tile(VegetationType vegetation, DensityType density, BurnStateType burnState)
		{
			Vegetation = (double)vegetation / 10;
			Density = (double)density / 10;
			BurnState = burnState;
		}

		public double Vegetation { get; private set; }
		public double Density { get; private set; }
		public BurnStateType BurnState { get; set; }

		public static Dictionary<(VegetationType, DensityType), SKColor> toColors = new()
		{
			{ (VegetationType.Water, DensityType.None), 0xff0091ff },
			{ (VegetationType.Rock, DensityType.None), 0xff4d4d4d },

			{ (VegetationType.Low, DensityType.Sparse), 0xfffdff99 },
			{ (VegetationType.Medium, DensityType.Sparse), 0xfffbff4a },
			{ (VegetationType.High, DensityType.Sparse), 0xff747527 },

			{ (VegetationType.Low, DensityType.Medium), 0xffd1a3ff },
			{ (VegetationType.Medium, DensityType.Medium), 0xff9d3bff },
			{ (VegetationType.High, DensityType.Medium), 0xff4c2275 },

			{ (VegetationType.Low, DensityType.Dense), 0xff90c96f },
			{ (VegetationType.Medium, DensityType.Dense), 0xff568c37 },
			{ (VegetationType.High, DensityType.Dense), 0xff3f6927 },
		};

		public static Dictionary<SKColor, (VegetationType, DensityType)> fromColor = new()
		{
			{ 0xff0091ff, (VegetationType.Water, DensityType.None) },
			{ 0xff4d4d4d, (VegetationType.Rock, DensityType.None) },

			{ 0xfffdff99, (VegetationType.Low, DensityType.Sparse) },
			{ 0xfffbff4a, (VegetationType.Medium, DensityType.Sparse) },
			{ 0xff747527, (VegetationType.High, DensityType.Sparse) },

			{ 0xffd1a3ff, (VegetationType.Low, DensityType.Medium) },
			{ 0xff9d3bff, (VegetationType.Medium, DensityType.Medium) },
			{ 0xff4c2275, (VegetationType.High, DensityType.Medium) },

			{ 0xff90c96f, (VegetationType.Low, DensityType.Dense) },
			{ 0xff568c37, (VegetationType.Medium, DensityType.Dense) },
			{ 0xff3f6927, (VegetationType.High, DensityType.Dense) },
		};

	}
}