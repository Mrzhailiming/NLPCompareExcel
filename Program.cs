using NLP;
using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    public enum Flags
    {
        Same,       //相同
        Insert,     //对于源文件来说有，目标文件里没有； 对目标文件来说就是插入
        Delete,     //删除
        Update,     //修改
        Gray,       // 代表另一个文件有的本文件没有
    }


    class Program
    {
        static void Main(string[] args)
        {
            //int[] src = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //int[] tar = { 0, 1, 3, 4, 4, 5, 6, 111, 7 };

            //int[] src = { 0, 1, 2, 3, 4, 5, 6 };
            //int[] tar = { 0, 1, 4, 5, 6 };

            //CompareTest compareTest = new CompareTest();

            //var ret = compareTest.CompareLine(src, tar);

            List<int> srcList = new List<int> { 0, 2, 3, 4, 7 };
            List<int> tarList = new List<int> { 0, 1, 3, 5, 6, 7 };

            NLPCompare compare = new NLPCompare();
            compare.Compare(srcList, tarList);

            Console.WriteLine("Hello World!");
        }
    }
}
