using System;

namespace DatabaseExampleCore
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableReferenceAttribute : Attribute
    {
        public Type Type { get; }

        public TableReferenceAttribute(Type type)
        {
            Type = type;
        }
    }
}