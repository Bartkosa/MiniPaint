using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Resources;
using System.Drawing.Imaging;

namespace WinFormsPaint
{
    public partial class Form1 : Form
    {
        enum buttons
        {
            none=0,
            pen=1,
            rectangle=2,
            ellipse=3,
        }

        buttons currBut = buttons.none;
        Graphics g;
        Bitmap drawArea, temp;
        Pen pen=new Pen(Color.Black, 2);
        bool is_painting = false, is_right_click = false;
        int knownColorSize;
        PictureBox[] pictureBoxArray;
        Point p_start, p_end, p_size, p_curr;
        int fit;
            
        public Form1()
        {
            
            InitializeComponent();
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("aa");
            drawArea = new Bitmap(drawingArea.Width, drawingArea.Height);
            drawingArea.Image = drawArea;
            g = Graphics.FromImage(drawArea);
            g.Clear(Color.White);

            knownColorSize = Enum.GetNames(typeof(KnownColor)).Length;
            pictureBoxArray = new PictureBox[knownColorSize];
            initializePictureBox();

            
            
        }

        private void initializePictureBox()
        {
            fit = (boxesPanel.Width - SystemInformation.VerticalScrollBarWidth) / 30;
            if (fit == 0) fit = 1;
            int j=0, k=0;
            KnownColor color;
            for(int i=0; i<knownColorSize; i++ )
            {
                color = (KnownColor)(i + 1);
                pictureBoxArray[i] = new PictureBox();
                int xLoc = 1+30 * j;
                int yLoc = 1 + (30 * k);
                pictureBoxArray[i].Size = new Size(25, 25);
                pictureBoxArray[i].BackColor = Color.FromKnownColor(color);
                pictureBoxArray[i].Location = new Point(xLoc, yLoc);
                boxesPanel.Controls.Add(pictureBoxArray[i]);
                pictureBoxArray[i].Click += Form1_Click;
                
                if (j + 1 == fit)
                {
                    j = -1;
                    k++;
                }
                j++;
            }
        }
        
        private void Form1_Click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            pen.Color = p.BackColor;
            colorCurrent.BackColor = pen.Color;
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            temp = drawArea;
            drawArea = new Bitmap(drawingArea.Size.Width, drawingArea.Size.Height);
            drawingArea.Image = drawArea;
            g = Graphics.FromImage(drawArea);
            g.Clear(Color.White);
            g.DrawImage(temp, new Point(0, 0));
            
            int newFit = (boxesPanel.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth) / 30;
            if (newFit != fit)
            {
                boxesPanel.Controls.Clear();
                initializePictureBox();
            }

            

        }

        private void drawingArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                is_painting = true;
                p_start = e.Location;
                is_right_click = false;
            }
            else
                is_right_click = true;
            
        }

        private void drawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            
            p_end = e.Location;
            
            if (is_painting)
            {
                if (p_start.X < p_end.X)
                    p_curr.X = p_start.X;
                else
                    p_curr.X = p_end.X;
                if (p_start.Y < p_end.Y)
                    p_curr.Y = p_start.Y;
                else
                    p_curr.Y = p_end.Y;
                p_size.X = Math.Abs(p_end.X - p_start.X);
                p_size.Y = Math.Abs(p_end.Y - p_start.Y);

                if (currBut==buttons.pen)
                {
                    if (e.Button == MouseButtons.Right)
                        is_painting = false;
                    g.DrawLine(pen, p_end, p_start);
                    p_start = p_end;
                }
            }
            drawingArea.Refresh();
        }
        private void drawingArea_MouseUp(object sender, MouseEventArgs e)
        {
            is_painting = false;


            if (!is_right_click)
            {
                if (currBut == buttons.rectangle)
                {
                    g.DrawRectangle(pen, p_curr.X, p_curr.Y, p_size.X, p_size.Y);
                }
                if (currBut == buttons.ellipse)
                {
                    g.DrawEllipse(pen, p_curr.X, p_curr.Y, p_size.X, p_size.Y);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (currBut == buttons.pen)
            {
                currBut = buttons.none;
            }
            else
            {
                currBut = buttons.pen;
                toolStripButton1.Checked = true;
                rectangleButton.Checked = false;
                ellipseButton.Checked = false;
            }
        }

        

        private void rectangleButton_Click(object sender, EventArgs e)
        {
            if (currBut == buttons.rectangle)
            {
                currBut = buttons.none;
            }
            else
            {
                currBut = buttons.rectangle;
                toolStripButton1.Checked = false;
                rectangleButton.Checked = true;
                ellipseButton.Checked = false;
            }
        }
        private void ellipseButton_Click(object sender, EventArgs e)
        {
            if (currBut == buttons.ellipse)
            {
                currBut = buttons.none;
            }
            else
            {
                currBut = buttons.ellipse;
                toolStripButton1.Checked = false;
                rectangleButton.Checked = false;
                ellipseButton.Checked = true;
            }
        }

        private void thicknessChoose_Click(object sender, EventArgs e)
        {

        }


        public void SetCulture(string cultureName)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture =
               System.Globalization.CultureInfo.GetCultureInfo(cultureName);
            var resources = new System.ComponentModel.ComponentResourceManager(this.GetType());
            GetChildren(this).ToList().ForEach(c => {
                resources.ApplyResources(c, c.Name);
            });
        }
        public IEnumerable<Control> GetChildren(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetChildren(ctrl)).Concat(controls);
        }
        private void polishButton_Click(object sender, EventArgs e)
        {
            var plCulture = new CultureInfo("pl-PL");

            Thread.CurrentThread.CurrentCulture = plCulture;
            Thread.CurrentThread.CurrentUICulture = plCulture;
            foreach (Control item in Form1.ActiveForm.Controls.OfType<ToolStrip>())
            {
                Form1.ActiveForm.Controls.Remove(item);
            }
            Form1.ActiveForm.Controls.Remove(groupBox1);
            InitializeComponent();

            temp = drawArea;
            drawArea = new Bitmap(drawingArea.Size.Width, drawingArea.Size.Height);
            drawingArea.Image = drawArea;
            g = Graphics.FromImage(drawArea);

            g.Clear(Color.White);
            g.DrawImage(temp, new Point(0, 0));

            //Bitmap drawArea = new Bitmap(drawingArea.Width, drawingArea.Height);
            //drawingArea.Image = drawArea;
            //g = Graphics.FromImage(drawArea);
            //g.Clear(Color.White);

            knownColorSize = Enum.GetNames(typeof(KnownColor)).Length;
            pictureBoxArray = new PictureBox[knownColorSize];
            initializePictureBox();


        }

        private void englishButton_Click(object sender, EventArgs e)
        {
            var usCulture = new CultureInfo("en_US");

            Thread.CurrentThread.CurrentCulture = usCulture;
            Thread.CurrentThread.CurrentUICulture = usCulture;
            foreach (Control item in Form1.ActiveForm.Controls.OfType<ToolStrip>())
            {
                Form1.ActiveForm.Controls.Remove(item);
            }
            foreach (Control item in Form1.ActiveForm.Controls.OfType<GroupBox>())
            {
                Form1.ActiveForm.Controls.Remove(item);
            }
                InitializeComponent();
            temp = drawArea;
            drawArea = new Bitmap(drawingArea.Size.Width, drawingArea.Size.Height);
            drawingArea.Image = drawArea;
            g = Graphics.FromImage(drawArea);

            g.Clear(Color.White);
            g.DrawImage(temp, new Point(0, 0));


        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG Image|.png|Bitmap Image|.bmp|JPEG Image|*.jpeg";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(drawingArea.Width);
                int height = Convert.ToInt32(drawingArea.Height);
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    drawingArea.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                    bmp.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "bmp files (*.bmp)|*.bmp";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    
                    drawArea = new Bitmap(dlg.FileName);
                    drawingArea.Image = drawArea;
                    
                }
            }
            this.Size = new Size(drawArea.Width+groupBox1.Width+39, drawArea.Height+88);
        }

        private void thicknessChoose_SelectedIndexChanged(object sender, EventArgs e)
        {
            pen.Width = int.Parse(thicknessChoose.Text);
        }

        
        private void trash_Click(object sender, EventArgs e)
        {
            currBut = buttons.none;
            g = Graphics.FromImage(drawArea);
            g.Clear(Color.White);
            drawingArea.Invalidate();
        }
        private void drawingArea_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            

            if(is_painting)
            {
                if (currBut==buttons.rectangle)
                {
                        g.DrawRectangle(pen, p_curr.X, p_curr.Y, p_size.X, p_size.Y);

                }
                if (currBut==buttons.ellipse)
                {
                    g.DrawEllipse(pen, p_curr.X, p_curr.Y, p_size.X, p_size.Y);
                }
            }


        }

        

        

        

       
    }
}
