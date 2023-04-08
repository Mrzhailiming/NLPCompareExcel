using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Base.Data
{
    /// <summary>
    /// 以二维list的形式存储Excel的数据
    /// </summary>
    public class CompareParams
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
        /// 虚假的对比结果
        /// </summary>
        public List<List<string>> ResultData;

        /// <summary>
        /// 真正的对比结果
        /// </summary>
        public List<List<CompareResultItem>> CompareCommonResult;

        /// <summary>
        /// 最小相似度, 两个的相似度>=MinSimilarity, 即认为是相同的
        /// </summary>
        public float MinSimilarity { get; set; } = 1.0f;

        /// <summary>
        /// 读写文件
        /// </summary>
        public IFileHelper_Interface FileHelper { get; set; } = null;

        /// <summary>
        /// 源文件完整路径
        /// </summary>
        public string SrcFileFullPath { get; set; } = "";

        /// <summary>
        /// 目标文件完整路径
        /// </summary>
        public string TarFileFullPath { get; set; } = "";

        /// <summary>
        /// 对比结果输出路径
        /// </summary>
        public string OutPath { get; set; } = "";

        /// <summary>
        /// 对比结果输出文件名, 带后缀
        /// </summary>
        public string OutFileName{ get; set; } = "";


        public bool CanUse
        {
            get
            {
                return Check();
            }
            private set { }
        }

        private bool Check()
        {

            bool ret = true;

            if(MinSimilarity < 0.0f || MinSimilarity > 1.0f)
            {
                Console.WriteLine("MinSimilarity 范围在 0 - 1 之间");
                ret = false;
            }

            if (Directory.Exists(OutPath))
            {
                Directory.CreateDirectory(OutPath);
                //Console.WriteLine($"OutPath not exist:{OutPath}");
                //ret = false;
            }

            if (!File.Exists(SrcFileFullPath))
            {
                Console.WriteLine($"SrcFileFullPath not exist:{SrcFileFullPath}");
                ret = false;
            }

            if (!File.Exists(TarFileFullPath))
            {
                Console.WriteLine($"TarFileFullPath not exist:{TarFileFullPath}");
                ret = false;
            }

            if(null == FileHelper)
            {
                Console.WriteLine($"FileHelper is null");
                ret = false;
            }

            return ret;
        }
    }
}
