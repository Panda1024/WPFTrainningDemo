using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Forms;
using Ninject;
using Ninject.Modules;
using TranningDemo.Service;

namespace TranningDemo.Model
{
    public class DataSource: ObservableObject
    {
        public DataSource()
        {
            data = new List<ExamClass>();
            fileService = new XmlFileService();
        }

        public DataSource(IFileIO fileService)
        {
            data = new List<ExamClass>();
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            this.fileService = fileService;
        }

        #region Field
        private List<ExamClass> data;

        public List<ExamClass> Data { get => data; set => data = value; }

        readonly IFileIO fileService;                     // File read and write interface

        internal static int index = 0;
        #endregion

        #region Method

        public void ImportData(string fullFileName)
        {
            data = this.fileService.ImportData(fullFileName);
        }

        public void SaveData(string fullFileName)
        {
            this.fileService.SaveData(fullFileName, data);
        }

        public ExamClass GetById(int id)
        {
            var model =  data.Find(item => item.Id == id);
            if (model != null)
                return model.DeepClone();
            return null;
        }

        public void Add(ExamClass examClass)
        {
            data.Add(examClass);
        }

        public void Insert(int index, ExamClass examClass)
        {
            if (index < 0)
                index = 0;
            else if (index > data.Count)
                index = data.Count;
            data.Insert(index, examClass);
        }
        public void Delete(int id)
        {
            var element = data.Find(item => item.Id == id);
            if (element != null)
                data.Remove(element);
        }

        public List<ExamClass> SerachByClassNo(string classNo)
        {
            if (data == null || data.Count == 0)
                return null;
            return data.Where(s => s.ClassNo.Contains(classNo)).ToList();
        }
        #endregion
    }

    public class DataSourceModule : NinjectModule
    {
        private readonly string mode;
        public DataSourceModule(string mode)
        {
            this.mode = mode;
        }

        public override void Load()
        {
            switch (mode) {
                case "xml":
                    Bind<IFileIO>().To<XmlFileService>();
                    break;
                case "json":
                    Bind<IFileIO>().To<JsonFileService>();
                    break;
            }
                
        }
    }
}
