using System;
using System.IO;

namespace Test.Controller
{
    public static class TestHelper
    {
        private static string _appDirectory = AppContext.BaseDirectory;

        static TestHelper()
        {
            NormalizeAppDirectoryPath();
        }

        public static string GetDbResponseByCollectionAndFileName(string fileName)
        {
            return File.ReadAllText(_appDirectory + "\\Mockup\\" + fileName + ".json");
        }

        private static void NormalizeAppDirectoryPath()
        {
            var index = _appDirectory.IndexOf("bin", StringComparison.Ordinal);
            if (index >= 0)
                _appDirectory = _appDirectory.Remove(index);
        }
    }
}