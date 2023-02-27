using Microsoft.EntityFrameworkCore;
using WebApp.Context;

namespace WebApp.Tools
{
    public class SettingHelper
    {
        public static bool TryGetValue<T>(WebAppContext context, string settingName, out T? value)
        {
            var setting = context.Settings
                .Include(x => x.SettingValue)
                .FirstOrDefault(x => x.Code == settingName);

            if (setting == null)
            {
                value = default;
                return false;
            }

            value = setting.Type switch
            {
                "string" => (T)(object)setting.SettingValue.TextValue,
                "bool" => (T)(object)setting.SettingValue.BooleanValue,
                "guid" => (T)(object)setting.SettingValue.GuidValue,
                "datetime" => (T)(object)setting.SettingValue.DateTimeValue,
                "int" => (T)(object)setting.SettingValue.IntegerValue,
                "float" => (T)(object)setting.SettingValue.FloatValue,
                _ => default
            };

            if (value is null)
            {
                return false;
            }

            return true;
        }
    }
}
