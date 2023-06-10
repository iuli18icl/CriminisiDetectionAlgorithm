using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CriminisiDetectionAlgorithm
{
    internal class ImageProcessed
    {
        public static ImageStructure imageLoaded()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png)|*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;

                Bitmap bitmap = new Bitmap(selectedFile);

                int width = bitmap.Width;
                int height = bitmap.Height;

                ImageStructure imageStructure = new ImageStructure()
                {
                    matR = new byte[height, width],
                    matG = new byte[height, width],
                    matB = new byte[height, width]
                };

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        Color color = bitmap.GetPixel(j, i);
                        imageStructure.matR[i, j] = color.R;
                        imageStructure.matG[i, j] = color.G;
                        imageStructure.matB[i, j] = color.B;
                    }
                }

                return imageStructure;

            }
            else

                return null;

        }

        //functia trb sa ia valorile date in text box-uri
        // Lista blocuri din toata imaginea
        public static List<byte[,]> DivideImageIntoBlocks(byte[,] imageStructure, int blockSize, int stepSize)
        {
            int width = imageStructure.GetLength(1);
            int height = imageStructure.GetLength(0);

            List<byte[,]> IMGblocksList = new List<byte[,]>();

            for (int i = 0; i <= height - blockSize; i += stepSize)
            {
                for (int j = 0; j <= width - blockSize; j += stepSize)
                {
                    byte[,] block = new byte[blockSize, blockSize];

                    for (int y = 0; y < blockSize; y++)
                    {
                        for (int x = 0; x < blockSize; x++)
                        {
                            block[y, x] = imageStructure[i + y, j + x];
                        }
                    }

                    IMGblocksList.Add(block);
                }
            }

            return IMGblocksList;
        }


        //functia trb sa ia valorile date in text box-uri
        // lista blocuri din ROS
        public static List<byte[,]> DivideROSIntoBlocks(byte[,] image, int blockSize, int startX, int startY, int rosWidth, int rosHeight, int stepSize)
        {
            List<byte[,]> ROSblocksList = new List<byte[,]>();

            int width = image.GetLength(1);
            int height = image.GetLength(0);

            rosWidth = Math.Min(rosWidth, width - startX);
            rosHeight = Math.Min(rosHeight, height - startY);

            int endX = startX + rosHeight;
            int endY = startY + rosWidth;


            for (int i = startX; i <= endX; i =+ stepSize)
            {
                for (int j = startY; j <= endY; j =+ stepSize)
                {
                    byte[,] block = new byte[blockSize, blockSize];

                    for (int x = 0; x < blockSize; x++)
                    {
                        for (int y = 0; y < blockSize; y++)
                        {
                            block[x, y] = image[i + x, j + y];
                        }
                    }

                    ROSblocksList.Add(block);
                }
            }

            return ROSblocksList;
        }

        // transformam din <byte[,]> in <rectangle>
        public static List<Rectangle> ToRectangles(List<byte[,]> blocks)
        {
            List<Rectangle> rectanglesList = new List<Rectangle>();

            foreach (byte[,] block in blocks)
            {
                int blockWidth = block.GetLength(1);
                int blockHeight = block.GetLength(0);

                Rectangle rectangle = new Rectangle(0, 0, blockWidth, blockHeight);

                rectanglesList.Add(rectangle);
            }

            return rectanglesList;
        }

        // verificam overlapping
        public static bool AreaOverlapping(List<Rectangle> rectanglesList)
        {
            int count = rectanglesList.Count;

            for (int i = 0; i < count - 1; i++)
            {
                Rectangle rectangleA = rectanglesList[i];

                for (int j = i + 1; j < count; j++)
                {
                    Rectangle rectangleB = rectanglesList[j];

                    if (CheckOverlap(rectangleA, rectangleB))
                        return true;
                }
            }

            return false;
        }

        private static bool CheckOverlap(Rectangle blockA, Rectangle blockB)
        {
            return blockA.IntersectsWith(blockB);
        }

        private static int GetMatrixSum(byte[,] matrix)
        {
            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);

            int sum = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sum += matrix[i, j];
                }
            }

            return sum;
        }

        //obtinem diferenta dintre toate blocurile
        public static List<byte[,]> DifferenceMatrix(List<byte[,]> blocksList1, List<byte[,]> blocksList2, int blockSize, int limit)
        {
            //diferenta dintre matrici
            List<byte[,]> differenceMatrix = new List<byte[,]>();

            foreach (byte[,] block1 in blocksList1)
            {
                foreach (byte[,] block2 in blocksList2)
                {
                    byte[,] diferenta = new byte[blockSize, blockSize];

                    for (int x = 0; x < blockSize; x++)
                    {
                        for (int y = 0; y < blockSize; y++)
                        {
                            diferenta[x, y] = (byte)(block1[x, y] - block2[x, y]);
                            diferenta[x, y] = (byte)Math.Abs(diferenta[x, y]);
                        }
                    }

                    differenceMatrix.Add(diferenta);

                }
            }

            return differenceMatrix;
        }

        //binarizam matricile diferenta in functie de prag (=lim)
        public static List<byte[,]> SimilarMatrix(List<byte[,]> differenceMatrix, int blockSize, int limit)
        {
            List<byte[,]> SimilarMatrix = new List<byte[,]>();

            foreach (byte[,] diffMatrix in differenceMatrix)
            {
                byte lim = Convert.ToByte(limit);

                for (int x = 0; x < blockSize; x++)
                {
                    for (int y = 0; y < blockSize; y++)
                    {
                        if (diffMatrix[x, y] <= lim)
                            diffMatrix[x, y] = 255;    //(1)
                        else
                            diffMatrix[x, y] = 0; ;
                    }
                }

                SimilarMatrix.Add(diffMatrix);  
            }

            return SimilarMatrix;
        }

        
    }
}
