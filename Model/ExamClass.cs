using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TranningDemo.Model
{
    public class ExamClass
    {
        public ExamClass() { }

        public ExamClass(string id, string instituteStudents, int numberStudents, string instituteProctors, int numberProctors)
        {
            this.id = id;
            this.instituteStudents = instituteStudents;
            this.numberStudents = numberStudents;
            this.instituteProctors = instituteProctors;
            this.numberProctors = numberProctors;
        }

        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; }     // RaisePropertyChanged() 实现动态通知更新
        }

        private string instituteStudents;
        public string InstituteStudents
        {
            get { return instituteStudents; }
            set { instituteStudents = value; }
        }

        private int numberStudents;
        public int NumberStudents
        {
            get { return numberStudents; }
            set { numberStudents = value; }
        }

        private string instituteProctors;
        public string InstituteProctors
        {
            get { return instituteProctors; }
            set { instituteProctors = value; }
        }

        private int numberProctors;
        public int NumberProctors
        {
            get { return numberProctors; }
            set { numberProctors = value; }
        }

        public ExamClass Clone()
        {
            ExamClass deepClone = (ExamClass)this.MemberwiseClone();
            deepClone.Id = String.Copy(Id);                                 // 逐个复制类中的引用字段
            deepClone.InstituteStudents = String.Copy(InstituteStudents);
            deepClone.InstituteProctors = String.Copy(InstituteProctors);
            return deepClone;
        }
    }
        
}
