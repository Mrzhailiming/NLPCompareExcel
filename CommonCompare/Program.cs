using Base.Data;
using System;

namespace Common
{
    class Program
    {
        static void TestCommonCompare()
        {
            string line = Console.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            var arr = line.Split(' ');

            if (arr is null || arr.Length < 2)
            {
                return;
            }

            string exePath = System.IO.Directory.GetCurrentDirectory();

            string srcFilePath = arr[0];
            string srcFileFullPath = $"{srcFilePath}";

            string tarFilePath = arr[1];
            string tarFileFullPath = $"{tarFilePath}";

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

            Console.WriteLine($"处理结束,输出文件为:{compareParams.OutPath}\\{compareParams.OutFileName}");
        }

        static void Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine("输入两个Excel的路径,空格分开,然后按回车开始对比");

                TestCommonCompare();
            }

            Console.WriteLine("key to quit");
            Console.ReadKey();
        }
    }
}
