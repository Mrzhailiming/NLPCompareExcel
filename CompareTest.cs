using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class CompareTest
    {
        int srcLen = 0;
        int tarLen = 0;

        int srcIndex = 0;
        int tarIndex = 0;

        public Dictionary<int, List<Flags>> CompareLine(int[] src, int[] tar)
        {
            srcLen = src.Length;
            tarLen = tar.Length;

            // 源和目标文件显示相同的信息，那么Flags长度是两个文件最长的那个（为了两边都能显示flags）
            int maxLen = srcLen > tarLen ? srcLen : tarLen;
            List<Flags> srcFlags = new List<Flags>(maxLen);
            List<Flags> tarFlags = new List<Flags>(maxLen);

            // 初始化flags, 默认标记为gray （在目标文件删除一行的时候，源文件不用动）
            for (int i = 0; i < maxLen; ++i)
            {
                srcFlags.Add(Flags.Gray);
                tarFlags.Add(Flags.Gray);
            }


            Dictionary<int, List<Flags>> ret = new Dictionary<int, List<Flags>>();
            ret.Add(0, srcFlags);
            ret.Add(1, tarFlags);


            while (srcIndex < srcLen && tarIndex < tarLen)
            {
                //1。找相同的两行
                List<KeyValuePair<int, int>> sameInfo = FindSame(src, srcIndex, tar, tarIndex);
                if (null == sameInfo)
                {
                    //1.代表从srcIndex tarIndex开始，两边就不同了

                    //考虑到如果srcIndex tarIndex是文件最后一行
                    if (srcIndex == srcLen - 1 && tarIndex == tarLen - 1)
                    {
                        //跳出循环
                        break;
                    }

                    //2. 两个文件的剩余行数谁多谁少也需要区分
                    //剩余行数多的
                    int moreLinesIndex = srcLen - srcIndex > tarLen - tarIndex ? srcIndex : tarIndex;
                    int moreLinesLen = srcLen - srcIndex > tarLen - tarIndex ? srcLen : tarLen;
                    List<Flags> moreFlags = srcLen - srcIndex > tarLen - tarIndex ? srcFlags : tarFlags;
                    //剩余行数少的
                    int lessLinesIndex = srcLen - srcIndex < tarLen - tarIndex ? srcIndex : tarIndex;
                    int lessLinesLen = srcLen - srcIndex < tarLen - tarIndex ? srcLen : tarLen;
                    List<Flags> lessFlags = srcLen - srcIndex < tarLen - tarIndex ? srcFlags : tarFlags;

                    //需要标记修改(Update)的行数
                    int updateNum = srcLen - srcIndex < tarLen - tarIndex ? srcLen - srcIndex : tarLen - tarIndex;

                    // 标记修改 Update
                    for (int i = 1; i < updateNum; ++i)
                    {
                        moreFlags[moreLinesIndex + i] = Flags.Update;
                        lessFlags[lessLinesIndex + i] = Flags.Update;
                    }
                    //2.1 标记Insert 和 Gary
                    //剩余行数多的标记 Insert （可能是源文件，也可能是目标文件）
                    for (int i = moreLinesIndex + updateNum; i < moreLinesLen; ++i)
                    {
                        moreFlags[i] = Flags.Insert;
                    }
                    //剩余行数少的标记 Gary
                    for (int i = lessLinesIndex + updateNum; i < lessLinesLen; ++i)
                    {
                        lessFlags[i] = Flags.Gray;
                    }

                    //跳出循环
                    break;
                }

                //key存的源文件的行号, value存的目标文件的行号
                KeyValuePair<int, int> beginInfo = sameInfo[0];
                KeyValuePair<int, int> endInfo = sameInfo[1];

                //源文件
                int srcCount = endInfo.Key - beginInfo.Key;
                //目标文件
                int tarCount = endInfo.Value - beginInfo.Value;

                //插入或删除了几行
                int changeCount = Math.Abs(srcCount - tarCount);

                //1.相同的两行是相邻的，行数相同且srcCount == 1
                if (changeCount == 0 && srcCount == 1)
                {
                    //index不要忘了改
                    srcIndex = endInfo.Key;
                    tarIndex = endInfo.Value;
                    continue;
                }

                //2.相同的两行是不相邻
                //2.1 源和目标中间行数一样，即修改
                if (srcCount == tarCount)
                {
                    //考虑到行号可能不同， 分开标记
                    //源
                    for (int i = beginInfo.Key + 1; i < endInfo.Key; ++i)
                    {
                        srcFlags[i] = Flags.Update;//标记为修改
                    }
                    //目标
                    for (int i = beginInfo.Value + 1; i < endInfo.Value; ++i)
                    {
                        tarFlags[i] = Flags.Update;//标记为修改
                    }
                }
                //2.2 插入
                else if (srcCount < tarCount)
                {
                    for (int i = 1; i <= changeCount; ++i)
                    {
                        //是插入, 源文件要标记为Gary， 默认是gray， 不需要动srcflags
                        if (IsInsert(src[srcIndex], tar[tarIndex + i]))
                        {
                            //srcFlags[srcIndex + i] = 1;//标记为插入
                            tarFlags[tarIndex + i] = Flags.Insert;//标记为插入
                        }
                        //不是插入
                        else
                        {
                            ++srcIndex;
                        }
                    }

                }
                //2.3 删除
                else
                {
                    for (int i = 1; i <= changeCount; ++i)
                    {
                        //是删除，目标文件要标记为Gary， 默认是gray， 不需要动tarflags
                        if (IsDelete(src[srcIndex + i], tar[tarIndex]))
                        {
                            srcFlags[srcIndex + i] = Flags.Delete;//标记为删除，
                            //tarFlags[srcIndex + i] = 1;//删除，标记为-1 删除tar怎么搞，回头整
                        }
                        //不是删除, 目标index往后走
                        else
                        {
                            ++tarIndex;
                        }
                    }
                }

                srcIndex = endInfo.Key;
                tarIndex = endInfo.Value;
            }

            //while结束的index都会走到头

            Reset();
            return ret;
        }

        bool IsInsert(int srcNum, int tarNum)
        {
            return srcNum != tarNum;
        }

        bool IsDelete(int srcNum, int tarNum)
        {
            return srcNum != tarNum;
        }

        /// <summary>
        /// 
        /// </summary>
        List<KeyValuePair<int, int>> FindSame(int[] src, int srcIndex, int[] tar, int tarIndex)
        {
            List<KeyValuePair<int, int>> ret = new List<KeyValuePair<int, int>>();
            while (srcIndex < srcLen && tarIndex < tarLen)
            {
                if (src[srcIndex] == tar[tarIndex])
                {
                    //key存的源文件的行号, value存的目标文件的行号
                    KeyValuePair<int, int> tmp = new KeyValuePair<int, int>(srcIndex, tarIndex);
                    ret.Add(tmp);

                    if (ret.Count >= 2)
                    {
                        return ret;
                    }
                }

                ++srcIndex;
                ++tarIndex;
            }



            //有一边达到文档末尾
            while (srcIndex < srcLen)//tar到达文件末尾
            {
                --tarIndex;//挪到tar文件末尾

                if (src[srcIndex] == tar[tarIndex])
                {
                    //key存的源文件的行号, value存的目标文件的行号
                    KeyValuePair<int, int> tmp = new KeyValuePair<int, int>(srcIndex, tarIndex);
                    ret.Add(tmp);

                    if (ret.Count >= 2)
                    {
                        return ret;
                    }
                }

                ++srcIndex;
            }

            while (tarIndex < tarLen)//src到达文件末尾
            {
                --srcIndex;//挪到src文件末尾

                if (src[srcIndex] == tar[tarIndex])
                {
                    //key存的源文件的行号, value存的目标文件的行号
                    KeyValuePair<int, int> tmp = new KeyValuePair<int, int>(srcIndex, tarIndex);
                    ret.Add(tmp);

                    if (ret.Count >= 2)
                    {
                        return ret;
                    }
                }

                ++tarIndex;
            }

            //代表从srcIndex tarIndex开始，两边就不同了
            return null;
        }


        private void Reset()
        {
            srcLen = 0;
            tarLen = 0;

            srcIndex = 0;
            tarIndex = 0;
        }
    }
}
