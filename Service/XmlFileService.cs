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
    class XmlFileService : IImportData, ISaveData
    {
        public List<ExamClass> ImportData(string filePath)
        {
            List<ExamClass>  data = new List<ExamClass>();
            XElement file = XElement.Load(filePath);
            IEnumerable<XElement> xElements = from item in file.Elements("ExamClass")
                                              select item;
            foreach (XElement item in xElements)
            {
                data.Add(new ExamClass(item.Element("ClassNo.").Value,
                                        item.Element("InstituteOfStudents").Value,
                                        int.Parse(item.Element("NumberOfStudents").Value),
                                        item.Element("InstituteOfProctors").Value,
                                        int.Parse(item.Element("NumberOfProctors").Value))
                    );
            }
            return data;
        }

        public void SaveData(string fullFileName, List<ExamClass> dataList )
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
    }
}
