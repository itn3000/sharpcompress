using SharpCompress.Common;
using Xunit;
using System.IO;
using System.Linq;

namespace SharpCompress.Test.Zip
{
    public class ZipWriterTests : WriterTests
    {
        public ZipWriterTests()
            : base(ArchiveType.Zip)
        {
        }

        [Fact]
        public void Zip_Deflate_Write()
        {
            Write(CompressionType.Deflate, "Zip.deflate.noEmptyDirs.zip", "Zip.deflate.noEmptyDirs.zip");
        }


        [Fact]
        public void Zip_BZip2_Write()
        {
            Write(CompressionType.BZip2, "Zip.bzip2.noEmptyDirs.zip", "Zip.bzip2.noEmptyDirs.zip");
        }


        [Fact]
        public void Zip_None_Write()
        {
            Write(CompressionType.None, "Zip.none.noEmptyDirs.zip", "Zip.none.noEmptyDirs.zip");
        }


        [Fact]
        public void Zip_LZMA_Write()
        {
            Write(CompressionType.LZMA, "Zip.lzma.noEmptyDirs.zip", "Zip.lzma.noEmptyDirs.zip");
        }

        [Fact]
        public void Zip_PPMd_Write()
        {
            Write(CompressionType.PPMd, "Zip.ppmd.noEmptyDirs.zip", "Zip.ppmd.noEmptyDirs.zip");
        }

        [Fact]
        public void Zip_Rar_Write()
        {
            Assert.Throws<InvalidFormatException>(() => Write(CompressionType.Rar, "Zip.ppmd.noEmptyDirs.zip", "Zip.ppmd.noEmptyDirs.zip"));
        }
        // 0x3042 == Japanese 'あ'
        [Theory()]
        [InlineData(1, 0x3042)]
        [InlineData(200, 0x3042)]
        public void Zip_Japanese_FileName(int length, int value)
        {
            var data = new byte[1];
            var fname = new string(Enumerable.Range(0, length).Select(i => (char)value).ToArray());
            var enc = System.Text.Encoding.UTF8;
            using (var mstm = new MemoryStream())
            {
                var opts = new SharpCompress.Writers.Zip.ZipWriterOptions(CompressionType.Deflate);
                opts.ArchiveEncoding.Default = enc;
                using (var tw = new SharpCompress.Writers.Zip.ZipWriter(mstm, opts))
                {
                    using (var dstm = new MemoryStream(data))
                    {
                        tw.Write(fname, dstm, System.DateTime.Now);
                    }
                }
                using(var mstm2 = new MemoryStream(mstm.ToArray()))
                {
                    var ropts = new SharpCompress.Readers.ReaderOptions();
                    ropts.ArchiveEncoding.Default = enc;
                    using(var tr = SharpCompress.Readers.Zip.ZipReader.Open(mstm2, ropts))
                    {
                        Assert.True(tr.MoveToNextEntry());
                        Assert.Equal(fname, tr.Entry.Key);
                    }
                }
            }
        }
    }
}
