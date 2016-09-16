using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CategoriseUsingSVM
{
    public class Document
    {
        public string Path;
        public bool[] Vector;
        public Point Position;
        public string Category;

        public Document(string path, bool[] vector)
        {
            Path = path;
            Vector = vector;
            var count = vector.Count() / 2;
            if (vector.Count() % 2 == 1)
                count++;
            Position = new Point(SumOfElementLessThan(vector, count),
                                 SumOfElementMoreThanAndEqual(vector, count));
        }

        private int SumOfElementLessThan(bool[] vector, int count)
        {
            int sum = 0;
            for (int iter = 0; iter < count; iter++)
                if (vector[iter])
                    sum += (int)Math.Pow(2, iter);
            return sum;
        }

        private int SumOfElementMoreThanAndEqual(bool[] vector, int count)
        {
            int sum = 0;
            for (int iter = count; iter < vector.Count(); iter++)
                if (vector[iter])
                    sum += (int)Math.Pow(2, iter - count);
            return sum;
        }
    }

    public class DocumentAnalyzer
    {
        public List<string> Dictionary;
        public List<Document> Documents;

        public DocumentAnalyzer()
        {
            Dictionary = new List<string> ();
            Documents = new List<Document> ();
        }

        public DocumentAnalyzer(string[] dictionary)
        {
            Dictionary = new List<string>(dictionary);
            Documents = new List<Document>();
        }

        public DocumentAnalyzer(string[] dictionary, string[] files)
        {
            Dictionary = new List<string>(dictionary);
            Documents = new List<Document>();
            foreach (var file in files)
                AddNewDocument(file);
        }

        public bool AddNewDocument(string path)
        {
            path = path.Trim();
            foreach (var document in Documents)
                if (path == document.Path)
                    return false;
            Documents.Add(new Document(path, CountVector(path)));
            return true;
        }

        private bool[] CountVector(string path)
        {
            var vector = new bool[Dictionary.Count];
            var data = File.ReadAllText(path);
            for (var iter = 0; iter < Dictionary.Count; iter++)
                if (data.IndexOf(Dictionary[iter]) >= 0)
                    vector[iter] = true;
            return vector;
        }
    }
}
