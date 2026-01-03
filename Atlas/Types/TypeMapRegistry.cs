
using System.Reflection;

namespace Atlas.Types;

public static class TypeMapRegistry
{
    /// <summary> All active/registered <see cref="TypeMap"/>s.
    /// <para>Key: The C++ name of the type.</para>
    /// <para>Value: The <see cref="TypeMap"/> instance.</para>
    /// </summary>
    public static Dictionary<string, TypeMap> Maps = new Dictionary<string, TypeMap>();

    /// <summary> Registers all <see cref="TypeMap"/>s marked with <see cref="TypeMapAttribute"/>. </summary>
    public static void RegisterTypeMaps()
    {
        var typeMaps = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsSubclassOf(typeof(TypeMap)) && x.IsDefined(typeof(TypeMapAttribute), false));

        foreach (var typeMap in typeMaps)
        {
            var attribute = typeMap.GetCustomAttribute<TypeMapAttribute>(false)!;
            Maps.Add(attribute.NativeType, Activator.CreateInstance(typeMap) as TypeMap ?? throw new Exception());
        }
    }
}
