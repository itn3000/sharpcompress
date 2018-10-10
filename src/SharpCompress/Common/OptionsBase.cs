
namespace SharpCompress.Common
{
    public class OptionsBase
    {
        /// <summary>
        /// SharpCompress will keep the supplied streams open.  Default is true.
        /// </summary>
        public bool LeaveStreamOpen { get; set; } = true;

        public ArchiveEncoding ArchiveEncoding { get; set; } = new ArchiveEncoding();
        public OptionsBase(OptionsBase options)
        {
            LeaveStreamOpen = options.LeaveStreamOpen;
            ArchiveEncoding = options.ArchiveEncoding;
        }
        public OptionsBase()
        {
            // default ctor
        }
    }
}