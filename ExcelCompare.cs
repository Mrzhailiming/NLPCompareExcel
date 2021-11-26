using NLP;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{

    

    class ExcelCompare
    {
        static CompareFuncInt mCompareFuncInt = new CompareFuncInt(CompareFuncInt);
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
