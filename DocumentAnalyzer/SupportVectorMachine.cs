using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoriseUsingSVM
{
    public struct Element
    {
        public double[] Vector;
        public int Class;
    }

    public struct LinearFunction
    {
        public double[] Vector;
        public double b;
    }


    // References:
    // 1. "Support Vector Machine Solvers" by Leon Bottou and Chih-Jen Lin.
    public class SupportVectorMachine
    {
        public LinearFunction Classificator;

        public SupportVectorMachine(Element[] learningSet)
        {
            ComputeClassificator(learningSet);
        }

        public int CheckClass(Element element)
        {
            if ((VectorMultiply(element.Vector, Classificator.Vector) + Classificator.b) > 0)
                return 1;
            else
                return -1;
        }

        // using Langrangian duality theory
        // Equation:
        // Max D(alfa) -> sum elements i of (alfa(i)) - 1/2 * sum elements i,j of (y(i) * y(j) * alfa(i) * transpose(alfa(j)) * x(i) * transpose(x(j)))
        // Conditions:
        // 1) alfa(i) >= 0
        // 2) sum of (alfa(i) * y(i)) = 0
        // Then we can get new Classifiator function:
        // w = sum of (alfa(i) * y(i) * x(i));
        // b = y(i) - w * x(i)
        private void ComputeClassificator(Element[] learningSet)
        {
            if (learningSet.Count() <= 0)
                throw new FormatException("Learning set should contain at least one element!");
            List<Element> B = learningSet.ToList();
            // number of elements in learning sets.
            int len = learningSet[0].Vector.Count(); // number of elements in vector.
            int num = learningSet.Count();
            double[] alfa = new double[len];
            // Idea is to compute direction search for alfa vector.
            // new alfa = previous alfa + step length * moving direction
            // (optional)
            // Condition to end iteration is
            // y(i)*y$(x(i)) >= 1
            // y$ - (classificator function)
            while (B.Count != 0)
            {
                double[] g = new double[num];
                for (int i = 0; i < num; i++)
                {
                    // Compute g vector
                    double sum = 0;
                    for (int j = 0; j < num; j++)
                        sum += learningSet[j].Class * alfa[j] * VectorMultiply(learningSet[i].Vector, learningSet[j].Vector);
                    g[i] = 1 - learningSet[i].Class * sum;
                }
                double[] u = new double[num];
                int BCount = 0;
                do
                {
                    // Setting elements of vector u to 0 if position of element is higher than count of set B.
                    for (int i = B.Count - 1; i < num; i++)
                        u[i] = 0;
                    BCount = B.Count;
                    // Counting mean of vector y multiply g.
                    double p = 0;
                    for (int i = 0; i < BCount; i++)
                        p += g[i] * learningSet[i].Class;
                    p /= num;
                    // Computing new values for vector u.
                    for (int i = 0; i < BCount; i++)
                        u[i] = g[i] - learningSet[i].Class * p;
                    // Condition to check if sum of (u(k)*y(k)) == 0
                    double sum = 0;
                    for (int i = 0; i < num; i++)
                        sum += learningSet[i].Class * u[i];
                    if (sum != 0)
                        throw new Exception("Condition \"sum of (u(k)*y(k)) == 0\" is broken!");
                    // Remove unnecessary element from set B.
                    for (int i = 0; i < B.Count; i++)
                        if ((u[i] > 0) || (u[i] < 0 && alfa[i] == 0))
                        {
                            B.RemoveAt(i);
                            i--;
                        }
                }
                while (BCount != B.Count); // Condition that check if set B is still changing.
                // If u == 0 then break loop.
                for (int i = 0; i < num; i++)
                    if (u[i] == 0)
                        break;
                // Computing new lambda
                double[,] H = new double[num, num];
                for (int i = 0; i < num; i++)
                    for (int j = 0; j < num; j++)
                        H[i, j] = ComputeLambda(learningSet[i], learningSet[j]);// learningSet[i].Class * learningSet[j].Class * VectorMultiply(learningSet[i].Vector, learningSet[j].Vector);
                double[] tmp = new double[num];
                for (int i = 0; i < num; i++)
                    for (int j = 0; j < num; j++)
                        tmp[i] += H[i, j] * u[j];
                var lambda = VectorMultiply(g, u) / (VectorMultiply(tmp, u));
                lambda = lambda > 0 ? lambda : 0;
                // Add step to alfa vector.
                for (int i = 0; i < num; i++)
                    alfa[i] = alfa[i] + lambda * u[i];
            }
            // Setting new Classificator function.
            Classificator = new LinearFunction();
            Classificator.Vector = new double[len];
            for (int i = 0; i < len; i++)
            {
                double sum = 0;
                for (int j = 0; j < learningSet.Count(); j++)
                    sum += alfa[j] * learningSet[j].Class * learningSet[j].Vector[i];
                Classificator.Vector[i] = sum;
            }
            Classificator.b = learningSet[0].Class - VectorMultiply(Classificator.Vector, learningSet[0].Vector);
        } // End of ComputeClassificator.

        // Vector1 * transpose(Vector2) = result
        private double VectorMultiply(double[] vector1, double[] vector2)
        {
            if (vector1.Count() != vector2.Count())
                throw new FormatException("Vectors should have the same size!");
            double result = 0;
            for (int i = 0; i < vector2.Count(); i++)
                result += vector1[i] * vector2[i];
            return result;
        }

        // lambda = y(i) * y(j) * x(i) * transpose(x(j))
        private double ComputeLambda(Element element1, Element element2)
        {
            return element1.Class * element2.Class * VectorMultiply(element1.Vector, element2.Vector);
        }
    }
}
