using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<ExamClass> Data { get => data; }

        readonly IFileIO fileService;                     // File read and write interface

        internal static uint index = 0;
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

        public ExamClass GetById(uint id)
        {
            var model =  data.Find(item => item.Id == id);
            if (model == null)
                return null;
            return model.DeepClone();
        }

        public void Insert(int index, ExamClass examClass)
        {
            if (index == -1)
                return;
            data.Insert(index, examClass);
        }

        public bool Delete(uint id)
        {
            var element = data.Find(item => item.Id == id);
            if (element == null)
                return false;
            else
            {
                data.Remove(element);
                return true;
            }
            
        }

        public List<ExamClass> SerachByClassNo(string classNo)
        {
            if (data == null || data.Count == 0)
                return null;
            return data.Where(s => s.ClassNo.Contains(classNo)).ToList();
        }

        public int GetIndex(uint id)
        {
            return data.FindIndex(item => item.Id == id);
        }
        #endregion
    }

    public class DataSourceModule : NinjectModule
    {
        private readonly bool useXml;
        public DataSourceModule(bool useXml)
        {
            this.useXml = useXml;
        }

        public override void Load()
        {
            if (this.useXml)
                Bind<IFileIO>().To<XmlFileService>();
            else
                Bind<IFileIO>().To<JsonFileService>();
        }
    }
}
