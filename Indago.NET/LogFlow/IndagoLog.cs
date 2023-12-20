using System.Collections;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Indago.LogFlow;

public static class IndagoLog
{
    public static bool IndagoScriptingClientDebug => 
        bool.Parse(Environment.GetEnvironmentVariable("INDAGO_SCRIPTING_CLIENT_DEBUG") ?? "false");

    private static string DumpYaml(object data)
    {
        var yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithIndentedSequences()
            .Build();

        return yamlSerializer.Serialize(data);
    }
    
    public static void Log(object data, Action<object> logger, string methodName, string dataName)
    {
        string logData = DumpYaml(data);
        var prefix = $"{methodName} - {dataName}";
        logger($"{prefix}:\n{logData}");
    }
}