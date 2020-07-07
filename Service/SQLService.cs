using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranningDemo.Model;

namespace TranningDemo.Service
{
    class SQLService : IDataService
    {
        public SQLService()
        {
            connectServerSql = "Host=localhost;Port=5432;Username=postgres;Password=123456;";
            sqlName = "trainningdemo";
            tableName = "ExamClass";
            connectSql = string.Format("{0}Database={1}", connectServerSql, sqlName);
        }

        private readonly string connectServerSql;
        private readonly string connectSql;
        private readonly string sqlName;
        private readonly string tableName;

        public List<ExamClass> Query(List<ExamClass> data, string searchKey)
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

        public List<ExamClass> ImportData(string sqlName)
        {
            DataSet dataSet = GetDataSet("ALL");
            if (dataSet != null)
            {
                Console.WriteLine("成功从数据库中导入数据");
                return ConvertTolist(dataSet);
            }
                
            else
                return null;
        }

        /// <summary>
        /// Upload localData to SQL
        /// </summary>
        public int SaveData(string sqlName, List<ExamClass> localData)
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
                        Console.WriteLine("成功存储数据到SQL");
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

        private DataSet GetDataSet(string selectSql)
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
        private List<ExamClass> ConvertTolist(DataSet dataSet)
        {
            List<ExamClass> modelList = new List<ExamClass>();
            DataTable dataTable = dataSet.Tables[0];
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                string Id = row.ItemArray[0].ToString();
                string ClassNo = row.ItemArray[1].ToString();
                string InstituteStudents = row.ItemArray[2].ToString();
                string NumberStudents = row.ItemArray[3].ToString();
                string InstituteProctors = row.ItemArray[4].ToString();
                string NumberProctors = row.ItemArray[5].ToString();
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

        public List<ExamClass> CompareWith(List<ExamClass> localData, string dataFileName)
        {
            List<ExamClass> sqlData = ImportData(dataFileName);
            //sqlData = sqlData.OrderBy(it => it.Id).ToList();
            if (localData.Count != sqlData.Count && sqlData != null)
            {
                localData = sqlData;
                return localData;
            }
            for (int i = 0; i < localData.Count; i++)
            {
                var equal = localData[i].Equals(sqlData[i]);
                if (!equal)
                {
                    localData = sqlData;
                    return localData;
                }
            }
            return null;
        }
    }




}
