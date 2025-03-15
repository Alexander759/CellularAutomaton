using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Utilities
{
	class Model
	{
		private static int[,] moveDirections = { { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
		private static double baseProbability = 0.58;
		public Model(int rows, int cols, Tile[,] grid, double windDirection)
		{
			Rows = rows;
			Cols = cols;
			Grid = new Tile[Rows, Cols];
			Array.Copy(grid, Grid, grid.Length);
			WindDirection = 3 * Math.PI / 2 - windDirection;
		}

		public int Rows { get; set; }
		public int Cols { get; set; }
		public Tile[,] Grid { get; set; }
		public double WindDirection { get; set; }

		// Simulate fire spread for one time step
		public void SimulateFireSpread()
		{
			Tile[,] newGrid = new Tile[Rows, Cols];
			Array.Copy(Grid, newGrid, Grid.Length);

			Random r = new Random();

			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Cols; j++)
				{
					if (Grid[i, j].BurnState != BurnStateType.Burning) continue;
					// Spread fire to neighbors
					double angle = -WindDirection;

					for (int dir = 0; dir < 8; dir++, angle += Math.PI / 4.0)
					{

						int ni = i + moveDirections[dir, 0];
						int nj = j + moveDirections[dir, 1];

						if (ni < 0 || ni >= Rows || nj < 0 || nj >= Cols || Grid[ni, nj].BurnState == BurnStateType.Burning
							|| Grid[ni, nj].BurnState == BurnStateType.Burnt || Grid[ni, nj].BurnState == BurnStateType.None) continue;

						double probability = CalculateSpreadProbability(i, j, angle);
						if (dir % 2 == 1) probability /= Math.Sqrt(2); // In case of diagonal
						double randomProb = r.NextDouble();

						if (randomProb < probability)
						{
							newGrid[ni, nj].BurnState = BurnStateType.Burning;
						}
					}

					// Transition burning cell to burned after burning time
					newGrid[i, j].BurnState = BurnStateType.Burnt;
				}
			}
			Grid = newGrid;
		}

		// Calculate the probability of fire spreading from (i,j) to (ni,nj)
		double CalculateSpreadProbability(int i, int j, double angle)
		{
			double Pw, ft;
			double burnProbability;
			const double C1 = 0.045, C2 = 0.131, V = 8;

			double exponent = Math.Exp(C1 * V);

			burnProbability = baseProbability * (1 + Grid[i, j].Density) * (1 + Grid[i, j].Vegetation);

			ft = Math.Exp(V * C2 * (Math.Cos(angle) - 1));

			Pw = exponent * ft;

			burnProbability *= Pw;
			burnProbability = Math.Min(burnProbability, 1.0);

			return burnProbability;
		}
	}
}