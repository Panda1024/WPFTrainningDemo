using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.IO;
using System.Windows.Threading;
using Ninject;
using TranningDemo.Model;
using TranningDemo.View;
using TranningDemo.Service;
using System.Threading;
using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace TranningDemo.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
            dataSource = new DataSource();
            GridModelList = new ObservableCollection<ExamClass>();
            if (IsInDesignMode)                  // Design Pattern
            {

                dataSource.Insert(0, new ExamClass("N666", "��еѧԺ", 60, "��еѧԺ", 2));
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

        private DataSource dataSource;

        private string searchKey = string.Empty;
        public string SearchKey
        {
            get { return searchKey; }
            set { searchKey = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ExamClass> gridModelList;
        public ObservableCollection<ExamClass> GridModelList
        {
            get { return gridModelList; }
            set { gridModelList = value; RaisePropertyChanged(); }
        }

        private readonly object editLocker = new object();
        private readonly object deleteLocker = new object();
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
                openFileDialog.InitialDirectory = System.IO.Path.Combine(Application.StartupPath, @"..\..\Data");
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
            Thread thread = new Thread(() =>
            {
                ExamClass newModel = new ExamClass();
                UserEditWindowView view = new UserEditWindowView(ref newModel);
                var v = view.ShowDialog();
                if (v.Value)
                {
                    /* GridModelList�� CollectionView �࣬����UI����Ԫ�أ���֧�ֿ��̲߳���
                     * ��Ҫ����Dispather �н��а�ȫ����
                     * CheckBeginInvokeOnUI ����ִ�м�飬�����ö����Ƿ��Ѿ������߳��ϣ�����ھ�ֱ��ִ�к����ί�У�������Ǿ�ȥִ�е���
                     * Bug��¼�����Թ�ʹ��Dispatcher.CurrentDispatcher.BeginInvoke����BeginInvoke�ڵĴ���û��ִ��
                     */
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        dataSource.Insert(0, newModel);
                        GridModelList.Insert(0, newModel);
                    });
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        private void Edit(ExamClass selectedCell)
        {
            if (selectedCell == null)
                return;
            Thread thread = new Thread(EditModel);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(selectedCell);
        }

        private void EditModel(object obj)
        {
            ExamClass model = (ExamClass)obj;
            int gridIndex = GridModelList.IndexOf(model);
            ExamClass editModel = model.DeepClone();

            UserEditWindowView view = new UserEditWindowView(ref editModel);
            var v = view.ShowDialog();
            if (v.Value)                // �Ӵ��ڵ��Save
            {
                lock (editLocker)       // ͬһʱ�̽�����һ���̱߳༭����
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        int dataIndex = dataSource.GetIndex(editModel.Id);
                        if (dataIndex == -1)
                            return;
                        dataSource.Delete(editModel.Id);
                        dataSource.Insert(dataIndex, editModel);
                        GridModelList.RemoveAt(gridIndex);
                        GridModelList.Insert(gridIndex, editModel);
                    });
                }

            }
        }

        private void Delete(ExamClass selectedCell)
        {
            if (selectedCell == null)
                return;
            Thread thread = new Thread(DeleteModel);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(selectedCell);
        }

        private void DeleteModel(object obj)
        {
            ExamClass model = (ExamClass)obj;
            var v = MessageBox.Show("Are you sure to delete the selected cell?",  // Mention string
                                    "Delete Selected Cell",                       // Box Title
                                    MessageBoxButtons.YesNo,                      // Button: Yes, No
                                    MessageBoxIcon.Question,                      // Icon: ?
                                    MessageBoxDefaultButton.Button2);             // DefaultButton: No

            if (v == DialogResult.Yes)  // �Ӵ��ڵ��Yes
            {
                lock (deleteLocker)     // ͬһʱ�̽�����һ���߳�ɾ������
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        dataSource.Delete(model.Id);
                        GridModelList.Remove(model);
                    });
                }
                

            }
        }
        #endregion

    }

}