# Introduction 
Generate unique identifiers based on Twitter's Snowflake ID.

Parse a Snowflake to get information about it's creation.

# Usage

## Generate

1. Instantiate class `SnowflakeIDGenerator`
```c#
SnowflakeIDGenerator gen = new SnowflakeIDGenerator(machineId) 
```
where `machineId` is the number of the system currently trying to get an id

2. Now you have 3 options:
   1. Call `GetSnowflake` to get a `Snowflake` object
   2. Call `GetCode` to get an Id in number (ulong) format
   3. Call `GetCodeString` to get an Id in string format


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