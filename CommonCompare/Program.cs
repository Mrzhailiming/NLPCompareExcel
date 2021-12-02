using System;
using Base.Data;

namespace Common
{
    class Program
    {

        static void testExcelHelper()
        {
            string exePath = System.IO.Directory.GetCurrentDirectory();
            string srcFilePath = "src\\testSrc.xlsx";
            string srcFileFullPath = $"{exePath}\\{srcFilePath}";

            string tarFilePath = "src\\testTar.xlsx";
            string tarFileFullPath = $"{exePath}\\{tarFilePath}";

            CompareParams compareParams = new CompareParams();
            compareParams.MinSimilarity = 1.0f;

            ExcelHelper.ReadExcel(srcFileFullPath, out compareParams.SrcData);
            ExcelHelper.ReadExcel(tarFileFullPath, out compareParams.TarData);

            CommonCompareHelper commonCompare = new CommonCompareHelper(compareParams);
            commonCompare.Run(compareParams);


            ExcelHelper.WriteCSV($"{exePath}\\src", "result", compareParams.ResultData);
        }

        static void TestCommonCompare()
        {
            //用这种方式完成上面函数的功能 testExcelHelper()
            CompareParams compareParams = new CompareParams();
            CommonCompareHelper excelCompare = new CommonCompareHelper(compareParams);
            excelCompare.Run(compareParams);
        }

        static void Main(string[] args)
        {
            //int[] src = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //int[] tar = { 0, 1, 3, 4, 4, 5, 6, 111, 7 };
            //int[] src = { 0, 1, 2, 3, 4, 5, 6 };
            //int[] tar = { 0, 1, 4, 5, 6 };
            //CompareTest compareTest = new CompareTest();
            //var ret = compareTest.CompareLine(src, tar);


            //List<int> srcList = new List<int> { 0, 2, 3, 4, 7, 1, 1, 1, 1 };
            //List<int> tarList = new List<int> { 0, 1, 3, 5, 6, 7 };
            //NLPCompare compare = new NLPCompare();
            //compare.Compare(srcList, tarList);


            //List<List<string>> src = new List<List<string>>
            //{
            //    new List<string> { "0", "1", "2" },
            //    new List<string> { "0", "1", "2" },
            //};
            //List<List<string>> tar = new List<List<string>>
            //{

            //};
            //ExcelCompare compare = new ExcelCompare();
            //compare.MinSimilarity = 1.0f;
            //compare.Run(src, tar);

            testExcelHelper();

            Console.WriteLine("key to quit");
            Console.ReadKey();
        }
    }
}
