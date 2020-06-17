using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TranningDemo.Service;

namespace TranningDemo.Model
{
    public class DataSource: ObservableObject
    {
        public DataSource()
        {
            data = new List<ExamClass>();
            xmlFileService = new XmlFileService();
        }

        private List<ExamClass> data;

        public List<ExamClass> Data { get => data; }

        private XmlFileService xmlFileService;
        #region Method
        public void ImportData(string filePath)
        {
            if (filePath != string.Empty)
                data = xmlFileService.ImportData(filePath);
            else
                return;
        }
        public void SaveData(string saveFileName)
        {
            if (saveFileName != string.Empty)
                xmlFileService.SaveData(saveFileName, data);
            else
                return;
        }
        public ExamClass GetById(string id)
        {
            var model =  data.Find(t => t.Id == id);
            if (model != null)
                return model.Clone();
            return null;
        }
        public void Add(ExamClass examClass)
        {
            data.Add(examClass);
        }

        public void Delete(string id)
        {
            var element = data.Find(s => s.Id == id);
            if (element != null)
                data.Remove(element);
        }

        public List<ExamClass> SerachById(string id)
        {
            return data.Where(s => s.Id.Contains(id)).ToList();
        }
        #endregion
    }
}
