using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using TranningDemo.Model;

namespace TableView
{
    /// <summary>
    ///   IO_Npgsql类用于  数据库 文件的查询，增删改查，可存储和读取数据库文件，
    /// 并以ObservableCollection格式存储在程序中
    /// </summary>
    public class IO_Npgsql
    {
        public string connection_pgsql;//
        public string cmd_pgsql;    //创建 增删改命令
        public string dataBaseName; //创建数据库
        public string tableName;    //创建 表格
        public bool bSaveIsOk;      //创建 表格
        public DataSet myDataSet;
        public DataSet myDataSetLast;
        public DataSet myDataSetFind;
        public ObservableCollection<ExamClass> infos_pgsql;
        public IO_Npgsql()
        {
            Console.WriteLine("Hello PostgreSQL");
            // Host info
            /*  //测试部分
            connection_pgsql = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=demo";//数据库名称是demo
            using (var conn = new NpgsqlConnection(connection_pgsql))
            {
                conn.Open();
                Console.WriteLine("读取");
                // Retrieve all rows
                using (var cmd = new NpgsqlCommand("SELECT * FROM student", conn))
                using (var reader = cmd.ExecuteReader())//这里面有执行的命令
                    while (reader.Read())
                    {
                        // 读取3列
                       // Console.WriteLine("{0}  {1} {2}", reader.GetString(0), reader.GetString(1), int.Parse(reader.GetString(2)));
                        // 读取4列
                        Console.WriteLine("{0}  {1} {2}  {3}", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    }
                Console.WriteLine("读取数据库完成");
                conn.Close();
            }
            */
            //上面是一些测试，到时候可以删掉，下面创建数据库
            CreateDataBase();
            CreateTable();
            infos_pgsql = new ObservableCollection<ExamClass>() { };
            myDataSet = new DataSet();
            myDataSetLast = new DataSet();
            myDataSetFind = new DataSet();
            GetDataToInfos();
            Console.WriteLine("构造完成");
        }

        /// <summary>
        /// 数据库是否存在
        /// </summary>
        /// <param name="name">无参输入</param>
        /// <returns>0-不存在，1-存在，2-报错</returns>
        public int IsExistsDataBase()
        {
            dataBaseName = "iron";//只能小写
            tableName = "mydatatable";
            connection_pgsql = "Host=localhost;Port=2333;Username=postgres;Password=123456;";
            //cmd_pgsql = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=dem1o1";
            cmd_pgsql = "SELECT u.datname  FROM pg_catalog.pg_database u where u.datname='" + dataBaseName + "';";
            Console.WriteLine("查询是否有数据库命令 {0}", cmd_pgsql);
            try
            {
                using (var connect = new NpgsqlConnection(connection_pgsql))
                {
                    connect.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(cmd_pgsql, connect);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("是否存在数据库  结果{0}", reader.GetString(0));
                            if (reader.GetString(0) == dataBaseName)
                            {
                                Console.WriteLine("已经存在数据库");
                                connect.Close();
                                return 1;
                            }
                        }
                        Console.WriteLine("不存在数据库  ");
                        connect.Close();
                        return 0;
                    }
                }
            }
            catch
            {
                Console.WriteLine("数据库操作出错");
                return 2;//报错
            }
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="name">无参输入</param>
        /// <returns>无参返回，但可以通过IsExistsDataBase()验证</returns>
        public void CreateDataBase()
        {
            int temp = IsExistsDataBase();
            if(temp==0)
            {
                Console.WriteLine("数据库不存在");
                try
                {
                    using (var connection = new NpgsqlConnection(connection_pgsql))
                    {
                        connection.Open();
                        cmd_pgsql = "CREATE DATABASE " + dataBaseName; //+ ";";
                        NpgsqlCommand cmd = new NpgsqlCommand(cmd_pgsql, connection);
                        cmd.ExecuteNonQuery();//执行查询命令
                        Console.WriteLine("数据库创建命令{0}", cmd_pgsql);
                        int temp2 = IsExistsDataBase();
                        if(temp2==1)
                            Console.WriteLine("数据库确实创建成功");
                        else
                            Console.WriteLine("数据库没有创建成功");
                        connection.Close();
                    }
                }
                catch
                {
                    Console.WriteLine("数据库创建失败");
                }
            }
            else if(temp == 1)
            {
                Console.WriteLine("数据库存在");
            }
            else
            {
                Console.WriteLine("异常");
            }
        }

        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="name">无参输入</param>
        /// <returns>无参返回，但可以通过能否获得数据进行验证</returns>
        public void CreateTable()
        {
            connection_pgsql = "Host=localhost;Port=2333;Username=postgres;Password=123456;Database=" + dataBaseName;
            Console.WriteLine("创建表命令 查询{0}", connection_pgsql);
            try
            {
                using (var conn = new NpgsqlConnection(connection_pgsql))
                {
                    conn.Open();
                    cmd_pgsql = "CREATE TABLE IF NOT EXISTS "+tableName+"(Designation TEXT, type TEXT, Name TEXT PRIMARY KEY, Value integer); ";//名字是主键  不要大写 
                    NpgsqlCommand cmd = new NpgsqlCommand(cmd_pgsql, conn);
                    cmd.ExecuteNonQuery();//执行查询命令
                    Console.WriteLine("数据库创建表操作成功");
                    conn.Close();
                }
            }
            catch
            {
                Console.WriteLine("数据库创建表操作出错");
            }
        }

        /// <summary>
        /// 1s获得一次数据库内容
        /// </summary>
        /// <param name="name">无参输入</param>
        /// <returns>无参返回，被twoDataSetIsNotEqual调用可以验证</returns>
        public void Timer_1s_getDataSet()//每隔1s获取一次
        {
            cmd_pgsql = "SELECT * FROM " + tableName;//
            myDataSetLast.Clear();
            myDataSetLast = GetDataSet(cmd_pgsql);
        }

        /// <summary>
        /// 比较两个表格内容是否完全一致
        /// </summary>
        /// <param name="name">无参输入</param>
        /// <returns>true-有效，false0-无效，可以设定myDataSet和myDataSetLast的内容进行验证</returns>
        public bool compare_TwoData()
        {
            try
            {
                Console.WriteLine("myDataSet几行几个列  {0}  {1}", myDataSet.Tables[0].Rows.Count, myDataSet.Tables[0].Columns.Count);
                Console.WriteLine("myDataSetLast几行几个列  {0}  {1}", myDataSetLast.Tables[0].Rows.Count, myDataSetLast.Tables[0].Columns.Count);
            }
            catch
            {
                Console.WriteLine("有一个表格不存在，暂时认为相等");
                return true;
            }

            if (myDataSetLast.Tables[0].Rows.Count != myDataSet.Tables[0].Rows.Count|| myDataSetLast.Tables[0].Columns.Count != myDataSet.Tables[0].Columns.Count)
                return false;
            string a, b;
            
            for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < myDataSet.Tables[0].Columns.Count; j++)
                {
                    //string name = ((XmlElement)node).GetAttribute("name");
                    a =myDataSet.Tables[0].Rows[i].ItemArray[j].ToString();
                    b =myDataSetLast.Tables[0].Rows[i].ItemArray[j].ToString();
                    if(a != b)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 比较两个表格内容是否完全一致，由界面函数进行定时调用
        /// </summary>
        /// <param name="name">无参输入</param>
        /// <returns>true-有效，false0-无效，分两个函数验证，重点验证compare_TwoData</returns>
        public bool twoDataSetIsNotEqual()
        {
            Timer_1s_getDataSet();
            if (compare_TwoData()== false)//不相等
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取数据库内容到Infos数组中
        /// </summary>
        /// <param name="name">无参输入</param>
        /// <returns>无参返回，界面调用可以验证</returns>
        public void GetDataToInfos()
        {
            cmd_pgsql = "SELECT * FROM "+ tableName;//
            myDataSet.Clear();
            infos_pgsql.Clear();
            myDataSet =GetDataSet(cmd_pgsql);
            try
            {
                if(myDataSet.Tables[0].Rows.Count == 0)
                {
                } 
            }
            catch
            {
                MessageBox.Show("请打开数据库的PgAdmin，启动数据库服务");
                return;
            }
            for (int i=0;i< myDataSet.Tables[0].Rows.Count;i++)
            {
                //string name = ((XmlElement)node).GetAttribute("name");
                string one1 = myDataSet.Tables[0].Rows[i].ItemArray[0].ToString();
                string one2 = myDataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                string one3 = myDataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                string one4 = myDataSet.Tables[0].Rows[i].ItemArray[3].ToString();
                Console.WriteLine("w:{0},x:{1},y:{2},z:{3}", one1, one2, one3, one4);
                infos_pgsql.Add(new ExamClass() { Designation = one1, type = one2, Name = one3, Value = int.Parse(one4) });
            }
            Console.WriteLine("数据库读取完成");
        }

        /// <summary>
        /// 判断输入是否是数字
        /// </summary>
        /// <param name="name">字符串输入</param>
        /// <returns>true有效，无效</returns>
        public bool IsNumer(string s_find)
        {
            int number;
            if(int.TryParse(s_find,out number)==true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 返回查询表格所在行号
        /// </summary>
        /// <param name="name">字符串输入</param>
        /// <returns>true有效，无效</returns>
        public DataSet GetDataSet_by_sql_query(string s_find)//给主函数用   模糊查找返回
        {
            string sql;
            if (IsNumer(s_find))//带数字型查找
            {
                sql = "select * FROM (select t.*,ROW_NUMBER() OVER() as rownum  FROM  " + tableName + " t) AS foo where " + "Designation" + " ilike '%" + s_find + "%'" + " or ";
                sql = sql + "type" + " ilike '%" + s_find + "%'" + " or " + "Name" + " ilike '%" + s_find + "%'" + " or " + "Value=" + s_find;
                Console.WriteLine("查询命令 包含 {0}", sql);
            }
            else
            {
                sql = "select * FROM (select t.*,ROW_NUMBER() OVER() as rownum  FROM  " + tableName + " t) AS foo where " + "Designation" + " ilike '%" + s_find + "%'" + " or ";
                sql = sql + "type" + " ilike '%" + s_find + "%'" + " or " + "Name" + " ilike '%" + s_find + "%'";
                Console.WriteLine("查询命令 只有字符串 {0}", sql);
            }
            //select * from table1 where name like '%iphone%' or name like '%iphone%' or id=5 
            //SELECT t.*,ROW_NUMBER() OVER() as rownum  FROM  table2 t where name ilike '%Huawei%'
            //嵌套查询select * FROM (select t.*,ROW_NUMBER() OVER() as rownum  FROM  mydatatable t) AS foo where Designation ilike '%12%'or type ilike '%12%'  or Name ilike '%12%' or Value = 12;
            myDataSetFind = GetDataSet(sql);
            return myDataSetFind;
        }
        //4.获取DataSet
        public DataSet GetDataSet(string sql)

        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connection_pgsql))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                    NpgsqlDataAdapter NpgDa = new NpgsqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    NpgDa.Fill(ds);
                    conn.Close();
                    return ds;
                }
            }
            catch (Exception ex)
            {
                return new DataSet();
            }
        }
         public int ExecuteNonQuery(string sql)//5.增删改
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connection_pgsql))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                    int r = cmd.ExecuteNonQuery();  //执行查询并返回受影响的行数
                    conn.Close();
                    return r;
                }
            }
            catch// (Exception ex)
            {
                return 0;

            }

        }
        public bool IsRepeatedInPrimarKey(string s_name)
        {
            string sql="select* from mydatatable where name = "+ s_name+ ";";//select * from mydatatable where name='12';
            DataSet DataSet_cur=GetDataSet(sql);
            if (DataSet_cur != null)
            {
                try
                {
                    if (DataSet_cur.Tables[0].Rows.Count >= 1)
                        return true; 
                }
                catch
                {
                    return false;
                }
            }
           return false;

        }

        public void DataSet_updata_to_pgsql(ObservableCollection<ExamClass> infos_temp)
        {
            DataSet DataSetTemp = new DataSet();
            DataSetTemp.Clear();
            DataTable dtb = new DataTable(tableName);//细节的坑在这里
            DataSetTemp.Tables.Add(dtb);
            DataColumn mycl0 = new DataColumn("Designation", Type.GetType("System.String"));
            DataColumn mycl1 = new DataColumn("type", Type.GetType("System.String"));
            DataColumn mycl2 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn mycl3 = new DataColumn("Value", Type.GetType("System.Int32"));
            dtb.Columns.Add(mycl0);
            dtb.Columns.Add(mycl1);
            dtb.Columns.Add(mycl2);
            dtb.Columns.Add(mycl3);
            for (int i=0;i<infos_temp.Count;i++)
            {
                DataRow drw = dtb.NewRow();
                dtb.Rows.Add(drw);
                //myDataSet.Tables[0].Rows[i].ItemArray[0] = infos_temp[i].Designation;
                // myDataSet.Tables[0].Rows[i].ItemArray[1] = infos_temp[i].type;
                //myDataSet.Tables[0].Rows[i].ItemArray[2] = infos_temp[i].Name;
                // myDataSet.Tables[0].Rows[i].ItemArray[3] = infos_temp[i].Value;
                drw[0] = infos_temp[i].Designation;
                drw[1] = infos_temp[i].type;
                drw[2] = infos_temp[i].Name;
                drw[3] = infos_temp[i].Value;
            }
            for (int i = 0; i < DataSetTemp.Tables[0].Rows.Count; i++)
            {
                //string name = ((XmlElement)node).GetAttribute("name");
                string one1 = DataSetTemp.Tables[0].Rows[i].ItemArray[0].ToString();
                string one2 = DataSetTemp.Tables[0].Rows[i].ItemArray[1].ToString();
                string one3 = DataSetTemp.Tables[0].Rows[i].ItemArray[2].ToString();
                string one4 = DataSetTemp.Tables[0].Rows[i].ItemArray[3].ToString();
                Console.WriteLine("查看dataset  w:{0},x:{1},y:{2},z:{3}", one1, one2, one3, one4);
            }
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connection_pgsql))
                {
                    conn.Open();
                    string mySQL0 = "DELETE FROM " + tableName;
                    NpgsqlCommand cmd = new NpgsqlCommand(mySQL0, conn);
                    cmd.ExecuteNonQuery();//执行查询命令
                    string mySQL = "SELECT * FROM " + tableName;
                    Console.WriteLine("查看连接符号:{0}", connection_pgsql);
                    Console.WriteLine("查看{0}", mySQL);
                    NpgsqlDataAdapter myAdapter = new NpgsqlDataAdapter(mySQL, conn);//传递给下一句
                    NpgsqlCommandBuilder myCommendBuilder = new NpgsqlCommandBuilder(myAdapter);      
                    myAdapter.Update(DataSetTemp, tableName);
                    conn.Close();
                    Console.WriteLine("数据库更新完成");
                    bSaveIsOk = true;
                }
            }
            catch
            {
                Console.WriteLine("数据库更新表操作出错");
                bSaveIsOk = false;
            }
            GetDataToInfos();//更新自己的数据
        }
    }

}
