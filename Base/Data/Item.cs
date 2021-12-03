using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Data
{
    /// <summary>
    /// 要对比的元素
    /// 可以是一行,可以是一个格子
    /// </summary>
    public class Item
    {
        public Flags mFlags;

        public object mValue;//可以是list<string> 也可以 string

        /// <summary>
        /// 如果这一行标记是update. 就给当前字段赋值
        /// 表示每一行的标记
        /// </summary>
        public PairResult mRowUpdateFlags;
    }

    /// <summary>
    /// 对比结果(二维数组)的一个元素
    /// </summary>
    public class CompareResultItem
    {
        /// <summary>
        /// mSrcFlag = gary 对应 mTarFlag = insert
        /// mSrcFlag = delete 对应 mTarFlag = gray
        /// mSrcFlag = same 对应 mTarFlag = same
        /// mSrcFlag = update 对应 mTarFlag = update
        /// </summary>
        public Flags mSrcFlag;

        public string mSrcValue;

        /// <summary>
        /// mSrcFlag = gary 对应 mTarFlag = insert
        /// mSrcFlag = delete 对应 mTarFlag = gray
        /// mSrcFlag = same 对应 mTarFlag = same
        /// mSrcFlag = update 对应 mTarFlag = update
        /// </summary>
        public Flags mTarFlag;

        public string mTarValue;
    }
}
