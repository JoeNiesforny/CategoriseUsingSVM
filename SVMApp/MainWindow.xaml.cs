using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SVMApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Model model;
        public MainWindow()
        {
            InitializeComponent();
            model = new Model();
            dictionaryDataGrid.ItemsSource = model.DictionaryTable.DefaultView;
        }
        private void setReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if (result == true)
                model.SetReference(dlg.FileName);
        }

        private void loadLearningSetButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = GetAllFilesFromRecursivly(dialog.SelectedPath);
                model.LoadLearningSet(files);
                learningSetDataGrid.ItemsSource = model.LearningSetTable.DefaultView;
            }
        }

        private void computeClassificatorButton_Click(object sender, RoutedEventArgs e)
        {
            model.ComputeClassificator();
            vectorDataGrid.ItemsSource = model.ClassificatorVectorTable.DefaultView;
            resultLabel.Content = resultLabel.Content.ToString().Replace("b = 0", "b = " + model.ClassificatorB);
        }

        private void classifyNewDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if (result == true)
                classifyNewDocumentButton.Content = "Document class is " + model.ClassifyDocument(dlg.FileName);
        }
        private static string[] GetAllFilesFromRecursivly(string targetDirectory)
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
