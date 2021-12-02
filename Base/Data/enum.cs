using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Data
{
    public enum Flags
    {
        Same,       // 相同 (>=预定相似度的认为是相同,可能两组元素并不完全相同)
        Insert,     // 插入
        Delete,     // 删除
        Update,     // 修改
        Gray,       // 代表另一个二维数组有的本二维数组没有
    }

    /// <summary>
    /// 不同操作对编辑距离的影响权重
    /// </summary>
    public class OperationWeight
    {
        /// <summary>
        /// 插入
        /// </summary>
        public static int INSERT = 1;
        /// <summary>
        /// 删除
        /// </summary>
        public static int DELETE = 1;
        /// <summary>
        /// 修改
        /// </summary>
        public static int UPDATE = 2;
    }

    /// <summary>
    /// 插入和删除的defaultString
    /// </summary>
    public class OperationString
    {
        /// <summary>
        /// 插入
        /// </summary>
        public static string INSERT = $"_";
        /// <summary>
        /// 删除
        /// </summary>
        public static string DELETE = $"_";
        /// <summary>
        /// 分隔符  例: U:src|tar
        /// </summary>
        public static string SEPARATOR = $"|";
    }


    public class Operation2StringTable
    {

        static Dictionary<Flags, string> mTable = null;
        public static Dictionary<Flags, string> Table
        {
            get
            {
                // 初始化table
                if(null == mTable)
                {
                    mTable = new Dictionary<Flags, string>();
                    mTable.Add(Flags.Update, "U");
                    mTable.Add(Flags.Delete, "D");
                    mTable.Add(Flags.Insert, "I");
                    mTable.Add(Flags.Same, "S");
                    mTable.Add(Flags.Gray, "G");
                }

                return mTable;
            }
        }

    }
}
