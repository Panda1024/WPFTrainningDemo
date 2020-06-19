using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.IO;
using Ninject;
using Ninject.Modules;
using TranningDemo.Model;
using TranningDemo.View;
using TranningDemo.Service;

namespace TranningDemo.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
            dataSource = new DataSource();
            if (IsInDesignMode)                  // Design Pattern
            {
                
                dataSource.Add(new ExamClass("N666", "机械学院", 60, "机械学院", 2));
                this.Query();
            }
            else                                // Runtime Pattern
            {
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
                openFileDialog.Filter = "xml files (*.xml)|*.xml|json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileExtension = Path.GetExtension(openFileDialog.FileName);
                    IKernel kernel;
                    bool useXml;
                    switch (fileExtension)
                    {
                        case ".xml":
                            useXml = true;
                            kernel = new StandardKernel(new DataSourceModule(useXml));
                            dataSource = kernel.Get<DataSource>();
                            break;
                        case ".json":
                            useXml = false;
                            kernel = new StandardKernel(new DataSourceModule(useXml));
                            dataSource = kernel.Get<DataSource>();
                            break;
                    }
                    dataSource.ImportData(openFileDialog.FileName);
                    GridModelList = new ObservableCollection<ExamClass>();
                    foreach (ExamClass item in dataSource.Data)
                    {
                        GridModelList.Add(item);
                    }
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
                dataSource.SaveData(saveFileDialog.FileName);
            }
            else
            {
                return;
            }
        }

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
                gridModelList.Insert(0, newModel);
            }
        }
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
                    gridModelList.Remove(selectedCell);
                }
                    
            }
        }
        #endregion

    }

}