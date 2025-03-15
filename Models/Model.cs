using System;
using System.Runtime.CompilerServices;
using System.Threading;

class Program
{
    // Grid dimensions
    const int Rows = 50;
    const int Cols = 50;

    // Cell states
    enum CellState { Unburned, Burning, Burned }

    // Grid representing the landscape
    static CellState[,] grid = new CellState[Rows, Cols];

    static int [,] HowToMove = {{0, 1}, {-1, 1}, {-1, 0}, {-1, -1}, {0, -1}, {1, -1}, {1, 0}, {1, 1}};

    // Parameters
    static double baseProbability = 0.58; // Base probability of fire spread
    static int burningTime = 1; // Time steps a cell remains burning

    // Wind direction (0: no wind, 1: right, 2: left, 3: up, 4: down)
    static double windDirection = Math.PI / 3.0;

    // Slope (0: flat, 1: uphill, -1: downhill)
    static double[,] density = new double[Rows, Cols]; // Slope values for each cell

    // Vegetation factors (1.0: normal, >1.0: more flammable, <1.0: less flammable)
    static double[,] vegetation = new double[Rows, Cols];

    static void Main(string[] args)
    {
        InitializeGrid();
        PrintGrid();

        // Simulate fire spread
        while (true)
        {
            SimulateFireSpread();
            PrintGrid();
            Thread.Sleep(1000); // Pause for 1 second between steps
        }
    }

    // Initialize the grid with initial conditions
    static void InitializeGrid()
    {
        // Set all cells to unburned
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                grid[i, j] = CellState.Unburned;
                density[i, j] = 0; // Flat terrain by default
                vegetation[i, j] = 0; // Normal vegetation by default
            }
        }

        // Set initial fire in the center
        grid[Rows / 2, Cols / 2] = CellState.Burning;

        // Set vegetation (top half: dry grass, bottom half: wet soil)
        for (int i = 0; i < Rows / 2; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                vegetation[i, j] = 0.4; // Dry grass
            }
        }
        for (int i = Rows / 2; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                vegetation[i, j] = 0.4; // Dry grass
            }
        }

        for (int i = 20; i <= 30; i++)
        {
            for (int j = 30; j <= 35; j++)
            {
                vegetation[i, j] = -0.3;
                density[i, j] = -0.4;
            }
        }

    }

    // Simulate fire spread for one time step
    static void SimulateFireSpread()
    {
        CellState[,] newGrid = new CellState[Rows, Cols];
        Array.Copy(grid, newGrid, grid.Length);

        Random r = new Random();
        

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                if (grid[i, j] != CellState.Burning) continue;
                // Spread fire to neighbors
                double angle = -windDirection;

                for (int dir = 0; dir < 8; dir++, angle += Math.PI / 4.0)
                {

                    int ni = i + HowToMove[dir, 0];
                    int nj = j + HowToMove[dir, 1];

                    if (ni < 0 || ni >= Rows || nj < 0 || nj >= Cols || grid[ni,nj] == CellState.Burning
                        || grid[ni,nj] == CellState.Burned) continue;

                    double probability = CalculateSpreadProbability(i, j, angle);
                    double randomProb = r.NextDouble();

                    
                    if (randomProb < probability)
                    {
                        if(j == 30 && i >= 20 && i <= 30)
                        {
                            ;
                        }
                        newGrid[ni, nj] = CellState.Burning;
                    }
                }

                // Transition burning cell to burned after burning time
                newGrid[i, j] = CellState.Burned;
            }
        }

        grid = newGrid;
    }

    // Calculate the probability of fire spreading from (i,j) to (ni,nj)
    static double CalculateSpreadProbability(int i, int j, double angle)
    {
        double Pw, C1, C2, V, ft;
        double Pburn;
        C1 = 0.045; C2 = 0.131;
        V = 8;

        double exponent = Math.Exp(C1 * V);

        Pburn = baseProbability * (1 + density[i, j]) * (1 + vegetation[i, j]);

        ft = Math.Exp(V * C2 * (Math.Cos(angle) - 1));

        Pw = exponent * ft;

        Pburn *= Pw;
        Pburn = Math.Min(Pburn, 1.0);

        return Pburn;
    }

    // Print the grid to the console
    static void PrintGrid()
    {
        Console.Clear();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                char cellChar = '.';
                if (grid[i, j] == CellState.Unburned) cellChar = '.';
                else if (grid[i, j] == CellState.Burning) cellChar = '*';
                else if (grid[i, j] == CellState.Burned) cellChar = '#';
                if (i == 26 && j >= 24 && j <= 26)
                {
                    ;
                }
                Console.Write(cellChar);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}