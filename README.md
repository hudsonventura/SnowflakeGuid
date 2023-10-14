# Snowflake Id Generator

Generate unique identifiers based on Twitter's [Snowflake ID](https://en.wikipedia.org/wiki/Snowflake_ID).
Parse a Snowflake to get information about it's creation.


| ![SnowflakeId components](https://raw.githubusercontent.com/fenase/SnowflakeIDGenerator/master/ReadmeImages/SnowflakeId-Wikipedia.png) |
|:--| 
| *Image Source & credit available in [wikimedia](https://commons.wikimedia.org/wiki/File:Snowflake-identifier.png)* <br>"instance" in this image replaces machineId in the library / package |


[![Nuget](https://img.shields.io/nuget/v/SnowflakeIDGenerator)](https://www.nuget.org/packages/SnowflakeIDGenerator)
[![Build status](https://dev.azure.com/fenase/SnowflakeIDGenerator/_apis/build/status/SnowflakeIDGenerator-CI-1.0.0)](https://dev.azure.com/fenase/SnowflakeIDGenerator/_build/latest?definitionId=7)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=alert_status)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=ncloc)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=coverage)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)


- [Usage](#usage)
  - [Generate](#generate)
    - [Using the `SnowflakeIDGenerator` class](#using-the-snowflakeidgenerator-class)
    - [Using the `SnowflakeIDGenerator` class as static](#using-the-snowflakeidgenerator-class-as-static)
    - [Using a non-standard date as epoch](#using-a-non-standard-date-as-epoch)
  - [Parsing an Id](#parsing-an-id)
  - [Change Epoch on generated codes](#change-epoch-on-generated-codes)


# Usage

## Generate

There are 2 ways of using the generator:
* Using the `SnowflakeIDGenerator` class as a static class.
Useful when generating a single code to avoid dealing with constructors and the scope of the generator object.
* Instantiating the `SnowflakeIDGenerator` class. 
Recommended if you plan to generate more than a few codes at the same time.

### Using the `SnowflakeIDGenerator` class

1. Instantiate class `SnowflakeIDGenerator`
```c#
SnowflakeIDGenerator generator = new SnowflakeIDGenerator(machineId);
```
where `machineId` is the number / identifier of the system currently trying to get an id

Starting on version 1.1.2023 you can instruct the generator to use a custom date as epoch 
from which the timestamps are derived for the current date.
```c#
SnowflakeIDGenerator generator = new SnowflakeIDGenerator(machineId, CustomEpoch);
```

2. Using the generator object, there are 3 ways of obtaining the code:
   1. Call `generator.GetSnowflake()` to get a `Snowflake` object
   2. Call `generator.GetCode()` to get an Id in number (ulong) format
   3. Call `generator.GetCodeString()` to get an Id in string format

### Using the `SnowflakeIDGenerator` class as static

If you only need to get a single Id, it's easier to just use the generator class as static.

The method names are the same as when using the generator, except they need the `machineId` as parameter:
   
1. Call `SnowflakeIDGenerator.GetSnowflake(machineId)` to get a `Snowflake` object
2. Call `SnowflakeIDGenerator.GetCode(machineId)` to get an Id in number (ulong) format
3. Call `SnowflakeIDGenerator.GetCodeString(machineId)` to get an Id in string format

### Using a non-standard date as epoch

*version 1.1.2023 and up*

The first component of the codes is the amount of milliseconds elapsed since a set point in time, called epoch.
By default, the generator uses the unix epoch (jan-1-1970 12:00:00am) as starting point to count.

Adding a `DateTime` object as an extra parameter when using the generator allows to change the zero value to be used to count milliseconds to.

```c#
DateTime customEpoch = new DateTime(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, DateTimeKind.Utc);

// Creating the generator class
SnowflakeIDGenerator generator = new SnowflakeIDGenerator(machineId, customEpoch);

// This works when using as static too!
SnowflakeIDGenerator.GetSnowflake(machineId, customEpoch)
```

## Parsing an Id

Parse a Snowflake either from a string or a number (ulong) in order to get information regarding the generation,
such as the time or the machine that generated the code.

If a custom epoch was used when generating, that DateTime must be passed as second parameter when parsing in order to get the right generation date.

```c#
string s = "06975580616378931208";
ulong n = 6975580821430984519ul;
Snowflake fromString = Snowflake.Parse(s);
var utcDateTimeFromString = fromString.UtcDateTime; // 13/9/2022 22:26:58
var timestampFromString = fromString.Timestamp;     // 1663108018965
var machineIdFromString = fromString.MachineId;     // 477
var sequenceFromString = fromString.Sequence;       // 2056

Snowflake fromNumber = Snowflake.Parse(n);
var utcDateTimeFromNumber = fromNumber.UtcDateTime; // 13/9/2022 22:27:47
var timestampFromNumber = fromNumber.Timestamp;     // 1663108067853
var machineIdFromNumber = fromNumber.MachineId;     // 701
var sequenceFromNumber = fromNumber.Sequence;       // 3911
```

Additionally, starting on version 1.2.2023 you can cast a string or a number (ulong) directly into a Snowflake
without using the `Parse()` method (only when using the default epoch).

```c#
string s = "06975580616378931208";
ulong n = 6975580821430984519ul;
Snowflake fromString = (Snowflake)s;
var utcDateTimeFromString = fromString.UtcDateTime; // 13/9/2022 22:26:58
var timestampFromString = fromString.Timestamp;     // 1663108018965
var machineIdFromString = fromString.MachineId;     // 477
var sequenceFromString = fromString.Sequence;       // 2056

Snowflake fromNumber = (Snowflake)n;
var utcDateTimeFromNumber = fromNumber.UtcDateTime; // 13/9/2022 22:27:47
var timestampFromNumber = fromNumber.Timestamp;     // 1663108067853
var machineIdFromNumber = fromNumber.MachineId;     // 701
var sequenceFromNumber = fromNumber.Sequence;       // 3911
```

## Change Epoch on generated codes

If you need to change the epoch on an already generated code,
use `ChangeEpoch()` to change it keeping the same code but changing the represented date,
or use `RebaseEpoch()` to keep the date but changing the final code.
