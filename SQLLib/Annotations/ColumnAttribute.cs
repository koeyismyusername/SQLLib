using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLLib.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; private set; }
        public bool IsNullable { get; set; } = true;
        public ColumnAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name은 null이거나 비어있을 수 없습니다.");
            Name = name;
        }
    }
}
