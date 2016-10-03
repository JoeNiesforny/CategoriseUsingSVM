using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CategoriseUsingSVM
{
    public class DocumentAnalyzer
    {
        public List<string> Dictionary;
        public byte[] ReferenceVector;
        public List<Document> Documents;

        public DocumentAnalyzer(string[] dictionary)
        {
            Dictionary = new List<string>(dictionary);
            Documents = new List<Document>();
            ReferenceVector = new byte[dictionary.Count()];
            for (int i = 0; i < dictionary.Count(); i++)
                ReferenceVector[i] = 1;
        }

        public DocumentAnalyzer(string[] dictionary, string referenceFile)
        {
            Dictionary = new List<string>(dictionary);
            Documents = new List<Document>();
            ReferenceVector = ComputeVector(referenceFile);
        }

        public DocumentAnalyzer(string[] dictionary, byte[] referenceVector)
        {
            Dictionary = new List<string>(dictionary);
            Documents = new List<Document>();
            ReferenceVector = referenceVector;
        }

        public DocumentAnalyzer(string[] dictionary, string referenceFile, string[] files)
        {
            Dictionary = new List<string>(dictionary);
            Documents = new List<Document>();
            ReferenceVector = ComputeVector(referenceFile);
            foreach (var file in files)
                AddNewDocument(file);
        }

        public DocumentAnalyzer(string[] dictionary, byte[] referenceVector, string[] files)
        {
            Dictionary = new List<string>(dictionary);
            Documents = new List<Document>();
            ReferenceVector = referenceVector;
            foreach (var file in files)
                AddNewDocument(file);
        }

        public void AddNewDocument(string path)
        {
            path = path.Trim();
            foreach (var document in Documents)
                if (path == document.Path)
                    return;
            var vector = ComputeVector(path);
            var similarity = ComputeSimilatrity(vector);
            var distance = ComputeDistance(similarity);
            var position = ComputePosition(distance, similarity);
            Documents.Add(new Document()
            {
                Path = path,
                Vector = vector,
                Similarity = similarity,
                Distance = distance,
                Position = position
            });
        }

        public byte[] ComputeVector(string path)
        {
            var vector = new byte[Dictionary.Count];
            var data = File.ReadAllText(path);
            for (var iter = 0; iter < Dictionary.Count; iter++)
                if (data.IndexOf(Dictionary[iter]) >= 0)
                    vector[iter] = 1;
                else
                    vector[iter] = 0;
            return vector;
        }

        private double ComputeSimilatrity(byte[] vector)
        {
            var count = ReferenceVector.Count();
            if (count != vector.Count())
                throw new FormatException("Length of vectors are different. Should be the same.");
            var sum = 0;
            for (int i = 0; i < count; i++)
                sum += ReferenceVector[i] * vector[i];
            var sumOfA = 0;
            var sumOfB = 0;
            for (int i = 0; i < count; i++)
            {
                sumOfA += ReferenceVector[i] * ReferenceVector[i];
                sumOfB += vector[i] * vector[i];
            }
            return sum / (Math.Sqrt(sumOfA) * Math.Sqrt(sumOfB));
        }

        private double ComputeDistance(double similarity)
        {
            return Math.Acos(similarity) / Math.PI;
        }

        private Position ComputePosition(double distance, double similarity)
        {
            return new Position()
            {
                X = distance * similarity,
                Y = Math.Sqrt(distance * distance - (similarity * distance) * (similarity * distance))
            };
        }

        public Element[] GetLearningSet(double condition_high = 0.55, double condition_low = 0.45)
        {
            List<Element> learningSet = new List<Element>();
            foreach (var doc in Documents)
            {
                var category = 0;
                if (doc.Similarity > condition_high)
                    category = 1;
                else
                    if (doc.Similarity <= condition_low)
                        category = -1;
                    else
                        continue;
                var lengthOfVector = doc.Vector.Count();
                var newElement = new Element();
                newElement.Class = category;
                newElement.Vector = new double[lengthOfVector];
                for (int j = 0; j < lengthOfVector; j++)
                    newElement.Vector[j] = doc.Vector[j];
                learningSet.Add(newElement);
            }
            return learningSet.ToArray();
        }
    }
}
