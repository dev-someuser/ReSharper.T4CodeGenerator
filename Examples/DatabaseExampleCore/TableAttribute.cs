using System;

namespace DatabaseExampleCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; }

        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}