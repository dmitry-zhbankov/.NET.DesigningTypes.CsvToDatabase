# Csv To Database
The goal is to сreate a сonsole application which is able to read data from csv file and write it to the database
## Requirements
* Code style
* Create data access level using Repository pattern. The goal of this task is not to dig into details of working with database but to understand how asynchronous code works. So there is no need to use Entity Framework or any other ORM to implement this task. You can use any Sql/NoSql database and any library/framework which doesn’t require a lot of effort to be set up. You can use RavenDb or any other database.
* Each method invocation of Repository should be logged using Logging Proxy
* Use csv enumerable developed at task Csv Enumerable
## Advanced Requirements
* Store application configuration settings like connection strings and csv file path in config file