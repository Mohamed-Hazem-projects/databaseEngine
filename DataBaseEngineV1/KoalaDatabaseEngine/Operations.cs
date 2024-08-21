using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Koala
{
    internal class Operations
    {
        internal static void StartOperations(List<Table> tables, string path)
        {
            bool operationsFlag = false;
            while (!operationsFlag)
            {
                Console.WriteLine();
                Console.WriteLine($"Please enter your desired operation:");
                Console.WriteLine("1- Insert Into Table");
                Console.WriteLine("2- Select From Table");
                Console.WriteLine("3- Update Table");
                Console.WriteLine("4- Delete from Table");
                Console.WriteLine("5- View Log");
                Console.WriteLine("6- Close Operations");
                short operation = 0;
                if (short.TryParse(Console.ReadLine(), out short op))
                {
                    operation = op;
                }
                switch (operation)
                {
                    case 1:
                        InsertIntoTable(tables, path);
                        break;
                    case 2:
                        SelectFromTable(tables, path);
                        break;
                    case 3:
                        UpdateTable(tables, path);
                        break;
                    case 4:
                        DeleteFromTable();
                        break;
                    case 5:
                        ViewLog();
                        break;
                    case 6:
                        operationsFlag = true;
                        break;
                    default:
                        Console.WriteLine("Please choose an operation from (1~6)");
                        break;
                }
            }
        }
        private static void InsertIntoTable(List<Table> tables, string path)
        {
            //user choosing which table to insert into
            Table table = GetTable(tables);
            //creating an instance of the class
            object DynamicTableInstance = Reflection.CreateDynamicClass(table);
            //getting the class itself from the created instance
            Type DynamicTableClass = DynamicTableInstance.GetType();
            string json = File.ReadAllText($"{path}\\{table.TableName}.txt");
            // Initialize the list and try to deserialize it from JSON
            var list = new List<Dictionary<string, object>>();
            if (!string.IsNullOrEmpty(json))
            {
                list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            }

            // If deserialization fails, ensure the list is not null
            if (list == null)
            {
                list = new List<Dictionary<string, object>>();
            }

            using StreamWriter streamWriter = new StreamWriter($"{path}\\{table.TableName}.txt", false);

            var propDict = new Dictionary<string, object>();
            foreach (var property in DynamicTableClass.GetProperties())
            {
                bool PropertyFlag = false;
                while (!PropertyFlag)
                {
                    try
                    {
                        Console.WriteLine($"Please enter the value for {property.Name} of type {property.PropertyType}");
                        // Read user input
                        string PropertyValueStr = Console.ReadLine()!;
                        // Convert the input to the appropriate type
                        object? PropertyValue = Convert.ChangeType(PropertyValueStr, property.PropertyType);
                        DynamicTableClass.GetProperty(property.Name.ToString())!.SetValue(DynamicTableInstance, PropertyValue);
                        PropertyInfo[] properties = DynamicTableClass.GetProperties();
                        foreach (var propertyInfo in properties)
                        {
                            string name = property.Name;
                            object value = PropertyValue;
                            propDict[name] = value;
                        }
                        PropertyFlag = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Please enter a value with valid datatype");
                    }
                }
            }
            list.Add(propDict);
            json = JsonConvert.SerializeObject(list, Formatting.Indented);
            streamWriter.WriteLine(json);
            streamWriter.Close();

        }
        private static Table GetTable(List<Table> tables)
        {
            bool FindTableFlag = false;
            Table table = new Table();
            while (!FindTableFlag)
            {
                Console.WriteLine();
                Console.WriteLine("Please choose a table by number from list of tables:");
                for (int i = 0; i < tables.Count; i++)
                {
                    Console.WriteLine($"{i + 1} - {tables[i].TableName}");
                }
                if (short.TryParse(Console.ReadLine(), out short no) && no <= tables.Count)
                {
                    table = tables[no - 1];
                    FindTableFlag = true;
                }
                else
                {
                    Console.WriteLine("Please enter a valid table number!.");
                }
            }
            return table;
        }
        private static void SelectFromTable(List<Table> tables, string path)
        {
            // Get Table User will Select from.
            Table table = GetTable(tables);
            // Check if File of Table is created if not will return from the function. 
            if (!File.Exists(path + $@"\{table.TableName}.txt"))
            {
                Console.WriteLine("Table does not Exist!");
                return;
            }
            bool flag = false;
            List<String> listOfProperties = new List<String>();

            // Read json data from the file
            string json = File.ReadAllText(path + $@"\{table.TableName}.txt");
            var list = new List<Dictionary<string, object>>();
            if (!string.IsNullOrEmpty(json))
            {
                list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            }

            // If deserialization fails, ensure the list is not null
            if (list == null)
            {
                list = new List<Dictionary<string, object>>();
            }

            // convert Json format to List of Dictionary<string, object>
            list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            while (!flag)
            {
                Console.WriteLine();
                Console.WriteLine($"Choose property name to filter {table.TableName} By (Enter Exact Name): ");
                // List Names of the properties
                foreach (var prop in table.Properties)
                {
                    Console.WriteLine(prop.Key);
                    // append names of properties to list
                    listOfProperties.Add(prop.Key);
                }
                Console.WriteLine();
                Console.Write("Property: ");
                string filteredName = Console.ReadLine()!;
                // Check if user entered exact name of the property
                if (!listOfProperties.Any(x => x == filteredName))
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter the exact name of the property!");
                    continue;
                }
                Console.WriteLine($"Enter value to filter by: ");
                // Get value to filter List of Dictionaries using it
                object value = Console.ReadLine()!;
                //
                //
                //ENTER TYPE VALIDATION HERE
                Type valueType = value.GetType();
                // Get Type of the Property the user choosed
                Type type = Type.GetType(Reflection.TypeMap[table.Properties[filteredName]])!;
                // If Types are not the same repeat the function again.
                if (type == typeof(Int32))
                {
                    try
                    {
                        int x = Convert.ToInt32(value);
                        valueType = typeof(Int32);
                    }
                    catch
                    {
                        Console.WriteLine("Enter a valid Type for the property");
                        continue;
                    }

                }
                else if (type == typeof(System.String))
                {
                    try
                    {
                        int x = Convert.ToInt32(value);
                        Console.WriteLine("Enter a valid value for the property");
                        continue;
                    }
                    catch
                    {

                    }
                }
                if (valueType != type)
                {

                    Console.WriteLine("Enter a valid Type for the property");
                    continue;
                }
                //
                //
                // Create new List of Objects to save json data back to objects.
                var objects = new List<Object>();

                // Loop through List of dictionaries and convert it to List of ExpandoObject
                foreach (var dict in list)
                {
                    var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                    foreach (var kvp in dict)
                    {
                        expando.Add(kvp.Key, kvp.Value);
                    }
                    objects.Add(expando);
                }
                // Filter List of ExpandoObject (You have to cast the objects to IDictionary<string,object>)
                // because it implements it and to be able to filter and convert it to String
                var filteredList = objects.Where(x => ((IDictionary<string, object>)x)[filteredName].ToString() == value.ToString()).ToList();
                Console.WriteLine();
                Console.WriteLine("Results: ");
                // Loop through filtered list and print the objects
                foreach (var obj in filteredList)
                {
                    foreach (var kvp in (IDictionary<string, object>)obj)
                    {
                        Console.WriteLine($"{kvp.Key} : {kvp.Value}");
                    }
                    Console.WriteLine();
                }
                flag = true;
            }
        }
        private static void UpdateTable(List<Table> tables, string path)
        {
            Table table = GetTable(tables);
            string json = File.ReadAllText(path + $@"\{table.TableName}.txt");
            var list = new List<Dictionary<string, object>>();
            if (!string.IsNullOrEmpty(json))
            {
                list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            }
            // If deserialization fails, ensure the list is not null
            if (list == null)
            {
                list = new List<Dictionary<string, object>>();
            }
            // convert Json format to List of Dictionary<string, object>
            list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            var objects = new List<Object>();

            // Loop through List of dictionaries and convert it to List of ExpandoObject
            foreach (var dict in list)
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                foreach (var kvp in dict)
                {
                    expando.Add(kvp.Key, kvp.Value);
                }
                objects.Add(expando);
            }
            foreach(var obj in objects)
            {
                foreach (var kvp in (IDictionary<string, object>)obj)
                {
                    Console.WriteLine($"{kvp.Key} : {kvp.Value}");
                }
                Console.WriteLine();
            }
        }
        private static void DeleteFromTable()
        {
            Console.WriteLine("Not yet implemented");
        }
        private static void ViewLog()
        {
            Console.WriteLine("Not yet implemented");
        }

        private static void PrintTables(string path)
        {
            // Get names of all the table inside DB folder
            var files = Directory.GetFiles(path + @"\").ToList();
            foreach (var file in files)
            {
                var f = Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"- {f}");
            }
        }
    }
}
