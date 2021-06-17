using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Threading.Tasks;

namespace AzTableDemo
{
    class Program
    {
        static string ConnectionString = "";

        static string rowKey = "Customer";
        static string partitionKey = "India";
        public static async Task Main(string[] args)
        {
            await ProcessAsync();

            Console.WriteLine("Program Finished....");
            Console.ReadLine();

        }

        private static async Task ProcessAsync()
        {
            TableServiceClient client = new TableServiceClient(ConnectionString);
            var table = await CreateTable(client);
            InsertRecord(client, table);
            ReadRecord(client,table);
            DeleteTable(client,table);
        }

        private static void DeleteTable(TableServiceClient client, TableItem table)
        {
            client.DeleteTable(table.Name);
        }

        private static async Task<TableItem> CreateTable(TableServiceClient client)
        {
            string tableName = "CustomerTable";
            TableItem table = client.CreateTable(tableName);;
            return table;
        }

        private static void ReadRecord(TableServiceClient client, TableItem tableItem)
        {
            TableClient table = client.GetTableClient(tableItem.Name);
            Pageable<TableEntity> queryResultsFilter = table.Query<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'");
            foreach (TableEntity qEntity in queryResultsFilter)
            {
                Console.WriteLine($"{qEntity.GetString("FirstName")}: {qEntity.GetString("City")}");
            }

        }

        private static void InsertRecord(TableServiceClient client, TableItem tableItem)
        {
            TableClient table = client.GetTableClient(tableItem.Name);
            var entity = new TableEntity(partitionKey, rowKey)
            {
                { "FirstName", "Piyush" },
                { "City", "Mumbai" }
            };
            table.AddEntity(entity);
        }
    }
}
