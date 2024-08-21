using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogicClasses
{
    public static class UserPrompt
    {
        public static List<string> columnDataTypeInSqlForm { get; private set; } = new List<string>();
        public static string TableName()
        {
            string? input;

            do
            {
                Console.WriteLine("Enter Table Name:");
                input = Console.ReadLine();
            }
            while (string.IsNullOrEmpty(input));

            // Check if the table already exists
            if (CheckIfFileExist(input))
            {
                Console.WriteLine($"The {input} table already exists.");
            }
            else
            {
                return input;
            }
            return null;
        }

        public static string TableNameExist()
        {
            string? input;

            do
            {
                Console.WriteLine("Enter Table Name:");
                input = Console.ReadLine();
            }
            while (string.IsNullOrEmpty(input));

            // Check if the table already exists
            if (CheckIfFileExist(input))
            {
                return input;
            }
            else
            {
                Console.WriteLine($"The {input} table doesn't exist.");
            }
            return null;
        }

        public static bool CheckIfFileExist(string fileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader($@".\Created Tables\{fileName.ToLower()}.txt"))
                {
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public static List<string> Columns(out List<Type> colsDataType)
        {
            List<string> cols = new List<string>();
            colsDataType = new List<Type>();

            string? input;
            uint colsNumber;

            do
            {
                Console.WriteLine("Enter How Many Columns: ");
                input = Console.ReadLine();
            }
            while (string.IsNullOrEmpty(input) || !uint.TryParse(input, out colsNumber));

            for (uint i = 0; i < colsNumber; i++)
            {
                do
                {
                    Console.WriteLine($"Enter The {i + 1} Column Name: ");
                    input = Console.ReadLine();
                }
                while (string.IsNullOrEmpty(input));
                cols.Add(input);

                do
                {
                    Console.WriteLine($"Enter The {i + 1} Column Data Type: ");
                    Console.WriteLine("Note: Write (char, varchar), no range specification allowed");
                    input = Console.ReadLine();
                }
                while (string.IsNullOrEmpty(input) || CheckColumnDataType(input) == null);
                columnDataTypeInSqlForm.Add(input);
                colsDataType.Add(CheckColumnDataType(input));
            }

            Console.WriteLine("\n-----------------------------------");
            return cols;
        }

        public static Type? CheckColumnDataType(string typeString)
        {
            switch (typeString.ToLower())
            {
                // Exact numerics
                case "bit":
                    return typeof(bool);
                case "tinyint":
                    return typeof(byte);
                case "smallint":
                    return typeof(short);
                case "int":
                    return typeof(int);
                case "bigint":
                    return typeof(long);
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    return typeof(decimal); // Placeholder for numeric types

                // Approximate numerics
                case "float":
                    return typeof(float);
                case "real":
                    return typeof(double); // Placeholder for approximate numeric types

                // Date and time
                case "date":
                case "datetime":
                case "datetime2":
                case "datetimeoffset":
                case "smalldatetime":
                case "time":
                    return typeof(DateTime);

                // Character strings
                case "char":
                case "varchar":
                case "text":
                    return typeof(string);

                // Unicode character strings
                case "nchar":
                case "nvarchar":
                    return typeof(string);

                // Binary strings
                case "binary":
                case "varbinary":
                    return typeof(byte[]);

                // Other data types (add more as needed)
                case "xml":
                    return typeof(string); // Placeholder for XML type

                default:
                    return null;
                    /*throw new ArgumentException("Invalid SQL Server data type. Please enter a valid type.")*/
                    ;
            }
        }

        public static void CreateFileForTable(string tableName, List<string> columns)
        {
            using (StreamWriter sw = new StreamWriter(@$".\Created Tables\{tableName.ToLower()}.txt", false))
            {
                sw.WriteLine($"{tableName}");
                for (int i = 0; i < columns.Count; i++)
                {
                    if (i == columns.Count - 1)
                    {
                        sw.WriteLine($"{columnDataTypeInSqlForm[i]}:{columns[i]}");
                    }
                    else
                    {
                        sw.Write($"{columnDataTypeInSqlForm[i]}:{columns[i]},");
                    }
                }
                sw.WriteLine("RECORDS_START");
            }
        }

        public static void Start()
        {
            string tableName = TableNameExist();

            AskToDisplayRecords(tableName);

            //Console.WriteLine("Proceed to insert data into the table...");

            //AskToDisplayRecords(tableName);
        }

        public static void AskToDisplayRecords(string tableName)
        {
            Console.WriteLine("Do you want to display records? (all/specific/none)");
            string displayChoice = Console.ReadLine()?.ToLower();

            switch (displayChoice)
            {
                case "all":
                    DisplayAllRecords(tableName);
                    break;
                case "specific":
                    DisplaySpecificRecord(tableName);
                    break;
                case "none":
                    Console.WriteLine("No records will be displayed.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. No records will be displayed.");
                    break;
            }

        }

        public static void DisplayAllRecords(string tableName)
        {
            string filePath = @$".\Created Tables\{tableName.ToLower()}.txt";
            bool recordsStart = false;

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string? line;
                    Console.WriteLine($"Records from table: {tableName}");
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "RECORDS_START")
                        {
                            recordsStart = true;
                            continue;
                        }

                        if (recordsStart && !string.IsNullOrWhiteSpace(line))
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Can't find the file for the {tableName} table.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void DisplaySpecificRecord(string tableName)
        {
            string filePath = @$".\Created Tables\{tableName.ToLower()}.txt";
            bool recordsStart = false;
            Dictionary<string, string> records = new Dictionary<string, string>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string? line;
                    Console.WriteLine($"Records from table: {tableName}");
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "RECORDS_START")
                        {
                            recordsStart = true;
                            continue;
                        }

                        if (recordsStart && !string.IsNullOrWhiteSpace(line))
                        {
                            var recordParts = line.Split(',');
                            if (recordParts.Length > 0)
                            {
                                records[recordParts[0]] = line;
                            }
                        }
                    }
                }

                Console.WriteLine("Enter the ID of the record you want to display:");
                string id = Console.ReadLine();
                if (records.TryGetValue(id, out string record))
                {
                    Console.WriteLine($"Record with ID {id}: {record}");
                }
                else
                {
                    Console.WriteLine($"No record found with ID {id}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Can't find the file for the {tableName} table.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void DeleteRecord(string tableName)
        {
            Console.WriteLine("Do you want to delete by ID or by Name? (id/name)");
            string deleteChoice = Console.ReadLine()?.ToLower();
            string filePath = @$".\Created Tables\{tableName.ToLower()}.txt";
            string tempFilePath = @$".\Created Tables\{tableName.ToLower()}_temp.txt";

            switch (deleteChoice)
            {
                case "id":
                    Console.WriteLine("Enter the ID of the record to delete:");
                    string idToDelete = Console.ReadLine();
                    DeleteRecordById(filePath, tempFilePath, idToDelete);
                    Console.WriteLine("The record is deleted successfully");
                    break;
                case "name":
                    Console.WriteLine("Enter the Name of the record to delete:");
                    string nameToDelete = Console.ReadLine();
                    DeleteRecordByName(filePath, tempFilePath, nameToDelete);
                    Console.WriteLine("The record is deleted successfully");
                    break;
                default:
                    Console.WriteLine("Invalid choice. No record will be deleted.");
                    return;
            }
        }

        private static void DeleteRecordById(string filePath, string tempFilePath, string idToDelete)
        {
            using (StreamReader sr = new StreamReader(filePath))
            using (StreamWriter sw = new StreamWriter(tempFilePath))
            {
                string? line;
                bool recordsStart = false;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "RECORDS_START")
                    {
                        recordsStart = true;
                        sw.WriteLine(line);
                        continue;
                    }

                    if (!recordsStart)
                    {
                        sw.WriteLine(line);  // Preserve the headers
                    }
                    else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(idToDelete))
                    {
                        sw.WriteLine(line);  // Write the record if it doesn't match the ID
                    }
                }
            }

            // Replace original file with the temp file
            File.Delete(filePath);
            File.Move(tempFilePath, filePath);
        }

        private static void DeleteRecordByName(string filePath, string tempFilePath, string nameToDelete)
        {
            using (StreamReader sr = new StreamReader(filePath))
            using (StreamWriter sw = new StreamWriter(tempFilePath))
            {
                string? line;
                bool recordsStart = false;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "RECORDS_START")
                    {
                        recordsStart = true;
                        sw.WriteLine(line);
                        continue;
                    }

                    if (!recordsStart)
                    {
                        sw.WriteLine(line);  // Preserve the headers
                    }
                    else if (!string.IsNullOrWhiteSpace(line) && !line.Contains(nameToDelete))
                    {
                        sw.WriteLine(line);  // Write the record if it doesn't match the Name
                    }
                }
            }

            // Replace original file with the temp file
            File.Delete(filePath);
            File.Move(tempFilePath, filePath);
        }

        public static void UpdateRecord(string tableName)
        {
            string filePath = @$".\Created Tables\{tableName.ToLower()}.txt";
            List<string> records = new List<string>();
            //save each line in the table to a list
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string? line;
                    bool recordsEnd = false;
                    string record;
                    while (!recordsEnd)
                    {
                        record = sr.ReadLine();
                        if (record != null)
                        {
                            records.Add(record);
                        }
                        else
                        {
                            recordsEnd = true;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Can't find the file for the {tableName} table.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            //make an array of IDs
            string[] idArray = new string[records.Count - 3];
            for (int i = 3; i < records.Count; i++)
            {
                idArray[i - 3] = records[i].Split(",")[0];
            }

            //extract each column name and it's type for validation
            IDictionary<string, Type> columnsWithDataType = new Dictionary<string, Type>();
            string[] column;
            foreach (string col in records[1].Split(','))
            {
                column = col.Split(":");
                columnsWithDataType.Add(column[1], CheckColumnDataType(column[0]) ?? typeof(string));
            }

            ////next two loops for testing
            //foreach (string record in records)
            //    {
            //        Console.WriteLine(record);
            //    }
            //Console.WriteLine();
            //foreach (KeyValuePair<string, Type> kvp in columnsWithDataType)
            //{
            //    Console.WriteLine($"Column Name : {kvp.Key} Column Type : {kvp.Value}");
            //}

            //taking ID from the user
            Console.WriteLine("Enter the ID of the record you want to update:");
            bool recordFlag = false;
            int IDindex =0;
            while (!recordFlag)
            {
                string id = Console.ReadLine();
                //checking if id exists and take its index
                for (int i = 0; i < idArray.Length; i++)
                {
                    if (id == idArray[i])
                    {
                        IDindex = i + 3;
                        recordFlag = true;
                    }
                }
                if (!recordFlag)
                {
                    Console.WriteLine("ID doesn't exist.");
                }
            }

            //getting the new record from the user and validating it
            string newRecord = "";
            foreach (KeyValuePair<string, Type> kvp in columnsWithDataType)
            {
                bool colFlag = false;
                while (!colFlag)
                {
                    Console.WriteLine($"Please enter the new value for {kvp.Key} of type {kvp.Value}");
                    string newValue = Console.ReadLine();
                    if (Convert.ChangeType(newValue,kvp.Value) != null)
                    {
                        newRecord += $"{newValue},";
                        colFlag = true;
                    }
                    else
                    {
                        Console.WriteLine($"Please enter a valid value of type {kvp.Value}");
                    }
                }
            }
            //removing the last , from the new record
            newRecord=newRecord.Remove(newRecord.Length-1);
            //updating the list with the new record
            records[IDindex] = newRecord;
            //clearing the file
            File.WriteAllText(filePath, String.Empty);
            //updating the file
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath,true))
                {
                    foreach(string record in records)
                    {
                        sw.WriteLine(record);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Can't find the file for the {tableName} table.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
