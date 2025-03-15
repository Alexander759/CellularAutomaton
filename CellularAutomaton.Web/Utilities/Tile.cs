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
			{ (VegetationType.Water, DensityType.None), SKColors.CornflowerBlue },
			{ (VegetationType.Rock, DensityType.None), SKColors.Gray },

			{ (VegetationType.Low, DensityType.Sparse), SKColors.Yellow },
			{ (VegetationType.Medium, DensityType.Sparse), SKColors.YellowGreen },
			{ (VegetationType.High, DensityType.Sparse), SKColors.GreenYellow },

			{ (VegetationType.Low, DensityType.Medium), SKColors.LightGreen },
			{ (VegetationType.Medium, DensityType.Medium), SKColors.Lime },
			{ (VegetationType.High, DensityType.Medium), SKColors.Green },

			{ (VegetationType.Low, DensityType.Dense), SKColors.Teal },
			{ (VegetationType.Medium, DensityType.Dense), SKColors.ForestGreen },
			{ (VegetationType.High, DensityType.Dense), SKColors.DarkGreen },
		};

		public static Dictionary<SKColor, (VegetationType, DensityType)> fromColor = new()
		{
			{ SKColors.CornflowerBlue, (VegetationType.Water, DensityType.None) },
			{ SKColors.Gray, (VegetationType.Rock, DensityType.None) },

			{ SKColors.Yellow , (VegetationType.Low, DensityType.Sparse) },
			{ SKColors.YellowGreen , (VegetationType.Medium, DensityType.Sparse) },
			{ SKColors.GreenYellow , (VegetationType.High, DensityType.Sparse) },

			{ SKColors.LightGreen , (VegetationType.Low, DensityType.Medium) },
			{ SKColors.Lime , (VegetationType.Medium, DensityType.Medium) },
			{ SKColors.Green , (VegetationType.High, DensityType.Medium) },

			{ SKColors.Teal , (VegetationType.Low, DensityType.Dense) },
			{ SKColors.ForestGreen , (VegetationType.Medium, DensityType.Dense) },
			{ SKColors.DarkGreen , (VegetationType.High, DensityType.Dense) },
		};

	}
}
