using System.IO;

namespace Giles.Specs.Core.Utility
{
    public class FakeFileSystemWatcher : FileSystemWatcher
    {
        public FakeFileSystemWatcher(string s)
            : base(s)
        { }

        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;

            base.Dispose(disposing);
        }

        public bool WasDisposed { get; set; }
    }
}