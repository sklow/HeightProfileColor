using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using NUnit.Framework;
using OpenCvSharp;

namespace HeightProfileColor.Tests
{
    [TestFixture]
    [Explicit]
    public class ColorTest
    {
        [Test]
        public void RunTest()
        {
            var imagePath = "path_to_your_image.jpg";
            var csvPath = "path_to_your_csv.csv";
            var outputPath = "output_csv.csv";

            // 画像の読み込み
            using (var image = new Mat(imagePath))
            {
                // CSVファイルの読み込み
                var lines = File.ReadAllLines(csvPath);
                var records = lines.Select(line =>
                {
                    var tokens = line.Split(',');
                    return new UVColor(
                        double.Parse(tokens[0], CultureInfo.InvariantCulture),
                        double.Parse(tokens[1], CultureInfo.InvariantCulture)
                    );
                }).ToList();

                // CSVの各行に対して画像の色を取得
                foreach (var record in records)
                {
                    var x = (int)record.U;
                    var y = (int)record.V;

                    if (x >= 0 && y >= 0 && x < image.Width && y < image.Height)
                    {
                        var color = image.At<Vec3b>(y, x);
                        record.SetColor(color[2], color[1], color[0]); // Red, Green, Blue
                    }
                }

                // 結果を新しいCSVファイルに書き出し
                using (var writer = new StreamWriter(outputPath))
                {
                    writer.WriteLine("U,V,R,G,B");
                    foreach (var record in records)
                        writer.WriteLine($"{record.U},{record.V},{record.R},{record.G},{record.B}");
                }
            }
        }
    }

    public class UVColor
    {
        public double U { get; }
        public double V { get; }
        public double R { get; private set; }
        public double G { get; private set; }
        public double B { get; private set; }

        public UVColor(double u, double v)
        {
            U = u;
            V = v;
        }

        public void SetColor(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
