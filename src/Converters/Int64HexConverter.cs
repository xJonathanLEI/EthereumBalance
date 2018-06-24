using System;
using System.Globalization;
using Newtonsoft.Json;

namespace EthereumBalance.Converters
{
    public class Int64HexConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue("0x" + ((long)value).ToString("x"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (((string)reader.Value == "0x")) return (long)0;
            if (((string)reader.Value).StartsWith("0x"))
                return long.Parse("0" + ((string)reader.Value).Remove(0, 2), NumberStyles.HexNumber);
            else
                return long.Parse((string)reader.Value, NumberStyles.None);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(long).IsAssignableFrom(objectType);
        }
    }
}