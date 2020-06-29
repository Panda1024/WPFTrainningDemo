using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System;
using Threading = System.Threading;
using System.Windows.Forms;
using System.IO;
using Ninject;
using TranningDemo.Model;
using TranningDemo.View;

namespace TranningDemo.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(string dataFileName)
        {
            this.dataFileName = dataFileName;
            IKernel kernel = new StandardKernel(new DataSourceModule(true));
            dataSource = kernel.Get<DataSource>();
            dataSource.ImportData(dataFileName);
            this.Query();

            SearchCommand = new RelayCommand(Query);
            ClearCommand = new RelayCommand(ClearSearchBar);
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand<ExamClass>(Edit);
            DeleteCommand = new RelayCommand<ExamClass>(Delete);

            StartListenFileTime();
        }

        #region Field
        /* Scope: 内部
         * Description: 窗口间共享的数据文件 */
        private string dataFileName;

        /* Scope: 内部
         * Description: 数据文件最后修改时间 */
        private DateTime lastAccessTime;

        private Threading.Timer timer;

        /* Scope: 内部
         * Description:  */
        private event Action<string> upLoadToFile;

        /* Scope: 内部
         * Description: 窗口后台数据*/
        private DataSource dataSource;

        /* Scope: 窗口绑定
         * Description: 搜索栏词条*/
        private string searchKey = string.Empty;
        public string SearchKey
        {
            get { return searchKey; }
            set { searchKey = value; RaisePropertyChanged(); }
        }

        /* Scope: 窗口绑定
         * Description: DataGrid绑定数据集合*/
        private ObservableCollection<ExamClass> gridModelList;
        public ObservableCollection<ExamClass> GridModelList
        {
            get { return gridModelList; }
            set { gridModelList = value; RaisePropertyChanged(); }
        }

        /* Scope: 窗口绑定
         * Description: 底部状态栏打印信息*/
        private string printText;
        public string PrintText
        {
            get { return printText; }
            set { printText = value; RaisePropertyChanged(); }
        }
        #endregion

        /* Scope: 窗口绑定
         * Description: 按钮绑定命令*/
        #region Command
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand<ExamClass> EditCommand { get; set; }
        public RelayCommand<ExamClass> DeleteCommand { get; set; }

        #endregion

        #region Method

        /* Scope: 命令绑定
         * Description: 根据词条查询元素，将查找到的元素覆盖到DataGrid中 */
        private void Query()
        {
            var models = dataSource.SerachByClassNo(SearchKey);
            GridModelList = new ObservableCollection<ExamClass>();
            if (models != null)
            {
                models.ForEach(arg =>
                {
                    GridModelList.Add(arg);
                });
            }
        }

        /* Scope: 绑定命令
         * Description: 清除搜索栏，并刷新DataGird */
        private void ClearSearchBar()
        {
            SearchKey = string.Empty;
            this.Query();
        }

        /* Scope: 绑定命令
         * Description: 打开子窗口，编辑新元素并添加到后台和前台数据中 */
        private void Add()
        {
            ExamClass newModel = new ExamClass();
            UserEditWindowView view = new UserEditWindowView(ref newModel);
            var v = view.ShowDialog();
            if(v.Value)
            {
                dataSource.Insert(0, newModel);
                gridModelList.Insert(0, newModel);
                upLoadToFile(dataFileName);
            }
        }

        /* Scope: 绑定命令
         * Description: 根据选中行元素的ID，打开子窗口，编辑后台和前台数据中对应元素 */
        private void Edit(ExamClass selectedCell)
        {         
            if (selectedCell != null)
            {
                var editModel = selectedCell.DeepClone();
                UserEditWindowView view = new UserEditWindowView(ref editModel);
                var v = view.ShowDialog();
                if (v.Value)
                {
                    int index = dataSource.Data.FindIndex(item => item.Id == selectedCell.Id);
                    dataSource.Delete(selectedCell.Id);
                    dataSource.Data.Insert(index, editModel);
                    index = gridModelList.IndexOf(selectedCell);
                    gridModelList.Remove(selectedCell);
                    gridModelList.Insert(index, editModel);
                    upLoadToFile(dataFileName);
                }
            }
        }

        /* Scope: 绑定命令
         * Description: 根据选中行元素的ID，删除后台和前台数据中对应元素 */
        private void Delete(ExamClass selectedCell)
        {
            var model = dataSource.GetById(selectedCell.Id);
            if (model != null)
            {
                var v = MessageBox.Show("Are you sure to delete the selected cell?",  // Mention string
                                        "Delete",                                     // Title
                                        MessageBoxButtons.YesNo,                      // Button: Yes, No
                                        MessageBoxIcon.Question,                      // Icon: ?
                                        MessageBoxDefaultButton.Button2);             // DefaultButton: No
                
                if (v == DialogResult.Yes)
                {
                    dataSource.Delete(model.Id);
                    gridModelList.Remove(selectedCell);
                    upLoadToFile(dataFileName);
                }
                    
            }
        }

        /* Scope: 内部
         * Description: 启动定时器，周期0.5s。当最后修改时间发生变化时，更新后台数据 */
        private void StartListenFileTime()
        {
            lastAccessTime = File.GetLastAccessTime(dataFileName);
            upLoadToFile += (dataFileName) => dataSource.SaveData(dataFileName);
            /* 回调函数：ListenFileTimer，参数传递：无，立即启动，周期500ms */
            timer = new Threading.Timer(new Threading.TimerCallback(ListenFileTime), timer, 0, 500);
        }

        /* Scope: 内部
         * Description: 监听数据文件的修改时间，周期0.5s。当最后修改时间发生变化时，更新后台数据 */
        private void ListenFileTime(object obj)
        {
            DateTime fileAccessTime = File.GetLastAccessTime(dataFileName);
            PrintText = String.Format("Last Access Time: {0}, Current Access Time: {1}", lastAccessTime.ToString(), fileAccessTime.ToString());
            if (DateTime.Compare(fileAccessTime, lastAccessTime) > 0)
            {
                dataSource.ImportData(dataFileName);
                this.Query();
                lastAccessTime = fileAccessTime;
            }
        }

        ~MainWindowViewModel()
        {
            timer.Dispose();        // 回收定时器
        }
        #endregion

    }

}