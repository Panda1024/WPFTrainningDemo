using Ninject.Modules;
using System;
using System.Collections.Generic;
using TranningDemo.Service;

namespace TranningDemo.Model
{
    public class DataSource
    {
        public DataSource(IDataService dataService)
        {
            LocalData = new List<ExamClass>();
            this.dataService = dataService ?? throw new ArgumentNullException("IDataService");
        }

        public DataSource()
        {
            LocalData = new List<ExamClass>();
            this.dataService = new XmlFileService();
        }

        #region Field

        internal static int index = 0;

        private List<ExamClass> localdata;

        public List<ExamClass> LocalData
        {
            get
            {
                return localdata;
            }
            set
            {
                if (value != null)
                    localdata = value;
            }
        }

        readonly IDataService dataService;                     // File read and write interface

        #endregion

        public void ImportData(string fullFileName)
        {
            LocalData = this.dataService.ImportData(fullFileName);
        }

        public void SaveData(string fullFileName)
        {
            this.dataService.SaveData(fullFileName, LocalData);
        }

        public List<ExamClass> Search(string searchKey)
        {
            return dataService.Query(LocalData, searchKey);

        }

        public void Add(ExamClass model)
        {
            try
            {
                /* 操作本地数据 */
                LocalData.Insert(0, model);
            }
            catch
            {
                Console.WriteLine("Error of adding data");
                return;
            }
        }

        public void Edit(int id, ExamClass model)
        {
            try
            {
                /* 操作本地数据 */
                int ind = LocalData.FindIndex(item => item.Id == id);
                LocalData.RemoveAt(ind);
                LocalData.Insert(ind, model);
            }
            catch
            {
                Console.WriteLine("Error of editing data");
                return;
            }
        }

        public void DeleteByID(int id)
        {
            try
            {
                /* 操作本地数据 */
                int ind = LocalData.FindIndex(item => item.Id == id);
                if (ind >= 0 && ind < LocalData.Count)
                {
                    LocalData.RemoveAt(ind);
                }
            }
            catch
            {
                Console.WriteLine("Error of deleting the element 'Id = {0}' ");
                return ;
            }
        }

        public List<ExamClass> CompareWith(string fullFileName)
        {
            return this.dataService.CompareWith(LocalData, fullFileName);
        }
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
            switch (mode)
            {
                case "XML":
                    Bind<IDataService>().To<XmlFileService>();
                    break;
                case "JSON":
                    Bind<IDataService>().To<JsonFileService>();
                    break;
                case "POSTGRESQL":
                    Bind<IDataService>().To<SQLService>();
                    break;
            }
        }
    }
}
