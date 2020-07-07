using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranningDemo.Model;

namespace TranningDemo.Service
{
    public interface IDataService
    {
        List<ExamClass> ImportData(string filePath);

        int SaveData(string fileName, List<ExamClass> dataList);

        List<ExamClass> Query(List<ExamClass> data, string searchKey);

        List<ExamClass> CompareWith(List<ExamClass> localData, string dataFileName);
    }
}
