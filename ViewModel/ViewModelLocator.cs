using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;     // MVVMLights v5.4.1�������ռ�
using TranningDemo.View;
using TranningDemo.Model;

namespace TranningDemo.ViewModel
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<StartWindowViewModel>();                    // ע��MainWindowViewModel
        }

        public StartWindowViewModel Start
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartWindowViewModel>(); // ͨ���������ʵ������
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}