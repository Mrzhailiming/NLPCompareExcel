using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Data
{
    /// <summary>
    /// 保存源和目标二维数组的对比结果
    /// </summary>
    public class PairResult
    {
        public PairResult()
        {
        }
        public PairResult(PairResult pairResult)
        {
            //深拷贝
            srcResult = new Result(pairResult.SrcResult);
            tarResult = new Result(pairResult.TarResult);
        }
        Result srcResult = new Result();
        /// <summary>
        /// 源
        /// </summary>
        public Result SrcResult { get => srcResult; }

        Result tarResult = new Result();
        /// <summary>
        /// 目标
        /// </summary>
        public Result TarResult { get => tarResult; }
    }
}
