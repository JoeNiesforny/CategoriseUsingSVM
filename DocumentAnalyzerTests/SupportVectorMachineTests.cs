using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CategoriseUsingSVM;
using System.Windows;

namespace DocumentAnalyzerTests
{
    [TestClass]
    public class SupportVectorMachineTests
    {
        [TestMethod]
        public void TwoElementsInLearningSet_ComputeClassificator_CheckIfClassificatorIsValid()
        {
            var learningSet = new Element[]
            {
                new Element { Vector = new double[2] { 0, 1 }, Class = -1 },
                new Element { Vector = new double[2] { 1, 0 }, Class = 1 },
            };
            var svm = new SupportVectorMachine(learningSet);
            Assert.AreEqual(-1, svm.CheckClass(learningSet[0]));
            Assert.AreEqual(1, svm.CheckClass(learningSet[1]));
        }


        [TestMethod]
        public void SixElementsInLearningSet_ComputeClassificator_CheckIfClassificatorIsValidWithValueNotFromLearningSet()
        {
            var learningSet = new Element[]
            {
                new Element { Vector = new double[2] { 0, 1 }, Class = -1 },
                new Element { Vector = new double[2] { 1, 0 }, Class = 1 },
                new Element { Vector = new double[2] { 2, 1 }, Class = 1 },
                new Element { Vector = new double[2] { 3, 2 }, Class = 1 },
                new Element { Vector = new double[2] { 0, 2 }, Class = -1 },
                new Element { Vector = new double[2] { 1, 2 }, Class = -1 },
            };
            var svm = new SupportVectorMachine(learningSet);
            Assert.AreEqual(-1, svm.CheckClass(new Element { Vector = new double[2] { 2 , 4 }, Class = 0 }));
            Assert.AreEqual(1, svm.CheckClass(new Element { Vector = new double[2] { 4, 2 }, Class = 0 }));
        }

        [TestMethod]
        public void TenElementsInLearningSet_ComputeClassificator_CheckIfClassificatorIsValidWithValuesNotFromLearningSet()
        {
            var learningSet = new Element[]
            {
                new Element { Vector = new double[2] { 0, 1 }, Class = 1 },
                new Element { Vector = new double[2] { 0, 2 }, Class = 1 },
                new Element { Vector = new double[2] { 2, 3 }, Class = 1 },
                new Element { Vector = new double[2] { 3, 4 }, Class = 1 },
                new Element { Vector = new double[2] { 4, 5 }, Class = 1 },
                new Element { Vector = new double[2] { 1, 0 }, Class = -1 },
                new Element { Vector = new double[2] { 2, 1 }, Class = -1 },
                new Element { Vector = new double[2] { 3, 2 }, Class = -1 },
                new Element { Vector = new double[2] { 4, 3 }, Class = -1 },
                new Element { Vector = new double[2] { 5, 4 }, Class = -1 },
            };
            var svm = new SupportVectorMachine(learningSet);
            Assert.AreEqual(-1, svm.CheckClass(new Element { Vector = new double[2] { 2, 1 }, Class = 0 }), "First");
            Assert.AreEqual(-1, svm.CheckClass(new Element { Vector = new double[2] { 2, 0 }, Class = 0 }), "Second");
            Assert.AreEqual(1, svm.CheckClass(new Element { Vector = new double[2] { 1, 2 }, Class = 0 }), "Third");
            Assert.AreEqual(1, svm.CheckClass(new Element { Vector = new double[2] { 2, 5 }, Class = 0 }), "Fourth");
        }
    }
}
