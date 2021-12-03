using Base;
using Base.Data;
using Base.DelegateDefine;
using System;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 二维List对比工具
    /// </summary>
    public class CommonCompareHelper
    {
        /// <summary>
        /// 元素类型是int
        /// </summary>
        //static CompareFuncInt mCompareFuncInt = new CompareFuncInt(CompareFuncInt);

        /// <summary>
        /// 元素类型是string
        /// </summary>
        static CompareFuncString mCompareFuncString = new CompareFuncString(CompareFuncString);

        private NLPCompare mNLPCompare = new NLPCompare();

        public void Run(CompareParams compareParams)
        {
            if (!compareParams.CanUse)
            {
                //参数check错误
                Console.WriteLine("mCompareParams 检查错误");
                return;
            }

            compareParams.FileHelper.Read(compareParams.SrcFileFullPath, out compareParams.SrcData);
            compareParams.FileHelper.Read(compareParams.TarFileFullPath, out compareParams.TarData);

            mNLPCompare.Compare(compareParams.SrcData, compareParams.TarData, compareParams.MinSimilarity,
                mCompareFuncString,/* out mCompareParams.ResultData,*/ out compareParams.CompareCommonResult);

            compareParams.FileHelper.Write(compareParams.OutPath, compareParams.OutFileName, compareParams.CompareCommonResult);
        }


        public static bool CompareFuncInt(int src, int tar)
        {
            return src == tar;
        }

        public static bool CompareFuncString(string src, string tar)
        {
            return src == tar;
        }
    }
}
