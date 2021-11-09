using System.Text;
using Furball.Engine.Engine.Helpers;
using Xunit;

namespace Furball.Tests.Helpers {
    public class CryptoHelperTests {
        private readonly byte[] Original = Encoding.ASCII.GetBytes("test");

        private const string MD5    = "098f6bcd4621d373cade4e832627b4f6";
        private const string SHA256 = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";
        private const string SHA384 = "768412320f7b0aa5812fce428dc4706b3cae50e02a64caa16a782249bfe8efc4b7ef1ccb126255d196047dfedf17a0a9";
        private const string SHA512 =
            "ee26b0dd4af7e749aa1a8ee3c10ae9923f618980772e473f8819a5d4940e0db27ac185f8a0e1d5f84f88bc887fd67b143732c304cc5fa9ad8e6f57f50028a8ff";

        private const byte   CRC8  = 192;
        private const ushort CRC16 = 448;
        private const uint   CRC32 = 448;
        private const ulong  CRC64 = 448;

        [Fact]
        public void GetMd5Test() {
            Assert.True(CryptoHelper.GetMd5(this.Original) == MD5, "The Md5 hash did not match up!");
        }

        [Fact]
        public void GetSha256() {
            Assert.True(CryptoHelper.GetSha256(this.Original) == SHA256, "The Sha256 hash did not match up!");
        }

        [Fact]
        public void GetSha384() {
            Assert.True(CryptoHelper.GetSha384(this.Original) == SHA384, "The Sha384 hash did not match up!");
        }

        [Fact]
        public void GetSha512() {
            Assert.True(CryptoHelper.GetSha512(this.Original) == SHA512, "The Sha512 hash did not match up!");
        }

        [Fact]
        public void GetCrc8() {
            Assert.True(CryptoHelper.Crc8(this.Original, this.Original.Length) == CRC8, "The Sha512 hash did not match up!");
        }

        [Fact]
        public void GetCrc16() {
            Assert.True(CryptoHelper.Crc16(this.Original, this.Original.Length) == CRC16, "The Sha512 hash did not match up!");
        }

        [Fact]
        public void GetCrc32() {
            Assert.True(CryptoHelper.Crc32(this.Original, this.Original.Length) == CRC32, "The Sha512 hash did not match up!");
        }

        [Fact]
        public void GetCrc64() {
            Assert.True(CryptoHelper.Crc64(this.Original, this.Original.Length) == CRC64, "The Sha512 hash did not match up!");
        }
    }
}
