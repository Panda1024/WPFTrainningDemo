using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranningDemo.Model;

namespace TranningDemo.Service
{
    public interface IFileIO
    {
        List<ExamClass> ImportData(string filePath);

        void SaveData(string fileName, List<ExamClass> dataList);
    }
}
