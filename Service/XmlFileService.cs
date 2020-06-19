using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                    dataList.Add(new ExamClass(item.Element("ClassNo.").Value,
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
                XDocument xmlFile = new XDocument();
                XElement root = new XElement("ExamClassArrangement");
                foreach (var item in dataList)
                {
                    root.Add(new XElement("ClassNo.", item.Id),
                             new XElement("InstituteOfStudents", item.InstituteStudents),
                             new XElement("NumberOfStudents", item.NumberStudents),
                             new XElement("InstituteOfProctors", item.InstituteProctors),
                             new XElement("NumberOfProctors", item.NumberProctors));
                }
                xmlFile.Add(root);
                xmlFile.Save(fullFileName);
            }
            else
                return;
        }
    }
}
