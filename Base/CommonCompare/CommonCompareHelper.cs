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

        private CompareParams mCompareParams = null;

        CommonCompareHelper() { }

        public CommonCompareHelper(CompareParams compareParams)
        {
            mCompareParams = compareParams;
        }

        private void Init()
        {
            if (!mCompareParams.Check())
            {

            }
        }

        public void Run()
        {
            if (!mCompareParams.Check())
            {
                //参数check错误
                Console.WriteLine("mCompareParams 检查错误");
                return;
            }

            NLPCompare tmpNLPCompare = new NLPCompare();

            mCompareParams.FileHelper.Read(mCompareParams.SrcFileFullPath, out mCompareParams.SrcData);
            mCompareParams.FileHelper.Read(mCompareParams.TarFileFullPath, out mCompareParams.TarData);

            tmpNLPCompare.Compare(mCompareParams.SrcData, mCompareParams.TarData, mCompareParams.MinSimilarity,
                mCompareFuncString,/* out mCompareParams.ResultData,*/ out mCompareParams.CompareCommonResult);

            mCompareParams.FileHelper.Write(mCompareParams.OutPath, mCompareParams.OutFileName, mCompareParams.CompareCommonResult);

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
