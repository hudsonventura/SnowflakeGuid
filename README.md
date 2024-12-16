# SnowflakeGuid

This lib is a fork from `SnowflakeIDGenerator`.  
Credits: [https://www.nuget.org/packages/SnowflakeIDGenerator](https://www.nuget.org/packages/SnowflakeIDGenerator)

## About  
This is a package to generate a System.Guid, but not completely random, and there is some information exactly in SnowflakeID, like timestamp, machineID and a string, but it looks like a uuid (see usage examples below).   
You can convert SnowflakeID (as number) to SnowflakeGuid, Guid to SnowflakeGuid, or the other way around.  




# Usage

### Usage - Install and configure

Install
``` bash
dotnet add package SnowflakeGuid
```

Configure machine ID. If ignored, `0` will be considered.  
You must configure the machine ID on starting app. You cannot change it after generate the first SnowfalkeGuid.
``` C#
SnowflakeGuid.SetMachineID(1);
```



### Usage - Generate
```C#
  //Create a SnowflakeGuid with timestamp, datetime, machineID, sequence and Guid properties, like a comon SnowflakeID
  SnowflakeGuid snow = SnowflakeGuid.Create();
  Guid guid = snow.Guid;

  //Generate a System.Guid from Microsoft, but after you can convert back to SnowflakeGuid
  Guid guid = SnowflakeGuid.NewGuid();


  //Generate a System.Guid compatible json or EFCore
  using System.ComponentModel.DataAnnotations;
  
  [Key]
  public Guid guid { get; private set; } = SnowflakeGuid.NewGuid();

  //Optionally you can use SnowflakeGuid as system Guid, using this.
  //It will use the string uuid do save to a database and json, for example.
  //NOT RECOMENDED. It not works with EFCore
  using System.Text.Json.Serialization;

  [JsonConverter(typeof(SnowflakeGuidJsonConverter))]
  public SnowflakeGuid snow { get; private set; } = SnowflakeGuid.Create();
```

### Usage - Parsing SnowflakeID from long, ulong, Guid and string
```C#
/*Parse from a System.Guid*/
Guid vaguidguidlue = Guid.Parse("64f3414f-3d00-1000-0000-a1d12c9e2ef9");
Snowflake snow = SnowflakeGuid.Parse(guid);

/*Pase from a string (valid guid)*/
Snowflake snow = SnowflakeGuid.Parse("64f3414f-3d00-1000-0000-a1d12c9e2ef9");

/*Parse from long or ulong*/
Snowflake snow = SnowflakeGuid.Parse(1864424336924868608);

/*Parse from string (long or ulong valid)*/
Snowflake snow = SnowflakeGuid.ParseFromString("1864424336924868608");
```





### Usage - Properties of SnowflakeID
```C#
Snowflake snow = new Snowflake(guid);

Console.WriteLine(snow.MachineId);      // Till 1024 configurable, see session `Install and configure`
Console.WriteLine(snow.Sequence);       // Till 4096

Console.WriteLine(snow.DateTimeUTC);    // 13/9/2022 22:27:47 in +0UTC
Console.WriteLine(snow.TimestampUTC);   // 1663108067853  in +0UTC

Console.WriteLine(snow.DateTime);    // 13/9/2022 22:27:47 in your local timezone
Console.WriteLine(snow.Timestamp);   // 1663108067853 in your local timezone
```


## Reason

SnowflakeID is a 64-bit number value. In dotnet this value can be represented by a ulong. However, when converting a ulong to a string, dotnet ends up rounding it and it tends to be filled with 3 zeros at the end. So you need to work with the conversion to string even if the value is a ulong.  

ulong: `1864424336924868608`  
 

We have a new `Guidv7` on `net9` but there is not a way to get the properties about the creation reason. We can't get the timestamp or machine ID for example. By the way with a original SnowflakeID and this SnowflakeGuid you can set and get back these information.

Therefore, I created this lib to convert the ulong value to a uuid (Guid in dotnet).  
You can either convert a uuid to snowflake and obtain its properties, or generate a snowflake and convert it to uuid.  

Guid appearance: `8b7bbd43-d429-49c6-b64c-11586f994e75` 


I think it's more beautiful to see `http://yourdomain.com/user/8b7bbd43-d429-49c6-b64c-11586f994e75`  
than `http://yourdomain.com/user/1864424336924868608`.