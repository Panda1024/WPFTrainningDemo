using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using Threading = System.Threading;
using System.Windows.Forms;
using Ninject;
using TranningDemo.Model;
using TranningDemo.View;

namespace TranningDemo.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(string dataSourceMode, string dataFileName)
        {
            this.dataFileName = dataFileName;
            IKernel kernel = new StandardKernel(new DataSourceModule(dataSourceMode));
            dataSource = kernel.Get<DataSource>();

            dataSource.ImportData(dataFileName);
            RefreshGrid();

            SearchCommand = new RelayCommand(Query);
            ClearCommand = new RelayCommand(ClearSearchBar);
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand<ExamClass>(Edit);
            DeleteCommand = new RelayCommand<ExamClass>(Delete);

            StartListen();
        }

        #region Field

        /* Scope: �ڲ�
         * Description: �������ݿ�Ķ�ʱ�� */
        private Threading.Timer timer;

        /* Scope: �ڲ�
         * Description: ���ڶ�Ӧ�������ļ���/���ݿ��� */
        private readonly string dataFileName;

        /* Scope: �ڲ�
         * Description: ���ں�̨����ģ�� */
        private readonly DataSource dataSource;

        /* Scope: ���ڰ�
         * Description: ����������*/
        private string searchKey = string.Empty;
        public string SearchKey
        {
            get { return searchKey; }
            set { searchKey = value; RaisePropertyChanged(); }
        }

        /* Scope: ���ڰ�
         * Description: DataGrid���б�*/
        private ObservableCollection<ExamClass> gridModelList;
        public ObservableCollection<ExamClass> GridModelList
        {
            get { return gridModelList; }
            set { gridModelList = value; RaisePropertyChanged(); }
        }

        /* Scope: ���ڰ�
         * Description: �ײ�״̬����ӡ��Ϣ*/
        private string printText;
        public string PrintText
        {
            get { return printText; }
            set { printText = value; RaisePropertyChanged(); }
        }
        #endregion

        /* Scope: ���ڰ�
         * Description: ��ť������*/
        #region Command
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand<ExamClass> EditCommand { get; set; }
        public RelayCommand<ExamClass> DeleteCommand { get; set; }

        #endregion

        #region Method

        /* Scope: �����
         * Description: ���ݴ�����ѯԪ�أ������ҵ���Ԫ�ظ��ǵ�DataGrid�� */
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
                PrintText = string.Format("{0} Rows of Serach Results", GridModelList.Count);
            }
        }

        /* Scope: ������ + �ڲ�
         * Description: �������������ˢ��DataGird */
        private void ClearSearchBar()
        {
            SearchKey = string.Empty;
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var data = dataSource.LocalData;
            if(SearchKey == string.Empty)       // �������йؼ��ʵ�ʱ�򣬲����� DataGrid
            {
                GridModelList = new ObservableCollection<ExamClass>();
                data.ForEach(arg =>
                {
                    GridModelList.Add(arg);
                });
            }
            
        }

        /* Scope: ������
         * Description: ���Ӵ��ڣ��༭��Ԫ�ز���ӵ���̨��ǰ̨������ */
        private void Add()
        {
            ExamClass newModel = new ExamClass();
            UserEditWindowView view = new UserEditWindowView(ref newModel);
            var v = view.ShowDialog();
            if (v.Value)
            {
                dataSource.Add(newModel);
                GridModelList.Insert(0, newModel);
                dataSource.SaveData(dataFileName);
            }
        }

        /* Scope: ������
         * Description: ����ѡ����Ԫ�ص�ID�����Ӵ��ڣ��༭��̨��ǰ̨�����ж�ӦԪ�� */
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
                    dataSource.SaveData(dataFileName);
                    int index = GridModelList.IndexOf(selectedCell);    // ��������ֵ
                    if(index>=0 && index<GridModelList.Count)
                    {
                        GridModelList.RemoveAt(index);                 // ɾ��Ԫ��
                        GridModelList.Insert(index, editModel);        // ��ԭλ�ò�����Ԫ��
                    }
                    
                }
            }
        }

        /* Scope: ������
         * Description: ����ѡ����Ԫ�ص�ID��ɾ����̨��ǰ̨�����ж�ӦԪ�� */
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
                    dataSource.SaveData(dataFileName);
                }

            }
        }

        /* Scope: �ڲ�
         * Description: ������ʱ��������0.5s��������޸�ʱ�䷢���仯ʱ�����º�̨���� */
        private void StartListen()
        {
            /* �ص�������ListenFileTimer���������ݣ��ޣ���������������500ms */
            timer = new Threading.Timer(new Threading.TimerCallback(Listen), null, 0, 500);
        }

        /* Scope: �ڲ�
         * Description: �����ݿ��������ݲ����µ� DataGrid������0.5s */
        private void Listen(object obj)
        {
            var sqlData = dataSource.CompareWith(dataFileName);
            if (sqlData != null)
            {
                dataSource.LocalData = sqlData;
                RefreshGrid();
            }
            if(SearchKey == string.Empty)
                PrintText = string.Format("{0} Rows of Records", dataSource.LocalData.Count);

        }

        ~MainWindowViewModel()
        {
            timer.Dispose();        // ���ն�ʱ��
        }
        #endregion

    }

}