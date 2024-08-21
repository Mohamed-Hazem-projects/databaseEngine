namespace Koala
{
    internal class Utility
    {
        public static List<string> Types { get; set; } = ["bool","byte"
            ,"char","double","int","long","short","string"];

        internal static bool WithinTypes(string type)
        {
            var typeFlag = Types.Where(x => x == type).FirstOrDefault();
            return typeFlag == null ? false : true;
        }
        internal static void PrintTypes()
        {
            for (int i = 0; i < Types.Count; i++)
            {
                Console.WriteLine($"{i + 1}- {Types[i]}");
            }
        }
        internal static void CreateFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
        }
        internal enum Numbers
        {
            First = 1, Second, Third, Fourth, Fifth, Sixth, Seventh, Eighth, ninth, tenth, eleventh
        }

    }
}
