using Base.Data;
using Base.DelegateDefine;
using System;
using System.Collections.Generic;
using System.Text;

namespace NLP
{
    /// <summary>
    /// 输出两个二维数组的不同(根据最小编辑距离的策略)
    /// </summary>
    public class NLPCompare
    {
        /// <summary>
        /// 存放二维数组最小编辑距离table
        /// </summary>
        int[,] mTable = null;
        /// <summary>
        /// 存放二维数组标记的策略table
        /// </summary>
        PairResult[,] mFlagsTable = null;
        /// <summary>
        /// 存放两个二维数组的对比结果, 
        /// 例目标数组[i, j]元素被修改 那么本二维数组[i,j]的元素即为"U:源:目标"
        /// 例目标数组[i, j]元素被删除 那么本二维数组[i,j]的元素即为"D:源:-.-"
        /// 例目标数组[i, j]元素是增加 那么本二维数组[i,j]的元素即为"I:^.^:目标"
        /// 定义在 OperationDefaultString 类中
        /// </summary>
        List<List<string>> mCompareResult = new List<List<string>>();

        /// <summary>
        /// 保存二维数组的行数
        /// </summary>
        int srcLen = -1;
        /// <summary>
        /// 保存二维数组的列数
        /// </summary>
        int tarLen = -1;

        /// <summary>
        /// !!!!!!!!!对比完一次就手动重置!!!!!!!!!!
        ///  todo : reset之前吧自己想要的信息导出来
        /// </summary>
        public void Reset()
        {
            mTable = null;
            mFlagsTable = null;
            mCompareResult = new List<List<string>>();
            srcLen = -1;
            tarLen = -1;
        }

        /// <summary>
        /// 对比函数
        /// </summary>
        /// <param name="srcList">源二维数组</param>
        /// <param name="tarList">目标二维数组</param>
        /// <param name="compareFunc">二维数组元素是否相同的比较函数</param>
        /// <param name="minSimilarity">两个二维数组的每一行相似度大于多少才认为是相同的一行</param>
        public void Compare(List<List<string>> srcList, List<List<string>> tarList,
            float minSimilarity, CompareFuncString compareFunc, out List<List<string>> compareResult)
        {
            srcLen = srcList.Count;
            tarLen = tarList.Count;
            mTable = new int[srcLen + 1, tarLen + 1];//[i,j]处最小编辑距离

            mFlagsTable = new PairResult[srcLen + 1, tarLen + 1];//[i,j]处最小编辑距离下的最优标记策略

            InitDistanceTable(mTable, srcLen, tarLen);
            InitTwoDimensionFlagsTable(srcList, tarList, mFlagsTable, srcLen, tarLen);

            for (int i = 1; i <= srcLen; ++i)
            {
                int srcValueIndex = i - 1;
                for (int j = 1; j <= tarLen; ++j)
                {
                    int tarValueIndex = j - 1;

                    int deleteDis = mTable[i - 1, j] + OperationWeight.DELETE;//删除
                    int insertDis = mTable[i, j - 1] + OperationWeight.INSERT;//插入
                    PairResult updateFlagResult;//只要两组元素不完全相同,就赋值
                    bool sameFlag = CompareRow(srcList[srcValueIndex], tarList[tarValueIndex], compareFunc, out updateFlagResult, minSimilarity);
                    int updateDis = mTable[i - 1, j - 1] + (sameFlag ? 0 : OperationWeight.UPDATE);//修改

                    mTable[i, j] = Math.Min(deleteDis, Math.Min(insertDis, updateDis));

                    //是哪一种操作 删除:往下走 增加:往右走
                    //删除
                    PairResult newPairResult;
                    if (deleteDis < insertDis && deleteDis < updateDis)
                    {
                        newPairResult = new PairResult(mFlagsTable[i - 1, j]);
                        //源二维数组这个值标记为被删除
                        newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Delete, mValue = srcList[srcValueIndex] });
                        //目标二维数组加空值
                        newPairResult.TarResult.Add(new Item() { mFlags = Flags.Gray });

                    }
                    //插入
                    else if (insertDis < deleteDis && insertDis < updateDis)
                    {
                        newPairResult = new PairResult(mFlagsTable[i, j - 1]);
                        //源二维数组加空值
                        newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Gray });
                        //目标二维数组标记为插入
                        newPairResult.TarResult.Add(new Item() { mFlags = Flags.Insert, mValue = tarList[tarValueIndex] });
                    }
                    //修改或者不变
                    else
                    {
                        newPairResult = new PairResult(mFlagsTable[i - 1, j - 1]);
                        //两组元素相同, 即相似度>=预定相似度
                        if (sameFlag)
                        {
                            //源二维数组标记为same
                            newPairResult.SrcResult.Add(new Item()
                            {
                                mFlags = Flags.Same,
                                mValue = srcList[srcValueIndex],
                                mRowUpdateFlags = updateFlagResult // 在两组元素完全相同时为空
                            });

                            //目标二维数组标记为same
                            newPairResult.TarResult.Add(new Item()
                            {
                                mFlags = Flags.Same,
                                mValue = tarList[tarValueIndex],
                                mRowUpdateFlags = updateFlagResult // 在两组元素完全相同时为空
                            });

                        }
                        //两组元素不同, 即相似度<预定相似度
                        else
                        {
                            //源二维数组标记为Update 并存储二维数组这一行的对比结果
                            newPairResult.SrcResult.Add(new Item()
                            {
                                mFlags = Flags.Update,
                                mValue = srcList[srcValueIndex],
                                mRowUpdateFlags = updateFlagResult
                            });
                            //目标二维数组标记为Update 并存储二维数组这一行的对比结果
                            newPairResult.TarResult.Add(new Item()
                            {
                                mFlags = Flags.Update,
                                mValue = tarList[tarValueIndex],
                                mRowUpdateFlags = updateFlagResult
                            });

                        }
                    }

                    mFlagsTable[i, j] = newPairResult;
                }
            }

            //输出编辑距离
            //PrintDis(srcLen, tarLen);

            //输出差异
            PrintDifferent(srcLen, tarLen);

            //给对比结果赋值
            compareResult = mCompareResult;
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
            int[,] mTable = new int[srcLen + 1, tarLen + 1];//[i,j]最小编辑距离
            PairResult[,] mFlagsTable = new PairResult[srcLen + 1, tarLen + 1];//[i,j]处最小编辑距离下的最优标记策略

            InitDistanceTable(mTable, srcLen, tarLen);
            InitOneDimensionFlagsTable(srcList, tarList, mFlagsTable, srcLen, tarLen);

            for (int i = 1; i <= srcLen; ++i)
            {
                int srcValueIndex = i - 1;
                for (int j = 1; j <= tarLen; ++j)
                {
                    int tarValueIndex = j - 1;

                    int deleteDis = mTable[i - 1, j] + OperationWeight.DELETE;//删除
                    int insertDis = mTable[i, j - 1] + OperationWeight.INSERT;//插入
                    bool sameFlag = compareFuncString(srcList[srcValueIndex], tarList[tarValueIndex]);
                    int updateDis = mTable[i - 1, j - 1] + (sameFlag ? 0 : OperationWeight.UPDATE);//修改

                    mTable[i, j] = Math.Min(deleteDis, Math.Min(insertDis, updateDis));

                    //是哪一种操作 删除:往下走 增加:往右走
                    //删除
                    PairResult newPairResult;
                    if (deleteDis < insertDis && deleteDis < updateDis)
                    {
                        newPairResult = new PairResult(mFlagsTable[i - 1, j]);
                        //源二维数组这个值标记为被删除
                        newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Delete, mValue = srcList[srcValueIndex] });
                        //目标二维数组加空值
                        newPairResult.TarResult.Add(new Item() { mFlags = Flags.Gray });

                    }
                    //插入
                    else if (insertDis < deleteDis && insertDis < updateDis)
                    {
                        newPairResult = new PairResult(mFlagsTable[i, j - 1]);
                        //源二维数组加空值
                        newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Gray });
                        //目标二维数组标记为插入
                        newPairResult.TarResult.Add(new Item() { mFlags = Flags.Insert, mValue = tarList[tarValueIndex] });
                    }
                    //修改或者不变
                    else
                    {
                        newPairResult = new PairResult(mFlagsTable[i - 1, j - 1]);
                        //值相同
                        if (sameFlag)
                        {
                            //源二维数组标记为same
                            newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Same, mValue = srcList[srcValueIndex] });
                            //目标二维数组标记为same
                            newPairResult.TarResult.Add(new Item() { mFlags = Flags.Same, mValue = tarList[tarValueIndex] });

                        }
                        //值不同, 即修改, todo:得标记哪些格子被修改了
                        else
                        {
                            //源二维数组标记为Update
                            newPairResult.SrcResult.Add(new Item() { mFlags = Flags.Update, mValue = srcList[srcValueIndex] });
                            //目标二维数组标记为Update
                            newPairResult.TarResult.Add(new Item() { mFlags = Flags.Update, mValue = tarList[tarValueIndex] });

                        }
                    }

                    mFlagsTable[i, j] = newPairResult;
                }
            }

            Result tarResult = mFlagsTable[srcLen, tarLen].TarResult;
            float curSimilarity = Similarity(tarResult);
            bool isSame = curSimilarity >= minSimilarity;

            //两组元素相似且不完全相同时才给updateFlagsResult赋值
            if (isSame && 1.0 > curSimilarity)
            {
                updateFlagsResult = mFlagsTable[srcLen, tarLen];
            }

            //返回这两行是否相同 相似度>=预定相似度
            return isSame;
        }

        /// <summary>
        /// 输出两二维数组差异
        /// </summary>
        void PrintDifferent(int srcLen, int tarLen)
        {
            Result srcResult = mFlagsTable[srcLen, tarLen].SrcResult;
            Result tarResult = mFlagsTable[srcLen, tarLen].TarResult;

            Console.WriteLine("--srcFile result begin--");
            PrintOneFile(srcResult, true);
            Console.WriteLine("--srcFile result end--\r\n");

            Console.WriteLine("--tarFile result begin--");
            PrintOneFile(tarResult, false);
            Console.WriteLine("--tarFile result end--");

        }

        /// <summary>
        /// 源二维数组和目标二维数组要分开输出
        /// </summary>
        /// <param name="result"></param>
        /// <param name="isSrcFile">是不是输出源文件</param>
        void PrintOneFile(Result result, bool isSrcFile)
        {
            for (int srcIndex = 0; srcIndex < result.Count; ++srcIndex)
            {
                string messageStr = $"line:{srcIndex} value:{result[srcIndex].mValue} flag:{result[srcIndex].mFlags}";
                Console.WriteLine(messageStr);

                //mRowUpdateFlags不为空, 两组元素就有差异
                PairResult tmpLineResult = result[srcIndex].mRowUpdateFlags;
                if (null != tmpLineResult)//这个判断, 只有不完全相同的两行才会输出 update 或者 same(不完全相同的情况)
                {
                    PrintLineDiff(tmpLineResult, srcIndex, isSrcFile);
                }

                Write(result[srcIndex], srcIndex, isSrcFile);
            }
        }

        /// <summary>
        /// 写入mResult
        /// </summary>
        public void Write(Item lineItem, int lineIndex, bool isSrcFile)
        {
            #region 写入mResult

            List<string> line;

            // 写源文件
            if (mCompareResult.Count <= lineIndex)
            {
                line = new List<string>();
                mCompareResult.Add(line);

                
                {
                    if (lineItem.mFlags == Flags.Same)
                    {
                        foreach (string value in (List<string>)lineItem.mValue)
                        {
                            line.Add(value);
                        }
                    }
                    else if (lineItem.mFlags == Flags.Update) // 代表修改
                    {
                        foreach (string value in (List<string>)lineItem.mValue)
                        {
                            line.Add($"U:{value}");
                        }
                    }
                    else if (lineItem.mFlags == Flags.Insert)
                    {
                        //源数组不会有这个标记
                    }
                    else if (lineItem.mFlags == Flags.Gray)//代表插入 src = "" tar = "abc"
                    {
                        line.Add($"占位");
                        //line.Add($"I:{OperationString.INSERT}");//源二维数组是没有值,也不知道这一行有多少元素,就不在这赋值了
                    }
                    else if (lineItem.mFlags == Flags.Delete)//代表删除 src = "abc" tar = ""
                    {
                        foreach (string value in (List<string>)lineItem.mValue)
                        {
                            line.Add($"D:{value}");
                        }
                    }
                }
            }
            // 写目标文件
            else
            {
                line = mCompareResult[lineIndex];
                int itemIndex = -1;

                
                if (lineItem.mFlags == Flags.Same)//same 就只写源二维数组的值就行了
                {
                    //foreach (string value in (List<string>)lineItem.mValue)
                    //{
                    //    line[++itemIndex] = $"{line[itemIndex]}:{value}";
                    //}
                }
                else if (lineItem.mFlags == Flags.Update) // 代表修改
                {
                    foreach (string value in (List<string>)lineItem.mValue)
                    {
                        line[++itemIndex] = $"{line[itemIndex]}{OperationString.SEPARATOR}{value}";
                    }
                }
                else if (lineItem.mFlags == Flags.Insert)//插入 
                {
                    line.Clear();
                    foreach (string value in (List<string>)lineItem.mValue)
                    {
                        line.Add($"I:{OperationString.INSERT}{OperationString.SEPARATOR}{value}");
                    }
                }
                else if (lineItem.mFlags == Flags.Gray)//代表目标文件的这个元素被删除 src = "abc" tar = ""
                {
                    //lineItem是没有值的, 所以只能遍历源文件里的值
                    //foreach (string value in line)//不能范围for, 因为要修改自己的值
                    for(int index = 0; index < line.Count; ++index)
                    {
                        line[index] = $"{line[index]}{OperationString.SEPARATOR}{OperationString.DELETE}";
                    }
                }
                else if (lineItem.mFlags == Flags.Delete)//代表删除 src = "abc" tar = ""
                {
                    //源数组不会有这个标记
                }
            }

            #endregion 写入mResult
        }

        /// <summary>
        /// 有差异的才会输出
        /// </summary>
        /// <param name="result"></param>
        /// <param name="lineIndex"></param>
        void PrintLineDiff(PairResult pairResult, int lineIndex, bool isSrcFile)
        {
            Result result = isSrcFile ? pairResult.SrcResult : pairResult.TarResult;

            #region 控制台输出

            Console.WriteLine($"----line{lineIndex}-where-update begin----");
            int i = -1;
            foreach (Item item in result)
            {
                string updateStr = $"row:{++i} value:{item.mValue} flag:{item.mFlags}";
                Console.WriteLine(updateStr);
            }
            Console.WriteLine($"----line{lineIndex}-where-update end----");

            #endregion 控制台输出
        }


        /// <summary>
        /// 两组元素的相似度
        /// 如果不是完全相似,还需要输出两组元素的不同
        /// </summary>
        /// <returns></returns>
        public float Similarity(Result tarResult)
        {
            int sameCount = 0;
            foreach (Item item in tarResult)
            {
                if (item.mFlags == Flags.Same)
                {
                    ++sameCount;
                }
            }

            return sameCount / (float)tarResult.Count;
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
        /// 初始化二维flagstable
        /// </summary>
        void InitTwoDimensionFlagsTable(List<List<string>> src, List<List<string>> tar, PairResult[,] flagsTable, int col, int row)
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
                //源二维数组这个值标记为被删除
                colPairResult.SrcResult.Add(new Item() { mFlags = Flags.Delete, mValue = src[srcValueIndex] });
                //目标二维数组加空值
                colPairResult.TarResult.Add(new Item() { mFlags = Flags.Gray });

                flagsTable[colIndex, 0] = colPairResult;
            }

            //第一行, 走插入的逻辑 src = ""  -> tar = "abc"
            for (int rowIndex = 1; rowIndex <= row; ++rowIndex)
            {
                int tarValueIndex = rowIndex - 1;

                PairResult rowPairResult = new PairResult(flagsTable[0, rowIndex - 1]);
                //源二维数组加空值
                rowPairResult.SrcResult.Add(new Item() { mFlags = Flags.Gray });
                //目标二维数组标记为插入
                rowPairResult.TarResult.Add(new Item() { mFlags = Flags.Insert, mValue = tar[tarValueIndex] });

                flagsTable[0, rowIndex] = rowPairResult;
            }
        }

        /// <summary>
        /// 初始化一维FlagsTable
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tar"></param>
        /// <param name="flagsTable"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        void InitOneDimensionFlagsTable(List<string> src, List<string> tar, PairResult[,] flagsTable, int col, int row)
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
                //源二维数组这个值标记为被删除
                colPairResult.SrcResult.Add(new Item() { mFlags = Flags.Delete, mValue = src[srcValueIndex] });
                //目标二维数组加空值
                colPairResult.TarResult.Add(new Item() { mFlags = Flags.Gray });
            }

            //第一行, 走插入的逻辑 src = ""  -> tar = "abc"
            for (int rowIndex = 1; rowIndex <= row; ++rowIndex)
            {
                PairResult rowPairResult = flagsTable[0, rowIndex];
                int tarValueIndex = rowIndex - 1;
                //源二维数组加空值
                rowPairResult.SrcResult.Add(new Item() { mFlags = Flags.Gray });
                //目标二维数组标记为插入
                rowPairResult.TarResult.Add(new Item() { mFlags = Flags.Insert, mValue = tar[tarValueIndex] });
            }
        }
    }
}
