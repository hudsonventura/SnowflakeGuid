# Introduction 
Generate unique identifiers based on Twitter's Snowflake ID

# Usage
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




# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)