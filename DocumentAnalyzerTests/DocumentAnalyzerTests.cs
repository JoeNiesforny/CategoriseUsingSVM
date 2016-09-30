using CategoriseUsingSVM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Windows;

namespace DocumentAnalyzerTests
{
    [TestClass]
    public class DocumentAnalyzerTests
    {
        string Path;
        string Data;
        string[] Dictionary;

        [TestInitialize]
        public void Initialize()
        {
            Path = "reference-document";
            Data =
                "Training a support vector machine (SVM) leads to" +
                " a quadratic optimization problem with bound constraints" +
                " and one linear equality constraint.";
            Dictionary = new string[] { "Training", "SVM", "neural", "network", "quadratic", "optimization" };
            File.WriteAllText(Path, Data);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }

        [TestMethod]
        public void CreateNewAnalyzer_AddNewDocumentSameAsReference_ValidateDocumentProperties()
        {
            var analyzer = new DocumentAnalyzer(Dictionary, Path);
            analyzer.AddNewDocument(Path);
            CollectionAssert.AreEqual(analyzer.Documents[0].Vector,
                new byte[] { 1, 1, 0, 0, 1, 1 });
            Assert.AreEqual(analyzer.Documents[0].Similarity, 1);
            Assert.AreEqual(analyzer.Documents[0].Distance, 0);
        }

        [TestMethod]
        public void CreateNewAnalyzer_AddNewDocumentCompletelyDifferentFromReference_ValidateDocumentProperties()
        {
            var analyzer = new DocumentAnalyzer(Dictionary, Path);
            var tmpPath = "tmp-document";
            File.WriteAllText(tmpPath, "As the Rosetta spacecraft - and landing module Philae -" +
                                       " conclude their final mission, Arrow salutes the ESA - " +
                                       "European Space Agency and all those who have worked so " +
                                       "hard over the years to make this project a reality.");
            analyzer.AddNewDocument(tmpPath);
            CollectionAssert.AreEqual(analyzer.Documents[0].Vector,
                new byte[] { 0, 0, 0, 0, 0, 0 });
            Assert.AreEqual(analyzer.Documents[0].Similarity, double.NaN);
            Assert.AreEqual(analyzer.Documents[0].Distance, double.NaN);
            File.Delete(tmpPath);
        }

        [TestMethod]
        public void CreateNewAnalyzer_AddNewDocumentSimilarToReference_ValidateDocumentProperties()
        {
            var analyzer = new DocumentAnalyzer(Dictionary, Path);
            var tmpPath = "tmp-document";
            File.WriteAllText(tmpPath,
                "Training a support vector machine (SVM) leads to a problem.");
            analyzer.AddNewDocument(tmpPath);
            CollectionAssert.AreEqual(analyzer.Documents[0].Vector,
                new byte[] { 1, 1, 0, 0, 0, 0 });
            Assert.AreEqual(0.7, Math.Round(analyzer.Documents[0].Similarity, 1));
            Assert.AreEqual(0.25, Math.Round(analyzer.Documents[0].Distance, 2));
            File.Delete(tmpPath);
        }
    }
}
