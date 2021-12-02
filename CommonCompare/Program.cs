using Base.Data;
using System;

namespace Common
{
    class Program
    {
        static void TestCommonCompare()
        {
            string exePath = System.IO.Directory.GetCurrentDirectory();

            string srcFilePath = "src\\testSrc.xlsx";
            string srcFileFullPath = $"{exePath}\\{srcFilePath}";

            string tarFilePath = "src\\testTar.xlsx";
            string tarFileFullPath = $"{exePath}\\{tarFilePath}";

            //用这种方式
            CompareParams compareParams = new CompareParams()
            {
                //五个必须赋值的成员
                SrcFileFullPath = srcFileFullPath,
                TarFileFullPath = tarFileFullPath,
                OutPath = $"{exePath}\\src",
                OutFileName = "out.csv",
                FileHelper = new ExcelHelper(),
            };

            CommonCompareHelper excelCompare = new CommonCompareHelper(compareParams);
            excelCompare.Run();
        }

        static void Main(string[] args)
        {
            TestCommonCompare();

            Console.WriteLine("key to quit");
            Console.ReadKey();
        }
    }
}
