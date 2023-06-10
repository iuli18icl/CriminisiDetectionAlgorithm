using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CriminisiDetectionAlgorithm
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
  
        private void loadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                Image image = Image.FromFile(selectedFile);

                pictureBox1.Image = image;
            }

            ////image structure => in clasa image structure
            
            //Bitmap loadedImage = new Bitmap(pictureBox1.Image);

            //int width = loadedImage.Width;
            //int height = loadedImage.Height;

            //int[,] redMat = new int[width, height];
            //int[,] greenMat = new int[width, height];
            //int[,] blueMat = new int[width, height];



            ////read each pixel from image (RGB) => in image processed
            
            //if (pictureBox1.Image != null)
            //{

            //    for (int i = 0; i < height; i++)
            //    {
            //        for (int j = 0; j < width; j++)
            //        {
            //            Color pixelColor = loadedImage.GetPixel(i, j);

            //            redMat[i, j] = pixelColor.R;
            //            greenMat[i, j] = pixelColor.G;
            //            blueMat[i, j] = pixelColor.B;

            //            Console.Write($"R={redMat}, G={greenMat}, B={blueMat}\t");
            //        }
            //        Console.WriteLine();
            //    }
            //}


            //    //dimensiune bloc
            //    string size = textBox1.Text;
            //    int blockSize = int.Parse(size);

            //    int blockWidth = width;
            //    int blockHeight = height;


            //    int[,] redBlock = new int[blockWidth, blockHeight];
            //    int[,] greenBlock = new int[blockWidth, blockHeight];
            //    int[,] blueBlock = new int[blockWidth, blockHeight];

            //    void ProcessBlock(Bitmap blockImage)
            //    {

            //        for (int y = 0; y < blockImage.Height; y++)
            //        {
            //            for (int x = 0; x < blockImage.Width; x++)
            //            {
            //                Color pixelColor = blockImage.GetPixel(x, y);

            //                redBlock[x, y] = pixelColor.R;
            //                greenBlock[x, y] = pixelColor.G;
            //                blueBlock[x, y] = pixelColor.B;

            //                Console.Write($"R={redBlock}, G={greenBlock}, B={blueBlock}\t");
            //            }
            //            Console.WriteLine();
            //        }
            //    }

            //    for (int i = 0; i < blockHeight; i++)
            //    {
            //        for (int j = 0; j < blockWidth; j++)
            //        {
            //            Rectangle block = new Rectangle(j*blockSize, i*blockSize, blockSize, blockSize);
            //            Bitmap blockImage = loadedImage.Clone(block, loadedImage.PixelFormat);

            //            ProcessBlock(blockImage);
            //        }
            //    }

        }


        public void Form1_Load(object sender, EventArgs e)
        {

        }

        public void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int stepSize = int.Parse(textBox1.Text);         //?????????????????????
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void step_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void startX_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void startY_TextChanged(object sender, EventArgs e)
        {
        }

        private void widthVal_TextChanged(object sender, EventArgs e)
        {
        }

        private void heightVal_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Add("RGB");
            comboBox1.Items.Add("Grayscale");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                Image image = Image.FromFile(selectedFile);

                pictureBox2.Image = image;
            }
        }

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


            for (int i = startX; i <= endX; i = +stepSize)
            {
                for (int j = startY; j <= endY; j = +stepSize)
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
                            diffMatrix[x, y] = 255;
                        else
                            diffMatrix[x, y] = 0; ;
                    }
                }

                SimilarMatrix.Add(diffMatrix);
            }

            return SimilarMatrix;
        }


    

        private void Compute_Click(object sender, EventArgs e)
        {

        }
    }
}
 