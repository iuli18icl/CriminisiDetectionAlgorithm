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
using static Emgu.CV.Structure.MCvMatND;
using System.Xml.Linq;
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

        public Image image;

        public void LoadImageFromFile(PictureBox pictureBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                image = Image.FromFile(selectedFile);

                pictureBox.Image = image;
            }
        }

        public static List<BlockStructure> DivideImageIntoBlocks(byte[,,] imageStructure, int blockSize, int stepSize)
        {
            int width = imageStructure.GetLength(1);
            int height = imageStructure.GetLength(0);

            List<BlockStructure> blocksList = new List<BlockStructure>();

            for (int i = 0; i <= height - blockSize; i += stepSize)
            {
                for (int j = 0; j <= width - blockSize; j += stepSize)
                {
                    byte[,] blockR = new byte[blockSize, blockSize];
                    byte[,] blockG = new byte[blockSize, blockSize];
                    byte[,] blockB = new byte[blockSize, blockSize];

                    for (int y = 0; y < blockSize; y++)
                    {
                        for (int x = 0; x < blockSize; x++)
                        {
                            blockR[y, x] = imageStructure[i + y, j + x, 0]; // Red channel
                            blockG[y, x] = imageStructure[i + y, j + x, 1]; // Green channel
                            blockB[y, x] = imageStructure[i + y, j + x, 2]; // Blue channel
                        }
                    }

                    BlockStructure blockStructure = new BlockStructure
                    {
                        MatR = blockR,
                        MatG = blockG,
                        MatB = blockB,
                        X = j,
                        Y = i
                    };

                    blocksList.Add(blockStructure);
                }
            }

            return blocksList;
        }

        public static byte[,,] ConvertImageToByteArray(Image image)
        {
            Bitmap bitmap = new Bitmap(image);
            int width = bitmap.Width;
            int height = bitmap.Height;

            byte[,,] imageArray = new byte[height, width, 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    imageArray[y, x, 0] = pixelColor.R;
                    imageArray[y, x, 1] = pixelColor.G;
                    imageArray[y, x, 2] = pixelColor.B;
                }
            }

            return imageArray;
        }

        // lista blocuri din ROS
        public static List<BlockStructure> DivideROSIntoBlocks(byte[,,] image, int blockSize, int startX, int startY, int rosWidth, int rosHeight, int stepSize)
        {
            List<BlockStructure> ROSblocksList = new List<BlockStructure>();

            int width = image.GetLength(1);
            int height = image.GetLength(0);

            rosWidth = Math.Min(rosWidth, width - startX);
            rosHeight = Math.Min(rosHeight, height - startY);

            int endX = startX + rosWidth;
            int endY = startY + rosHeight;

            for (int i = startX; i <= endX - blockSize; i += stepSize)
            {
                for (int j = startY; j <= endY - blockSize; j += stepSize)
                {
                    byte[,] blockR = new byte[blockSize, blockSize];
                    byte[,] blockG = new byte[blockSize, blockSize];
                    byte[,] blockB = new byte[blockSize, blockSize];

                    for (int x = 0; x < blockSize; x++)
                    {
                        for (int y = 0; y < blockSize; y++)
                        {
                            blockR[x, y] = image[i + x, j + y, 0]; // Red channel
                            blockG[x, y] = image[i + x, j + y, 1]; // Green channel
                            blockB[x, y] = image[i + x, j + y, 2]; // Blue channel
                        }
                    }

                    BlockStructure blockStructure = new BlockStructure
                    {
                        MatR = blockR,
                        MatG = blockG,
                        MatB = blockB,
                        X = j,
                        Y = i
                    };

                    ROSblocksList.Add(blockStructure);
                }
            }

            return ROSblocksList;
        }

        public static List<byte[,]> BlockStructuresToBytes(List<BlockStructure> blockStructures)
        {
            List<byte[,]> bytes = new List<byte[,]>();

            foreach (BlockStructure blockStructure in blockStructures)
            {
                bytes.Add(blockStructure.MatR);
            }

            return bytes;
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


                    //int blockSize = int.Parse(textBox1.Text);
                    //int stepSize = int.Parse(textBox2.Text);
                    //int startX = int.Parse(textBox3.Text);
                    //int startY = int.Parse(textBox4.Text);
                    //int rosWidth = int.Parse(textBox5.Text);
                    //int rosHeight = int.Parse(textBox6.Text);
                    //int limit = int.Parse(textBox7.Text);

                    int blockSize = 5;
                    int stepSize = 5;
                    int startX = 100;
                    int startY = 200;
                    int rosWidth = width / 2;
                    int rosHeight = height / 2;

                    //Rectangle first = new Rectangle(1, 1, 5, 5);
                    //Rectangle second = new Rectangle(4, 7, 3, 3);
                    //bool rez = CheckOverlap(first, second);
                    //Console.WriteLine(rez);


                    byte[,,] imageArray = ConvertImageToByteArray(image);

                    List<BlockStructure> imageBlocks = DivideImageIntoBlocks(imageArray, blockSize, stepSize);

                    List<BlockStructure> rosBlocks = DivideROSIntoBlocks(imageArray, blockSize, startX, startY, rosWidth, rosHeight, stepSize);

                    foreach (BlockStructure rosBlock in rosBlocks)
                    {
                        foreach (BlockStructure imageBlock in imageBlocks)
                        {
                            if (!CheckOverlap(BlockToRectangle(rosBlock), BlockToRectangle(imageBlock)))
                            {
                                BlockStructure differenceBlock = imageBlock.Subtract(rosBlock);  //differenceBlock.MatR, differenceBlock.MarG, differenceBlock.MarB

                            }
                        }
                    }

                }
            }
        }


        //functie pt a transforma din <byte[,]> in <rectangle>
        public static Rectangle BlockToRectangle(BlockStructure block)
        {

            return new Rectangle(block.X, block.Y, block.Width, block.Height);
        }


        // functii pt overlapping
        //public static bool AreaOverlapping(List<Rectangle> rectanglesList1, List<Rectangle> rectanglesList2)
        //{

        //    foreach (Rectangle rectangleROS in rectanglesList1)
        //    {
        //        foreach (Rectangle rectangleIMG in rectanglesList2)
        //        {
        //            if (CheckOverlap(rectangleROS, rectangleIMG))
        //                return true;
        //        }
        //    }

        //    return false;
        //}

        public static bool CheckOverlap(Rectangle blockA, Rectangle blockB)
        {
            return blockA.IntersectsWith(blockB);
        }


        //obtinem diferenta dintre toate blocurile
        public static List<byte[,]> DifferenceList(List<byte[,]> blocksList1, List<byte[,]> blocksList2, int blockSize)
        {
            List<byte[,]> differenceList = new List<byte[,]>();

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

                    differenceList.Add(diferenta);

                }
            }

            return differenceList;
        }

        //binarizam matricile diferenta in functie de prag (=lim)
        public static List<byte[,]> FinalList(List<byte[,]> differenceMatrix, int blockSize, int limit)
        {
            List<byte[,]> finalList = new List<byte[,]>();
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

                finalList.Add(diffMatrix);
            }

            return finalList;
        }



        public void Form1_Load(object sender, EventArgs e)
        {

        }
        public void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void loadImage_Click(object sender, EventArgs e)
        {
             LoadImageFromFile(pictureBox1);
        }

       
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void loadMask_Click(object sender, EventArgs e)
        {
            LoadImageFromFile(pictureBox2);
        }


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

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private bool isANDChecked = false;
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            isANDChecked = checkBox3.Checked;
        }

        private bool isORChecked = false;
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            isORChecked = checkBox4.Checked;
        }
    }
}
 