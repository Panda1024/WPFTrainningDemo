using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using TranningDemo.Model;
using TranningDemo.View;

namespace TranningDemo.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
            if(IsInDesignMode)                  // 设计模式
            {
                dataSource = new DataSource();
                dataSource.Add(new ExamClass("N666", "机械学院", 60, "机械学院", 2));
                this.Query();
            }
            else
            {
                dataSource = new DataSource();  // 运行时模式

                OpenFileCommand = new RelayCommand(OpenFile);
                SaveFileCommand = new RelayCommand(SaveFile);
                SearchCommand = new RelayCommand(Query);
                ClearCommand = new RelayCommand(ClearSearchBar);
                AddCommand = new RelayCommand(Add);
                EditCommand = new RelayCommand<ExamClass>(Edit);
                DeleteCommand = new RelayCommand<ExamClass>(Delete);
            }
            
        }

        #region Field

        private string searchKey = string.Empty;
        public string SearchKey
        {
            get { return searchKey; }
            set { searchKey = value; RaisePropertyChanged(); }
        }

        private DataSource dataSource;
        private ObservableCollection<ExamClass> gridModelList;
        public ObservableCollection<ExamClass> GridModelList
        {
            get { return gridModelList; }
            set { gridModelList = value; RaisePropertyChanged(); }
        }

        private ExamClass selectedCell;          // DataGrid 当前鼠标选中单元
        public ExamClass SelectedCell
        {
            get { return selectedCell; }
            set { selectedCell = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Command
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand SaveFileCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand<ExamClass> EditCommand { get; set; }
        public RelayCommand<ExamClass> DeleteCommand { get; set; }

        #endregion

        #region Method
        private void OpenFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = System.IO.Path.Combine( Application.StartupPath, @"..\..\Data" );
                openFileDialog.Filter = "xml files (*.xml)|*.xml";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataSource.ImportData(openFileDialog.FileName);
                    GridModelList = new ObservableCollection<ExamClass>();
                    foreach (ExamClass item in dataSource.Data)
                    {
                        GridModelList.Add(item);
                    }
                }
            }
        }

        private void SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.Filter = "xml files (*.xml)|*.xml";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                dataSource.SaveData(saveFileDialog.FileName);
            }
        }

        private void Query()
        {
            var models = dataSource.SerachById(SearchKey);
            GridModelList = new ObservableCollection<ExamClass>();
            if (models != null)
            {
                models.ForEach(arg =>
                {
                    GridModelList.Add(arg);
                });
            }
        }

        private void ClearSearchBar()
        {
            SearchKey = string.Empty;
            this.Query();
        }

        private void Add()
        {
            ExamClass newModel = new ExamClass();
            UserEditWindowView view = new UserEditWindowView(ref newModel);
            var v = view.ShowDialog();
            if(v.Value)
            {
                dataSource.Add(newModel);
                this.Query();
            }
        }
        private void Edit(ExamClass selectedCell)
        {         
            if (selectedCell != null)
            {
                var editModel = selectedCell.Clone();
                UserEditWindowView view = new UserEditWindowView(ref editModel);
                var v = view.ShowDialog();
                if (v.Value)
                {
                    int indexData = dataSource.Data.FindIndex(item => item.Id == selectedCell.Id);
                    dataSource.Delete(selectedCell.Id);
                    dataSource.Data.Insert(indexData, editModel);
                    this.Query();
                }
            }
        }

        private void Delete(ExamClass selectedCell)
        {
            var model = dataSource.GetById(selectedCell.Id);
            if (model != null)
            {
                var v = MessageBox.Show("Are you sure to delete the selected cell?",  // Mention string
                                        "Delete Selected Cell",                       // Title
                                        MessageBoxButtons.YesNo,                      // Button: Yes, No
                                        MessageBoxIcon.Question,                      // Icon: ?
                                        MessageBoxDefaultButton.Button2);             // DefaultButton: No
                
                if (v == DialogResult.Yes)
                {
                    dataSource.Delete(model.Id);
                    this.Query();
                }
                    
            }
        }
        #endregion

    }
}