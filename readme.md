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
    SrcFileFullPath = srcFileFullPath,
    TarFileFullPath = tarFileFullPath,
    OutPath = $"{exePath}\\src",
    OutFileName = "out.csv",
    FileHelper = new ExcelHelper(),
};

CommonCompareHelper excelCompare = new CommonCompareHelper(compareParams);
excelCompare.Run();
```
