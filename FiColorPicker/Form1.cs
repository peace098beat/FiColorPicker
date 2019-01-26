using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FiColorPicker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int CaptureSize = 50;
        int Zoom = 4;
        int PictureBoxSize = 75;

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(PictureBoxSize, PictureBoxSize);
            pictureBox2.Size = new Size(PictureBoxSize, PictureBoxSize);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            X = X - CaptureSize / 2;
            Y = Y - CaptureSize / 2;

            // Screen Copy
            Bitmap bmp = new Bitmap(CaptureSize, CaptureSize);
            using (Graphics gg = Graphics.FromImage(bmp))
            {
                gg.CopyFromScreen(new Point(X, Y), new Point(0, 0), bmp.Size);
            }
            this.pictureBox1.Image = bmp;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.Refresh();

            // 画像処理
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                ImageLockMode.ReadOnly,
                                                PixelFormat.Format32bppArgb);

            int r = 0; int g = 0; int b = 0; int a = 0;
            byte[] buf = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);
            double N = (double)buf.Length / 4.0;
            for (int i = 0; i < buf.Length;)
            {
                b += (int)buf[i++];
                g += (int)buf[i++];
                r += (int)buf[i++];
                a += (int)buf[i++];
            }

            r = (int)(r / N);
            g = (int)(g / N);
            b = (int)(b / N);
            a = 1;

            bmp.UnlockBits(data);
            Color ave_color = Color.FromArgb(r, g, b);

            this.pictureBox1_Ave.BackColor = ave_color;

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            X = X - CaptureSize / 2 / Zoom;
            Y = Y - CaptureSize / 2 / Zoom;

            // Screen Copy
            Bitmap bmp = new Bitmap(CaptureSize / Zoom, CaptureSize / Zoom);
            using (Graphics gg = Graphics.FromImage(bmp))
            {
                gg.CopyFromScreen(new Point(X, Y), new Point(0, 0), bmp.Size);
            }
            this.pictureBox2.Image = bmp;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Refresh();

            // Center Color
            Color CenterColor = bmp.GetPixel(bmp.Width / 2, bmp.Height / 2);
            var H = CenterColor.GetHue();
            var S = CenterColor.GetSaturation();
            var B = CenterColor.GetBrightness();

            this.Text = CenterColor.ToString();

            // 画像処理
            pictureBox2_Ave.BackColor = CenterColor;


            //現在どのマウスボタンが押されているか調べる
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if ((Control.MouseButtons & MouseButtons.Middle) == MouseButtons.Middle)
                {
                    Console.WriteLine("マウスの中央ボタンが押されています。" + Cursor.Position.ToString());

                    pictureBox_PickColor.BackColor = CenterColor;
                    label_PickColor.Text = CenterColor.ToString();
                    label_PickColor.Text += String.Format("{0},{1},{2}", H, S, B);
                }
                if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                {
                    Console.WriteLine("マウスの左ボタンが押されています。" + Cursor.Position.ToString());
                }
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Text = e.Location.ToString();
        }
    }
}
