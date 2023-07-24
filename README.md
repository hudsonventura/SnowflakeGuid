# Snowflake Id Generator

Generate unique identifiers based on Twitter's [Snowflake ID](https://en.wikipedia.org/wiki/Snowflake_ID).

Parse a Snowflake to get information about it's creation.

[![Nuget](https://img.shields.io/nuget/v/SnowflakeIDGenerator)](https://www.nuget.org/packages/SnowflakeIDGenerator)
[![Build status](https://dev.azure.com/fenase/SnowflakeIDGenerator/_apis/build/status/SnowflakeIDGenerator-CI-1.0.0)](https://dev.azure.com/fenase/SnowflakeIDGenerator/_build/latest?definitionId=7)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=alert_status)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=ncloc)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=fenase_SnowflakeIDGenerator2&metric=coverage)](https://sonarcloud.io/summary/overall?id=fenase_SnowflakeIDGenerator2)


- [Usage](#usage)
  - [Generate](#generate)
  - [Parse](#parse)
  - [Change Epoch on generated codes](#change-epoch-on-generated-codes)


# Usage

## Generate

1. Instantiate class `SnowflakeIDGenerator`
```c#
SnowflakeIDGenerator gen = new SnowflakeIDGenerator(machineId);
```
where `machineId` is the number of the system currently trying to get an id

Starting on version 1.1.2023 you can instruct the generator to use a custom date as epoch 
from which the timestamps are derived for the current date.
```c#
SnowflakeIDGenerator gen = new SnowflakeIDGenerator(machineId, CustomEpoch);
```

2. Now you have 3 options:
   1. Call `GetSnowflake()` to get a `Snowflake` object
   2. Call `GetCode()` to get an Id in number (ulong) format
   3. Call `GetCodeString()` to get an Id in string format


Additionally, the `SnowflakeIDGenerator` class methods can be used as static.
IE.: `GetCode(machineId)` or `GetCodeString(machineId)`

## Parse

Parse a Snowflake either from a string or a number (ulong).

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
