# Snowflake Id Generator
## Dependency injection

This package extends package [SnowflakeIDGenerator](https://www.nuget.org/packages/SnowflakeIDGenerator) 
to help in configuring the snowflakeId generator using dependency injection


[![Nuget](https://img.shields.io/nuget/v/SnowflakeIDGenerator.DependencyInjection?logo=nuget)](https://www.nuget.org/packages/SnowflakeIDGenerator.DependencyInjection)
[![Build Status](https://dev.azure.com/fenase/SnowflakeIDGenerator/_apis/build/status%2FSnowflakeIDGenerator-CI?branchName=master)](https://dev.azure.com/fenase/SnowflakeIDGenerator/_build/latest?definitionId=21)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=alert_status)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=ncloc)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=coverage)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)



# Usage

For example, if using ASP.net minimal api, use the following to register the generator using default values:

```C#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSnowflakeIdGeneratorService();
```

Alternatively, the following will use custom values contained in appsettings.json

```C#
builder.Services.AddSnowflakeIdGeneratorService(builder.Configuration.GetSection("SnowflakeIdGeneratorOptions").Get<SnowflakeIdGeneratorOptions>());
```

```JSON
{
  . . .
  "SnowflakeIdGeneratorOptions": {
    //Both parameters are optional
    "MachineId": 5,
    "Epoch": "2023-12-25"
  }
}
```

Either way will register the interface `ISnowflakeIDGenerator` to be use in your project.



See an example in the [Example directory](https://github.com/fenase/SnowflakeIDGenerator/tree/master/Examples/SnowflakeIDGenerator.Example.Web).