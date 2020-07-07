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
    public class FileModel
    {
        public FileModel(IDataService fileService)
        {
            data = new List<ExamClass>();
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            this.fileService = fileService;
        }

        #region Field
        private List<ExamClass> data;

        public List<ExamClass> Data { get => data; set => data = value; }

        readonly IDataService fileService;                     // File read and write interface

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

   
}
