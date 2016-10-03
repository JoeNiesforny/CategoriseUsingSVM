using CategoriseUsingSVM;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace SVMApp
{
    public class Model
    {
        public DataTable DictionaryTable;
        public DataTable LearningSetTable;
        public DataTable ClassificatorVectorTable;
        public double ClassificatorB;

        private string[] _Dictionary;
        private Element[] _LearningSet;
        private DocumentAnalyzer _DocumentAnalyzer;
        private SupportVectorMachine _SVM;

        public Model()
        {
            _Dictionary = new string[] { "Politechnika", "Gdańsk", "studia", "stypendia", "stypendium", "zawody", "władze", "Władze", "wydział", "Wydziały" };
            Converter.ConvertToDataTable(out DictionaryTable, _Dictionary);
        }

        public void SetReference(string pathToReferenceFile)
        {
            Converter.ConvertToArray(out _Dictionary, DictionaryTable);
            if (File.Exists(pathToReferenceFile))
                _DocumentAnalyzer = new DocumentAnalyzer(_Dictionary, pathToReferenceFile);
            else
                throw new Exception("Reference file doesn't exist!");
        }

        public void LoadLearningSet(string[] documents)
        {
            if (_DocumentAnalyzer == null)
                _DocumentAnalyzer = new DocumentAnalyzer(_Dictionary);
            foreach (var doc in documents)
                _DocumentAnalyzer.AddNewDocument(doc);
            _LearningSet = _DocumentAnalyzer.GetLearningSet();
            Converter.ConvertToDataTable(out LearningSetTable, _LearningSet);
        }

        public void ComputeClassificator()
        {
            Converter.ConvertToArray(out _LearningSet, LearningSetTable);
            _SVM = new SupportVectorMachine(_LearningSet);
            Converter.ConvertToDataTable(out ClassificatorVectorTable, _SVM.Classificator.Vector);
            ClassificatorB = _SVM.Classificator.b;
        }

        public int ClassifyDocument(string path)
        {
            var vector = _DocumentAnalyzer.ComputeVector(path);
            var len = vector.Count();
            var element = new Element();
            element.Vector = new double[len];
            for (int i = 0; i < len; i++)
                element.Vector[i] = vector[i];
            return _SVM.CheckClass(element);
        }
    }

    static class Converter
    {
        public static void ConvertToDataTable(out DataTable table, string[] array)
        {
            table = new DataTable();
            table.Columns.Add("Words");
            for (int i = 0; i < array.GetLength(0); i++)
            {
                var newRow = table.NewRow();
                newRow[0] = array[i];
                table.Rows.Add(newRow);
            }
        }
        public static void ConvertToArray(out string[] array, DataTable table)
        {
            array = new string[table.Rows.Count];
            for (var row = 0; row < table.Rows.Count; row++)
                array[row] = table.Rows[row][0].ToString();
        }
        public static void ConvertToDataTable(out DataTable table, Element[] array)
        {
            table = new DataTable();
            for (int i = 0; i < array[0].Vector.Count(); i++)
                table.Columns.Add("V_" + i);
            table.Columns.Add("Class");
            for (int i = 0; i < array.Count(); i++)
            {
                var newRow = table.NewRow();
                for (int j = 0; j < array[i].Vector.Count(); j++)
                    newRow[j] = array[i].Vector[j];
                newRow[array[i].Vector.Count()] = array[i].Class;
                table.Rows.Add(newRow);
            }
        }
        public static void ConvertToArray(out Element[] array, DataTable table)
        {
            array = new Element[table.Rows.Count];
            for (var i = 0; i < table.Rows.Count; i++)
            {
                array[i].Vector = new double[table.Columns.Count - 1];
                for (var j = 0; j < table.Columns.Count - 1; j++)
                    array[i].Vector[j] = double.Parse(table.Rows[i][j].ToString().Replace('.', ','));
                array[i].Class = int.Parse(table.Rows[i][table.Columns.Count - 1].ToString());
            }
        }
        public static void ConvertToDataTable(out DataTable table, double[] array)
        {
            table = new DataTable();
            for (int i = 0; i < array.GetLength(0); i++)
                table.Columns.Add("V_" + i);
            var newRow = table.NewRow();
            for (int i = 0; i < array.GetLength(0); i++)
                newRow[i] = array[i];
            table.Rows.Add(newRow);
            table.DefaultView.AllowNew = false;
            table.DefaultView.AllowDelete = false;
        }
        public static void ConvertToArray(out double[] array, DataTable table)
        {
            array = new double[table.Columns.Count];
            for (var i = 0; i < table.Columns.Count; i++)
                array[i] = double.Parse(table.Rows[0][i].ToString().Replace('.', ','));
        }
    }
}
