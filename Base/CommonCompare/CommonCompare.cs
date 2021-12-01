﻿using Base.Data;
using Base.DelegateDefine;

namespace Common
{
    /// <summary>
    /// 二维List对比工具
    /// </summary>
    public class CommonCompare
    {
        /// <summary>
        /// 元素类型是int
        /// </summary>
        //static CompareFuncInt mCompareFuncInt = new CompareFuncInt(CompareFuncInt);

        /// <summary>
        /// 元素类型是string
        /// </summary>
        static CompareFuncString mCompareFuncString = new CompareFuncString(CompareFuncString);

        public void Run(ExcelCompareParams excelCompareParams)
        {
            NLPCompare tmpNLPCompare = new NLPCompare();
            tmpNLPCompare.Compare(excelCompareParams.SrcData, excelCompareParams.TarData, excelCompareParams.MinSimilarity, 
                mCompareFuncString, out excelCompareParams.ResultData);

            //如果重复使用这个对象的Compare(),就得用完之后调用Reset()
            tmpNLPCompare.Reset();
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