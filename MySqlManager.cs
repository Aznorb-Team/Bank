using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Bank
{
    public class MySqlManager
    {
        protected static string dataSource, port, username, password, dataBase, SSLMode, connectionString, query;

        protected static MySqlConnection databaseConnection;
        protected static MySqlCommand commandDatabase;
        protected static MySqlDataReader reader;

        protected enum type { create, insert, select, delete, drop, update};
        protected static type typeQuery;

        public MySqlManager(string dataSource, string port, string username, string password, string dataBase, string SSLMode)
        {
            MySqlManager.dataSource = dataSource;
            MySqlManager.port = port;
            MySqlManager.username = username;
            MySqlManager.password = password;
            MySqlManager.dataBase = dataBase;
            MySqlManager.SSLMode = SSLMode;
            Connect();
            //StartQuery("CREATE TABLE users (id int, login text, password text);");
        }
        public static void Connect()
        {
            connectionString = $"datasource={dataSource};port={port};username={username};password={password};database={dataBase};SSL Mode={SSLMode};charset=utf8";
            databaseConnection = new MySqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                Console.WriteLine("Вход успешно выполнен!");
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка входа!");
                databaseConnection.Close();
            }
        }
        public static void StartQuery(string query)
        {
            setQuery(query);
            if (query.Contains("CREATE"))
                typeQuery = type.create;
            if (query.Contains("DELETE"))
                typeQuery = type.delete;
            if (query.Contains("SELECT"))
                typeQuery = type.select;
            if (query.Contains("INSERT"))
                typeQuery = type.insert;
            if (query.Contains("DROP"))
                typeQuery = type.drop;
            if (query.Contains("UPDATE"))
                typeQuery = type.update;

            switch (typeQuery)
            {
                case type.create:
                    createTable();
                    break;
                case type.delete:
                    deleteData();
                    break;
                case type.select:
                    selectData();
                    break;
                case type.insert:
                    insertTable();
                    break;
                case type.drop:
                    deleteTable();
                    break;
                case type.update:
                    updateTable();
                    break;
            }
        }
        protected static void loadQuery(string successfullyQuery, string notSuccessfullyQuery)
        {
            commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            databaseConnection.Close();
            try
            {
                databaseConnection.Open();
                //databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();
                Console.WriteLine(successfullyQuery);
                if (successfullyQuery.Contains("Чтение"))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string[] row = { reader.GetString(0), reader.GetString(1) };
                            foreach (string r in row)
                            {
                                Console.Write(r + " ");
                            }
                            Console.WriteLine();
                        }

                    }
                    else
                    {
                        Console.WriteLine("Не найдено данных!");
                    }
                }
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(notSuccessfullyQuery);
                databaseConnection.Close();
                Console.WriteLine(ex.Message);
            }
        }
        protected static void createTable()
        {
            loadQuery("Таблица создана!", "Ошибка создания таблицы!");
        }
        protected static void insertTable()
        {
            loadQuery("Вставка произведена!", "Ошибка вставки!");
        }
        protected static void deleteData()
        {
            loadQuery("Удаление данных таблицы успешно!", "Ошибка удаления данных таблицы!");
        }
        protected static void deleteTable()
        {
            loadQuery("Удаление таблицы успешно!", "Ошибка удаления таблицы!");
        }
        protected static void selectData()
        {
            loadQuery("Чтение таблицы!", "Ошибка в чтении таблицы!");
        }

        protected static void updateTable()
        {
            loadQuery("Изменение таблицы!", "Ошибка в изменении таблицы!");
        }

        public static void setQuery(string query)
        {
            MySqlManager.query = query;
        }
        public static string getQuery()
        {
            return query;
        }
        public static double getResultsQuery(string query)
        {
            commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            try
            {
                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            double a = Convert.ToDouble(reader.GetString(0));
                            databaseConnection.Close();
                            return a;
                            
                        }

                    }
                    else
                    {
                        Console.WriteLine("Не найдено данных!");
                    }
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
            return 0;
        }
    }
}
