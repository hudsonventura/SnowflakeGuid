# Changelog

## 1.3.2023
- Comparisons should be between snowflakes using the same epoch
- Equality comparer returns false if the snowflakes being compared use different epochs

## 1.2.2023
- Added ability to change and rebase epoch to an already generated code
- Added implicit casting from Snowflake to string and ulong
- Added explicit casting to Snowflake from string and ulong

## 1.1.2023
- Added ability to use a custom epoch

## 1.0.2023
- Throw exception if time moves backwards

## 1.0.2022
- Initial version
