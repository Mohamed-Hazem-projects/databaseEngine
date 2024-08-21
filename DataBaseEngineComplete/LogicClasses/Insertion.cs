using System.Reflection;

namespace LogicClasses
{
    public class Insertion
    {
        public static void InsertDataIntoTable(Type type)
        {
            try
            {
                // Create an instance of the dynamic type
                var instance = Activator.CreateInstance(type);

                PropertyInfo[] props = type.GetProperties();
                using (StreamWriter sw = new StreamWriter($@".\Created Tables\{type.Name.ToLower()}.txt", true))
                {
                    for (int i = 0; i < props.Length; i++)
                    {
                        PropertyInfo prop = props[i];
                        object value = InsertUserPrompt(prop);
                        prop.SetValue(instance, value);

                        if (i == props.Length - 1)
                        {
                            sw.WriteLine($"{value}");
                        }
                        else
                        {
                            sw.Write($"{value},");
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Can't find this file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private static object InsertUserPrompt(PropertyInfo prop)
        {
            string? input;

            do
            {
                Console.Write($"{prop.Name} = ");
                input = Console.ReadLine();
            }
            while (string.IsNullOrEmpty(input) || ConvertToPropertyType(input, prop.PropertyType) == null);

            return ConvertToPropertyType(input, prop.PropertyType);
        }


        static object ConvertToPropertyType(string input, Type targetType)
        {
            try
            {
                if (targetType == typeof(int))
                {
                    return int.Parse(input);
                }
                else if (targetType == typeof(double))
                {
                    return double.Parse(input);
                }
                else if (targetType == typeof(DateTime))
                {
                    return DateTime.Parse(input);
                }
                else
                {
                    return Convert.ChangeType(input, targetType);
                }
            }
            catch
            {
                Console.WriteLine("Invalid Value");
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }

    }
}
