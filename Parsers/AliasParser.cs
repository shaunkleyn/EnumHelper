using EnumHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace EnumHelper.Parsers
{
    public static class AliasParser
    {
        public static TEnum? ParseEnumByAliasOrName<TEnum>(string text) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                // 1. Try match [EnumMember(Value = "")]
                var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
                if (enumMember != null && string.Equals(enumMember.Value, text, StringComparison.OrdinalIgnoreCase))
                    return (TEnum)field.GetValue(null);

                // 2. Try match [Lookup(...)]
                var lookup = field.GetCustomAttribute<AliasAttribute>();
                if (lookup?.Aliases.Any(alias => string.Equals(alias, text, StringComparison.OrdinalIgnoreCase)) == true)
                    return (TEnum)field.GetValue(null);

                // 3. Fallback to matching the enum name
                if (field.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
                    return (TEnum)field.GetValue(null);
            }

            return null;
        }
    }
}
