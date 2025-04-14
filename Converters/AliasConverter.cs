using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using EnumHelper.Parsers;

namespace EnumHelper.Converters
{
    public class AliasConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public AliasConverter() { }

        public override TEnum ReadJson(JsonReader reader, Type objectType, TEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var stringValue = (string)reader.Value;

                // Use EnumHelper to try to match the string to an enum value
                var enumValue = AliasParser.ParseEnumByAliasOrName<TEnum>(stringValue);

                if (enumValue.HasValue)
                    return enumValue.Value;
            }

            Array options = objectType.IsGenericType ? Enum.GetValues(objectType.GenericTypeArguments[0]) : Enum.GetValues(objectType);
            throw new JsonSerializationException($"Accepted values are: {string.Join(", ", options.Cast<TEnum>())}");
        }

        public override void WriteJson(JsonWriter writer, TEnum value, JsonSerializer serializer)
        {
            var field = typeof(TEnum).GetField(value.ToString());
            var enumMemberAttr = field?.GetCustomAttribute<EnumMemberAttribute>();

            if (enumMemberAttr != null)
                writer.WriteValue(enumMemberAttr.Value);
            else
                writer.WriteValue(value.ToString());
        }
    }
}
