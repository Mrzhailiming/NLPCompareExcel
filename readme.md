### 写在前面
***
1. 读取Excel数据的代码依赖于 https://github.com/ExcelDataReader/ExcelDataReader.git
	1. 包括:ExcelDataReader
	2. ExcelDataReader.DataSet两个项目

### 使用方法
***
```c#
CompareParams compareParams = new CompareParams()
{
	//五个必须赋值的成员
    SrcFileFullPath = srcFileFullPath,	//源文件的完整路径
    TarFileFullPath = tarFileFullPath, 	//目标文件的完整路径
    OutPath = $"{exePath}\\src", 		//输出结果文件的目录
    OutFileName = "out.csv",			//文件名
    FileHelper = new ExcelHelper(),		//自定制读取和写入文件 (继承IFileHelper_Interface, 实现读写文件即可)
};

CommonCompareHelper excelCompare = new CommonCompareHelper(compareParams);
excelCompare.Run();
```
