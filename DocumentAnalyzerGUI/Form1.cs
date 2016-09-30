using System;
using System.IO;
using System.Linq;
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
                string[] files = GetAllFilesFromRecursivly(openFolder.SelectedPath);
                var dictionary = new string[] { "Politechnika", "Gdańsk", "studia", "stypendia", "stypendium", "zawody", "władze", "Władze", "wydział", "Wydziały" };
                var analyzer = new DocumentAnalyzer(dictionary);
                foreach (var file in files)
                    analyzer.AddNewDocument(file);
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

        public static string[] GetAllFilesFromRecursivly(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] newFiles = GetAllFilesFromRecursivly(subdirectory);
                string[] temp = fileEntries;
                fileEntries = new string[temp.Count() + newFiles.Count()];
                for (int i = 0; i < temp.Count() + newFiles.Count(); i++)
                    if (i < temp.Count())
                        fileEntries[i] = temp[i];
                    else
                        fileEntries[i] = newFiles[i - temp.Count()];
            }
            return fileEntries;
        }
    }
}
