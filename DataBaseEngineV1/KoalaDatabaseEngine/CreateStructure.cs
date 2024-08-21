namespace Koala
{
    internal static class CreateStructure
    {
        internal static List<Table> CreateTables(string path)
        {
            List<Table> tables = new List<Table>();
            short NoOfTables = GetNumber("Tables");
            for (int i = 0; i < NoOfTables; i++)
            {
                tables.Add(new Table());
                tables[i].TableName = CreateTable(i + 1, path);
                short NoOfProperties = GetNumber("Properties");
                for (int j = 0; j < NoOfProperties; j++)
                {
                    tables[i].Properties.Add(CreateProperty(j + 1, tables[i], path));
                }
                Console.WriteLine(tables[i]);
            }
            return tables;
        }
        private static string CreateTable(int noOfTable, string path)
        {
            bool TableNameFlag = false;
            string TableName = "";
            while (!TableNameFlag)
            {
                Console.WriteLine();
                Console.WriteLine($"Please enter the name of the {(Utility.Numbers)noOfTable} Table:");
                TableName = Console.ReadLine() ?? "";
                if (TableName != "")
                {
                    path = path + @"\" + TableName + ".txt";
                    Utility.CreateFile(path);
                    TableNameFlag = true;
                }
                else
                {
                    Console.WriteLine("Please enter a valid table name: ");
                }
            }
            return TableName;
        }
        private static KeyValuePair<string, string> CreateProperty(int noOfProperty, Table tableName, string path)
        {
            bool PropertyNameFlag = false;
            string[] PropertyArray = ["",""];
            KeyValuePair<string, string> Property;
            while (!PropertyNameFlag)
            {
                Console.WriteLine();
                Console.WriteLine($"Please enter the type and name of the {(Utility.Numbers)noOfProperty} Property of {tableName.TableName}:");
                Console.WriteLine("Example: 'int ID'  |||| Example2: 'string first_name'");
                Console.WriteLine("To view available types write 'help'.");
                Console.WriteLine();
                PropertyArray = Console.ReadLine()!.Split(" ");

                if(PropertyArray[0] == "help")
                {
                    Utility.PrintTypes();
                }
                else if (PropertyArray.Length == 2 && PropertyArray[1] != null)
                {
                    if (Utility.WithinTypes(PropertyArray[0]))
                    {
                        PropertyNameFlag = true;
                    }

                }
                else
                {
                    Console.WriteLine("Please enter a valid property ex:(bool numFlag): ");
                }
            }
            Property = new KeyValuePair<string, string>(PropertyArray[1], PropertyArray[0]);
            return Property;
        }
        private static short GetNumber(string noOfWhat)
        {
            bool NumberFlag = false;
            short Number = 0;
            while (!NumberFlag)
            {
                Console.WriteLine();
                Console.WriteLine($"Please enter the desired number of {noOfWhat}:");
                if (short.TryParse(Console.ReadLine(), out short no))
                {
                    Number = no;
                    NumberFlag = true;
                }
                else
                {
                    Console.WriteLine("Please enter a valid number");
                }
            }
            return Number;
        }
    }
}
