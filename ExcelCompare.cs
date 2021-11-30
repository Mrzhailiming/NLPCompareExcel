using Base.DelegateDefine;//先用本项目里的common
using NLP;
using System.Collections.Generic;

namespace Compare.Excel
{
    /// <summary>
    /// Excel对比工具
    /// </summary>
    public class ExcelCompare
    {
        /// <summary>
        /// 元素类型是int
        /// </summary>
        //static CompareFuncInt mCompareFuncInt = new CompareFuncInt(CompareFuncInt);

        /// <summary>
        /// 元素类型是string
        /// </summary>
        static CompareFuncString mCompareFuncString = new CompareFuncString(CompareFuncString);

        private float mMinSimilarity = 1.0f;
        /// <summary>
        /// 相似度大于等于多少是认为相同的
        /// </summary>
        public float MinSimilarity
        {
            get { return mMinSimilarity; }
            set { mMinSimilarity = value; }
        }


        public void Run(List<List<string>> src, List<List<string>> tar)
        {
            NLPCompare tmpNLPCompare = new NLPCompare();
            tmpNLPCompare.Compare(src, tar, mCompareFuncString, MinSimilarity);


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
