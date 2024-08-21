namespace Koala
{
    internal class Table
    {
        public string TableName { get; set; }
        //this is a list of dictionaries each one representing a property
        //the key of the dictionary is the property name
        //the value of the dictionary is the property type
        public IDictionary<string, string> Properties { get; set; }
                = new Dictionary<string, string>();
        public override string ToString()
        {
            Console.WriteLine();
            Console.WriteLine($"Table: {TableName}");
            Console.WriteLine("Properties:");
            int count = 1;
            foreach(KeyValuePair<string, string> kvp in Properties)
            {
                Console.WriteLine($"{count++}- Type: {kvp.Value}, Name: {kvp.Key}");
            }
            return "";
        }
    }
}
