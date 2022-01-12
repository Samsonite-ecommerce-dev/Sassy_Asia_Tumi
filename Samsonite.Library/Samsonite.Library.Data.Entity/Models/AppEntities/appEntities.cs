using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Samsonite.Library.Data.Entity.Models
{
    public partial class appEntities : DbContext
    {
        public appEntities()
        {
        }

        public appEntities(DbContextOptions<appEntities> options) : base(options)
        {
        }

        public virtual DbSet<GroupInfo> GroupInfo { get; set; }
        public virtual DbSet<SysFunction> SysFunction { get; set; }
        public virtual DbSet<SysFunctionGroup> SysFunctionGroup { get; set; }
        public virtual DbSet<SysRole> SysRole { get; set; }
        public virtual DbSet<View_SparePart> View_SparePart { get; set; }
        public virtual DbSet<SysRoleFunction> SysRoleFunction { get; set; }
        public virtual DbSet<SparePart> SparePart { get; set; }
        public virtual DbSet<SysUploadModel> SysUploadModel { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<View_ProductSparePart> View_ProductSparePart { get; set; }
        public virtual DbSet<SysConfig> SysConfig { get; set; }
        public virtual DbSet<ServiceModuleInfo> ServiceModuleInfo { get; set; }
        public virtual DbSet<View_SysFunction> View_SysFunction { get; set; }
        public virtual DbSet<ProductSparePart> ProductSparePart { get; set; }
        public virtual DbSet<FTPInfo> FTPInfo { get; set; }
        public virtual DbSet<LanguagePackKey> LanguagePackKey { get; set; }
        public virtual DbSet<LanguagePackValue> LanguagePackValue { get; set; }
        public virtual DbSet<LanguageType> LanguageType { get; set; }
        public virtual DbSet<View_LanguagePack> View_LanguagePack { get; set; }
        public virtual DbSet<SendMailGroup> SendMailGroup { get; set; }
        public virtual DbSet<ServiceModuleJob> ServiceModuleJob { get; set; }
        public virtual DbSet<SMMailSend> SMMailSend { get; set; }
        public virtual DbSet<SMMailSended> SMMailSended { get; set; }
        public virtual DbSet<SMSSend> SMSSend { get; set; }
        public virtual DbSet<SMSSended> SMSSended { get; set; }
        public virtual DbSet<WebApiAccount> WebApiAccount { get; set; }
        public virtual DbSet<ProductLineSize> ProductLineSize { get; set; }
        public virtual DbSet<WebApiRoles> WebApiRoles { get; set; }
        public virtual DbSet<ProductLineColor> ProductLineColor { get; set; }
        public virtual DbSet<ProductLine> ProductLine { get; set; }
        public virtual DbSet<Product> Product { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupInfo>(entity =>
            {
                entity.HasKey(e => e.GroupID);

                entity.Property(e => e.GroupID)
                    .HasColumnName("GroupID")
                    .IsRequired();

                entity.Property(e => e.GroupDescription)
                    .HasColumnName("GroupDescription")
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasComment("分组名称");

                entity.Property(e => e.GroupText)
                    .HasColumnName("GroupText")
                    .HasMaxLength(500)
                    .HasComment("描述");

                entity.Property(e => e.AddDate)
                    .HasColumnName("AddDate")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.EditDate)
                    .HasColumnName("EditDate")
                    .HasColumnType("datetime")
                    .HasComment("编辑时间");

            });

            modelBuilder.Entity<SysFunction>(entity =>
            {
                entity.HasKey(e => e.Funcid);

                entity.Property(e => e.Funcid)
                    .HasColumnName("Funcid")
                    .IsRequired()
                    .HasComment("功能ID，自动增长");

                entity.Property(e => e.FuncName)
                    .HasColumnName("FuncName")
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasComment("功能名称");

                entity.Property(e => e.Groupid)
                    .HasColumnName("Groupid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("分组ID");

                entity.Property(e => e.SeqNumber)
                    .HasColumnName("SeqNumber")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("排序号");

                entity.Property(e => e.FuncType)
                    .HasColumnName("FuncType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("1：栏目，2：功能");

                entity.Property(e => e.FuncSign)
                    .HasColumnName("FuncSign")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.FuncUrl)
                    .HasColumnName("FuncUrl")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasComment("功能链接地址，适用于菜单");

                entity.Property(e => e.FuncPower)
                    .HasColumnName("FuncPower")
                    .HasMaxLength(512)
                    .HasComment("功能权限");

                entity.Property(e => e.FuncTarget)
                    .HasColumnName("FuncTarget")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("链接地址方式,值为blank,parent,self,top,iframe");

                entity.Property(e => e.FuncMemo)
                    .HasColumnName("FuncMemo")
                    .HasMaxLength(512)
                    .HasComment("简要描述");

                entity.Property(e => e.IsShow)
                    .HasColumnName("IsShow")
                    .IsRequired()
                    .HasComment("是否显示，1：显示，0：不显示");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

            });

            modelBuilder.Entity<SysFunctionGroup>(entity =>
            {
                entity.HasKey(e => e.Groupid);

                entity.Property(e => e.Groupid)
                    .HasColumnName("Groupid")
                    .IsRequired()
                    .HasComment("分组ID，自动增长");

                entity.Property(e => e.GroupName)
                    .HasColumnName("GroupName")
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasComment("分组名称");

                entity.Property(e => e.GroupIcon)
                    .HasColumnName("GroupIcon")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("小图标");

                entity.Property(e => e.Rootid)
                    .HasColumnName("Rootid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("主排序号");

                entity.Property(e => e.SeqNumber)
                    .HasColumnName("SeqNumber")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("排序号");

                entity.Property(e => e.Parentid)
                    .HasColumnName("Parentid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("上级ID");

                entity.Property(e => e.GroupMemo)
                    .HasColumnName("GroupMemo")
                    .HasMaxLength(512)
                    .HasComment("简要描述");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

            });

            modelBuilder.Entity<SysRole>(entity =>
            {
                entity.HasKey(e => e.Roleid);

                entity.Property(e => e.Roleid)
                    .HasColumnName("Roleid")
                    .IsRequired();

                entity.Property(e => e.RoleName)
                    .HasColumnName("RoleName")
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasComment("角色名称");

                entity.Property(e => e.RoleWeight)
                    .HasColumnName("RoleWeight")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("权重:1最大");

                entity.Property(e => e.RoleMemo)
                    .HasColumnName("RoleMemo")
                    .HasMaxLength(512)
                    .HasComment("角色简要说明");

                entity.Property(e => e.SeqNumber)
                    .HasColumnName("SeqNumber")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("排序");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

            });

            modelBuilder.Entity<SysRoleFunction>(entity =>
            {
                entity.HasKey(e => e.RoleFunid);

                entity.Property(e => e.RoleFunid)
                    .HasColumnName("RoleFunid")
                    .IsRequired();

                entity.Property(e => e.Roleid)
                    .HasColumnName("Roleid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("权限组ID");

                entity.Property(e => e.Funid)
                    .HasColumnName("Funid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("功能ID");

                entity.Property(e => e.Powers)
                    .HasColumnName("Powers")
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasComment("权限集合");

            });

            modelBuilder.Entity<SparePart>(entity =>
            {
                entity.HasKey(e => e.SparePartID);

                entity.Property(e => e.SparePartID)
                    .HasColumnName("SparePartID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.SparePartDescription)
                    .HasColumnName("SparePartDescription")
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasComment("配件名称");

                entity.Property(e => e.GroupID)
                    .HasColumnName("GroupID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("关联GroupInfo的主键ID");

                entity.Property(e => e.CustomizeSparePart)
                    .HasColumnName("CustomizeSparePart")
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasComment("自定义编号");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("ImageUrl")
                    .HasMaxLength(500)
                    .HasComment("图片路径");

                entity.Property(e => e.BasicPrice)
                    .HasColumnName("BasicPrice")
                    .IsRequired()
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("基础价格");

                entity.Property(e => e.CostPrice)
                    .HasColumnName("CostPrice")
                    .IsRequired()
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Currency)
                    .HasColumnName("Currency")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("货币单位");

                entity.Property(e => e.UnitofMeasure)
                    .HasColumnName("UnitofMeasure")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.ValidFrom)
                    .HasColumnName("ValidFrom")
                    .HasColumnType("datetime")
                    .HasComment("价格有效期");

                entity.Property(e => e.ValidTo)
                    .HasColumnName("ValidTo")
                    .HasColumnType("datetime")
                    .HasComment("价格有效期");

                entity.Property(e => e.PriceUpdateDate)
                    .HasColumnName("PriceUpdateDate")
                    .HasColumnType("datetime")
                    .HasComment("价格更新时间");

                entity.Property(e => e.AvailableStock)
                    .HasColumnName("AvailableStock")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("有效库存");

                entity.Property(e => e.InventoryStock)
                    .HasColumnName("InventoryStock")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("库存数");

                entity.Property(e => e.InventoryUpdateDate)
                    .HasColumnName("InventoryUpdateDate")
                    .HasColumnType("datetime")
                    .HasComment("库存更新时间");

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("状态");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(500)
                    .HasComment("备注");

                entity.Property(e => e.AddDate)
                    .HasColumnName("AddDate")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.EditDate)
                    .HasColumnName("EditDate")
                    .HasColumnType("datetime")
                    .HasComment("编辑时间");

            });

            modelBuilder.Entity<SysUploadModel>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.UploadName)
                    .HasColumnName("UploadName")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("上传类型名");

                entity.Property(e => e.ModelMark)
                    .HasColumnName("ModelMark")
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("标识");

                entity.Property(e => e.FileType)
                    .HasColumnName("FileType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("1图片2文件3flash");

                entity.Property(e => e.MaxFileSize)
                    .HasColumnName("MaxFileSize")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("限制大小");

                entity.Property(e => e.MaxFileCount)
                    .HasColumnName("MaxFileCount")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("最大文件上传数量限制");

                entity.Property(e => e.AllowFile)
                    .HasColumnName("AllowFile")
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasComment("允许上传的格式");

                entity.Property(e => e.SaveCatalog)
                    .HasColumnName("SaveCatalog")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("保存目录");

                entity.Property(e => e.SaveStyle)
                    .HasColumnName("SaveStyle")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("保存方式(dateorder/fileorder)");

                entity.Property(e => e.IsRename)
                    .HasColumnName("IsRename")
                    .IsRequired()
                    .HasComment("文件重命名(0否1是)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.Property(e => e.Userid)
                    .HasColumnName("Userid")
                    .IsRequired();

                entity.Property(e => e.UserName)
                    .HasColumnName("UserName")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("账号名称");

                entity.Property(e => e.Pwd)
                    .HasColumnName("Pwd")
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasComment("登录密码");

                entity.Property(e => e.RealName)
                    .HasColumnName("RealName")
                    .HasMaxLength(50)
                    .HasComment("昵称");

                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(100);

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasComment("备注信息");

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("状态(0:正常,1:锁定,2:首次登入未修改密码)");

                entity.Property(e => e.Type)
                    .HasColumnName("Type")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("账号类型(1:内部员工,2:客户)");

                entity.Property(e => e.DefaultLanguage)
                    .HasColumnName("DefaultLanguage")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("默认语言");

                entity.Property(e => e.PrivateKey)
                    .HasColumnName("PrivateKey")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.PwdErrorNum)
                    .HasColumnName("PwdErrorNum")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("连续密码输入错误次数");

                entity.Property(e => e.LastPwdEditTime)
                    .HasColumnName("LastPwdEditTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.AddTime)
                    .HasColumnName("AddTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(e => e.UserRoleid);

                entity.Property(e => e.UserRoleid)
                    .HasColumnName("UserRoleid")
                    .IsRequired();

                entity.Property(e => e.Userid)
                    .HasColumnName("Userid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("用户ID");

                entity.Property(e => e.Roleid)
                    .HasColumnName("Roleid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("权限组ID");

            });

            modelBuilder.Entity<SysConfig>(entity =>
            {
                entity.HasKey(e => e.ConfigID);

                entity.Property(e => e.ConfigID)
                    .HasColumnName("ConfigID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ConfigKey)
                    .HasColumnName("ConfigKey")
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("配置标识");

                entity.Property(e => e.ConfigValue)
                    .HasColumnName("ConfigValue")
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasComment("配置参数");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(512)
                    .HasComment("备注");

            });

            modelBuilder.Entity<ServiceModuleInfo>(entity =>
            {
                entity.HasKey(e => e.ModuleID);

                entity.Property(e => e.ModuleID)
                    .HasColumnName("ModuleID")
                    .IsRequired();

                entity.Property(e => e.ModuleMark)
                    .HasColumnName("ModuleMark")
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasComment("服务标识");

                entity.Property(e => e.ModuleTitle)
                    .HasColumnName("ModuleTitle")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("服务运行模块名");

                entity.Property(e => e.ModuleWorkflowID)
                    .HasColumnName("ModuleWorkflowID")
                    .HasMaxLength(50)
                    .HasComment("工作流ID");

                entity.Property(e => e.ModuleAssembly)
                    .HasColumnName("ModuleAssembly")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("服务运行程序集");

                entity.Property(e => e.ModuleType)
                    .HasColumnName("ModuleType")
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasComment("服务运行程序集的类型");

                entity.Property(e => e.ModuleBLL)
                    .HasColumnName("ModuleBLL")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("BLL名");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(500)
                    .HasComment("备注信息");

                entity.Property(e => e.IsRun)
                    .HasColumnName("IsRun")
                    .IsRequired()
                    .HasComment("运行中(0否1是)");

                entity.Property(e => e.IsNotify)
                    .HasColumnName("IsNotify")
                    .IsRequired()
                    .HasComment("是否开启错误通知(0否1是)");

                entity.Property(e => e.FixType)
                    .HasColumnName("FixType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("定时执行类型(0不设置,1间隔执行,2定时某一时刻执行)");

                entity.Property(e => e.FixTime)
                    .HasColumnName("FixTime")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("执行时间");

                entity.Property(e => e.SortID)
                    .HasColumnName("SortID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("排序ID");

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("状态:0:停止;1:运行;2.暂停");

                entity.Property(e => e.NextRunTime)
                    .HasColumnName("NextRunTime")
                    .HasColumnType("datetime")
                    .HasComment("下次运行时间");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

            });

            modelBuilder.Entity<ProductSparePart>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.MaterialId)
                    .HasColumnName("MaterialId")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Gridval)
                    .HasColumnName("Gridval")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.LineID)
                    .HasColumnName("LineID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("关联ProductLine表主键ID");

                entity.Property(e => e.SizeID)
                    .HasColumnName("SizeID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.ColorID)
                    .HasColumnName("ColorID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.GroupID)
                    .HasColumnName("GroupID")
                    .IsRequired()
                    .HasComment("关联GroupInfo表主键ID");

                entity.Property(e => e.VersionID)
                    .HasColumnName("VersionID")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("版本号");

                entity.Property(e => e.SparePartID)
                    .HasColumnName("SparePartID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("SparePart表主键ID");

                entity.Property(e => e.AddDate)
                    .HasColumnName("AddDate")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.EditDate)
                    .HasColumnName("EditDate")
                    .HasColumnType("datetime")
                    .HasComment("编辑时间");

            });

            modelBuilder.Entity<FTPInfo>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.FTPIdentify)
                    .HasColumnName("FTPIdentify")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("FTP标识");

                entity.Property(e => e.FTPName)
                    .HasColumnName("FTPName")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("FTP名称");

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("IP地址");

                entity.Property(e => e.Port)
                    .HasColumnName("Port")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("端口号");

                entity.Property(e => e.UserName)
                    .HasColumnName("UserName")
                    .HasMaxLength(50)
                    .HasComment("账号");

                entity.Property(e => e.Password)
                    .HasColumnName("Password")
                    .HasMaxLength(50)
                    .HasComment("密码");

                entity.Property(e => e.FilePath)
                    .HasColumnName("FilePath")
                    .HasMaxLength(256)
                    .HasComment("远程路径");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(512)
                    .HasComment("备注信息");

                entity.Property(e => e.SortID)
                    .HasColumnName("SortID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("排序ID");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<LanguagePackKey>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.FunctionID)
                    .HasColumnName("FunctionID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("功能ID");

                entity.Property(e => e.PackKey)
                    .HasColumnName("PackKey")
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("标识");

                entity.Property(e => e.SeqNumber)
                    .HasColumnName("SeqNumber")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("排序");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("IsDelete")
                    .IsRequired()
                    .HasComment("是否删除(0否1是)");

            });

            modelBuilder.Entity<LanguagePackValue>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.KeyID)
                    .HasColumnName("KeyID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LanguageTypeID)
                    .HasColumnName("LanguageTypeID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PackValue)
                    .HasColumnName("PackValue")
                    .IsRequired()
                    .HasMaxLength(256);

            });

            modelBuilder.Entity<LanguageType>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.LanguageName)
                    .HasColumnName("LanguageName")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("语言名称");

                entity.Property(e => e.LanguageCode)
                    .HasColumnName("LanguageCode")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("语言编号");

                entity.Property(e => e.Lang)
                    .HasColumnName("Lang")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("语言包文件名");

                entity.Property(e => e.IsDefault)
                    .HasColumnName("IsDefault")
                    .IsRequired()
                    .HasComment("是否默认:1.是,2:否");

            });

            modelBuilder.Entity<SendMailGroup>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.GroupName)
                    .HasColumnName("GroupName")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("分组名称");

                entity.Property(e => e.MailAddresses)
                    .HasColumnName("MailAddresses")
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasComment("发送邮件集合,如a@163.com,b@163.com");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(256)
                    .HasComment("备注信息");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系统时间");

            });

            modelBuilder.Entity<ServiceModuleJob>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.ModuleID)
                    .HasColumnName("ModuleID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("ServiceModuleInfo表主键ID");

                entity.Property(e => e.OperType)
                    .HasColumnName("OperType")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("操作指令:1.启动;2.暂停;3.继续");

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("状态:0.未处理;1.处理中;2.成功;3.失败");

                entity.Property(e => e.StatusMessage)
                    .HasColumnName("StatusMessage")
                    .HasMaxLength(1024)
                    .HasComment("状态信息");

                entity.Property(e => e.AddTime)
                    .HasColumnName("AddTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

            });

            modelBuilder.Entity<SMMailSend>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.RecvMail)
                    .HasColumnName("RecvMail")
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasComment("接收人邮箱");

                entity.Property(e => e.MailTitle)
                    .HasColumnName("MailTitle")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("邮件标题");

                entity.Property(e => e.MailContent)
                    .HasColumnName("MailContent")
                    .IsRequired()
                    .HasComment("邮件内容");

                entity.Property(e => e.MailAttachment)
                    .HasColumnName("MailAttachment")
                    .HasMaxLength(500)
                    .HasComment("附件地址");

                entity.Property(e => e.SendUserid)
                    .HasColumnName("SendUserid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发件人ID，0表示系统");

                entity.Property(e => e.SendUserIP)
                    .HasColumnName("SendUserIP")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("发件人IP地址");

                entity.Property(e => e.SendState)
                    .HasColumnName("SendState")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发送状态(0:未发送,1:已发送,2:发送失败)");

                entity.Property(e => e.SendCount)
                    .HasColumnName("SendCount")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发送次数");

                entity.Property(e => e.SendMessage)
                    .HasColumnName("SendMessage")
                    .HasComment("发送结果");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("添加时间");

            });

            modelBuilder.Entity<SMMailSended>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.RecvMail)
                    .HasColumnName("RecvMail")
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasComment("接收人邮箱");

                entity.Property(e => e.MailTitle)
                    .HasColumnName("MailTitle")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("邮件标题");

                entity.Property(e => e.MailContent)
                    .HasColumnName("MailContent")
                    .IsRequired()
                    .HasComment("邮件内容");

                entity.Property(e => e.MailAttachment)
                    .HasColumnName("MailAttachment")
                    .HasMaxLength(500)
                    .HasComment("附件地址");

                entity.Property(e => e.SendUserid)
                    .HasColumnName("SendUserid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发件人ID，0表示系统");

                entity.Property(e => e.SendUserIP)
                    .HasColumnName("SendUserIP")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("发件人IP地址");

                entity.Property(e => e.SendState)
                    .HasColumnName("SendState")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发送状态(0:为发送,1:已发送)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("添加时间");

                entity.Property(e => e.SendTime)
                    .HasColumnName("SendTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("邮件发送时间");

            });

            modelBuilder.Entity<SMSSend>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.RecvMobile)
                    .HasColumnName("RecvMobile")
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasComment("接收人手机号码");

                entity.Property(e => e.Sender)
                    .HasColumnName("Sender")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("发送人");

                entity.Property(e => e.MessageTitle)
                    .HasColumnName("MessageTitle")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("短信标题");

                entity.Property(e => e.MessageContent)
                    .HasColumnName("MessageContent")
                    .IsRequired()
                    .HasComment("短信内容");

                entity.Property(e => e.SendUserid)
                    .HasColumnName("SendUserid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发件人ID，0表示系统");

                entity.Property(e => e.SendUserIP)
                    .HasColumnName("SendUserIP")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("发件人IP地址");

                entity.Property(e => e.SendState)
                    .HasColumnName("SendState")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发送状态(0:未发送,1:已发送,2:发送失败)");

                entity.Property(e => e.SendCount)
                    .HasColumnName("SendCount")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发送次数");

                entity.Property(e => e.SendMessage)
                    .HasColumnName("SendMessage")
                    .HasComment("发送结果");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("添加时间");

            });

            modelBuilder.Entity<SMSSended>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.RecvMobile)
                    .HasColumnName("RecvMobile")
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasComment("接收人手机号码");

                entity.Property(e => e.Sender)
                    .HasColumnName("Sender")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("发送人");

                entity.Property(e => e.MessageTitle)
                    .HasColumnName("MessageTitle")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("短信标题");

                entity.Property(e => e.MessageContent)
                    .HasColumnName("MessageContent")
                    .IsRequired()
                    .HasComment("短信内容");

                entity.Property(e => e.SendUserid)
                    .HasColumnName("SendUserid")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发件人ID，0表示系统");

                entity.Property(e => e.SendUserIP)
                    .HasColumnName("SendUserIP")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("发件人IP地址");

                entity.Property(e => e.SendState)
                    .HasColumnName("SendState")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("发送状态(0:未发送,1:已发送,2:发送失败)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CreateTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("添加时间");

                entity.Property(e => e.SendTime)
                    .HasColumnName("SendTime")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("短信发送时间");

            });

            modelBuilder.Entity<WebApiAccount>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.Appid)
                    .HasColumnName("Appid")
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("账号ID");

                entity.Property(e => e.Token)
                    .HasColumnName("Token")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("密钥");

                entity.Property(e => e.CompanyName)
                    .HasColumnName("CompanyName")
                    .HasMaxLength(100)
                    .HasComment("公司名称");

                entity.Property(e => e.Ips)
                    .HasColumnName("Ips")
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasComment("ip限制");

                entity.Property(e => e.Remark)
                    .HasColumnName("Remark")
                    .HasMaxLength(500)
                    .HasComment("备注");

                entity.Property(e => e.IsUsed)
                    .HasColumnName("IsUsed")
                    .IsRequired()
                    .HasComment("是否使用(0无效1有效)");

            });

            modelBuilder.Entity<ProductLineSize>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.LineID)
                    .HasColumnName("LineID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("ProductLine表主键ID");

                entity.Property(e => e.SizeID)
                    .HasColumnName("SizeID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.SizeDescription)
                    .HasColumnName("SizeDescription")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasComment("名称");

                entity.Property(e => e.Length)
                    .HasColumnName("Length")
                    .IsRequired()
                    .HasColumnType("decimal(18, 3)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Width)
                    .HasColumnName("Width")
                    .IsRequired()
                    .HasColumnType("decimal(18, 3)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Height)
                    .HasColumnName("Height")
                    .IsRequired()
                    .HasColumnType("decimal(18, 3)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Volume)
                    .HasColumnName("Volume")
                    .IsRequired()
                    .HasColumnType("decimal(18, 3)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.SizeText)
                    .HasColumnName("SizeText")
                    .HasMaxLength(500)
                    .HasComment("描述");

                entity.Property(e => e.AddDate)
                    .HasColumnName("AddDate")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.EditDate)
                    .HasColumnName("EditDate")
                    .HasColumnType("datetime")
                    .HasComment("编辑时间");

            });

            modelBuilder.Entity<WebApiRoles>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.AccountID)
                    .HasColumnName("AccountID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("API用户账号ID");

                entity.Property(e => e.InterfaceID)
                    .HasColumnName("InterfaceID")
                    .IsRequired()
                    .HasDefaultValueSql("((0))")
                    .HasComment("功能接口ID");

            });

            modelBuilder.Entity<ProductLineColor>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.LineID)
                    .HasColumnName("LineID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("ProductLine表主键ID");

                entity.Property(e => e.ColorID)
                    .HasColumnName("ColorID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.ColorDescription)
                    .HasColumnName("ColorDescription")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasComment("名称");

                entity.Property(e => e.ColorText)
                    .HasColumnName("ColorText")
                    .HasMaxLength(500)
                    .HasComment("描述");

                entity.Property(e => e.AddDate)
                    .HasColumnName("AddDate")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.EditDate)
                    .HasColumnName("EditDate")
                    .HasColumnType("datetime")
                    .HasComment("编辑时间");

            });

            modelBuilder.Entity<ProductLine>(entity =>
            {
                entity.HasKey(e => e.LineID);

                entity.Property(e => e.LineID)
                    .HasColumnName("LineID")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.LineDescription)
                    .HasColumnName("LineDescription")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasComment("名称");

                entity.Property(e => e.LineText)
                    .HasColumnName("LineText")
                    .HasMaxLength(500)
                    .HasComment("描述");

                entity.Property(e => e.AddDate)
                    .HasColumnName("AddDate")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.EditDate)
                    .HasColumnName("EditDate")
                    .HasColumnType("datetime")
                    .HasComment("编辑时间");

            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .IsRequired();

                entity.Property(e => e.SKU)
                    .HasColumnName("SKU")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MaterialId)
                    .HasColumnName("MaterialId")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Gridval)
                    .HasColumnName("Gridval")
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.MaterialDescription)
                    .HasColumnName("MaterialDescription")
                    .HasMaxLength(256);

                entity.Property(e => e.ColorDescription)
                    .HasColumnName("ColorDescription")
                    .HasMaxLength(256);

                entity.Property(e => e.MaterialGroup)
                    .HasColumnName("MaterialGroup")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Collection)
                    .HasColumnName("Collection")
                    .HasMaxLength(256);

                entity.Property(e => e.ConstructionType)
                    .HasColumnName("ConstructionType")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.AddDate)
                    .HasColumnName("AddDate")
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.EditDate)
                    .HasColumnName("EditDate")
                    .HasColumnType("datetime")
                    .HasComment("编辑时间");

            });

            modelBuilder.Entity<View_SparePart>().HasNoKey();

            modelBuilder.Entity<View_ProductSparePart>().HasNoKey();

            modelBuilder.Entity<View_SysFunction>().HasNoKey();

            modelBuilder.Entity<View_LanguagePack>().HasNoKey();

        }
    }
}
