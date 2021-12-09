using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class logEntities : DbContext
    {
        public logEntities()
        {
        }

        public logEntities(DbContextOptions<logEntities> options) : base(options)
        {
        }

        public virtual DbSet<ServiceLog> ServiceLog { get; set; }
        public virtual DbSet<WebAppErrorLog> WebAppErrorLog { get; set; }
        public virtual DbSet<WebAppLoginLog> WebAppLoginLog { get; set; }
        public virtual DbSet<WebAppOperationLog> WebAppOperationLog { get; set; }
        public virtual DbSet<WebAppPasswordLog> WebAppPasswordLog { get; set; }
        public virtual DbSet<WebApiAccessLog> WebApiAccessLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceLog>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.LogType)
                    .HasColumnName("LogType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("日志分类");

                entity.Property(e => e.LogLevel)
                    .HasColumnName("LogLevel")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("日志等级");

                entity.Property(e => e.LogMessage)
                    .HasColumnName("LogMessage")
                    .HasComment("日志描述");

                entity.Property(e => e.LogRemark)
                    .HasColumnName("LogRemark")
                    .HasMaxLength(500)
                    .HasComment("日志备注");

                entity.Property(e => e.LogIp)
                    .HasColumnName("LogIp")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("ip地址");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("日志时间");

            });

            modelBuilder.Entity<WebAppErrorLog>(entity =>
            {
                entity.HasKey(e => e.LogID);

                entity.Property(e => e.LogID)
                    .HasColumnName("LogID")
                    .IsRequired();

                entity.Property(e => e.UserID)
                    .HasColumnName("UserID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("操作人ID");

                entity.Property(e => e.UserIP)
                    .HasColumnName("UserIP")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("操作IP");

                entity.Property(e => e.LogLevel)
                    .HasColumnName("LogLevel")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("日志等级");

                entity.Property(e => e.LogMessage)
                    .HasColumnName("LogMessage")
                    .HasComment("日志记录");

                entity.Property(e => e.AddTime)
                    .HasColumnName("AddTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<WebAppLoginLog>(entity =>
            {
                entity.HasKey(e => e.LogID);

                entity.Property(e => e.LogID)
                    .HasColumnName("LogID")
                    .IsRequired();

                entity.Property(e => e.LoginStatus)
                    .HasColumnName("LoginStatus")
                    .IsRequired()
                    .HasComment("登入状态(0失败1成功)");

                entity.Property(e => e.LoginType)
                    .HasColumnName("LoginType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("登入方式:0.密码;1.邮箱;");

                entity.Property(e => e.Account)
                    .HasColumnName("Account")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("登入帐号");

                entity.Property(e => e.Password)
                    .HasColumnName("Password")
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasComment("登入密码(如果是成功登入则不需要填写)");

                entity.Property(e => e.UserID)
                    .HasColumnName("UserID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("用户ID");

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("ip地址");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(500)
                    .HasComment("备注信息");

                entity.Property(e => e.AddTime)
                    .HasColumnName("AddTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<WebAppOperationLog>(entity =>
            {
                entity.HasKey(e => e.LogID);

                entity.Property(e => e.LogID)
                    .HasColumnName("LogID")
                    .IsRequired();

                entity.Property(e => e.OperationType)
                    .HasColumnName("OperationType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("操作类型(1:添加,2:修改,3:删除)");

                entity.Property(e => e.TableName)
                    .HasColumnName("TableName")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasComment("操作的表名");

                entity.Property(e => e.UserID)
                    .HasColumnName("UserID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("操作人ID");

                entity.Property(e => e.UserIP)
                    .HasColumnName("UserIP")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("操作人IP");

                entity.Property(e => e.RecordID)
                    .HasColumnName("RecordID")
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))")
                    .HasComment("操作表ID");

                entity.Property(e => e.LogMessage)
                    .HasColumnName("LogMessage")
                    .HasComment("日志信息");

                entity.Property(e => e.AddTime)
                    .HasColumnName("AddTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<WebAppPasswordLog>(entity =>
            {
                entity.HasKey(e => e.LogID);

                entity.Property(e => e.LogID)
                    .HasColumnName("LogID")
                    .IsRequired();

                entity.Property(e => e.Account)
                    .HasColumnName("Account")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("登入帐号");

                entity.Property(e => e.Password)
                    .HasColumnName("Password")
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UserID)
                    .HasColumnName("UserID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("账号ID");

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("ip地址");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(500)
                    .HasComment("备注信息");

                entity.Property(e => e.AddTime)
                    .HasColumnName("AddTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<WebApiAccessLog>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.Property(e => e.id)
                    .HasColumnName("id")
                    .IsRequired();

                entity.Property(e => e.LogType)
                    .HasColumnName("LogType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("日志来源");

                entity.Property(e => e.Url)
                    .HasColumnName("Url")
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("访问的URL地址");

                entity.Property(e => e.RequestID)
                    .HasColumnName("RequestID")
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("请求ID");

                entity.Property(e => e.UserID)
                    .HasColumnName("UserID")
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("Token");

                entity.Property(e => e.Ip)
                    .HasColumnName("Ip")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("IP地址");

                entity.Property(e => e.State)
                    .HasColumnName("State")
                    .IsRequired()
                    .HasComment("访问结果(1成功0失败)");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(512);

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

        }
    }
}
