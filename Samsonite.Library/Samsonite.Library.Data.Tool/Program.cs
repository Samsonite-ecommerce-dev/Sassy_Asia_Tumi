using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Samsonite.Library.Data.Tool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Samsonite.Library.Data.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDataBaseModels();
        }

        //生成数据库model类
        private static void CreateDataBaseModels()
        {
            try
            {
                //显示选项
                var _dbList = DBOption();
                Console.WriteLine("Please select a option...");
                foreach (var item in _dbList)
                {
                    Console.WriteLine($"{item.ID}.{item.Name}");
                }

                //选择数据库
                DBConfig _dbConfig = null;
                var str = Console.ReadLine();
                foreach (var item in _dbList)
                {
                    if (item.ID.ToString() == str)
                    {
                        _dbConfig = item;
                        break;
                    }
                }

                //清空屏幕
                Console.Clear();

                //读取数据库结构
                if (_dbConfig != null)
                {
                    Console.WriteLine("DB Message...");
                    Console.WriteLine($"Name:{_dbConfig.Name}");
                    Console.WriteLine($"Namespace:{_dbConfig.Namespace}");
                    Console.WriteLine($"Class:{_dbConfig.Class}");
                    Console.WriteLine($"Connection:{_dbConfig.Connection}");
                    Console.WriteLine($"FileSavePath:{_dbConfig.FileSavePath}");
                    Console.WriteLine($"-----------------------------------------------");
                    Console.WriteLine("");

                    if (!Directory.Exists(_dbConfig.FileSavePath))
                    {
                        Directory.CreateDirectory(_dbConfig.FileSavePath);
                    }

                    using (dbEntities dB = new dbEntities(_dbConfig.Connection))
                    {
                        List<DBTableDetail> objDBTableDetails = new List<DBTableDetail>();

                        List<DBTable> objDBTables = dB.Set<DBTable>().FromSqlRaw("select id,name,xtype from sysobjects where (xtype='U' or xtype='V')").ToList();
                        foreach (var item in objDBTables)
                        {
                            var list = dB.Set<DBTableDetail>().FromSqlRaw("SELECT {0} as tablename, CASE WHEN EXISTS (SELECT 1 FROM sysobjects WHERE xtype = 'PK' AND parent_obj = a.id AND name IN (SELECT name FROM sysindexes WHERE indid IN (SELECT indid FROM sysindexkeys WHERE id = a.id AND colid = a.colid))) THEN 1 ELSE 0 END AS 'iskey', CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1 THEN 1 ELSE 0 END AS 'isidentity',a.name AS colname, c.name AS typename, a.length AS 'byte', COLUMNPROPERTY(a.id, a.name, 'PRECISION') AS 'length',a.xscale, a.isnullable, ISNULL(e.text, '') AS 'default', ISNULL(p.value, '') AS 'comment' FROM sys.syscolumns AS a INNER JOIN sys.sysobjects AS b ON a.id = b.id INNER JOIN sys.systypes AS c ON a.xtype = c.xtype LEFT OUTER JOIN sys.syscomments AS e ON a.cdefault = e.id LEFT OUTER JOIN sys.extended_properties AS p ON a.id = p.major_id AND a.colid = p.minor_id WHERE (b.name = {0}) AND (c.status <> '1') order by a.colid asc", item.Name);
                            objDBTableDetails.AddRange(list);
                        }

                        Console.WriteLine("Begin to create models...");
                        //--------------------生成表Model--------------------
                        SaveModels(_dbConfig, objDBTables, objDBTableDetails);

                        //--------------------生成Entities--------------------
                        SaveEntities(_dbConfig, objDBTables, objDBTableDetails);
                    }
                }
                else
                {
                    Console.WriteLine("The Database dose not exists,please select again!");
                    CreateDataBaseModels();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }

        private static void SaveModels(DBConfig dBConfig, List<DBTable> dBTables, List<DBTableDetail> dBTableDetails)
        {
            foreach (var item in dBTables)
            {
                StringBuilder _str = new StringBuilder();
                _str.AppendLine("using System;");
                _str.AppendLine("using System.Collections.Generic;");
                _str.AppendLine("");
                _str.AppendLine($"namespace {dBConfig.Namespace}");
                _str.AppendLine("{");
                _str.AppendLine($"    public partial class {item.Name}");
                _str.AppendLine("    {");
                foreach (var field in dBTableDetails.Where(p => p.TableName == item.Name).ToList())
                {
                    _str.AppendLine("        /// <summary>");
                    _str.AppendLine($"        /// {field.Comment}");
                    _str.AppendLine("        /// </summary>");
                    _str.AppendLine($"        public {Utility.ConvertTypeName(field.TypeName, (field.IsNullAble == 1), field.Comment)} {field.ColName} {{ get; set; }}");
                    _str.AppendLine("");
                }
                _str.AppendLine("    }");
                _str.AppendLine("}");

                //保存文件
                string _fileName = $"{item.Name}.cs";
                Utility.SaveFile(_str.ToString(), $"{dBConfig.FileSavePath}//{_fileName}");
                Console.WriteLine($"{_fileName}:Finished");
            }
        }

        private static void SaveEntities(DBConfig dBConfig, List<DBTable> dBTables, List<DBTableDetail> dBTableDetails)
        {
            StringBuilder _str = new StringBuilder();
            _str.AppendLine("using System;");
            _str.AppendLine("using Microsoft.EntityFrameworkCore;");
            _str.AppendLine("using Microsoft.EntityFrameworkCore.Metadata;");
            _str.AppendLine("");
            _str.AppendLine($"namespace {dBConfig.Namespace}");
            _str.AppendLine("{");
            _str.AppendLine($"    public partial class {dBConfig.Class} : DbContext");
            _str.AppendLine("    {");
            _str.AppendLine($"        public {dBConfig.Class}()");
            _str.AppendLine("        {");
            _str.AppendLine("        }");
            _str.AppendLine("");
            _str.AppendLine($"        public {dBConfig.Class}(DbContextOptions<{dBConfig.Class}> options) : base(options)");
            _str.AppendLine("        {");
            _str.AppendLine("        }");
            _str.AppendLine("");
            foreach (var item in dBTables)
            {
                _str.AppendLine($"        public virtual DbSet<{item.Name}> {item.Name} {{ get; set; }}");
            }
            _str.AppendLine("");
            _str.AppendLine("        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)");
            _str.AppendLine("        {");
            _str.AppendLine("            if (!optionsBuilder.IsConfigured)");
            _str.AppendLine("            {");
            //_str.AppendLine($"                optionsBuilder.UseSqlServer(\"{dBConfig.Connection}\");");
            _str.AppendLine("            }");
            _str.AppendLine("        }");
            _str.AppendLine("");
            _str.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            _str.AppendLine("        {");
            //表
            foreach (var item in dBTables.Where(p => p.XType.Trim() == "U"))
            {
                _str.AppendLine($"            modelBuilder.Entity<{item.Name}>(entity =>");
                _str.AppendLine("            {");
                foreach (var field in dBTableDetails.Where(p => p.TableName == item.Name).ToList())
                {
                    var _typeName = Utility.ConvertTypeName(field.TypeName, (field.IsNullAble == 1));
                    if (field.IsKey == 1)
                    {
                        _str.AppendLine($"                entity.HasKey(e => e.{field.ColName});");
                        _str.AppendLine("");
                    }
                    _str.AppendLine($"                entity.Property(e => e.{field.ColName})");
                    List<string> _tmpList = new List<string>();
                    _tmpList.Add($"                    .HasColumnName(\"{field.ColName}\")");
                    if (field.IsNullAble == 0)
                        _tmpList.Add("                    .IsRequired()");
                    if (_typeName == "DateTime" || _typeName == "DateTime?")
                        _tmpList.Add("                    .HasColumnType(\"datetime\")");
                    if (_typeName == "decimal" || _typeName == "decimal?")
                        _tmpList.Add($"                    .HasColumnType(\"decimal({field.Length}, {field.XScale})\")");
                    if (_typeName == "string")
                        //max长度不限制长度
                        if (field.Length > 0)
                        {
                            _tmpList.Add($"                    .HasMaxLength({field.Length})");
                        }
                    if (field.TypeName == "varchar" || field.TypeName == "char" || field.TypeName == "text")
                        _tmpList.Add("                    .IsUnicode(false)");
                    if (!string.IsNullOrEmpty(field.Default))
                        //bool不填写默认值
                        if (field.TypeName != "bit")
                        {
                            _tmpList.Add($"                    .HasDefaultValueSql(\"{field.Default}\")");
                        }
                    if (!string.IsNullOrEmpty(field.Comment))
                        _tmpList.Add($"                    .HasComment(\"{field.Comment}\")");
                    //结尾加上分号
                    for (int t = 0; t < _tmpList.Count; t++)
                    {
                        if (t == _tmpList.Count - 1)
                        {
                            _str.AppendLine($"{_tmpList[t]};");
                        }
                        else
                        {
                            _str.AppendLine(_tmpList[t]);
                        }
                    }
                    _str.AppendLine("");
                }
                _str.AppendLine("            });");
                _str.AppendLine("");
            }
            //视图
            foreach (var item in dBTables.Where(p => p.XType.Trim() == "V"))
            {
                _str.AppendLine($"            modelBuilder.Entity<{item.Name}>().HasNoKey();");
                _str.AppendLine("");
            }
            //自定义队列
            if (dBConfig.CustomQuerys != null)
            {
                foreach (var item in dBConfig.CustomQuerys)
                {
                    _str.AppendLine($"            modelBuilder.Entity<{item}>().HasNoKey();");
                    _str.AppendLine("");
                }
            }
            _str.AppendLine("        }");
            _str.AppendLine("    }");
            _str.AppendLine("}");

            //保存文件
            string _fileName = $"{dBConfig.Class}.cs";
            Utility.SaveFile(_str.ToString(), $"{dBConfig.FileSavePath}//{_fileName}");
            Console.WriteLine($"{_fileName}:Finished");
        }

        /// <summary>
        /// 数据库选项
        /// </summary>
        /// <returns></returns>
        private static List<DBConfig> DBOption()
        {
            List<DBConfig> _result = new List<DBConfig>();
            //读取配置文件
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false, true);
            //.AddEnvironmentVariables();
            var configurationRoot = builder.Build();
            var config = configurationRoot.GetSection("Config");
            _result = config.Get<List<DBConfig>>();

            return _result;
        }
    }
}
