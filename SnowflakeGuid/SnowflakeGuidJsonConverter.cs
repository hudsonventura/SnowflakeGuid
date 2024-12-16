using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SnowflakeGuidJsonConverter : JsonConverter<SnowflakeGuid>
{


    public override SnowflakeGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Guid id = Guid.Parse(reader.GetString());
        return SnowflakeGuid.Parse(id);
    }


    public override void Write(Utf8JsonWriter writer, SnowflakeGuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Guid.ToString());
    }
}
