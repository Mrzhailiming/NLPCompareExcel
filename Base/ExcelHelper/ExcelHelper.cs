using Base;
using ExcelDataReader;
using System.Collections.Generic;
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
                    } while (reader.NextResult());//调到下一个sheet

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
        public void Write(string filePath, string fileName, List<List<string>> Data)
        {
            if(null == Data)
            {
                return;
            }

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
    }
}
