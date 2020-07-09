using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranningDemo.Model;
using TranningDemo.View;

namespace TranningDemo.ViewModel
{
    public class StartWindowViewModel : ViewModelBase
    {
        public StartWindowViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile);
            ImportDataCommand = new RelayCommand(ImportXmlToSql);
            SaveFileCommand = new RelayCommand(SaveFile);
            StartMainCommand = new RelayCommand(StartMainView);
            SetMode11Command = new RelayCommand(SetMode11);
            SetMode12Command = new RelayCommand(SetModel2);
            SetMode13Command = new RelayCommand(SetMode13);

            dataSourceMode = "POSTGRESQL";
            pool = new Semaphore(2, 2);
        }

        #region Field

        private static Semaphore pool;

        private string dataFileName;

        private string dataSourceMode;
        #endregion

        #region Command
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand SaveFileCommand { get; set; }
        public RelayCommand ImportDataCommand { get; set; }
        public RelayCommand StartMainCommand { get; set; }
        public RelayCommand SetMode11Command { get; set; }
        public RelayCommand SetMode12Command { get; set; }
        public RelayCommand SetMode13Command { get; set; }
        #endregion

        #region Method
        /* Scope: 绑定命令
         * Description: 打开文件*/
        private void OpenFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"..\..\Data";
                openFileDialog.Filter = "xml files (*.xml)|*.xml|json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataFileName = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }
        }

        private void SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.Filter = "xml files (*.xml)|*.xml|json files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // dataSource.SaveData(saveFileDialog.FileName);
            }
            else
            {
                return;
            }
        }

        private void ImportXmlToSql()
        {
            OpenFile();
            NpgsqlModel npgsqlModel = new NpgsqlModel("trainningdemo");
            npgsqlModel.ImportFromXmlFileToSQL(dataFileName);

        }

        /* Scope: 内部
         * Description: 打开新的主窗口，pool限制了只能同时打开两个窗口 */
        private void StartMainView()
        {
            Thread thread = new Thread(()=>
            {
                pool.WaitOne();
                if (dataFileName == null)
                    dataFileName = @"..\..\Data\ExamClassArrangement.xml";     // 默认文件地址
                MainWindowView view =  new MainWindowView(dataSourceMode, dataFileName);
                view.ShowDialog();
                pool.Release();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void SetMode11()
        {
            dataSourceMode = "XML";
        }

        private void SetModel2()
        {
            dataSourceMode = "JSON";
        }
        private void SetMode13()
        {
            dataSourceMode = "POSTGRESQL";

        }
        #endregion
    }
}
