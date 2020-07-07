using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using TranningDemo.Service;

namespace TranningDemo.Model
{
    class DataSource
    {
        public DataSource(IDataService dataService)
        {
            localData = new List<ExamClass>();
            if (dataService == null)
                throw new ArgumentNullException("fileService");
            this.dataService = dataService;
        }


        #region Field

        private List<ExamClass> localdata;

        public List<ExamClass> localData
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

        internal static int index = 0;

        #endregion

        public void ImportData(string fullFileName)
        {
            localData = this.dataService.ImportData(fullFileName);
        }

        public void SaveData(string fullFileName)
        {
            this.dataService.SaveData(fullFileName, localData);
        }

        public List<ExamClass> Search(string searchKey)
        {
            return dataService.Query(localData, searchKey);

        }

        public void Add(ExamClass model)
        {
            try
            {
                /* 操作本地数据 */
                localData.Insert(0, model);
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
                int index = localData.FindIndex(item => item.Id == id);
                localData.RemoveAt(index);
                localData.Insert(index, model);
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
                int index = localData.FindIndex(item => item.Id == id);
                if (index >= 0 && index < localData.Count)
                {
                    localData.RemoveAt(index);
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
            return this.dataService.CompareWith(localData, fullFileName);
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
