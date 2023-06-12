using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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

        public byte[,] SubtractArrays(byte[,] arr1, byte[,] arr2)
        {
            int rows = arr1.GetLength(0);
            int columns = arr1.GetLength(1);

            byte[,] result = new byte[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = (byte)(arr1[i, j] - arr2[i, j]);
                }
            }

            return result;
        }


        public BlockStructure Subtract(BlockStructure secondBlockStructure)
        {
            return new BlockStructure
            {
                MatR = SubtractArrays(secondBlockStructure.MatR, this.MatR),
                MatG = SubtractArrays(secondBlockStructure.MatG, this.MatG),
                MatB = SubtractArrays(secondBlockStructure.MatB, this.MatB),
                X = this.X,
                Y = this.Y
            };

        }

    }

}
