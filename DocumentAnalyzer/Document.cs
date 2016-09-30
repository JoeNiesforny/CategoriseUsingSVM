namespace CategoriseUsingSVM
{
    public struct Position
    {
        public double X;
        public double Y;
    }
    public struct Document
    {
        public string Path;
        public byte[] Vector;
        public double Similarity;
        public double Distance;
        public Position Position;
    }
}
