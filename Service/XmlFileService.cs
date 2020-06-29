using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TranningDemo.Model;

namespace TranningDemo.Service
{
    class XmlFileService : IFileIO
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
                    dataList.Add(new ExamClass(item.Element("ClassNo").Value,
                                            item.Element("InstituteOfStudents").Value,
                                            int.Parse(item.Element("NumberOfStudents").Value),
                                            item.Element("InstituteOfProctors").Value,
                                            int.Parse(item.Element("NumberOfProctors").Value))
                        );
                }
                return dataList;
            }
            else
            {
                return null;
            }
        }

        public void SaveData(string fullFileName, List<ExamClass> dataList )
        {
            if (fullFileName != string.Empty)
            {
                XmlTextWriter xmlWrite= new XmlTextWriter(fullFileName, null);
                xmlWrite.Formatting = Formatting.Indented;
                xmlWrite.WriteStartElement("ExamClassArrangement"); // 添加根元素

                foreach (var item in dataList)
                {
                    xmlWrite.WriteStartElement("ExamClass");    // 添加子元素
                    xmlWrite.WriteElementString("ClassNo", item.ClassNo);
                    xmlWrite.WriteElementString("InstituteOfStudents", item.InstituteStudents);
                    xmlWrite.WriteElementString("NumberOfStudents", item.NumberStudents.ToString());
                    xmlWrite.WriteElementString("InstituteOfProctors", item.InstituteProctors);
                    xmlWrite.WriteElementString("NumberOfProctors", item.NumberProctors.ToString());
                    xmlWrite.WriteEndElement();                 // 关闭元素
                }
                xmlWrite.WriteFullEndElement();                 // 关闭根元素
                xmlWrite.Close();
                //File.SetLastAccessTime(fullFileName, DateTime.Now);
            }
            else
                return;
        }
    }
}
