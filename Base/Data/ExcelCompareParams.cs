using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Data
{
    /// <summary>
    /// 以二维list的形式存储Excel的数据
    /// </summary>
    public class ExcelCompareParams
    {
        /// <summary>
        /// 源文件
        /// </summary>
        public List<List<string>> SrcData;
        /// <summary>
        /// 目标文件
        /// </summary>
        public List<List<string>> TarData;
        /// <summary>
        /// 对比结果
        /// </summary>
        public List<List<string>> ResultData;

        /// <summary>
        /// 最小相似度, 两个的相似度>=MinSimilarity, 即认为是相同的
        /// </summary>
        public float MinSimilarity { get; set; } = 1.0f;
    }
}
