namespace Giles.Core.Watchers
{
    public interface IBuildRunner
    {
        void Run(string solution);
    }

    public class BuildRunner : IBuildRunner
    {
        public void Run(string solution)
        {
            // run msbuild here...
        }
    }
}