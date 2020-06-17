using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranningDemo.Model;

namespace TranningDemo.Service
{
    interface ISaveData
    {
        void SaveData(string fileName, List<ExamClass> dataList);
    }
}
