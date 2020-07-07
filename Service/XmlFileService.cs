using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using TranningDemo.Model;

namespace TranningDemo.Service
{
    class XmlFileService : IDataService
    {
        public List<ExamClass> ImportData(string fullFileName)
        {
            if (fullFileName != string.Empty)
            {
                List<ExamClass> dataList = new List<ExamClass>();
                XElement file = XElement.Load(fullFileName);
                IEnumerable<XElement> xElements = from item in file.Elements("ExamClass")
                                                  select item;
                foreach (XElement item in xElements)
                {
                    dataList.Add(new ExamClass(int.Parse(item.Element("Id").Value), 
                                            item.Element("ClassNo").Value,
                                            item.Element("InstituteOfStudents").Value,
                                            int.Parse(item.Element("NumberOfStudents").Value),
                                            item.Element("InstituteOfProctors").Value,
                                            int.Parse(item.Element("NumberOfProctors").Value))
                        );
                }
                Console.WriteLine("成功从xml文件从导入数据");
                return dataList;
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

        public int SaveData(string fullFileName, List<ExamClass> dataList )
        {
            if (fullFileName != string.Empty)
            {
                XmlTextWriter xmlWrite= new XmlTextWriter(fullFileName, null);
                xmlWrite.Formatting = Formatting.Indented;
                xmlWrite.WriteStartElement("ExamClassArrangement"); // 添加根元素

                foreach (var item in dataList)
                {
                    xmlWrite.WriteStartElement("ExamClass");    // 添加子元素
                    xmlWrite.WriteElementString("Id", item.Id.ToString());
                    xmlWrite.WriteElementString("ClassNo", item.ClassNo);
                    xmlWrite.WriteElementString("InstituteOfStudents", item.InstituteStudents);
                    xmlWrite.WriteElementString("NumberOfStudents", item.NumberStudents.ToString());
                    xmlWrite.WriteElementString("InstituteOfProctors", item.InstituteProctors);
                    xmlWrite.WriteElementString("NumberOfProctors", item.NumberProctors.ToString());
                    xmlWrite.WriteEndElement();                 // 关闭元素
                }
                xmlWrite.WriteFullEndElement();                 // 关闭根元素
                xmlWrite.Close();
                Console.WriteLine("成功存储数据到xml文件");
                return dataList.Count;
            }
            else
                return 0;
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

    }
}
