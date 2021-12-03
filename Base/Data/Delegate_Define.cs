using Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.DelegateDefine
{
    public delegate bool CompareFuncInt(int src, int tar);
    public delegate bool CompareFuncString(String src, String tar);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CompareCommonResult"></param>
    /// <returns></returns>
    public delegate List<List<string>> CompareResultItem2string(List<List<CompareResultItem>> CompareCommonResult);
}
