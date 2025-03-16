using SkiaSharp;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CellularAutomaton.Web.Utilities;

namespace Utilities
{
    struct CurrentState
    {
        public int cellX;
        public int cellY;
        public int currentSteps;

        public CurrentState()
        {
            this.cellX = 0;
            this.cellY = 0;
            this.currentSteps = 0;
        }

        public CurrentState(int CellX, int CellY, int CurrentSteps)
        {
            this.cellX = CellX;
            this.cellY = CellY;
            this.currentSteps = CurrentSteps;
        }
    }

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

        public bool finishedSimulation = false;

        public string getImageString(SKBitmap bitmap)
        {
            return BitMapBase64Converter.ConvertSKBitmapToBase64(bitmap);
        }

        // Simulate fire spread for a fixed ammount of time steps
        public List<string> SimulateFireSpread(int NumberOfFiles, int tileSize, SKBitmap currentBitmap)
        {
            using SKCanvas canvas = new SKCanvas(currentBitmap);
            SKPaint paint;

            List<string> result = new List<string>();

            IntPtr pixelsPtr = currentBitmap.GetPixels();

            if (pixelsPtr == IntPtr.Zero)
                throw new Exception("Failed to get pixel buffer!");

            Random r = new Random();

            int currentFile = 0;

            Queue<CurrentState> bfsQueue = new Queue<CurrentState>();

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Cols; y++)
                {
                    if (Grid[x, y].BurnState == BurnStateType.Burning)
                    {
                        bfsQueue.Enqueue(new CurrentState(x, y, 0));
                    }

                    // account for tilesize
                    if (Grid[x, y].BurnState == BurnStateType.Burning)
                    {
                        paint = new SKPaint { Color = 0xffff0000 };
                    }
                    else if (Grid[x, y].BurnState == BurnStateType.Burnt)
                    {
                        paint = new SKPaint { Color = 0xff1a120d };
                    }
                    else
                    {

                        paint = new SKPaint
                        {
                            Color = Tile.toColors[
                            ((VegetationType)(Grid[x, y].Vegetation * 10),
                            (DensityType)(Grid[x, y].Density * 10))]
                        };
                    }
                    canvas.DrawRect(x * tileSize, y * tileSize, tileSize, tileSize, paint);
                }
            }

            finishedSimulation = false;

            while (bfsQueue.Count > 0)
            {
                var state = bfsQueue.Dequeue();

                if (state.currentSteps > currentFile)
                {
                    result.Add(getImageString(currentBitmap));

                    currentFile++;
                    if (currentFile == NumberOfFiles) break;
                }

                // Transition burning cell to burned after burning time
                Grid[state.cellX, state.cellY].BurnState = BurnStateType.Burnt;

                paint = new SKPaint { Color = 0xff1a120d };
                canvas.DrawRect(state.cellX * tileSize, state.cellY * tileSize, tileSize, tileSize, paint);

                // Spread fire to neighbors
                double angle = -WindDirection;

                for (int dir = 0; dir < 8; dir++, angle += Math.PI / 4.0)
                {

                    int nextX = state.cellX + moveDirections[dir, 0];
                    int nextY = state.cellY + moveDirections[dir, 1];

                    if (nextX < 0 || nextX >= Rows || nextY < 0 || nextY >= Cols || Grid[nextX, nextY].BurnState == BurnStateType.Burning
                        || Grid[nextX, nextY].BurnState == BurnStateType.Burnt || Grid[nextX, nextY].BurnState == BurnStateType.None
                        || Grid[nextX, nextY].Density == (double)DensityType.None / 10.0) continue;

                    double probability = CalculateSpreadProbability(state.cellX, state.cellY, angle);
                    if (dir % 2 == 1) probability /= Math.Sqrt(2);

                    double randomProb = r.NextDouble();


                    if (randomProb < probability)
                    { // paint a burning cell
                        bfsQueue.Enqueue(new CurrentState(nextX, nextY, state.currentSteps + 1));

                        paint = new SKPaint { Color = 0xffff0000 };
                        canvas.DrawRect(nextX * tileSize, nextY * tileSize, tileSize, tileSize, paint);

                        Grid[nextX, nextY].BurnState = BurnStateType.Burning;
                    }
                }
            }

            while (currentFile < NumberOfFiles)
            {
                finishedSimulation = true;

                result.Add(getImageString(currentBitmap));

                currentFile++;
            }

            return result;
        }

        // Calculate the probability of fire spreading to (i,j)
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