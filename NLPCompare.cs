using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Text;

namespace NLP
{
    /// <summary>
    /// 源和目标的标记
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

    public class Result
    {
        public Result(){}


        public Result(Result result)
        {
            //深拷贝
            mResultList = new List<Item>(result.mResultList);
        }

        public List<Item> mResultList = new List<Item>();

        public int Count { get => mResultList.Count; }

        public void Add(Item item)
        {
            mResultList.Add(item);
        }
        public void Clear()
        {
            mResultList.Clear();
        }
        public Item this[int index]
        {
            get
            {
                return mResultList[index];
            }
            set
            {
                mResultList[index] = value;
            }
        }

    }


    /// <summary>
    /// 要对比的一个元素
    /// </summary>
    public class Item
    {
        public Flags mFlags;
        public int mValue;
    }

    /// <summary>
    /// 根据最小编辑距离策略,输出两个文件的不同
    /// 
    /// delete和gray对应
    /// insert和gray对应
    /// same和same对应
    /// update和update对应
    /// </summary>
    public class NLPCompare
    {
        /// <summary>
        /// 存放最小编辑距离
        /// </summary>
        int[,] mTable = null;
        /// <summary>
        /// 存放标记的策略, 源和目标
        /// </summary>
        PairResult[,] mFlagsTable = null;
        void Reset()
        {
            mTable = null;
            mFlagsTable = null;
        }
        public void Compare(List<int> srcList, List<int> tarList)
        {
            int srcLen = srcList.Count;
            int tarLen = tarList.Count;
            mTable = new int[srcLen + 1, tarLen + 1];//最小编辑距离

            mFlagsTable = new PairResult[srcLen + 1, tarLen + 1];//标记策略

            InitTable(mTable, srcLen, tarLen);
            InitFlagsTable(mFlagsTable, srcLen, tarLen);

            for (int i = 1; i <= srcLen; ++i)
            {
                int srcValueIndex = i - 1;
                for (int j = 1; j <= tarLen; ++j)
                {
                    int tarValueIndex = j - 1;

                    int deleteDis = mTable[i - 1, j] + 1;//删除+1
                    int insertDis = mTable[i, j - 1] + 1;//插入+1
                    int updateDis = mTable[i - 1, j - 1] + (ItemCompare(srcList[srcValueIndex], tarList[tarValueIndex]) ? 0 : 1);//修改+1
                    mTable[i, j] = Math.Min(deleteDis, Math.Min(insertDis, updateDis));

                    //是哪一种操作 删除:往下走 增加:往右走
                    //删除
                    PairResult newPairResult;
                    if (deleteDis < insertDis && deleteDis < updateDis)
                    {
                        newPairResult = new PairResult(mFlagsTable[i - 1, j]);
                        //源文件这个值标记为被删除
                        newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Delete, mValue = srcList[srcValueIndex] });
                        //目标文件加空值
                        newPairResult.TarResult.Add(new Item() { mFlags = Flags.Gray });

                    }
                    //插入
                    else if (insertDis < deleteDis && insertDis < updateDis)
                    {
                        newPairResult = new PairResult(mFlagsTable[i, j - 1]);
                        //源文件加空值
                        newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Gray });
                        //目标文件标记为插入
                        newPairResult.TarResult.Add(new Item() { mFlags = Flags.Insert, mValue = tarList[tarValueIndex] });
                    }
                    //修改或者不变
                    else
                    {
                        newPairResult = new PairResult(mFlagsTable[i - 1, j - 1]);
                        //值相同
                        if (ItemCompare(srcList[srcValueIndex], tarList[tarValueIndex]))
                        {
                            //源文件标记为same
                            newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Same, mValue = srcList[srcValueIndex] });
                            //目标文件标记为same
                            newPairResult.TarResult.Add(new Item() { mFlags = Flags.Same, mValue = tarList[tarValueIndex] });

                        }
                        //值不同, 即修改
                        else
                        {
                            //源文件标记为Update
                            newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Update, mValue = srcList[srcValueIndex] });
                            //目标文件标记为Update
                            newPairResult.TarResult.Add(new Item() { mFlags = Flags.Update, mValue = tarList[tarValueIndex] });

                        }
                    }

                    mFlagsTable[i, j] = newPairResult;
                }
            }

            //输出编辑距离
            //PrintDis(srcLen, tarLen);

            //输出差异
            PrintDifferent(srcLen, tarLen);

            //放最后
            Reset();
        }

        /// <summary>
        /// 输出两文件差异
        /// </summary>
        void PrintDifferent(int srcLen, int tarLen)
        {
            Result srcResult = mFlagsTable[srcLen, tarLen].SrcResult;
            Result tarResult = mFlagsTable[srcLen, tarLen].TarResult;
            for (int srcIndex = 0; srcIndex < srcResult.Count; ++srcIndex)
            {
                string srcStr = $"line:{srcIndex} value:{srcResult[srcIndex].mValue} flag:{srcResult[srcIndex].mFlags}";
                Console.WriteLine(srcStr);
            }
            Console.WriteLine("----------------------------------------");
            for (int tarIndex = 0; tarIndex < tarResult.Count; ++tarIndex)
            {
                string tarStr = $"line:{tarIndex} value:{tarResult[tarIndex].mValue} flag:{tarResult[tarIndex].mFlags}";
                Console.WriteLine(tarStr);
            }

        }

        /// <summary>
        /// 输出编辑距离table
        /// </summary>
        void PrintDis(int srcLen, int tarLen)
        {
            for (int i = 0; i <= srcLen; ++i)
            {
                StringBuilder line = new StringBuilder();
                for (int j = 0; j <= tarLen; ++j)
                {
                    line.Append(mTable[i, j]);
                }

                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// 两个元素对比, 相同返回true
        /// </summary>
        bool ItemCompare(int src, int tar)
        {
            return src == tar;
        }

        /// <summary>
        /// 只把第一行和第一列设置
        /// </summary>
        void InitTable(int[,] table, int col, int row)
        {
            //第一列
            for (int i = 0; i <= col; ++i)
            {
                table[i, 0] = i;
            }

            //第一行
            for (int i = 0; i <= row; ++i)
            {
                table[0, i] = i;
            }
        }

        /// <summary>
        /// 全部需要new对象
        /// </summary>
        void InitFlagsTable(PairResult[,] flagsTable, int col, int row)
        {
            //第一列
            for (int colIndex = 0; colIndex <= col; ++colIndex)
            {
                //第一行
                for (int rowIndex = 0; rowIndex <= row; ++rowIndex)
                {
                    flagsTable[colIndex, rowIndex] = new PairResult();
                }
            }
        }
    }
}
