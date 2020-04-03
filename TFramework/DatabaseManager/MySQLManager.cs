﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFramework.Singleton;

namespace TFramework.DatabaseManager
{
    public class MySQLManager : AutoGeneratedSingleton<MySQLManager>
    {
        private string m_connectString = string.Empty;

        /// <summary>
        /// 初始化 MySQL 的
        /// </summary>
        /// <param name="connectString"></param>
        public void Init(string connectString)
        {
            m_connectString = connectString;
        }
        /// <summary>
        /// 查找表的数据 
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <returns>一个DataSet的表单</returns>
        public DataSet SelectTables(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(m_connectString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(query, connection);
                    command.Fill(ds);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }

        /// <summary>
        /// 查询一条数据是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="conditions">条件, eg: "CardID=0"</param>
        /// <returns></returns>
        public bool ExistsData(string tableName, string conditions)
        {
            string query = string.Format("select count(*) from {0} where {1} limit 1;", tableName, conditions);
            int result = 0;
            using (MySqlConnection connection = new MySqlConnection(m_connectString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand mycmd1 = new MySqlCommand(query, connection);

                    //执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
                    object obj = mycmd1.ExecuteScalar();
                    if (obj != null)
                    {
                        result = Convert.ToInt32(obj);
                    }
                }
                catch (System.Data.SqlClient.SqlException msg)
                {
                    Console.WriteLine(string.Format("Mysql ExistsData Error: {0}", msg.Message));
                }
                finally
                {
                    connection.Close();
                }
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 插入 单条 数据 表示全列插入
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="sqlData">插入的数据</param>
        /// <returns></returns>
        public int InsesetData(string tableName, string sqlData)
        {
            if (string.IsNullOrEmpty(sqlData)) return 0;
            string query = string.Format("insert into {0} values({1});", tableName, sqlData);
            int result = 0;
            using (MySqlConnection connection = new MySqlConnection(m_connectString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand mycmd1 = new MySqlCommand(query, connection);
                    result = mycmd1.ExecuteNonQuery();
                }
                catch (SqlException msg)
                {
                    Console.WriteLine(string.Format("Mysql InsesetData Error: {0}", msg.Message));
                }
                finally
                {
                    connection.Close();
                }
                return result;
            }
        }

        /// <summary>
        /// 插入多条数据， 全列插入
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="SQLStringList">数据字符串</param>
        /// <returns></returns>
        public int InsesetBigData(string tableName, List<string> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(m_connectString))
            {
                int result = 0;
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string value = SQLStringList[n];
                        string strsql = string.Format("insert into {0} values({1});", tableName, value);
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            result += cmd.ExecuteNonQuery();
                        }
                        //后来加上的，防止数据量过大，事务卡死现象
                        if (n > 0 && (n % 500 == 0 || n == SQLStringList.Count - 1))
                        {
                            tx.Commit();
                            //二次事务处理  
                            tx = conn.BeginTransaction();
                            cmd.Transaction = tx;
                        }
                    }
                    //最后一次提交（网上提供的这句话是被注释掉的，大爷的，错了。该句必须有，不然最后一个循环的数据无法提交）
                    tx.Commit();
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    result = -1;
                    tx.Rollback();
                    Console.WriteLine(E.Message);
                }
                catch (Exception ex)
                {
                    result = -1;
                    tx.Rollback();
                    Console.WriteLine(ex.Message);
                }

                return result;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="query">sql语句</param>
        /// <returns></returns>
        public int UpdateData(string query)
        {
            if (string.IsNullOrEmpty(query)) return 0;
            int result = 0;
            using (MySqlConnection connection = new MySqlConnection(m_connectString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand mycmd1 = new MySqlCommand(query, connection);
                    result = mycmd1.ExecuteNonQuery();
                }
                catch (SqlException msg)
                {
                    Console.WriteLine(string.Format("Mysql UpdateData Error: {0}", msg.Message));
                }
                finally
                {
                    connection.Close();
                }
                return result;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <returns></returns>
        public int DeleteData(string query)
        {
            if (string.IsNullOrEmpty(query)) return 0;
            int result = 0;
            using (MySqlConnection connection = new MySqlConnection(m_connectString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand mycmd1 = new MySqlCommand(query, connection);
                    result = mycmd1.ExecuteNonQuery();
                }
                catch (SqlException msg)
                {
                    Console.WriteLine(string.Format("Mysql DeleteData Error: {0}", msg.Message));
                }
                finally
                {
                    connection.Close();
                }
                return result;
            }
        }

    }
}
