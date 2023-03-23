using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

static class WeaponBuilder
{   
    const string weaponsJsonFile = "content/weapon_test.json";
    static Dictionary<string, WeaponData> weapons;

    static WeaponBuilder()
    {
        Dictionary<string, WeaponData>? result = null;        

        using(StreamReader reader = new(weaponsJsonFile))
        {
            string json = reader.ReadToEnd();
            result = JsonConvert.DeserializeObject<Dictionary<string, WeaponData>>(json);
        }

        if(result == null)
            GD.PrintErr($"Unable to convert {weaponsJsonFile} to weaponData dictionary! Result is null");

        weapons = result ?? new();
    }

    public static Weapon Create(string name, int startMagazine)
    {
        WeaponData data = weapons[name];
        Weapon weapon = new(data, startMagazine);
        return weapon;
    }
}