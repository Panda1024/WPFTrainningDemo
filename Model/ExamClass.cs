using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TranningDemo.Model
{
    public class ExamClass : ICloneable
    {
        public ExamClass()
        {
            id = DataSource.index++;
        }

        public ExamClass(string classNo, string instituteStudents, int numberStudents, string instituteProctors, int numberProctors)
        {
            id = DataSource.index++;
            this.classNo = classNo;
            this.instituteStudents = instituteStudents;
            this.numberStudents = numberStudents;
            this.instituteProctors = instituteProctors;
            this.numberProctors = numberProctors;
        }


        private readonly uint id;
        public uint Id { get => id; }

        private string classNo;
        public string ClassNo { get => classNo; set => classNo = value; }

        private string instituteStudents;
        public string InstituteStudents { get => instituteStudents; set => instituteStudents = value; }

        private int numberStudents;
        public int NumberStudents { get => numberStudents; set => numberStudents = value; }

        private string instituteProctors;
        public string InstituteProctors { get => instituteProctors; set => instituteProctors = value; }

        private int numberProctors;
        public int NumberProctors { get => numberProctors; set => numberProctors = value; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public ExamClass DeepClone()
        {
            ExamClass deepClone = (ExamClass)this.Clone();
            deepClone.ClassNo = String.Copy(ClassNo);                                 // 逐个复制类中的引用字段
            deepClone.InstituteStudents = String.Copy(InstituteStudents);
            deepClone.InstituteProctors = String.Copy(InstituteProctors);
            return deepClone;
        }
    }
        
}
