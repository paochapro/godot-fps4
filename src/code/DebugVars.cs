using System.Collections.Generic;

static class DebugVars
{
    public static IReadOnlyDictionary<string, object> Vars => vars;

    public static void Set(string name, object value)
    {
        if(!vars.ContainsKey(name))
            vars.Add(name, value);
        else
            vars[name] = value;
    }

    static Dictionary<string, object> vars;

    static DebugVars() => vars = new();
}