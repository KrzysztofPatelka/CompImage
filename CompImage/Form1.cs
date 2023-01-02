using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spire.Pdf;

namespace CompImage
{
    public partial class Form1 : Form
    {
        public static Task task;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog1.Filter = "plik PDF|*.pdf";
                openFileDialog1.DefaultExt = "pdf";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    PdfDocument pdf = new PdfDocument();
                    pdf.LoadFromFile(openFileDialog1.FileName);
                    int i = 0;
                    foreach (PdfPageBase page in pdf.Pages)
                    {
                        foreach (Image image in page.ExtractImages())
                        {
                            i++;
                            image.Save(Directory.GetCurrentDirectory() + "\\oryginal\\" + Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + "-" + i + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    MessageBox.Show("Zapisano " + i + " pliki o nazwach rozpoczynajacyh się od " + Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + "-... w lokalizacji " + Directory.GetCurrentDirectory() + "\\oryginal\\", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                button2.Enabled = false;
                button3.Enabled = true;
                listBox1.Items.Clear();
                if (System.IO.Directory.Exists(folderBrowserDialog1.SelectedPath))
                {
                    foreach (string file in System.IO.Directory.GetFiles(folderBrowserDialog1.SelectedPath))
                    {
                        //if (Path.GetFileName(file).Substring(0, 1) == "P")
                        //{
                        //    listBox1.Items.Add(file);
                        //}
                        if (Path.GetExtension(file).ToLower() == ".pdf")
                        {
                            listBox1.Items.Add(file);
                        }
                    }
                }
                label1.Text = "Suma: " + listBox1.Items.Count;
                label2.Text = "0 z " + listBox1.Items.Count;
                progressBar1.Maximum = listBox1.Items.Count;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*button2.Enabled = false;
            button3.Enabled = false;
            richTextBox1.Clear();
            string fileError = "";
            int index = 0;
            PdfDocument pdf = new PdfDocument();
            try
            {
                foreach (string file in listBox1.Items)
                {
                    pdf.LoadFromFile(file);
                    int i = 1;
                    foreach (PdfPageBase page in pdf.Pages)
                    {
                        foreach (Image image in page.ExtractImages())
                        {
                            image.Save(Directory.GetCurrentDirectory() + "\\tmp\\" + Path.GetFileNameWithoutExtension(file) + "_" + i + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            if (ImageCompareArray(new Bitmap(Directory.GetCurrentDirectory() + "\\oryginal.png"), new Bitmap(Directory.GetCurrentDirectory() + "\\tmp\\" + Path.GetFileNameWithoutExtension(file) + "_" + i + ".png")))
                            {
                                //MessageBox.Show("Podpis jest OK", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                //MessageBox.Show("Podpis jest błędny, sprawdź plik...", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                if (fileError != Path.GetFileName(file))
                                {
                                    richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(file));
                                    fileError = Path.GetFileName(file);
                                }
                            }
                            i += 1;
                        }
                    }
                    index += 1;
                    progressBar1.Value += 1;
                    label2.Text = index + " z " + listBox1.Items.Count;
                }
                MessageBox.Show("Podpisy zostały sprawdzone.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\oryginal\\";
            openFileDialog1.Filter = "plik PNG|*.png";
            openFileDialog1.DefaultExt = "png";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                listBox2.Items.Clear();
                Task.Run(() =>
                {
                    string fileError = "";
                    int index = 0;
                    PdfDocument pdf = new PdfDocument();
                    try
                    {
                        foreach (string file in listBox1.Items)
                        {
                            pdf.LoadFromFile(file);
                            int i = 1;
                            foreach (PdfPageBase page in pdf.Pages)
                            {
                                foreach (Image image in page.ExtractImages())
                                {
                                    if (ImageCompareArray(new Bitmap(openFileDialog1.FileName), new Bitmap(image)))
                                    {
                                        //MessageBox.Show("Podpis jest OK", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        if (fileError != Path.GetFileName(file))
                                        {
                                            Invoke(new Action(delegate () {
                                                listBox2.Items.Add(Path.GetFileName(file));
                                            }));
                                            fileError = Path.GetFileName(file);
                                        }
                                    }
                                    i += 1;
                                }
                            }
                            index += 1;
                            Invoke(new Action(delegate () {
                                progressBar1.Value += 1;
                                label2.Text = index + " z " + listBox1.Items.Count;
                            }));
                        }
                        MessageBox.Show("Podpisy zostały sprawdzone.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
        }

        private bool ImageCompareArray(Bitmap firstImage, Bitmap secondImage)
        {
            bool flag = true;
            if (firstImage.Width == secondImage.Width && firstImage.Height == secondImage.Height)
            {
                for (int i = 0; i < firstImage.Width; i++)
                {
                    for (int j = 0; j < firstImage.Height; j++)
                    {
                        if (firstImage.GetPixel(i, j).ToString() != secondImage.GetPixel(i, j).ToString())
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
