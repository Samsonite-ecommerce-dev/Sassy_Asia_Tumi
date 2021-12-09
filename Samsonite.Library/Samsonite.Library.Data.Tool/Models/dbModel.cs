using System;
using System.Collections.Generic;

namespace Samsonite.Library.Data.Tool.Models
{
    public class DBTable
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string XType { get; set; }
    }

    public class DBTableDetail
    {
        public string TableName { get; set; }

        public int IsKey { get; set; }

        public int IsIdentity { get; set; }

        public string ColName { get; set; }

        public string TypeName { get; set; }

        public Int16 Byte { get; set; }

        public int Length { get; set; }

        public byte XScale { get; set; }

        public int IsNullAble { get; set; }

        public string Default { get; set; }

        public string Comment { get; set; }
    }

    public class DBConfig
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public string Class { get; set; }

        public string Connection { get; set; }

        public string FileSavePath { get; set; }

        public List<string> CustomQuerys { get; set; }
    }
}
