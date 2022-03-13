using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bank
{
    public class Bank
    {
        public void generateAccount(int count, int maxBalance)
        {
            string query = "INSERT INTO `account` (`balance`) VALUES ";
            Random rnd = new Random();
            for (int i = 0; i<count; i++)
            {
                double balanceRandom = rnd.Next(maxBalance);
                
                if(i == count - 1)
                {
                    query += $"('{balanceRandom}')";
                }
                else
                {
                    query += $"('{balanceRandom}'),";
                }
                //Console.WriteLine(balanceRandom);
            }
            query += ";";
            //Console.WriteLine(query);
            MySqlManager.StartQuery(query);
        }
        
        public void generateOperations(int idSender, int idRecipient, double value)
        {
            double balanceSender = MySqlManager.getResultsQuery($"SELECT balance FROM account WHERE account_id = '{idSender}';");
            
            //Console.WriteLine(balanceSender);
            if(balanceSender > value)
            {
                string transaction = "START TRANSACTION;";
                int transaction_id = Convert.ToInt32(getTransaction());
                string query = "INSERT INTO `operations` (`operation_id`, `operation`, `account_id`, `value`, `trans_id`) VALUES ";

                string querySender = query;
                querySender += $"(NULL, '1', '{idSender}', '{value}', '{transaction_id}');";

                transaction += querySender;
                transaction += $"UPDATE account SET balance = (balance - {value}) WHERE account_id = {idSender};";
                //MySqlManager.StartQuery(querySender);
                //MySqlManager.StartQuery($"UPDATE account SET balance = (balance - {value}) WHERE account_id = {idSender};");

                //INSERT INTO `operations` (`operation_id`, `operation`, `account_id`, `value`, `trans_id`) VALUES (NULL, '1', '1', '20', '1');

                string queryRecipient = query;
                queryRecipient += $"(NULL, '2', '{idRecipient}', '{value}', '{transaction_id}');";
                transaction += queryRecipient;
                transaction += $"UPDATE account SET balance = (balance + {value}) WHERE account_id = {idRecipient};";
                //MySqlManager.StartQuery(queryRecipient);
                //MySqlManager.StartQuery($"UPDATE account SET balance = (balance + {value}) WHERE account_id = {idRecipient};");
                transaction += "COMMIT;";
                MySqlManager.StartQuery(transaction);
                Console.WriteLine($"Транзакция между получателем ({idRecipient} и отправителем ({idSender}) произошла успешно, сумма: {value}");
            }
            else
            {
                Console.WriteLine($"Транзакция между получателем ({idRecipient} и отправителем ({idSender}) не удалась из-за недостаточного баланса у отправителя.");
            }
            //Console.WriteLine($"{idSender} - {idRecipient}: {value}");


        }
        private int getTransaction()
        {
            MySqlManager.StartQuery("INSERT INTO `transaction` (`transaction_id`) VALUES ('');");
            return Convert.ToInt32(MySqlManager.getResultsQuery("SELECT LAST_INSERT_ID() FROM transaction;"));
        }
    }
}
