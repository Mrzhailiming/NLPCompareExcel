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

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return mResultList[i];
            }
        }

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
    /// 要对比的元素
    /// 可以是一行,可以是一个格子
    /// </summary>
    public class Item
    {
        public Flags mFlags;

        public object mValue;

        /// <summary>
        /// 如果这一行标记是update. 就给当前字段赋值
        /// 表示每一行的标记
        /// </summary>
        public PairResult mRowUpdateFlags;
    }


    public delegate bool CompareFuncInt(int src, int tar);
    public delegate bool CompareFuncString(String src, String tar);


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

        int srcLen = -1;
        int tarLen = -1;

        /// <summary>
        /// !!!!!!!!!对比完一次就手动重置!!!!!!!!!!
        /// reset之前吧自己想要的信息导出来
        /// </summary>
        public void Reset()
        {
            mTable = null;
            mFlagsTable = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcList"></param>
        /// <param name="tarList"></param>
        /// <param name="compareFunc"></param>
        /// <param name="minSimilarity">相似度大于多少认为是相同的</param>
        public void Compare(List<List<string>> srcList, List<List<string>> tarList,
            CompareFuncString compareFunc, float minSimilarity = 1.0f)
        {
            srcLen = srcList.Count;
            tarLen = tarList.Count;
            mTable = new int[srcLen + 1, tarLen + 1];//最小编辑距离

            mFlagsTable = new PairResult[srcLen + 1, tarLen + 1];//标记策略

            InitDistanceTable(mTable, srcLen, tarLen);
            InitExcelFlagsTable(srcList, tarList, mFlagsTable, srcLen, tarLen);

            for (int i = 1; i <= srcLen; ++i)
            {
                int srcValueIndex = i - 1;
                for (int j = 1; j <= tarLen; ++j)
                {
                    int tarValueIndex = j - 1;

                    int deleteDis = mTable[i - 1, j] + 1;//删除+1
                    int insertDis = mTable[i, j - 1] + 1;//插入+1
                    PairResult updateFlagResult;
                    bool sameFlag = CompareRow(srcList[srcValueIndex], tarList[tarValueIndex], compareFunc, out updateFlagResult, minSimilarity);
                    int updateDis = mTable[i - 1, j - 1] + (sameFlag ? 0 : 1);//修改+1

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
                        if (sameFlag)
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
                            newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Update, mValue = srcList[srcValueIndex],
                                                                        mRowUpdateFlags = updateFlagResult });
                            //目标文件标记为Update
                            newPairResult.TarResult.Add(new Item() { mFlags = Flags.Update, mValue = tarList[tarValueIndex],
                                                                        mRowUpdateFlags = updateFlagResult });

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
            //Reset();//手动调用吧
        }

        /// <summary>
        /// 判断两行是否是同一行
        /// </summary>
        /// <param name="srcList"></param>
        /// <param name="tarList"></param>
        private bool CompareRow(List<string> srcList, List<string> tarList, CompareFuncString compareFuncString, out PairResult updateFlagsResult, float minSimilarity)
        {
            updateFlagsResult = null;

            //全用临时的
            int srcLen = srcList.Count;
            int tarLen = tarList.Count;
            int[,] mTable = new int[srcLen + 1, tarLen + 1];//最小编辑距离
            PairResult[,] mFlagsTable = new PairResult[srcLen + 1, tarLen + 1];//标记策略

            InitDistanceTable(mTable, srcLen, tarLen);
            InitLineFlagsTable(srcList, tarList, mFlagsTable, srcLen, tarLen);

            for (int i = 1; i <= srcLen; ++i)
            {
                int srcValueIndex = i - 1;
                for (int j = 1; j <= tarLen; ++j)
                {
                    int tarValueIndex = j - 1;

                    int deleteDis = mTable[i - 1, j] + 1;//删除+1
                    int insertDis = mTable[i, j - 1] + 1;//插入+1
                    bool sameFlag = compareFuncString(srcList[srcValueIndex], tarList[tarValueIndex]);
                    int updateDis = mTable[i - 1, j - 1] + (sameFlag ? 0 : 1);//修改+1

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
                        if (sameFlag)
                        {
                            //源文件标记为same
                            newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Same, mValue = srcList[srcValueIndex] });
                            //目标文件标记为same
                            newPairResult.TarResult.Add(new Item() { mFlags = Flags.Same, mValue = tarList[tarValueIndex] });

                        }
                        //值不同, 即修改, todo:得标记哪些格子被修改了
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

            Result tarResult = mFlagsTable[srcLen, tarLen].TarResult;
            updateFlagsResult = mFlagsTable[srcLen, tarLen];

            //返回这两行是否相同 相似度>=预定相似度
            return Similarity(tarResult) >= minSimilarity;
        }

        /// <summary>
        /// 输出两文件差异
        /// </summary>
        void PrintDifferent(int srcLen, int tarLen)
        {
            Result srcResult = mFlagsTable[srcLen, tarLen].SrcResult;
            Result tarResult = mFlagsTable[srcLen, tarLen].TarResult;
         
            Console.WriteLine("--srcFile result begin--");
            PrintOneFile(srcResult);
            Console.WriteLine("--srcFile result end--\r\n");

            Console.WriteLine("--tarFile result begin--");
            PrintOneFile(tarResult);
            Console.WriteLine("--tarFile result end--");

        }

        void PrintOneFile(Result result)
        {
            for (int srcIndex = 0; srcIndex < result.Count; ++srcIndex)
            {
                string srcStr = $"line:{srcIndex} value:{result[srcIndex].mValue} flag:{result[srcIndex].mFlags}";
                Console.WriteLine(srcStr);

                //这一行如果是更改,输出哪些更改了
                if (result[srcIndex].mFlags == Flags.Update)
                {
                    PairResult tmpLineResult = result[srcIndex].mRowUpdateFlags;

                    PrintLineDiff(tmpLineResult, srcIndex);
                }
            }
        }
        void PrintLineDiff(PairResult result, int lineIndex)
        {
            Console.WriteLine($"----line{lineIndex}-where-update begin----");

            Result srcRst = result.SrcResult;
            int i = -1;
            foreach (Item item in srcRst)
            {
                string updateStr = $"line:{++i} value:{item.mValue} flag:{item.mFlags}";
                Console.WriteLine(updateStr);
            }
            Console.WriteLine($"----line{lineIndex}-where-update end----");
        }


        /// <summary>
        /// 两组元素的相似度
        /// </summary>
        /// <returns></returns>
        public float Similarity(Result tarResult)
        {
            int sameCount = 0;
            foreach(Item item in tarResult)
            {
                if(item.mFlags == Flags.Same)
                {
                    ++sameCount;
                }
            }

            return sameCount / tarResult.Count;
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
        /// 只把第一行和第一列设置
        /// </summary>
        void InitDistanceTable(int[,] table, int col, int row)
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
        /// 全部需要new对象,其实也不用全部new吧...
        /// 初始状态有问题,改
        /// </summary>
        void InitExcelFlagsTable(List<List<string>> src, List<List<string>> tar, PairResult[,] flagsTable, int col, int row)
        {
            //注:第一列和第一行需要给一个初始状态

            //1.先把所有的状态new出来
            //第一列
            for (int colIndex = 0; colIndex <= col; ++colIndex)
            {
                //第一行
                for (int rowIndex = 0; rowIndex <= row; ++rowIndex)
                {
                    flagsTable[colIndex, rowIndex] = new PairResult();
                }
            }

            //2.把第一行和第一列的状态初始化了
            //[0,0]位置特殊, 不需要添加item
            //第一列, 走删除的逻辑 src = "abc" -> tar = ""
            for (int colIndex = 1; colIndex <= col; ++colIndex)
            {
                int srcValueIndex = colIndex - 1;

                PairResult colPairResult = new PairResult(flagsTable[colIndex - 1, 0]);
                //源文件这个值标记为被删除
                colPairResult.SrcResult.Add(new Item() { mFlags = Flags.Delete, mValue = src[srcValueIndex] });
                //目标文件加空值
                colPairResult.TarResult.Add(new Item() { mFlags = Flags.Gray });

                flagsTable[colIndex, 0] = colPairResult;
            }

            //第一行, 走插入的逻辑 src = ""  -> tar = "abc"
            for (int rowIndex = 1; rowIndex <= row; ++rowIndex)
            {
                int tarValueIndex = rowIndex - 1;

                PairResult rowPairResult = new PairResult(flagsTable[0, rowIndex - 1]);
                //源文件加空值
                rowPairResult.SrcResult.Add(new Item() { mFlags = Flags.Gray });
                //目标文件标记为插入
                rowPairResult.TarResult.Add(new Item() { mFlags = Flags.Insert, mValue = tar[tarValueIndex] });

                flagsTable[0, rowIndex] = rowPairResult;
            }
        }

        void InitLineFlagsTable(List<string> src, List<string> tar, PairResult[,] flagsTable, int col, int row)
        {
            //注:第一列和第一行需要给一个初始状态

            //1.先把所有的状态new出来
            //第一列
            for (int colIndex = 0; colIndex <= col; ++colIndex)
            {
                //第一行
                for (int rowIndex = 0; rowIndex <= row; ++rowIndex)
                {
                    flagsTable[colIndex, rowIndex] = new PairResult();
                }
            }

            //2.把第一行和第一列的状态初始化了
            //[0,0]位置特殊, 不需要添加item

            //第一列, 走删除的逻辑 src = "abc" -> tar = ""
            for (int colIndex = 1; colIndex <= col; ++colIndex)
            {
                PairResult colPairResult = flagsTable[colIndex, 0];
                int srcValueIndex = colIndex - 1;
                //源文件这个值标记为被删除
                colPairResult.SrcResult.Add(new Item() { mFlags = Flags.Delete, mValue = src[srcValueIndex] });
                //目标文件加空值
                colPairResult.TarResult.Add(new Item() { mFlags = Flags.Gray });
            }

            //第一行, 走插入的逻辑 src = ""  -> tar = "abc"
            for (int rowIndex = 1; rowIndex <= row; ++rowIndex)
            {
                PairResult rowPairResult = flagsTable[0, rowIndex];
                int tarValueIndex = rowIndex - 1;
                //源文件加空值
                rowPairResult.SrcResult.Add(new Item() { mFlags = Flags.Gray });
                //目标文件标记为插入
                rowPairResult.TarResult.Add(new Item() { mFlags = Flags.Insert, mValue = tar[tarValueIndex] });
            }
        }
    }
}
