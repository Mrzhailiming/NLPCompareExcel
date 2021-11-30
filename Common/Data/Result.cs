using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Data
{

    /// <summary>
    /// 保存一行(列)的对比结果
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 保存一行(列)的对比结果
        /// </summary>
        public List<Item> mResultList = new List<Item>();

        public Result() { }

        public Result(Result result)
        {
            //深拷贝
            mResultList = new List<Item>(result.mResultList);
        }

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

        /// <summary>
        /// 向mResultList添加元素
        /// </summary>
        /// <param name="item"></param>
        public void Add(Item item)
        {
            mResultList.Add(item);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            mResultList.Clear();
        }


        /// <summary>
        /// 下标访问
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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

}
