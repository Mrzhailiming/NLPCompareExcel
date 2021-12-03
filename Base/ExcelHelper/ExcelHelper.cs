using Base;
using Base.Data;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Common
{
    /// <summary>
    /// 读Excel
    /// 写csv
    /// </summary>
    public class ExcelHelper : IFileHelper_Interface
    {
        /// <summary>
        /// 读取Excel文件, 二维list
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="excelData"></param>
        public void Read(string fileFullPath, out List<List<string>> excelData)
        {
            excelData = new List<List<string>>();

            //先注册encoding, 不注册会抛异常  Encoding.GetEncoding();
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        int lineIndex = 0;
                        while (reader.Read())
                        {
                            excelData.Add(new List<string>());

                            for (int i = 0; i < reader.FieldCount; ++i)
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                stringBuilder.Append(reader.GetValue(i));
                                excelData[lineIndex].Add(stringBuilder.ToString());
                            }

                            ++lineIndex;
                        }
                    } while (false);//目前只读一个sheet
                    //} while (reader.NextResult());//调到下一个sheet


                    ////dataset
                    //var dataSet = reader.AsDataSet();

                    //int sheetIndex = -1;
                    //foreach (DataTable dt in dataSet.Tables)
                    //{
                    //    int rowIndex = -1;
                    //    //dt.Rows[rowIndex][colIndex] = newValue;
                    //    foreach (DataRow dr in dt.Rows)
                    //    {
                    //        int itemIndex = -1;
                    //        StringBuilder stringBuilder = new StringBuilder();
                    //        foreach (DataColumn dc in dr.Table.Columns) 
                    //        {
                    //            stringBuilder.Append($"{dr[dc]}\t");
                    //        }
                    //        Console.WriteLine($"{stringBuilder}end");
                    //    }
                    //}

                        
                }
            }

            //吧末尾的空字符串删了

            foreach(List<string> line in excelData)
            {
                do
                {
                    if (string.IsNullOrEmpty(line[line.Count - 1]))
                    {
                        line.RemoveAt(line.Count - 1);
                    }
                } while (string.IsNullOrEmpty(line[line.Count - 1]));
            }
        }

        /// <summary>
        /// 输出二维list到csv文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="Data"></param>
        public void Write(string filePath, string fileName, List<List<CompareResultItem>> CompareResultItems)
        {
            if(null == CompareResultItems)
            {
                return;
            }

            List<List<string>> Data = CompareResultItem2string(CompareResultItems);


            string fileFullPath = $"{filePath}\\{fileName}";

            using (var stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                foreach(List<string> line in Data)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach(string item in line)
                    {
                        stringBuilder.Append(item).Append($",");
                    }
                    stringBuilder.Append($"\r\n");

                    byte[] byteArray = Encoding.Default.GetBytes(stringBuilder.ToString());
                    stream.Write(byteArray);
                    stream.Flush();
                }
            }
        }

        /// <summary>
        /// 输出string的组织方式自己确定
        /// 把List<List<CompareResultItem>> 转为 <List<string>>
        /// </summary>
        /// <param name="CompareCommonResult"></param>
        /// <returns></returns>
        public List<List<string>> CompareResultItem2string(List<List<CompareResultItem>> CompareCommonResult)
        {
            List<List<string>> data = new List<List<string>>();

            foreach (List<CompareResultItem> line in CompareCommonResult)
            {
                List<string> lineData;
                data.Add(lineData = new List<string>());
                foreach (CompareResultItem item in line)
                {
                    if (item.mFlag == Flags.Same)
                    {
                        lineData.Add($"{item.mSrcValue}");
                    }
                    else if (item.mFlag == Flags.Update)
                    {
                        lineData.Add($"{Operation2StringTable.Table[item.mFlag]}:{item.mSrcValue}|{item.mTarValue}");
                    }
                    else if (item.mFlag == Flags.Delete)
                    {
                        lineData.Add($"{Operation2StringTable.Table[item.mFlag]}:{item.mSrcValue}|{OperationString.DELETE}");
                    }
                    else if (item.mFlag == Flags.Gray)
                    {
                        lineData.Add($"{Operation2StringTable.Table[item.mFlag]}:{item.mSrcValue}|{OperationString.DELETE}");
                    }
                    else if (item.mFlag == Flags.Insert)
                    {
                        lineData.Add($"{Operation2StringTable.Table[item.mFlag]}:{OperationString.INSERT}|{item.mTarValue}");
                    }
                }
            }

            return data;
        }
    }
}
