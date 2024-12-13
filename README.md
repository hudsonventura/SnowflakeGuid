# Snowflake Guid Generator

This lib is a fork from `SnowflakeIDGenerator`. [More info](https://www.nuget.org/packages/SnowflakeIDGenerator)

## Reason

SnowflakeID is a 64-bit number value. In dotnet this value can be represented by a ulong. However, when converting a ulong to a string, dotnet ends up rounding it and it tends to be filled with 3 zeros at the end. So you need to work with the conversion to string even if the value is a ulong.  

ulong: `1864424336924868608`  
uuid: `8b7bbd43-d429-49c6-b64c-11586f994e75`

Therefore, I created this lib to convert the ulong value to a uuid (Guid in dotnet).  
You can either convert a uuid to snowflake and obtain its properties, or generate a snowflake and convert it to uuid.  


I think it's more beautiful to see `http://yourdomain.com/user/8b7bbd43-d429-49c6-b64c-11586f994e75` than `http://yourdomain.com/user/1864424336924868608`.



# Usage

### Usage - Generate
```C#

  Guid guid = SnowflakeGuidGenerator.GetGuid();

  //OR
  Snowflake snow = SnowflakeGuidGenerator.GetSnowflake();
  Guid guid = snow.Guid;

  //OR
  Guid guid = SnowflakeGuidGenerator.GetSnowflake().Guid;

  //OR ... you can put it on you object class
  public Guid id { get; private set; } = SnowflakeGuidGenerator.GetSnowflake().Guid;
```

### Usage - Get Guid by SnowflakeID (Parse from ulong)
```C#
ulong value = 1864424336924868608;
Guid guid = new Snowflake(value);
```

### Usage - Get Guid by SnowflakeID (Parse from string (ulong valid))
```C#
string value = "01864424336924868608";
Guid guid = new Snowflake(value);
```

### Usage -  Get SnowflakeID by Guid (Parse from a common Guid)
```C#
Guid value = Guid.Parse("8b7bbd43-d429-49c6-b64c-11586f994e75");
Snowflake snow = new Snowflake(value);
```





### Usage - Properties of SnowflakeID
```C#
Snowflake snow = new Snowflake(guid);
Console.WriteLine(snow.UtcDateTime);  // 13/9/2022 22:27:47
Console.WriteLine(snow.Timestamp);    // 1663108067853
Console.WriteLine(snow.MachineId);    // 701
Console.WriteLine(snow.Sequence);     // 3911
```


