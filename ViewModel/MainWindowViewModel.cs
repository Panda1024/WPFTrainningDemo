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
        /* Scope: �ڲ�
         * Description: ���ڼ乲��������ļ� */
        private string dataFileName;

        /* Scope: �ڲ�
         * Description: �����ļ�����޸�ʱ�� */
        private DateTime lastAccessTime;

        private Threading.Timer timer;

        /* Scope: �ڲ�
         * Description:  */
        private event Action<string> upLoadToFile;

        /* Scope: �ڲ�
         * Description: ���ں�̨����*/
        private DataSource dataSource;

        /* Scope: ���ڰ�
         * Description: ����������*/
        private string searchKey = string.Empty;
        public string SearchKey
        {
            get { return searchKey; }
            set { searchKey = value; RaisePropertyChanged(); }
        }

        /* Scope: ���ڰ�
         * Description: DataGrid�����ݼ���*/
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

        /* Scope: ������
         * Description: �������������ˢ��DataGird */
        private void ClearSearchBar()
        {
            SearchKey = string.Empty;
            this.Query();
        }

        /* Scope: ������
         * Description: ���Ӵ��ڣ��༭��Ԫ�ز���ӵ���̨��ǰ̨������ */
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

        /* Scope: ������
         * Description: ����ѡ����Ԫ�ص�ID�����Ӵ��ڣ��༭��̨��ǰ̨�����ж�ӦԪ�� */
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

        /* Scope: ������
         * Description: ����ѡ����Ԫ�ص�ID��ɾ����̨��ǰ̨�����ж�ӦԪ�� */
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

        /* Scope: �ڲ�
         * Description: ������ʱ��������0.5s��������޸�ʱ�䷢���仯ʱ�����º�̨���� */
        private void StartListenFileTime()
        {
            lastAccessTime = File.GetLastAccessTime(dataFileName);
            upLoadToFile += (dataFileName) => dataSource.SaveData(dataFileName);
            /* �ص�������ListenFileTimer���������ݣ��ޣ���������������500ms */
            timer = new Threading.Timer(new Threading.TimerCallback(ListenFileTime), timer, 0, 500);
        }

        /* Scope: �ڲ�
         * Description: ���������ļ����޸�ʱ�䣬����0.5s��������޸�ʱ�䷢���仯ʱ�����º�̨���� */
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
            timer.Dispose();        // ���ն�ʱ��
        }
        #endregion

    }

}