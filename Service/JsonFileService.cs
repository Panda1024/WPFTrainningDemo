using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TranningDemo.Model;


namespace TranningDemo.Service
{
    class JsonFileService : IFileIO
    {
        public List<ExamClass> ImportData(string fullFileName)
        {
            if (fullFileName != string.Empty)
            {
                string jsonString = File.ReadAllText(fullFileName);
                Root root = JsonConvert.DeserializeObject<Root>(jsonString);
                return root.ExamClassItem;
            }
            else
            {
                return null;
            }
        }

        public void SaveData(string fullFileName, List<ExamClass> dataList)
        {
            if (fullFileName != string.Empty)
            {
                File.WriteAllText(fullFileName, JsonConvert.SerializeObject(dataList));
            }
            else
            {
                return;
            }
        }

        public class Root
        {
            public List<ExamClass> ExamClassItem { get; set; }
        }
    }
}
