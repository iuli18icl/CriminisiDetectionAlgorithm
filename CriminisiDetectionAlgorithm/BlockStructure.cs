using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriminisiDetectionAlgorithm
{
    public class BlockStructure
    {
        // Matrix for each color channel
        public byte[,] MatR { get; set; }
        public byte[,] MatG { get; set; }
        public byte[,] MatB { get; set; }

        // Address of the block
        public int X { get; set; }
        public int Y { get; set; }

        // Width and Height properties
        public int Width => MatR.GetLength(1);
        public int Height => MatR.GetLength(0);

        public override string ToString()
        {
            return string.Format("X: [{0}] and Y: [{1}].", X, Y);

        }

    }

}
