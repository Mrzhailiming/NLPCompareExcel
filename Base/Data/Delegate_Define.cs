using Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.DelegateDefine
{
    /// <summary>
    /// 判断两个int是否相等
    /// </summary>
    public delegate bool CompareFuncInt(int src, int tar);

    /// <summary>
    /// 判断两个字符串是否相等
    /// </summary>
    public delegate bool CompareFuncString(String src, String tar);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CompareCommonResult"></param>
    /// <returns></returns>
    public delegate List<List<string>> CompareResultItem2string(List<List<CompareResultItem>> CompareCommonResult);
}
