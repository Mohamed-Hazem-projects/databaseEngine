namespace Koala
{
    internal static class InitiateDB
    {
        internal static string GetLocation()
        {
            string location = "";
            bool CreateDBFlag = false;
            while (!CreateDBFlag)
            {
                int locationChoice = 0;
                Console.WriteLine("Please choose the database location: ");
                Console.WriteLine("1 : Desktop\n2 : Enter Location Manually");
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    locationChoice = value;
                }
                switch (locationChoice)
                {
                    case 1:
                        location = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        CreateDBFlag = true;
                        break;
                    case 2:
                        Console.WriteLine();
                        Console.WriteLine("Please enter the path: ");
                        location = Console.ReadLine()!;
                        if (Directory.Exists(location))
                        {
                            location = Path.GetFullPath(location);
                            CreateDBFlag = true;
                        }
                        else
                        {
                            Console.WriteLine(location + " doesn't exist\nEnter a valid path");
                        }
                        break;
                    default:
                        Console.WriteLine("Please choose either 1 or 2.");
                        break;
                }
            }
            return location!;
        }
        internal static string OpenDB(string path)
        {
            bool DBNameFlag = false;
            string DBName;
            while (!DBNameFlag)
            {
                Console.WriteLine();
                Console.WriteLine("Please write the database name: ");
                Console.WriteLine("( A new database will be created if it doesn't exist. )");
                DBName = Console.ReadLine() ?? "";
                if (DBName != "")
                {
                    path = path + @"\" + DBName;
                    Directory.CreateDirectory(path);
                    DBNameFlag = true;
                }
                else
                {
                    Console.WriteLine("Please enter a valid database name: ");
                }
            }
            return path!;
        }
    }
}
