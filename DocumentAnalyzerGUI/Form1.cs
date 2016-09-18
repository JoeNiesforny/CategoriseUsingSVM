﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CategoriseUsingSVM;

namespace DocumentAnalyzerGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();

            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(openFolder.SelectedPath);

                var dictionary = new string[] { "Training", "SVM", "neural", "network", "quadratic", "optimization", "linear" };
                var analyzer = new DocumentAnalyzer(dictionary);
                foreach (var file in files)
                {
                    analyzer.AddNewDocument(file);
                }

                foreach (var document in analyzer.Documents)
                {
                    var point = document.Position;
                    chart1.Series["Documents"].Points.AddXY(point.X, point.Y);
                }

            }


            
        }

        private void HTMLToTXTButton_Click(object sender, EventArgs e)
        {
            var toTxt = new HTMLtoTXT();
            FolderBrowserDialog openFolder = new FolderBrowserDialog();

            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(openFolder.SelectedPath);

                new FileInfo(openFolder.SelectedPath + "\\txt\\").Directory.Create();

                foreach (var file in files)
                {
                    var text = toTxt.Convert(file);
                    StreamWriter saveFile = new StreamWriter(openFolder.SelectedPath + "\\txt\\" + Path.GetFileNameWithoutExtension(file) + ".txt");

                    saveFile.WriteLine(text);

                    saveFile.Close();
                }

            }
        }
    }
}
