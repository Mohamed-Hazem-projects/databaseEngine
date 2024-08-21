namespace Koala
{
    class Program
    {
        static void Main(string[] args)
        {
            // get the location of the database
            string DBPath = InitiateDB.GetLocation();
            // open/create the database folder
            DBPath = InitiateDB.OpenDB(DBPath);
            //create tables and save table/properties/Types in a List
            //ed3y wnta da5el
            List<Table> tables = CreateStructure.CreateTables(DBPath);
            //select insert update delete
            Operations.StartOperations(tables,DBPath);
        }
    }

}