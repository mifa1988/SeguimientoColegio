using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data.Entities;

namespace SeguimientoColegio.Web.Data;

public sealed class SeguimientoDbContext(DbContextOptions<SeguimientoDbContext> options) : DbContext(options)
{
    private static readonly DateTime SeedTimestamp = new(2026, 4, 22, 0, 0, 0, DateTimeKind.Utc);

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Nino> Ninos => Set<Nino>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<Plataforma> Plataformas => Set<Plataforma>();
    public DbSet<TipoRegistro> TiposRegistro => Set<TipoRegistro>();
    public DbSet<EstadoSeguimiento> EstadosSeguimiento => Set<EstadoSeguimiento>();
    public DbSet<Prioridad> Prioridades => Set<Prioridad>();
    public DbSet<CorreoOrigen> CorreosOrigen => Set<CorreoOrigen>();
    public DbSet<RegistroEscolar> RegistrosEscolares => Set<RegistroEscolar>();
    public DbSet<RegistroUrl> RegistroUrls => Set<RegistroUrl>();
    public DbSet<EvidenciaRegistro> EvidenciasRegistro => Set<EvidenciaRegistro>();
    public DbSet<RegistroAuditoria> RegistroAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureRol(modelBuilder);
        ConfigureUsuario(modelBuilder);
        ConfigureNino(modelBuilder);
        ConfigureCatalogos(modelBuilder);
        ConfigureCorreoOrigen(modelBuilder);
        ConfigureRegistroEscolar(modelBuilder);
        ConfigureRegistroUrl(modelBuilder);
        ConfigureEvidencia(modelBuilder);
        ConfigureAuditoria(modelBuilder);
        SeedCatalogData(modelBuilder);
    }

    private static void ConfigureRol(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(250).IsRequired();
            entity.HasIndex(x => x.Nombre).IsUnique();
        });
    }

    private static void ConfigureUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombres).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Apellidos).HasMaxLength(150);
            entity.Property(x => x.Correo).HasMaxLength(180).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            entity.HasIndex(x => x.Correo).IsUnique();
            entity.HasIndex(x => new { x.Activo, x.RolId });
            entity.HasOne(x => x.Rol)
                .WithMany(x => x.Usuarios)
                .HasForeignKey(x => x.RolId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureNino(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nino>(entity =>
        {
            entity.ToTable("ninos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombres).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Apellidos).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Alias).HasMaxLength(120);
            entity.HasIndex(x => new { x.Activo, x.Nombres, x.Apellidos });
        });
    }

    private static void ConfigureCatalogos(ModelBuilder modelBuilder)
    {
        ConfigureCatalogoBase<Disciplina>(modelBuilder, "disciplinas");
        ConfigureCatalogoBase<Plataforma>(modelBuilder, "plataformas");
        ConfigureCatalogoBase<TipoRegistro>(modelBuilder, "tipos_registro");
        ConfigureCatalogoOrdenable<EstadoSeguimiento>(modelBuilder, "estados_seguimiento");
        ConfigureCatalogoOrdenable<Prioridad>(modelBuilder, "prioridades");
    }

    private static void ConfigureCorreoOrigen(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CorreoOrigen>(entity =>
        {
            entity.ToTable("correos_origen");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Asunto).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Remitente).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Resumen).HasMaxLength(1000);
            entity.Property(x => x.ReferenciaUrl).HasMaxLength(500);
            entity.Property(x => x.MessageId).HasMaxLength(255);
            entity.Property(x => x.Observaciones).HasMaxLength(1000);
            entity.HasIndex(x => x.FechaCorreo);
            entity.HasIndex(x => x.Remitente);
            entity.HasIndex(x => x.MessageId).IsUnique();
        });
    }

    private static void ConfigureRegistroEscolar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegistroEscolar>(entity =>
        {
            entity.ToTable("registros_escolares");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Titulo).HasMaxLength(220).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(4000);
            entity.Property(x => x.ResponsablePrincipal).HasMaxLength(180);
            entity.Property(x => x.ComentarioGeneral).HasMaxLength(4000);
            entity.HasIndex(x => new { x.Activo, x.NinoId, x.EstadoSeguimientoId });
            entity.HasIndex(x => new { x.NinoId, x.DisciplinaId, x.TipoRegistroId });
            entity.HasIndex(x => new { x.FechaRecibido, x.FechaVencimiento, x.FechaRealizado });
            entity.HasIndex(x => new { x.PrioridadId, x.FechaActualizacion });
            entity.HasOne(x => x.Nino)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.NinoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Disciplina)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.DisciplinaId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.TipoRegistro)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.TipoRegistroId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Plataforma)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.PlataformaId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CorreoOrigen)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.CorreoOrigenId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.EstadoSeguimiento)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.EstadoSeguimientoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Prioridad)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.PrioridadId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CreadoPorUsuario)
                .WithMany(x => x.RegistrosCreados)
                .HasForeignKey(x => x.CreadoPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ActualizadoPorUsuario)
                .WithMany(x => x.RegistrosActualizados)
                .HasForeignKey(x => x.ActualizadoPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRegistroUrl(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegistroUrl>(entity =>
        {
            entity.ToTable("registro_urls");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(250);
            entity.HasIndex(x => new { x.RegistroEscolarId, x.Orden });
            entity.HasOne(x => x.RegistroEscolar)
                .WithMany(x => x.Urls)
                .HasForeignKey(x => x.RegistroEscolarId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureEvidencia(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EvidenciaRegistro>(entity =>
        {
            entity.ToTable("evidencias_registro");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.NombreArchivo).HasMaxLength(260).IsRequired();
            entity.Property(x => x.StorageKey).HasMaxLength(500).IsRequired();
            entity.Property(x => x.TipoMime).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(400);
            entity.HasIndex(x => x.RegistroEscolarId);
            entity.HasOne(x => x.RegistroEscolar)
                .WithMany(x => x.Evidencias)
                .HasForeignKey(x => x.RegistroEscolarId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.SubidoPorUsuario)
                .WithMany(x => x.EvidenciasSubidas)
                .HasForeignKey(x => x.SubidoPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureAuditoria(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegistroAuditoria>(entity =>
        {
            entity.ToTable("registro_auditoria");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Accion).HasMaxLength(120).IsRequired();
            entity.Property(x => x.CampoModificado).HasMaxLength(120);
            entity.Property(x => x.ValorAnterior).HasMaxLength(4000);
            entity.Property(x => x.ValorNuevo).HasMaxLength(4000);
            entity.Property(x => x.Comentario).HasMaxLength(1000);
            entity.HasIndex(x => new { x.RegistroEscolarId, x.FechaCreacion });
            entity.HasOne(x => x.RegistroEscolar)
                .WithMany(x => x.Auditoria)
                .HasForeignKey(x => x.RegistroEscolarId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.Auditorias)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCatalogoBase<TEntity>(ModelBuilder modelBuilder, string tableName)
        where TEntity : CatalogoBaseEntity
    {
        modelBuilder.Entity<TEntity>(entity =>
        {
            entity.ToTable(tableName);
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(500);
            entity.HasIndex(x => x.Nombre).IsUnique();
            entity.HasIndex(x => x.Activo);
        });
    }

    private static void ConfigureCatalogoOrdenable<TEntity>(ModelBuilder modelBuilder, string tableName)
        where TEntity : CatalogoConOrdenEntity
    {
        modelBuilder.Entity<TEntity>(entity =>
        {
            entity.ToTable(tableName);
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(500);
            entity.Property(x => x.ColorCss).HasMaxLength(30);
            entity.HasIndex(x => x.Nombre).IsUnique();
            entity.HasIndex(x => new { x.Activo, x.Orden });
        });
    }

    private static void SeedCatalogData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>().HasData(
            new Rol
            {
                Id = SeedDataIds.Roles.Administrador,
                Nombre = "Administrador",
                Descripcion = "Gestiona usuarios, catalogos y tiene acceso total."
            },
            new Rol
            {
                Id = SeedDataIds.Roles.Editor,
                Nombre = "Editor",
                Descripcion = "Puede crear y actualizar registros y seguimientos."
            },
            new Rol
            {
                Id = SeedDataIds.Roles.Consulta,
                Nombre = "Consulta",
                Descripcion = "Solo puede revisar la informacion disponible."
            });

        modelBuilder.Entity<EstadoSeguimiento>().HasData(
            BuildEstado(SeedDataIds.Estados.Pendiente, "Pendiente", "Registro pendiente de atencion.", "#f0b33f", 1),
            BuildEstado(SeedDataIds.Estados.EnProceso, "En proceso", "Registro con avance parcial.", "#2f80ed", 2),
            BuildEstado(SeedDataIds.Estados.Concluido, "Concluido", "Registro completado.", "#2f9e44", 3),
            BuildEstado(SeedDataIds.Estados.Descartado, "Descartado", "Registro descartado o sin accion.", "#7a828f", 4),
            BuildEstado(SeedDataIds.Estados.Vencido, "Vencido", "Registro vencido.", "#d64545", 5));

        modelBuilder.Entity<Prioridad>().HasData(
            BuildPrioridad(SeedDataIds.Prioridades.Baja, "Baja", "Atencion baja.", "#7f8c8d", 1),
            BuildPrioridad(SeedDataIds.Prioridades.Media, "Media", "Atencion media.", "#3d7df0", 2),
            BuildPrioridad(SeedDataIds.Prioridades.Alta, "Alta", "Atencion alta.", "#ef8b2c", 3),
            BuildPrioridad(SeedDataIds.Prioridades.Critica, "Critica", "Atencion critica.", "#d64545", 4));

        modelBuilder.Entity<Plataforma>().HasData(
            BuildCatalogo<Plataforma>(SeedDataIds.Plataformas.Wordwall, "Wordwall", "Actividades interactivas."),
            BuildCatalogo<Plataforma>(SeedDataIds.Plataformas.Odilo, "Odilo", "Lecturas y biblioteca digital."),
            BuildCatalogo<Plataforma>(SeedDataIds.Plataformas.Canva, "Canva", "Materiales visuales."),
            BuildCatalogo<Plataforma>(SeedDataIds.Plataformas.Pdf, "PDF", "Documento PDF."),
            BuildCatalogo<Plataforma>(SeedDataIds.Plataformas.Youtube, "YouTube", "Videos educativos."),
            BuildCatalogo<Plataforma>(SeedDataIds.Plataformas.GoogleDrive, "Google Drive", "Archivos compartidos."),
            BuildCatalogo<Plataforma>(SeedDataIds.Plataformas.Otro, "Otro", "Otra plataforma o fuente."));

        modelBuilder.Entity<Disciplina>().HasData(
            BuildCatalogo<Disciplina>(SeedDataIds.Disciplinas.Matematicas, "Matematicas", "Seguimiento de ejercicios y refuerzos."),
            BuildCatalogo<Disciplina>(SeedDataIds.Disciplinas.Danza, "Danza", "Actividades y materiales de danza."),
            BuildCatalogo<Disciplina>(SeedDataIds.Disciplinas.Lectoescritura, "Lectoescritura", "Practicas y materiales de lectoescritura."),
            BuildCatalogo<Disciplina>(SeedDataIds.Disciplinas.Comunicacion, "Comunicacion", "Mensajes y actividades de comunicacion."),
            BuildCatalogo<Disciplina>(SeedDataIds.Disciplinas.PlanLector, "Plan lector", "Lecturas y avances."),
            BuildCatalogo<Disciplina>(SeedDataIds.Disciplinas.Arte, "Arte", "Actividades artisticas."));

        modelBuilder.Entity<TipoRegistro>().HasData(
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.MaterialRefuerzo, "Material de refuerzo", "Material para reforzar aprendizaje."),
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.Requerimiento, "Requerimiento", "Material o accion requerida por el colegio."),
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.Lectura, "Lectura", "Lecturas y plan lector."),
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.Tarea, "Tarea", "Tareas con seguimiento."),
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.AvisoImportante, "Aviso importante", "Aviso relevante para control."),
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.AcuerdoProfesora, "Acuerdo con profesora", "Acuerdos de reuniones presenciales o virtuales."),
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.Actividad, "Actividad", "Actividad escolar."),
            BuildCatalogo<TipoRegistro>(SeedDataIds.TiposRegistro.Evaluacion, "Evaluacion", "Evaluaciones y pruebas."));

        modelBuilder.Entity<Nino>().HasData(
            new Nino
            {
                Id = SeedDataIds.Ninos.Daniela,
                Nombres = "Daniela",
                Apellidos = "Pendiente",
                Alias = "Dani",
                FechaNacimiento = null,
                Activo = true,
                FechaCreacion = SeedTimestamp,
                FechaActualizacion = SeedTimestamp
            });
    }

    private static EstadoSeguimiento BuildEstado(int id, string nombre, string descripcion, string colorCss, int orden)
        => new()
        {
            Id = id,
            Nombre = nombre,
            Descripcion = descripcion,
            ColorCss = colorCss,
            Orden = orden,
            Activo = true,
            FechaCreacion = SeedTimestamp,
            FechaActualizacion = SeedTimestamp
        };

    private static Prioridad BuildPrioridad(int id, string nombre, string descripcion, string colorCss, int orden)
        => new()
        {
            Id = id,
            Nombre = nombre,
            Descripcion = descripcion,
            ColorCss = colorCss,
            Orden = orden,
            Activo = true,
            FechaCreacion = SeedTimestamp,
            FechaActualizacion = SeedTimestamp
        };

    private static TEntity BuildCatalogo<TEntity>(int id, string nombre, string? descripcion)
        where TEntity : CatalogoBaseEntity, new()
        => new()
        {
            Id = id,
            Nombre = nombre,
            Descripcion = descripcion,
            Activo = true,
            FechaCreacion = SeedTimestamp,
            FechaActualizacion = SeedTimestamp
        };
}
