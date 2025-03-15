using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ConsoleApp2
{
    class Model
    {
        // Grid dimensions
        const int rows = 50;
        const int cols = 50;

        // Grid representing the landscape
        static Tile[,] grid = new Tile[rows, cols];

        static int[,] moveDirections = { { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

        static double baseProbability = 0.58;

        static double windDirection = Math.PI / 3.0;

        static void Main(string[] args)
        {
            // Simulate fire spread
            while (true)
            {
                SimulateFireSpread();
            }
        }

        // Initialize the map with initial conditions
        static void SetGrid(Tile[,] map)
        {
            grid = map;
        }

        // Simulate fire spread for one time step
        static void SimulateFireSpread()
        {
            Tile[,] newGrid = new Tile[rows, cols];
            Array.Copy(grid, newGrid, grid.Length);

            Random r = new Random();


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i, j].BurnState != BurnState.Burning) continue;
                    // Spread fire to neighbors
                    double angle = -windDirection;

                    for (int dir = 0; dir < 8; dir++, angle += Math.PI / 4.0)
                    {

                        int ni = i + moveDirections[dir, 0];
                        int nj = j + moveDirections[dir, 1];

                        if (ni < 0 || ni >= rows || nj < 0 || nj >= cols || grid[ni, nj].BurnState == BurnState.Burning
                            || grid[ni, nj].BurnState == BurnState.Burnt) continue;

                        double probability = CalculateSpreadProbability(i, j, angle);
                        double randomProb = r.NextDouble();


                        if (randomProb < probability)
                        {
                            if (j == 30 && i >= 20 && i <= 30)
                            {
                                ;
                            }
                            newGrid[ni, nj].BurnState = BurnState.Burning;
                        }
                    }

                    // Transition burning cell to burned after burning time
                    newGrid[i, j].BurnState = BurnState.Burnt;
                }
            }

            grid = newGrid;
        }

        // Calculate the probability of fire spreading from (i,j) to (ni,nj)
        static double CalculateSpreadProbability(int i, int j, double angle)
        {
            double Pw, ft;
            double burnProbability;
            const double C1 = 0.045, C2 = 0.131, V = 8;

            double exponent = Math.Exp(C1 * V);

            burnProbability = baseProbability * (1 + grid[i, j].Density / 10) * (1 + grid[i, j].Vegetation);

            ft = Math.Exp(V * C2 * (Math.Cos(angle) - 1));

            Pw = exponent * ft;

            burnProbability *= Pw;
            burnProbability = Math.Min(burnProbability, 1.0);

            return burnProbability;
        }
    }
}