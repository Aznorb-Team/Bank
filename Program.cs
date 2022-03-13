using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bank
{
    class Program
    {
        public static int start, end;
        static void Main(string[] args)
        {
            MySqlManager database = new MySqlManager("127.0.0.1", "3306", "root", "", "bank", "none");

            int n = 10;
            int chapter = 0;
            int share = n / 8;

            Bank bank = new Bank();
            //Создаем 100000 аккаунтов с максимальной суммой денег 500000
            //bank.generateAccount(100, 500000);
            start = chapter;
            end = chapter += share * 8 + 1;
            generateTransactionChapter(bank);
            /*for (int i = 0; i < 8; i++)
            {
                Thread thread1 = new Thread(new ParameterizedThreadStart(generateTransactionChapter));
                start = chapter;
                end = chapter += share * i + 1;
                thread1.Start(bank);
            }*/
        }
        public static void generateTransactionChapter(Object? bank1)
        {
            int idSender = 0, idRecipient = 0, value = 0, randomMax = 0;
            Random rnd = new Random();
            Bank bank = (Bank)bank1;
            for (int i = start; i < end; i++)
            {
                while (idSender == idRecipient)
                {
                    idSender = Convert.ToInt32(MySqlManager.getResultsQuery("SELECT account_id FROM account ORDER BY RAND() LIMIT 1"));
                    idRecipient = Convert.ToInt32(MySqlManager.getResultsQuery("SELECT account_id FROM account ORDER BY RAND() LIMIT 1"));

                    randomMax = Convert.ToInt32(MySqlManager.getResultsQuery($"SELECT balance FROM account WHERE account_id = {idSender}"));
                }

                //Получить случайное число (в диапазоне от 0 до 10)
                value = rnd.Next(0, randomMax);
                Console.WriteLine($"Sender: {idSender}; Recipient: {idRecipient}; Value: {value}; Random Max {randomMax}");
                //Task task1 = new Task(() => bank.generateOperations(idSender, idRecipient, value));
                bank.generateOperations(idSender, idRecipient, value);
                // task1.Start();
                
                idSender = 0; idRecipient = 0; value = 0;
            }
        }
    }
}
