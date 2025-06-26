// Services/Implements/VectorProcess.cs
namespace AuthJWT.Services.Implements
{
    public class VectorProcess
    {
        // Phương thức hiện tại đã tốt cho việc chuẩn hóa vector ban đầu
        // Thêm các phương thức cho cosine similarity

        public double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new ArgumentException("Vectors must be of the same length");

            double dotProduct = 0;
            double normA = 0;
            double normB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                normA += Math.Pow(vectorA[i], 2);
                normB += Math.Pow(vectorB[i], 2);
            }

            if (normA == 0 || normB == 0)
                return 0;

            return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        // Thêm phương thức để tính trọng số cho các thành phần
        public double[] ApplyWeights(double[] vector, double[] weights)
        {
            if (vector.Length != weights.Length)
                throw new ArgumentException("Vector and weights must have the same length");

            var result = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                result[i] = vector[i] * weights[i];
            }

            return result;
        }
        public double[] GetFeatureWeights()
        {
            // Trọng số cho các tính năng theo thứ tự của vector
            // Giá phòng, số lượng phòng, sức chứa, số giường
            double[] baseWeights = { 1.5, 0.7, 1.2, 1.0 };

            // Thêm trọng số cho các view (10 loại)
            double[] viewWeights = Enumerable.Repeat(0.8, 10).ToArray();

            // Thêm trọng số cho các tiện ích (10 loại)
            double[] amenityWeights = Enumerable.Repeat(1.0, 10).ToArray();

            // Trọng số cho rating và booking count
            double[] finalWeights = { 2.0, 1.5 };

            // Gộp tất cả lại
            return baseWeights
                .Concat(viewWeights)
                .Concat(amenityWeights)
                .Concat(finalWeights)
                .ToArray();
        }
    }
}