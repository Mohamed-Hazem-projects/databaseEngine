using System.Reflection;
using System.Reflection.Emit;

namespace Koala
{
    internal class Reflection
    {
        internal static object CreateDynamicClass(Table table)
        {
            //define a new assembly(IL code)
            //this will contain the dynamically created classes

            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            //creates a public class with the name of the table dynamically
            var typeBuilder = moduleBuilder.DefineType(table.TableName, TypeAttributes.Public);

            foreach (var property in table.Properties)
            {
                //getting the property type
                TypeMap.TryGetValue(property.Value, out string typeName);
                var propertyType = Type.GetType(typeName);
                //creating a field for each property
                var fieldBuilder = typeBuilder.DefineField("_" + property.Key, propertyType, FieldAttributes.Private);
                //creating the property
                var propBuilder = typeBuilder.DefineProperty(property.Key, PropertyAttributes.HasDefault, propertyType, null);

                //creating a getter ex: (get_Dept_ID)
                var getPropMthdBldr = typeBuilder.DefineMethod("get_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, Type.GetType("System." + property.Value), Type.EmptyTypes);
                var getIL = getPropMthdBldr.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getIL.Emit(OpCodes.Ret);

                //creating a setter ex: (set_Dept_ID)
                var setPropMthdBldr = typeBuilder.DefineMethod("set_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
                var setIL = setPropMthdBldr.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, fieldBuilder);
                setIL.Emit(OpCodes.Ret);

                //attaching the getter and setter to the property
                propBuilder.SetGetMethod(getPropMthdBldr);
                propBuilder.SetSetMethod(setPropMthdBldr);
            }

            //finally creating the class then returning an instance of that class
            var dynamicClass = typeBuilder.CreateType();
            return Activator.CreateInstance(dynamicClass);
        }
        internal static readonly Dictionary<string, string> TypeMap = new Dictionary<string, string>
                {
                    { "bool", "System.Boolean" },
                    { "byte", "System.Byte" },
                    { "char", "System.Char" },
                    { "double", "System.Double" },
                    { "int", "System.Int32" },
                    { "long", "System.Int64" },
                    { "short", "System.Int16" },
                    { "string", "System.String" }
                };
    }
}
