﻿using Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    /// <summary>
    /// 读写都是二维list, 每个元素为string
    /// 继承本接口,自定义读什么文件,写入什么文件
    /// </summary>
    public interface IFileHelper_Interface
    {
        /// <summary>
        /// 读取文件到二维list
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="excelData">存储文件的内容</param>
        void Read(string fileFullPath, out List<List<string>> excelData);

        /// <summary>
        /// 将二维list写入到文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="Data">对比结果,自定制输出格式</param>
        void Write(string filePath, string fileName, List<List<CompareResultItem>> Data);
    }
}
