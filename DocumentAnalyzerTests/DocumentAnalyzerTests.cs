using CategoriseUsingSVM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Windows;

namespace DocumentAnalyzerTests
{
    [TestClass]
    public class DocumentAnalyzerTests
    {
        string Path;
        string Data;

        [TestInitialize]
        public void Initialize()
        {
            Path = "tmp-document";
            Data =
                "Training a support vector machine (SVM) leads to" +
                " a quadratic optimization problem with bound constraints" +
                " and one linear equality constraint.";
            File.WriteAllText(Path, Data);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }

        [TestMethod]
        public void AddNewDocument_ValidateDocumentProperties()
        {
            var dictionary = new string[] { "Training", "SVM", "neural", "network", "quadratic", "optimization"};
            var analyzer = new DocumentAnalyzer(dictionary);
            Assert.IsTrue(analyzer.AddNewDocument(Path));
            CollectionAssert.AreEqual(analyzer.Documents[0].Vector,
                new bool[] { true, true, false, false, true, true });
            Assert.AreEqual(analyzer.Documents[0].Position, new Point(3,6));
        }

        [TestMethod]
        public void AddNewDocument_DictionaryCountIsOdd_ValidateDocumentProperties()
        {
            var dictionary = new string[] { "Training", "SVM", "neural", "network", "quadratic", "optimization", "linear" };
            var analyzer = new DocumentAnalyzer(dictionary);
            Assert.IsTrue(analyzer.AddNewDocument(Path));
            CollectionAssert.AreEqual(analyzer.Documents[0].Vector,
                new bool[] { true, true, false, false, true, true, true });
            Assert.AreEqual(analyzer.Documents[0].Position, new Point(3, 7));
        }
    }
}
