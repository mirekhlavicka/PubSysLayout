using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PubSysLayout.Shared.Model
{
    public partial class LayoutDBContext : DbContext
    {
        public LayoutDBContext()
        {
        }

        public LayoutDBContext(DbContextOptions<LayoutDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dtproperty> Dtproperties { get; set; }
        public virtual DbSet<Layout> Layouts { get; set; }
        public virtual DbSet<LayoutAssign> LayoutAssigns { get; set; }
        public virtual DbSet<LayoutDefinition> LayoutDefinitions { get; set; }
        public virtual DbSet<LayoutSpot> LayoutSpots { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<ModuleLocal> ModuleLocals { get; set; }
        public virtual DbSet<ModuleSetting> ModuleSettings { get; set; }
        public virtual DbSet<ModuleSettingsServer> ModuleSettingsServers { get; set; }
        public virtual DbSet<ModuleUsage> ModuleUsages { get; set; }
        public virtual DbSet<ModuleUsageParam> ModuleUsageParams { get; set; }
        public virtual DbSet<Qslayout> Qslayouts { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<SectionStyle> SectionStyles { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Spot> Spots { get; set; }
        public virtual DbSet<Style> Styles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=sqlcluster1\\sqlcnc1,1433;uid=usr_zivecz;pwd=;database=ZiveCZ_Layout_1");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dtproperty>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Property })
                    .HasName("pk_dtproperties");

                entity.ToTable("dtproperties");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Property)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasColumnName("property");

                entity.Property(e => e.Lvalue)
                    .HasColumnType("image")
                    .HasColumnName("lvalue");

                entity.Property(e => e.Objectid).HasColumnName("objectid");

                entity.Property(e => e.Uvalue)
                    .HasMaxLength(255)
                    .HasColumnName("uvalue");

                entity.Property(e => e.Value)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("value");

                entity.Property(e => e.Version).HasColumnName("version");
            });

            modelBuilder.Entity<Layout>(entity =>
            {
                entity.HasKey(e => e.IdLayout);

                entity.Property(e => e.IdLayout).HasColumnName("id_layout");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.DesktopSrc)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.MobileSrc)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<LayoutAssign>(entity =>
            {
                entity.HasKey(e => new { e.IdServer, e.IdSection, e.IdQslayout });

                entity.ToTable("LayoutAssign");

                entity.Property(e => e.IdServer).HasColumnName("id_server");

                entity.Property(e => e.IdSection).HasColumnName("id_section");

                entity.Property(e => e.IdQslayout).HasColumnName("id_qslayout");

                entity.Property(e => e.IdLayoutdefinition).HasColumnName("id_layoutdefinition");

                entity.Property(e => e.RefererRequired).HasColumnName("referer_required");

                entity.HasOne(d => d.IdLayoutdefinitionNavigation)
                    .WithMany(p => p.LayoutAssigns)
                    .HasForeignKey(d => d.IdLayoutdefinition)
                    .HasConstraintName("FK_LayoutAssign_LayoutDefinitions");

                entity.HasOne(d => d.IdQslayoutNavigation)
                    .WithMany(p => p.LayoutAssigns)
                    .HasForeignKey(d => d.IdQslayout)
                    .HasConstraintName("FK_LayoutAssign_QSLayout");
            });

            modelBuilder.Entity<LayoutDefinition>(entity =>
            {
                entity.HasKey(e => e.IdLayoutdefinition);

                entity.Property(e => e.IdLayoutdefinition).HasColumnName("id_layoutdefinition");

                entity.Property(e => e.IdLayout).HasColumnName("id_layout");

                entity.Property(e => e.IdStyle).HasColumnName("id_style");

                entity.Property(e => e.Mainstyle).HasColumnName("mainstyle");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdLayoutNavigation)
                    .WithMany(p => p.LayoutDefinitions)
                    .HasForeignKey(d => d.IdLayout)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LayoutDefinitions_Layouts");

                entity.HasOne(d => d.IdStyleNavigation)
                    .WithMany(p => p.LayoutDefinitions)
                    .HasForeignKey(d => d.IdStyle)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LayoutDefinitions_Styles");
            });

            modelBuilder.Entity<LayoutSpot>(entity =>
            {
                entity.HasKey(e => new { e.IdSpot, e.IdLayout });

                entity.Property(e => e.IdSpot).HasColumnName("id_spot");

                entity.Property(e => e.IdLayout).HasColumnName("id_layout");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.IdModule);

                entity.Property(e => e.IdModule).HasColumnName("id_module");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Admin).HasColumnName("admin");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("description")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DesktopSrc)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.MobileSrc)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Qskey)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("QSkey")
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<ModuleLocal>(entity =>
            {
                entity.HasKey(e => new { e.IdModule, e.IdLanguage });

                entity.Property(e => e.IdModule).HasColumnName("id_module");

                entity.Property(e => e.IdLanguage).HasColumnName("id_language");

                entity.Property(e => e.DesktopSrc)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.MobileSrc)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.IdModuleNavigation)
                    .WithMany(p => p.ModuleLocals)
                    .HasForeignKey(d => d.IdModule)
                    .HasConstraintName("FK_ModuleLocals_Modules");
            });

            modelBuilder.Entity<ModuleSetting>(entity =>
            {
                entity.HasKey(e => e.IdSetting);

                entity.HasIndex(e => new { e.IdModule, e.SettingName }, "IX_ModuleSettings");

                entity.Property(e => e.IdSetting).HasColumnName("id_setting");

                entity.Property(e => e.IdModule).HasColumnName("id_module");

                entity.Property(e => e.IdModuleusage).HasColumnName("id_moduleusage");

                entity.Property(e => e.IdServer).HasColumnName("id_server");

                entity.Property(e => e.SettingName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SettingValue)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.IdModuleNavigation)
                    .WithMany(p => p.ModuleSettings)
                    .HasForeignKey(d => d.IdModule)
                    .HasConstraintName("FK_ModuleSettings_Modules");
            });

            modelBuilder.Entity<ModuleSettingsServer>(entity =>
            {
                entity.HasKey(e => e.IdServer);

                entity.ToTable("ModuleSettingsServer");

                entity.Property(e => e.IdServer)
                    .ValueGeneratedNever()
                    .HasColumnName("id_server");

                entity.Property(e => e.SettingValue)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SettingValueProxy)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<ModuleUsage>(entity =>
            {
                entity.HasKey(e => e.IdModuleusage);

                entity.ToTable("ModuleUsage");

                entity.Property(e => e.IdModuleusage).HasColumnName("id_moduleusage");

                entity.Property(e => e.IdLayoutdefinition).HasColumnName("id_layoutdefinition");

                entity.Property(e => e.IdModule).HasColumnName("id_module");

                entity.Property(e => e.IdSpot).HasColumnName("id_spot");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.HasOne(d => d.IdLayoutdefinitionNavigation)
                    .WithMany(p => p.ModuleUsages)
                    .HasForeignKey(d => d.IdLayoutdefinition)
                    .HasConstraintName("FK_ModuleUsage_LayoutDefinitions");

                entity.HasOne(d => d.IdModuleNavigation)
                    .WithMany(p => p.ModuleUsages)
                    .HasForeignKey(d => d.IdModule)
                    .HasConstraintName("FK_ModuleUsage_Modules");

                entity.HasOne(d => d.IdSpotNavigation)
                    .WithMany(p => p.ModuleUsages)
                    .HasForeignKey(d => d.IdSpot)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleUsage_Spots");
            });

            modelBuilder.Entity<ModuleUsageParam>(entity =>
            {
                entity.HasKey(e => new { e.IdModuleusage, e.Param });

                entity.Property(e => e.IdModuleusage).HasColumnName("id_moduleusage");

                entity.Property(e => e.Param)
                    .HasMaxLength(50)
                    .HasColumnName("param");

                entity.Property(e => e.Pvalue).HasColumnName("pvalue");

                entity.HasOne(d => d.IdModuleusageNavigation)
                    .WithMany(p => p.ModuleUsageParams)
                    .HasForeignKey(d => d.IdModuleusage)
                    .HasConstraintName("FK_ModuleUsageParams_ModuleUsage");
            });

            modelBuilder.Entity<Qslayout>(entity =>
            {
                entity.HasKey(e => e.IdQslayout);

                entity.ToTable("QSLayout");

                entity.Property(e => e.IdQslayout).HasColumnName("id_qslayout");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("description")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.ParamString)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("param_string");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Public).HasColumnName("public");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Sections");

                entity.Property(e => e.Del).HasColumnName("del");

                entity.Property(e => e.IdFile).HasColumnName("id_file");

                entity.Property(e => e.IdMetasection).HasColumnName("id_metasection");

                entity.Property(e => e.IdSection)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id_section");

                entity.Property(e => e.IdSectionParent).HasColumnName("id_section_parent");

                entity.Property(e => e.IdSectionParentTop).HasColumnName("id_section_parent_top");

                entity.Property(e => e.IdServer).HasColumnName("id_server");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("name");

                entity.Property(e => e.Options).HasColumnName("options");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.Redirurl)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("redirurl");

                entity.Property(e => e.Tag).HasColumnName("tag");

                entity.Property(e => e.Target).HasColumnName("target");

                entity.Property(e => e.Treelevel).HasColumnName("treelevel");

                entity.Property(e => e.Visible).HasColumnName("visible");
            });

            modelBuilder.Entity<SectionStyle>(entity =>
            {
                entity.HasKey(e => new { e.IdSection, e.IdStyle });

                entity.ToTable("SectionStyle");

                entity.Property(e => e.IdSection).HasColumnName("id_section");

                entity.Property(e => e.IdStyle).HasColumnName("id_style");

                entity.Property(e => e.Propagate).HasColumnName("propagate");
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Servers");

                entity.Property(e => e.Del).HasColumnName("del");

                entity.Property(e => e.IdLanguage).HasColumnName("id_language");

                entity.Property(e => e.IdSectionDefault).HasColumnName("id_section_default");

                entity.Property(e => e.IdServer)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id_server");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Spot>(entity =>
            {
                entity.HasKey(e => e.IdSpot);

                entity.Property(e => e.IdSpot).HasColumnName("id_spot");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Style>(entity =>
            {
                entity.HasKey(e => e.IdStyle);

                entity.Property(e => e.IdStyle).HasColumnName("id_style");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Skin).HasColumnName("skin");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
