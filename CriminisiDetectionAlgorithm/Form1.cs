using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        
    }
}
 