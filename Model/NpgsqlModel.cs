using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;
using Npgsql;
using NpgsqlTypes;
using TranningDemo.Service;

namespace TranningDemo.Model
{
    class NpgsqlModel
    {
        public NpgsqlModel(string sqlName)
        {
            Initialize(sqlName);
        }

        #region Field
        private string sqlName;

        private string tableName;

        private string connectServerSql;

        private string connectSql;

        private List<ExamClass> localdata;

        public List<ExamClass> localData
        {
            get
            {
                return localdata;
            }
            set
            {
                if (value != null)
                    localdata = value;
            }
        }

        internal static int index = 0;

        #endregion

        #region Method
        /// <summary>
        /// 构造函数参数初始化
        /// </summary>
        private void Initialize(string sqlName)
        {
            this.sqlName = sqlName;
            this.tableName = "ExamClass";
            this.connectServerSql = "Host=localhost;Port=5432;Username=postgres;Password=123456;";
            this.connectSql = string.Format("{0}Database={1}", connectServerSql, sqlName);
            
            CreateDataBase();
            CreateTable(tableName);

            this.localData = DownLoadFromSQL();
        }

        /// <summary>
        /// 数据库是否存在
        /// </summary>
        /// <param name="null">无参输入</param>
        /// <returns>0-不存在，1-存在，-1-报错</returns>
        private int IsExistDataBase()
        {
            string querySql = string.Format("SELECT u.datname  FROM pg_catalog.pg_database u where u.datname='{0}';", sqlName);  // 查询数据库是否存在
            try
            {
                using (var connect = new NpgsqlConnection(connectServerSql))
                {
                    connect.Open();
                    using (var cmd = new NpgsqlCommand(querySql, connect))
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == sqlName)
                            {
                                connect.Close();
                                Console.WriteLine("数据库{0}已存在", sqlName);
                                return 1;
                            }
                        }
                        connect.Close();
                        Console.WriteLine("数据库{0}不存在", sqlName);
                        return 0;
                    }
                }
            }
            catch
            {
                Console.WriteLine("数据库{0}访问异常", sqlName);
                return -1;
            }
        }

        /// <summary>
        /// 新建数据库
        /// </summary>
        /// <param name=null>无参输入</param>
        public bool CreateDataBase()
        {
            int existed = IsExistDataBase();
            if (existed == 1)
                return false;
            try
            {
                using (var connect = new NpgsqlConnection(connectServerSql))
                {
                    connect.Open();
                    string createSql = string.Format("CREATE DATABASE {0}", sqlName);
                    using (NpgsqlCommand cmd = new NpgsqlCommand(createSql, connect))
                    {
                        cmd.ExecuteNonQuery();
                    }
                        
                    if (IsExistDataBase() == 1)
                    {
                        Console.WriteLine("Succeed to CREATE DATABASE: {0}", sqlName);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Fail to CREATE DATABASE: {0}", sqlName);
                        return false;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error of CREATE DATABASE: {0}", sqlName);
                return false;
            }
        }

        /// <summary>
        /// Create Table
        /// </summary>
        /// <param tableName="name">tableName</param>
        /// <returns>ture-创建成功，false-创建失败</returns>
        public bool CreateTable(string tableName)
        {
            try
            {
                using (var connect = new NpgsqlConnection(connectSql))
                {
                    connect.Open();
                    string createTableSql = string.Format("CREATE TABLE IF NOT EXISTS {0}(Id integer PRIMARY KEY, ClassNo TEXT, InstituteStudents TEXT, NumberStudents integer, InstituteProctors TEXT, NumberProctors integer);", tableName);
                    using (var cmd = new NpgsqlCommand(createTableSql, connect))
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Succeed to Create Table: {0}", tableName);
                        return true;
                    }
                    
                }
            }
            catch
            { 
                Console.WriteLine("Error of Create Table: {0}", tableName);
                return false;
            }

        }

        public List<ExamClass> Search(string searchKey)
        {
            try
            {
                using (var connect = new NpgsqlConnection(connectSql))
                {
                    Console.WriteLine("连接数据库{0}成功", sqlName);
                    /* 模糊搜索字段中包含关键词的数据 */
                    /* 数据库的只能匹配字符进行搜索，NumberStudents 和 NumberProctors为非字符字段，这里做了转换 NumberStudents::text */
                    string searchSql = string.Format("SELECT * FROM {0} WHERE ClassNo ilike '%{1}%' or " +
                                                                            "InstituteStudents ilike '%{1}%' or " +
                                                                            "NumberStudents::text ilike '%{1}%' or " +
                                                                            "InstituteProctors ilike '%{1}%' or " +
                                                                            "NumberProctors::text ilike '%{1}%' ;", tableName, searchKey);
                    DataSet dataSet = GetDataSet(searchSql);
                    return ConvertTolist(dataSet);
                }
            }
            catch
            {
                Console.WriteLine("查询数据库出错");
                return null;
            }

        }

        public int Add(ExamClass model)
        {
            try
            {
                /* 操作本地数据 */
                localData.Insert(0, model);
                return UploadToSQL();
                ///* 数据库同步操作 */
                //using (var connect = new NpgsqlConnection(connectSql))
                //{
                //    connect.Open();
                //    string insertSql = string.Format("INSERT INTO {0}(Id, ClassNo, InstituteStudents, NumberStudents, InstituteProctors, NumberProctors) " +
                //                                    "VALUES(@param0, @param1, @param2, @param3, @param4, @param5);", tableName);

                //    using (var insertCmd = new NpgsqlCommand(insertSql, connect))
                //    {
                //        insertCmd.Parameters.AddWithValue("param0", NpgsqlDbType.Integer, model.Id);
                //        insertCmd.Parameters.AddWithValue("param1", NpgsqlDbType.Text, model.ClassNo);
                //        insertCmd.Parameters.AddWithValue("param2", NpgsqlDbType.Text, model.InstituteStudents);
                //        insertCmd.Parameters.AddWithValue("param3", NpgsqlDbType.Integer, model.NumberStudents);
                //        insertCmd.Parameters.AddWithValue("param4", NpgsqlDbType.Text, model.InstituteProctors);
                //        insertCmd.Parameters.AddWithValue("param5", NpgsqlDbType.Integer, model.NumberProctors);

                //        int count = insertCmd.ExecuteNonQuery();
                //        Console.WriteLine("Succeed to add data of 'ClassNo:{0}' to Table: {1}.{2} ", model.ClassNo, sqlName, tableName);
                //        return count;
                //    }
                //}
            }
            catch
            {
                Console.WriteLine("Error of adding data to SQL");
                return -1;
            }
        }

        public int Edit(int id, ExamClass model)
        {
            try
            {
                /* 操作本地数据 */
                int index = localData.FindIndex(item => item.Id == id);
                if(index>=0 && index<localData.Count)
                {
                    localData.RemoveAt(index);
                    localData.Insert(index, model);
                }

                return UploadToSQL();
                ///* 数据库同步操作 */
                //using (var connect = new NpgsqlConnection(connectSql))
                //{
                //    connect.Open();
                //    string updateSql = string.Format("UPDATE {0} SET Id=@param0, ClassNo=@param1, InstituteStudents=@param2, NumberStudents=@param3, " +
                //                                    "InstituteProctors=@param4, NumberProctors=@param5 WHERE Id = {1};", tableName, id);
                //    using (var updateCmd = new NpgsqlCommand(updateSql, connect))
                //    {
                //        updateCmd.Parameters.AddWithValue("param0", NpgsqlDbType.Integer, model.Id);
                //        updateCmd.Parameters.AddWithValue("param1", NpgsqlDbType.Text, model.ClassNo);
                //        updateCmd.Parameters.AddWithValue("param2", NpgsqlDbType.Text, model.InstituteStudents);
                //        updateCmd.Parameters.AddWithValue("param3", NpgsqlDbType.Integer, model.NumberStudents);
                //        updateCmd.Parameters.AddWithValue("param4", NpgsqlDbType.Text, model.InstituteProctors);
                //        updateCmd.Parameters.AddWithValue("param5", NpgsqlDbType.Integer, model.NumberProctors);

                //        int count = updateCmd.ExecuteNonQuery();
                //        Console.WriteLine("Succeed to edit data of 'ClassNo:{0}' to Table: {1}.{2} ", model.ClassNo, sqlName, tableName);
                //        return count;
                //    }
                //}
            }
            catch
            {
                Console.WriteLine("Error of editing data to {0}.{1}", sqlName, tableName);
                return -1;
            }
        }

        public int DeleteByID(int id)
        {
            try
            {
                /* 操作本地数据 */
                int index = localData.FindIndex(item => item.Id == id);
                if (index >= 0 && index < localData.Count)
                {
                    localData.RemoveAt(index);
                }

                return UploadToSQL();
                ///* 数据库同步操作 */
                //using (var connect = new NpgsqlConnection(connectSql))
                //{
                //    connect.Open();
                //    string deleteSql = string.Format("DELETE FROM {0} WHERE Id = {1};", tableName, id);
                //    using (var deleteCmd = new NpgsqlCommand(deleteSql, connect))
                //    {
                //        int count = deleteCmd.ExecuteNonQuery();
                //        Console.WriteLine("Delete the row from {0}.{1}",sqlName, tableName);
                //        return count;
                //    }

                //}
            }
            catch
            {   
                Console.WriteLine("Error of deleting the element 'Id = {0}' from {1}", id, tableName);
                return -1;
            }
        }

        public List<ExamClass> DownLoadFromSQL()
        {
            DataSet dataSet = GetDataSet("ALL");
            if (dataSet != null)
                return ConvertTolist(dataSet);
            else
                return null;
        }

        /// <summary>
        /// Upload localData to SQL
        /// </summary>
        public int UploadToSQL()
        {
            try
            {
                if (localData.Count == 0 || localData == null)
                    return -1;
                using (var connect = new NpgsqlConnection(connectSql))
                {
                    connect.Open();
                    // 创建DataTable
                    // 创建DataTable
                    DataSet dataSet = new DataSet();
                    DataTable dataTable = new DataTable(tableName);

                    // 设置table各列的字段
                    DataColumn dc = dataTable.Columns.Add("Id", Type.GetType("System.Int32"));
                    dc.AllowDBNull = false;         // 不可为null
                    dataTable.Columns.Add("ClassNo", Type.GetType("System.String"));
                    dataTable.Columns.Add("InstituteStudents", Type.GetType("System.String"));
                    dataTable.Columns.Add("NumberStudents", Type.GetType("System.Int32"));
                    dataTable.Columns.Add("InstituteProctors", Type.GetType("System.String"));
                    dataTable.Columns.Add("NumberProctors", Type.GetType("System.Int32"));

                    // 将 localData 添加到 dataTable 中
                    foreach (var model in localData)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Id"] = model.Id;
                        row["ClassNo"] = model.ClassNo;
                        row["InstituteStudents"] = model.InstituteStudents;
                        row["NumberStudents"] = model.NumberStudents;
                        row["InstituteProctors"] = model.InstituteProctors;
                        row["NumberProctors"] = model.NumberProctors;

                        dataTable.Rows.Add(row);
                    }

                    dataSet.Tables.Add(dataTable);

                    /* 清空table, 否则update操作会出错 */
                    string deleteSql = string.Format("DELETE FROM {0};", tableName);
                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSql, connect))
                    {
                        cmd.ExecuteNonQuery();//执行查询命令
                    }

                    /* 上传数据到数据库 */
                    string selectSql = string.Format("SELECT * FROM {0};", tableName);
                    using (NpgsqlDataAdapter nadapter = new NpgsqlDataAdapter(selectSql, connect))
                    {
                        NpgsqlCommandBuilder commendBuilder = new NpgsqlCommandBuilder(nadapter);
                        int count = nadapter.Update(dataSet, tableName);
                        Console.WriteLine("Update {0} items to {1}.{2}", count, sqlName, tableName);
                        return count;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error of uploading data to {0}.{1}", sqlName, tableName);
                return -1;
            }
        }

        /// <summary>
        /// Create Table
        /// </summary>
        /// <returns>ture-本地数据与数据库相同，false-本地数据与数据库不同</returns>
        public bool CompareWithSQL()
        {
            List<ExamClass> sqlData = DownLoadFromSQL();
            //sqlData = sqlData.OrderBy(it => it.Id).ToList();
            if (localData.Count != sqlData.Count && sqlData != null)
            {
                localData = sqlData;
                return false;
            }
            for(int i=0; i<localData.Count; i++)
            {
                //Console.WriteLine("{0} compare:{1} vs {2}", i, localData[i].ToString(), sqlData[i].ToString());
                //Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", localData[i].Id, localData[i].ClassNo, localData[i].InstituteStudents, localData[i].NumberStudents, localData[i].InstituteProctors, localData[i].NumberProctors);
                //Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", sqlData[i].Id, sqlData[i].ClassNo, sqlData[i].InstituteStudents, sqlData[i].NumberStudents, sqlData[i].InstituteProctors, sqlData[i].NumberProctors);
                var equal = localData[i].Equals(sqlData[i]);
                if (!equal)
                {
                    localData = sqlData;
                    return false;
                }
            }
            return true;
        }

        public DataSet GetDataSet(string selectSql)
        {
            try
            {
                if (selectSql == "ALL")
                    selectSql = string.Format("SELECT * FROM  {0};", tableName);
                using (var connect = new NpgsqlConnection(connectSql))
                {
                    connect.Open();
                    using (var cmd = new NpgsqlCommand(selectSql, connect))
                    {
                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(cmd);
                        DataSet dataSet = new DataSet();
                        npgsqlDataAdapter.Fill(dataSet);
                        connect.Close();
                        return dataSet;
                    }
                }
            }
            catch
            {
                Console.WriteLine("获取{0}.{1}数据出错", sqlName, tableName);
                return null;
            }
        }

        /// <summary>
        /// Convert DataSet to List
        /// </summary>
        /// <param>DataSet</param>
        public List<ExamClass> ConvertTolist(DataSet dataSet)
        {
            List<ExamClass> modelList = new List<ExamClass>();
            DataTable dataTable = dataSet.Tables[0];
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                string Id                   = row.ItemArray[0].ToString();
                string ClassNo              = row.ItemArray[1].ToString();
                string InstituteStudents    = row.ItemArray[2].ToString();
                string NumberStudents       = row.ItemArray[3].ToString();
                string InstituteProctors    = row.ItemArray[4].ToString();
                string NumberProctors       = row.ItemArray[5].ToString();
                modelList.Add(new ExamClass(int.Parse(Id), 
                                            ClassNo, 
                                            InstituteStudents, 
                                            int.Parse(NumberStudents), 
                                            InstituteProctors, 
                                            int.Parse(NumberProctors))
                    );
            }
            return modelList;
        }

        /// <summary>
        /// Import data from .xml file to SQL
        /// </summary>
        /// <param fullFileName=".xml">fullFileName</param>
        public List<ExamClass> ImportFromXmlFileToSQL(string xmlFileName)
        {
            try
            {
                XmlFileService xmlFileService = new XmlFileService();
                List<ExamClass> modelList = xmlFileService.ImportData(xmlFileName);
                using (var connect = new NpgsqlConnection(connectSql))
                {
                    connect.Open();
                    // 创建DataTable
                    DataSet dataSet = new DataSet();
                    DataTable dataTable = new DataTable(tableName);

                    // 设置table各列的字段
                    DataColumn dc = dataTable.Columns.Add("Id", Type.GetType("System.Int32"));
                    dc.AutoIncrement = true;    //自动增加
                    dc.AutoIncrementSeed = 1;   //起始为1
                    dc.AutoIncrementStep = 1;   //步长为1
                    dc.AllowDBNull = false;
                    dataTable.Columns.Add("ClassNo", Type.GetType("System.String"));
                    dataTable.Columns.Add("InstituteStudents", Type.GetType("System.String"));
                    dataTable.Columns.Add("NumberStudents", Type.GetType("System.Int32"));
                    dataTable.Columns.Add("InstituteProctors", Type.GetType("System.String"));
                    dataTable.Columns.Add("NumberProctors", Type.GetType("System.Int32"));

                    // 将列表数据添加到 dataTable 中
                    foreach (var model in modelList)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Id"] = model.Id;
                        row["ClassNo"] = model.ClassNo;
                        row["InstituteStudents"] = model.InstituteStudents;
                        row["NumberStudents"] = model.NumberStudents;
                        row["InstituteProctors"] = model.InstituteProctors;
                        row["NumberProctors"] = model.NumberProctors;

                        dataTable.Rows.Add(row);
                    }

                    dataSet.Tables.Add(dataTable);

                    /* 查看dataset中的数据 */
                    //for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    //{
                    //    string one1 = dataSet.Tables[0].Rows[i].ItemArray[0].ToString();
                    //    string one2 = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    //    string one3 = dataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                    //    string one4 = dataSet.Tables[0].Rows[i].ItemArray[3].ToString();
                    //    string one5 = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                    //    string one6 = dataSet.Tables[0].Rows[i].ItemArray[5].ToString();
                    //    Console.WriteLine("w:{0},x:{1},y:{2},z:{3},z:{4},z:{5}", one1, one2, one3, one4, one5, one6);
                    //}

                    /* 清空table, 否则update操作会出错 */
                    string deleteSql = string.Format("DELETE FROM  {0};", tableName);
                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSql, connect))
                    {
                        cmd.ExecuteNonQuery();//执行查询命令
                    }
                        

                    /* 上传数据到数据库 */
                    string selectSql = string.Format("SELECT * FROM  {0};", tableName);
                    using (NpgsqlDataAdapter nadapter = new NpgsqlDataAdapter(selectSql, connect))
                    {
                        NpgsqlCommandBuilder commendBuilder = new NpgsqlCommandBuilder(nadapter);
                        int count = nadapter.Update(dataSet, tableName);
                        Console.WriteLine("Succeed tot import {0} items from {1} to SQL:{2}.{3}", count, xmlFileName, sqlName, tableName);
                        return modelList;
                    }
                        
                }
            }
            catch
            {
                Console.WriteLine("Error of importing data from {0} to SQL {1}", xmlFileName, sqlName);
                return null;
            }
        }

        #endregion
    }
}
