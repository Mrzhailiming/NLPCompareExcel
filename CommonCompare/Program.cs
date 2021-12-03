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
                SrcFileFullPath = srcFileFullPath,  //源文件的完整路径
                TarFileFullPath = tarFileFullPath,  //目标文件的完整路径
                OutPath = $"{exePath}\\src",    //输出结果文件的目录
                OutFileName = "out.csv",        //文件名
                FileHelper = new ExcelHelper(), //自定制读取和写入文件 (继承IFileHelper_Interface, 实现读写文件)
            };

            CommonCompareHelper commonCompareHelper = new CommonCompareHelper();
            commonCompareHelper.Run(compareParams);

            //// change compareParams
            //commonCompareHelper.Run(compareParams);
            //// change compareParams
            //commonCompareHelper.Run(compareParams);
        }

        static void Main(string[] args)
        {
            TestCommonCompare();

            Console.WriteLine("key to quit");
            Console.ReadKey();
        }
    }
}
