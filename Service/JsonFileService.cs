using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TranningDemo.Model;


namespace TranningDemo.Service
{
    class JsonFileService : IDataService
    {
        public List<ExamClass> ImportData(string fullFileName)
        {
            if (fullFileName != string.Empty)
            {
                string jsonString = File.ReadAllText(fullFileName);
                Root root = JsonConvert.DeserializeObject<Root>(jsonString);
                Console.WriteLine("成功从 json文件从导入数据");
                return root.ExamClassItem;
            }
            else
            {
                return null;
            }
        }

        public List<ExamClass> Query(List<ExamClass> data, string searchKey)
        {
            if (data == null || data.Count == 0)
                return null;
            return data.Where(s => s.ClassNo.Contains(searchKey)).ToList();
        }

        public int SaveData(string fullFileName, List<ExamClass> dataList)
        {
            if (fullFileName != string.Empty)
            {
                File.WriteAllText(fullFileName, JsonConvert.SerializeObject(dataList));
                return dataList.Count;
            }
            else
            {
                return 0;
            }
        }

        public List<ExamClass> CompareWith(List<ExamClass> localData, string dataFileName)
        {
            List<ExamClass> sqlData = ImportData(dataFileName);
            //sqlData = sqlData.OrderBy(it => it.Id).ToList();
            if (localData.Count != sqlData.Count && sqlData != null)
            {
                localData = sqlData;
                return localData;
            }
            for (int i = 0; i < localData.Count; i++)
            {
                var equal = localData[i].Equals(sqlData[i]);
                if (!equal)
                {
                    localData = sqlData;
                    return localData;
                }
            }
            return null;
        }

        public class Root
        {
            public List<ExamClass> ExamClassItem { get; set; }
        }
    }
}
