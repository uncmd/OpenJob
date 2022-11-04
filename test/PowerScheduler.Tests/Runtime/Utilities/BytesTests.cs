namespace PowerScheduler.Runtime.Utilities;

public class BytesTests
{
    public class From
    {
        [Fact]
        public void Kilobytes()
        {
            var bytes = Bytes.FromKilobytes(1);
            bytes.ShouldBe(1024);
        }

        [Fact]
        public void Megabytes()
        {
            var bytes = Bytes.FromMegabytes(1);
            bytes.ShouldBe(1_048_576);
        }

        [Fact]
        public void Gigabytes()
        {
            var gigabyte = 1;
            var bytes = Bytes.FromGigabytes(gigabyte);
            bytes.ShouldBe(Math.Pow(1024, 3) * gigabyte);
        }

        [Fact]
        public void Terabytes()
        {
            var terabyte = 1;
            var bytes = Bytes.FromTerabytes(terabyte);
            bytes.ShouldBe(Math.Pow(1024, 4) * terabyte);
        }
    }

    public class To
    {
        [Fact]
        public void Kilobytes()
        {
            var kb = Bytes.ToKilobytes(1);
            kb.ShouldBe(0.0009765625);
        }

        [Fact]
        public void Megabytes()
        {
            var bytes = Bytes.ToMegabytes(1_000_000);
            Math.Round(bytes, 4).ShouldBe(0.9537);
        }

        [Fact]
        public void Gigabytes()
        {
            var bytes = Bytes.ToGigabytes(1_000_000);
            Math.Round(bytes, 6).ShouldBe(0.000931);
        }

        [Fact]
        public void Terabytes()
        {
            var gb = Bytes.FromGigabytes(1);
            var bytes = Bytes.ToTerabytes(gb);

            Math.Round(bytes, 6).ShouldBe(0.000977);
        }
    }
}
