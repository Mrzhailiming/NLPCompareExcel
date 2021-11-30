using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Data
{
    public enum Flags
    {
        Same,       // 相同
        Insert,     // 插入
        Delete,     // 删除
        Update,     // 修改
        Gray,       // 代表另一个二维数组有的本二维数组没有
    }
}
