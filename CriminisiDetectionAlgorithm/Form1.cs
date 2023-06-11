using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;


namespace CriminisiDetectionAlgorithm
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private Image image;

        public Image LoadImageFromFile(PictureBox pictureBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                Image image = Image.FromFile(selectedFile);

                pictureBox.Image = image;
                return image;
            }

            return null;
        }

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

        //functie pt a transforma din <byte[,]> in <rectangle>
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


        // functii pt overlapping
        public static bool AreaOverlapping(List<Rectangle> rectanglesList1, List<Rectangle> rectanglesList2)
        {

            foreach (Rectangle rectangleROS in rectanglesList1)
            {
                foreach (Rectangle rectangleIMG in rectanglesList2)
                {
                    if (CheckOverlap(rectangleROS, rectangleIMG))
                        return true;
                }
            }

            return false;
        }

        private static bool CheckOverlap(Rectangle blockA, Rectangle blockB)
        {
            return blockA.IntersectsWith(blockB);
        }


        //obtinem diferenta dintre toate blocurile
        public static List<byte[,]> DifferenceMatrix(List<byte[,]> blocksList1, List<byte[,]> blocksList2, int blockSize)
        {
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



        public void Form1_Load(object sender, EventArgs e)
        {

        }
        public void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void loadImage_Click(object sender, EventArgs e)
        {
            Image image = LoadImageFromFile(pictureBox1);
        }

        public void Compute_Click(object sender, EventArgs e)
        {
            if (isRGBChecked)
            {
                if (image != null)
                {
                    Bitmap bitmap = new Bitmap(image);

                    int width = bitmap.Width;
                    int height = bitmap.Height;

                    byte[,] redMatrix = new byte[width, height];
                    byte[,] greenMatrix = new byte[width, height];
                    byte[,] blueMatrix = new byte[width, height];

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color pixelColor = bitmap.GetPixel(x, y);
                            redMatrix[x, y] = pixelColor.R;
                            greenMatrix[x, y] = pixelColor.G;
                            blueMatrix[x, y] = pixelColor.B;
                        }
                    }

                    int blockSize = int.Parse(textBox1.Text);
                    int stepSize = int.Parse(textBox2.Text);
                    int startX = int.Parse(textBox3.Text);
                    int startY = int.Parse(textBox4.Text);
                    int rosWidth = int.Parse(textBox5.Text);
                    int rosHeight = int.Parse(textBox6.Text);

                    //Liste blocuri din imagine
                    List<byte[,]> redBlocksFromIMG = DivideImageIntoBlocks(redMatrix, blockSize, stepSize);
                    List<byte[,]> greenBlocksFromIMG = DivideImageIntoBlocks(greenMatrix, blockSize, stepSize);
                    List<byte[,]> blueBlocksFromIMG = DivideImageIntoBlocks(blueMatrix, blockSize, stepSize);

                    //Liste blocuri din ROS
                    List<byte[,]> redROSBlocks = DivideROSIntoBlocks(redMatrix, blockSize, startX, startY, rosWidth, rosHeight, stepSize);
                    List<byte[,]> greenROSBlocks = DivideROSIntoBlocks(greenMatrix, blockSize, startX, startY, rosWidth, rosHeight, stepSize);
                    List<byte[,]> blueROSBlocks = DivideROSIntoBlocks(blueMatrix, blockSize, startX, startY, rosWidth, rosHeight, stepSize);

                    //Liste blocuri din imagine -> to Rectangle
                    List<Rectangle> redIMGrectangles = ToRectangles(redBlocksFromIMG);
                    List<Rectangle> greenIMGrectangles = ToRectangles(greenBlocksFromIMG);
                    List<Rectangle> blueIMGrectangles = ToRectangles(blueBlocksFromIMG);

                    //lista cu listele din IMG totala (3 liste: R G B)
                    //List<List<Rectangle>> totalListIMG = new List<List<Rectangle>>();
                    //totalListIMG.Add(redIMGrectangles);
                    //totalListIMG.Add(greenIMGrectangles);
                    //totalListIMG.Add(blueIMGrectangles);

                    //Liste blocuri din ROS -> to Rectangle
                    List<Rectangle> redROSrectangles = ToRectangles(redROSBlocks);
                    List<Rectangle> greenROSrectangles = ToRectangles(greenROSBlocks);
                    List<Rectangle> blueROSrectangles = ToRectangles(blueROSBlocks);

                    //lista cu listele din ROI (3 liste: R G B)
                    List<List<Rectangle>> totalListROS = new List<List<Rectangle>>();
                    totalListROS.Add(redROSrectangles);
                    totalListROS.Add(greenROSrectangles);
                    totalListROS.Add(blueROSrectangles);

                    foreach (List<Rectangle> listaROI in totalListROS)
                    {
                        if (listaROI == redROSrectangles)
                        {
                            if (!AreaOverlapping(listaROI, redROSrectangles))
                            {
                                List<byte[,]> diffMatrix = DifferenceMatrix(redROSBlocks, redBlocksFromIMG, blockSize);
                            }
                        }

                    }

                    foreach (byte[,] matrix in diffMatrix)
                    {
                        int width = matrix.GetLength(1);
                        int height = matrix.GetLength(0);

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                byte value = matrix[y, x];

                            }
                        }
                    }
                }
            }

            //else if (isGrayscaleChecked == true)
            //{

            //}
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void loadMask_Click(object sender, EventArgs e)
        {
            LoadImageFromFile(pictureBox2);
        }


       

        //private static int GetMatrixSum(byte[,] matrix)
        //{
        //    int height = matrix.GetLength(0);
        //    int width = matrix.GetLength(1);

        //    int sum = 0;

        //    for (int i = 0; i < height; i++)
        //    {
        //        for (int j = 0; j < width; j++)
        //        {
        //            sum += matrix[i, j];
        //        }
        //    }

        //    return sum;
        //}

        private bool isRGBChecked = false;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isRGBChecked = checkBox1.Checked;
        }

        private bool isGrayscaleChecked = false;

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            isGrayscaleChecked = checkBox2.Checked;
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
 