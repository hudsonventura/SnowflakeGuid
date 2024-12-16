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

### Usage - Install and configure

Install
``` bash
dotnet add package SnowflakeGuid
```

Configure machine ID. If ignored, `0` will be considered.  
You must configure the machine ID on starting app. You cannot change it after.
``` C#
SnowflakeGuid.SetMachineID(1);
```



### Usage - Generate
```C#
  //Generate a System.Guid from Microsoft, but after you can convert Guid to SnowflakeGuid
  Guid guid = SnowflakeGuid.NewGuid();

  //Create a SnowflakeGuid with timestamp, datetime, machineID, sequence and Guid properties.
  SnowflakeGuid snow = SnowflakeGuid.Create();
  Guid guid = snow.Guid;


  //OR ... you can put it on you object class
  public Guid guid { get; private set; } = SnowflakeGuid.NewGuid();

  //Optionally you can use SnowflakeGuid as system Guid, using this.
  //It will use the string uuid do save to a database and json, for example.
  using System.Text.Json.Serialization;

  [JsonConverter(typeof(SnowflakeGuidJsonConverter))]
  public SnowflakeGuid snow { get; private set; } = SnowflakeGuid.Create();
```

### Usage - Parsing SnowflakeID
```C#
/*Parse from a System.Guid*/
Guid value = Guid.Parse("64f3414f-3d00-1000-0000-a1d12c9e2ef9");
Snowflake snow = SnowflakeGuid.Parse(guid);

/*Pase from a string (valid guid)*/
Snowflake snow = SnowflakeGuid.Parse("64f3414f-3d00-1000-0000-a1d12c9e2ef9");

/*Parse from ulong*/
Guid guid = SnowflakeGuid.Parse(1864424336924868608);

/*Parse from long*/
Guid guid = SnowflakeGuid.Parse(1864424336924868608);

/*Parse from string (long valid)*/
Guid guid = SnowflakeGuid.ParseFromString("1864424336924868608");
```





### Usage - Properties of SnowflakeID
```C#
Snowflake snow = new Snowflake(guid);

Console.WriteLine(snow.MachineId);      // Till 1024
Console.WriteLine(snow.Sequence);       // Till 4096

Console.WriteLine(snow.DateTimeUTC);    // 13/9/2022 22:27:47 in +0UTC
Console.WriteLine(snow.TimestampUTC);   // 1663108067853  in +0UTC

Console.WriteLine(snow.DateTime);    // 13/9/2022 22:27:47 in your local timezone
Console.WriteLine(snow.Timestamp);   // 1663108067853 in your local timezone
```


