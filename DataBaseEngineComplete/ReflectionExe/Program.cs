using LogicClasses;
using System;
using System.Reflection;

namespace ReflectionExe
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Fetch existing tables on application start
            //ExistingTables.FetchAll();
            //Console.WriteLine("\n-----------------------\n");

            bool exit = false;

            while (!exit)
            {
                // Display the menu
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. Create a new table");
                Console.WriteLine("2. Insert data into a table");
                Console.WriteLine("3. Select from a table");
                Console.WriteLine("4. Delete from a table");
                Console.WriteLine("5. Update from a table");
                Console.WriteLine("6. Exit");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateTableProcess();
                        break;
                    case "2":
                        InsertDataProcess();
                        break;
                    case "3":
                        SelectDataProcess();
                        break;
                    case "4":
                        DeleteDataProcess();
                        break;
                    case "5":
                        UpdateDataProcess();
                        break;
                    case "6":
                        exit = true;
                        Console.WriteLine("Exiting the application...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please select a valid option.");
                        break;
                }
                Console.WriteLine("\n-----------------------\n");
            }
        }

        private static void CreateTableProcess()
        {
            string? numOfTablesInput;
            uint numOfTables;

            do
            {
                Console.WriteLine("How many tables do you want to create?");
                numOfTablesInput = Console.ReadLine();
            }
            while (string.IsNullOrEmpty(numOfTablesInput) ||
                   !uint.TryParse(numOfTablesInput, out numOfTables));

            // Create tables based on the user input
            Creation.CreateTables(numOfTables);

            // Print all created tables for verification
            Console.WriteLine("Tables created:");
            foreach (KeyValuePair<Type, string> keyValue in Tables.UserTables)
            {
                Console.WriteLine($"Table Name: {keyValue.Value}");
                Console.WriteLine($"Table Type: {keyValue.Key}");
                foreach (var item in keyValue.Key.GetProperties())
                {
                    Console.WriteLine($"---{item.Name}: {item.PropertyType}");
                }
            }
        }

        private static void InsertDataProcess()
        {
            ExistingTables.FetchAll();

            string? input;
            Type? table = null;

            do
            {
                Console.WriteLine("Enter The Table Name That You Want To Insert Into:");
                input = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine($"Enter The Data of {input} Table :");
                // Convert input to lowercase for consistency
                string normalizedInput = input?.ToLower();

                // Check if the table exists in UserTables
                if (Tables.UserTables.ContainsValue(normalizedInput))
                {
                    table = Tables.UserTables.FirstOrDefault(kv => kv.Value == normalizedInput).Key;
                }
                else
                {
                    Console.WriteLine("Table not found. Please enter a valid table name.");
                }

            } while (table == null);

            // Proceed with inserting data into the identified table type
            Insertion.InsertDataIntoTable(table);
        }

        private static void SelectDataProcess()
        {
            UserPrompt.Start();
        }

        private static void DeleteDataProcess()
        {
            string tableName = UserPrompt.TableName();
            UserPrompt.DeleteRecord(tableName);
        }

        private static void UpdateDataProcess() 
        {
            ExistingTables.FetchAll();
            string tableName = UserPrompt.TableNameExist();
            if(tableName != null)
            {
                UserPrompt.UpdateRecord(tableName);
            }
            else
            {
                Console.WriteLine("Table doesn't exist.");
            }
        }


    }
}
    