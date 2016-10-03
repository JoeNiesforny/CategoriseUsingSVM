using System.IO;
using System.Linq;
using System.Windows;

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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ValidateNames = false;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = true;
            dlg.FileName = "Folder Selection";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string[] files = GetAllFilesFromRecursivly(dlg.FileName.Replace("Folder Selection", ""));
                model.LoadLearningSet(files);
                learningSetDataGrid.ItemsSource = model.LearningSetTable.DefaultView;
            }
        }

        private void computeClassificatorButton_Click(object sender, RoutedEventArgs e)
        {
            model.ComputeClassificator();
            vectorDataGrid.ItemsSource = model.ClassificatorVectorTable.DefaultView;
            classificationBLabel.Content = model.ClassificatorB.ToString();
        }

        private void classifyNewDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if (result == true)
                MessageBox.Show("Document class is " + model.ClassifyDocument(dlg.FileName));
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
