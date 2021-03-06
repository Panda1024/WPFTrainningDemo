﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using TranningDemo.Model;

namespace TranningDemo.ViewModel
{
    public class UserEditViewModel : ViewModelBase
    {
        public UserEditViewModel(ref ExamClass model) 
        {
            this.model = model;
        }

        #region Field
        private Window thisWindow;
        public Window ThisWindow
        {
            set { thisWindow = value; }
        }

        private ExamClass model;
        public ExamClass Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Command

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(Save);
                return saveCommand;
            }
            set { saveCommand = value; }
        }

        private RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                    cancelCommand = new RelayCommand(Cancel);
                return cancelCommand;
            }
            set { cancelCommand = value; }
        }

        #endregion

        #region Method

        private void Save()
        {
            thisWindow.DialogResult = true;
            thisWindow.Close();
        }

        private void Cancel()
        {
            thisWindow.DialogResult = false;
            thisWindow.Close();
        }
        #endregion
    }
}
