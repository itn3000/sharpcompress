using System.IO;
using SharpCompress.Common;
using SharpCompress.Writers.Tar;
using Xunit;
using System.Linq;

namespace SharpCompress.Test.Tar
{
    public class TarWriterTests : WriterTests
    {
        public TarWriterTests()
            : base(ArchiveType.Tar)
        {
            UseExtensionInsteadOfNameToVerify = true;
        }

        [Fact]
        public void Tar_Writer()
        {
            Write(CompressionType.None, "Tar.noEmptyDirs.tar", "Tar.noEmptyDirs.tar");
        }

        [Fact]
        public void Tar_BZip2_Writer()
        {
            Write(CompressionType.BZip2, "Tar.noEmptyDirs.tar.bz2", "Tar.noEmptyDirs.tar.bz2");
        }

        [Fact]
        public void Tar_LZip_Writer()
        {
            Write(CompressionType.LZip, "Tar.noEmptyDirs.tar.lz", "Tar.noEmptyDirs.tar.lz");
        }

        [Fact]
        public void Tar_Rar_Write()
        {
            Assert.Throws<InvalidFormatException>(() => Write(CompressionType.Rar, "Zip.ppmd.noEmptyDirs.zip", "Zip.ppmd.noEmptyDirs.zip"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Tar_Finalize_Archive(bool finalizeArchive)
        {
            using (MemoryStream stream = new MemoryStream())
            using (Stream content = File.OpenRead(Path.Combine(ORIGINAL_FILES_PATH, "jpg", "test.jpg")))
            {
                using (TarWriter writer = new TarWriter(stream, new TarWriterOptions(CompressionType.None, finalizeArchive)))
                {
                    writer.Write("doesn't matter", content, null);
                }

                var paddedContentWithHeader = content.Length / 512 * 512 + 512 + 512;
                var expectedStreamLength = finalizeArchive ? paddedContentWithHeader + 512 * 2 : paddedContentWithHeader;
                Assert.Equal(expectedStreamLength, stream.Length);
            }
        }

        [Theory]
        [InlineData(1, 0x3042)]
        [InlineData(50, 0x3042)]
        public void Tar_Japanese_FileName(int length, int codepoint)
        {
            var data = new byte[1];
            var fname = new string(Enumerable.Range(0, length).Select(i => (char)codepoint).ToArray());
            using (var mstm = new MemoryStream())
            {
                var opts = new TarWriterOptions(CompressionType.None);
                opts.ArchiveEncoding.Default = System.Text.Encoding.UTF8;
                using (var tw = new TarWriter(mstm, opts))
                {
                    using (var dstm = new MemoryStream(data))
                    {
                        tw.Write(fname, dstm, System.DateTime.Now);
                    }
                }
                using(var mstm2 = new MemoryStream(mstm.ToArray()))
                {
                    var ropts = new SharpCompress.Readers.ReaderOptions();
                    ropts.ArchiveEncoding.Default = System.Text.Encoding.UTF8;
                    using(var tr = new SharpCompress.Readers.Tar.TarReader(mstm2, ropts, CompressionType.None))
                    {
                        Assert.True(tr.MoveToNextEntry());
                        Assert.Equal(fname, tr.Entry.Key);
                    }
                }
            }
        }
    }
}
