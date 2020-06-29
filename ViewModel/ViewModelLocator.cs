using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;     // MVVMLights v5.4.1新命名空间
using TranningDemo.View;
using TranningDemo.Model;

namespace TranningDemo.ViewModel
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<StartWindowViewModel>();                    // 注册MainWindowViewModel
        }

        public StartWindowViewModel Start
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartWindowViewModel>(); // 通过容器获得实例对象
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}