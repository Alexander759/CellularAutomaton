using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public enum Vegetation : sbyte
    {
        Water = -11,
        Rock = -10,
        Low = -3,
        Medium = 0,
        High = 4
    }

    public enum Density : sbyte
    {
        None = -10,
        Sparce = -4,
        Medium = 0,
        Dense = 3
    }

    public enum BurnState : byte
    {
        None,
        Fuel,
        Burning,
        Burnt
    }

    public struct Tile
    {
        public Tile(Vegetation vegetation, Density density, BurnState burnState)
        {
            Vegetation = (double)vegetation / 10.0;
            Density = (double)density / 10.0;
            BurnState = burnState;
        }

        public double Vegetation { get; private set; }
        public double Density { get; set; }
        public BurnState BurnState { get; set; }
    }
}
