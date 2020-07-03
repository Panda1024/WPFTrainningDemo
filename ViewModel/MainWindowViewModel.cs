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
using System.Linq;

namespace TranningDemo.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(string dataFileName)
        {
            dataSource = new NpgsqlModel("trainningdemo");
            RefreshGrid();

            SearchCommand = new RelayCommand(Query);
            ClearCommand = new RelayCommand(ClearSearchBar);
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand<ExamClass>(Edit);
            DeleteCommand = new RelayCommand<ExamClass>(Delete);

            StartListenSQL();
        }

        #region Field

        /* Scope: 内部
         * Description: 监听数据库的定时器 */
        private Threading.Timer timer;

        /* Scope: 内部
         * Description: 窗口后台数据模型 */
        private NpgsqlModel dataSource;

        /* Scope: 窗口绑定
         * Description: 搜索栏词条*/
        private string searchKey = string.Empty;
        public string SearchKey
        {
            get { return searchKey; }
            set { searchKey = value; RaisePropertyChanged(); }
        }

        /* Scope: 窗口绑定
         * Description: DataGrid绑定列表*/
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
            var searchResult = dataSource.Search(SearchKey);
            GridModelList = new ObservableCollection<ExamClass>();
            if (searchResult != null)
            {
                searchResult.ForEach(arg =>
                {
                    GridModelList.Add(arg);
                });
            }
        }

        /* Scope: 绑定命令 + 内部
         * Description: 清除搜索栏，并刷新DataGird */
        private void ClearSearchBar()
        {
            SearchKey = string.Empty;
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var data = dataSource.localData;
            if(SearchKey == string.Empty)       // 搜索栏有关键词的时候，不更新 DataGrid
            {
                GridModelList = new ObservableCollection<ExamClass>();
                data.ForEach(arg =>
                {
                    GridModelList.Add(arg);
                });
            }
            
        }

        /* Scope: 绑定命令
         * Description: 打开子窗口，编辑新元素并添加到后台和前台数据中 */
        private void Add()
        {
            ExamClass newModel = new ExamClass();
            UserEditWindowView view = new UserEditWindowView(ref newModel);
            var v = view.ShowDialog();
            if (v.Value)
            {
                dataSource.Add(newModel);
                GridModelList.Insert(0, newModel);
            }
        }

        /* Scope: 绑定命令
         * Description: 根据选中行元素的ID，打开子窗口，编辑后台和前台数据中对应元素 */
        private void Edit(ExamClass selectedCell)
        {
            if (selectedCell != null)
            {
                int id = selectedCell.Id;
                var editModel = selectedCell.DeepClone();
                UserEditWindowView view = new UserEditWindowView(ref editModel);
                var v = view.ShowDialog();
                if (v.Value)
                {
                    dataSource.Edit(id, editModel);

                    int index = GridModelList.IndexOf(selectedCell);    // 查找索引值
                    if(index>=0 && index<GridModelList.Count)
                    {
                        GridModelList.RemoveAt(index);                 // 删除元素
                        GridModelList.Insert(index, editModel);        // 在原位置插入新元素
                    }
                }
            }
        }

        /* Scope: 绑定命令
         * Description: 根据选中行元素的ID，删除后台和前台数据中对应元素 */
        private void Delete(ExamClass selectedCell)
        {
            if (selectedCell != null)
            {
                var v = MessageBox.Show("Are you sure to delete the selected cell?",  // Mention string
                                        "Delete",                                     // Title
                                        MessageBoxButtons.YesNo,                      // Button: Yes, No
                                        MessageBoxIcon.Question,                      // Icon: ?
                                        MessageBoxDefaultButton.Button2);             // DefaultButton: No

                if (v == DialogResult.Yes)
                {
                    dataSource.DeleteByID(selectedCell.Id);
                    GridModelList.Remove(selectedCell);
                }

            }
        }

        /* Scope: 内部
         * Description: 启动定时器，周期1.0s。当最后修改时间发生变化时，更新后台数据 */
        private void StartListenSQL()
        {
            /* 回调函数：ListenFileTimer，参数传递：无，立即启动，周期500ms */
            timer = new Threading.Timer(new Threading.TimerCallback(ListenSQL), null, 0, 1000);
        }

        /* Scope: 内部
         * Description: 从数据库下载数据并更新到 DataGrid，周期1.0s */
        private void ListenSQL(object obj)
        {
            if (!dataSource.CompareWithSQL())
            {
                //dataSource.UploadToSQL();
                RefreshGrid();
                PrintText = "更新云端数据到本地";
            }
            else
                PrintText = "本地数据与云端相同";

        }

        ~MainWindowViewModel()
        {
            timer.Dispose();        // 回收定时器
        }
        #endregion

    }

}