using SharpCompress.Common;

namespace SharpCompress.Writers
{
    public class WriterOptions : OptionsBase
    {
        public WriterOptions(CompressionType compressionType)
        {
            CompressionType = compressionType;
        }
        public WriterOptions(WriterOptions options) : base(options)
        {
            CompressionType = options.CompressionType;
        }
        public CompressionType CompressionType { get; set; }

        public static implicit operator WriterOptions(CompressionType compressionType)
        {
            return new WriterOptions(compressionType);
        }
    }
}