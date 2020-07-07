using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TranningDemo.Model
{
    public class ExamClass : ICloneable, IEquatable<ExamClass>
    {
        public ExamClass()
        {
            this.id = DataSource.index++;
            this.classNo = string.Empty;
            this.instituteStudents = string.Empty;
            this.numberStudents = 0;
            this.instituteProctors = string.Empty;
            this.numberProctors = 0;
        }

        public ExamClass(string classNo, string instituteStudents, int numberStudents, string instituteProctors, int numberProctors)
        {
            this.id = DataSource.index++;
            this.classNo = classNo;
            this.instituteStudents = instituteStudents;
            this.numberStudents = numberStudents;
            this.instituteProctors = instituteProctors;
            this.numberProctors = numberProctors;
        }

        public ExamClass(int id, string classNo, string instituteStudents, int numberStudents, string instituteProctors, int numberProctors)
        {
            this.id = id;
            this.classNo = classNo;
            this.instituteStudents = instituteStudents;
            this.numberStudents = numberStudents;
            this.instituteProctors = instituteProctors;
            this.numberProctors = numberProctors;
        }


        private readonly int id;
        public int Id { get => id; }

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

        // 自定义的Equals方法。如果 other 不是 ExamClass 类型，则调用下面的基本 Object.Equals(Object) 方法
        public bool Equals(ExamClass other)
        {
            if (other == null) 
                return false;
            if (Id == other.Id && ClassNo == other.ClassNo && InstituteStudents == other.InstituteStudents 
                && NumberStudents == other.NumberStudents && InstituteProctors == other.InstituteProctors && NumberProctors == other.NumberProctors)
                return true;
            else
                return false;
        }
        // 重写基类中的Equals方法。类的静态 Equals(System.Object, System.Object) 方法中会调用该重写的实现
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ExamClass examClass = obj as ExamClass;   // 逆变转换
            if (examClass == null)
                return false;
            else
                return Equals(examClass);
        }
        // 重写哈希码比较函数
        public override int GetHashCode()
        {
            return new { Id, ClassNo, InstituteStudents, NumberStudents, InstituteProctors, NumberProctors }.GetHashCode();
        }
    }
        
}
