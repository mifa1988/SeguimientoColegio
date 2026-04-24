using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SeguimientoColegio.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "correos_origen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Asunto = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remitente = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCorreo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Resumen = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenciaUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MessageId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_correos_origen", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "disciplinas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disciplinas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "estados_seguimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    ColorCss = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados_seguimiento", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ninos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombres = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apellidos = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Alias = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ninos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "plataformas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plataformas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "prioridades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    ColorCss = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prioridades", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tipos_registro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_registro", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombres = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apellidos = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Correo = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_RolId",
                        column: x => x.RolId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "registros_escolares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NinoId = table.Column<int>(type: "int", nullable: false),
                    DisciplinaId = table.Column<int>(type: "int", nullable: true),
                    TipoRegistroId = table.Column<int>(type: "int", nullable: false),
                    PlataformaId = table.Column<int>(type: "int", nullable: true),
                    CorreoOrigenId = table.Column<int>(type: "int", nullable: true),
                    EstadoSeguimientoId = table.Column<int>(type: "int", nullable: false),
                    PrioridadId = table.Column<int>(type: "int", nullable: true),
                    Titulo = table.Column<string>(type: "varchar(220)", maxLength: 220, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsablePrincipal = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRecibido = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaVencimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaRealizado = table.Column<DateOnly>(type: "date", nullable: true),
                    DiasAlerta = table.Column<int>(type: "int", nullable: true),
                    RequiereSeguimiento = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ComentarioGeneral = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreadoPorUsuarioId = table.Column<int>(type: "int", nullable: false),
                    ActualizadoPorUsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_escolares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registros_escolares_correos_origen_CorreoOrigenId",
                        column: x => x.CorreoOrigenId,
                        principalTable: "correos_origen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_disciplinas_DisciplinaId",
                        column: x => x.DisciplinaId,
                        principalTable: "disciplinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_estados_seguimiento_EstadoSeguimientoId",
                        column: x => x.EstadoSeguimientoId,
                        principalTable: "estados_seguimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_ninos_NinoId",
                        column: x => x.NinoId,
                        principalTable: "ninos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_plataformas_PlataformaId",
                        column: x => x.PlataformaId,
                        principalTable: "plataformas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_prioridades_PrioridadId",
                        column: x => x.PrioridadId,
                        principalTable: "prioridades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_tipos_registro_TipoRegistroId",
                        column: x => x.TipoRegistroId,
                        principalTable: "tipos_registro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_usuarios_ActualizadoPorUsuarioId",
                        column: x => x.ActualizadoPorUsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_escolares_usuarios_CreadoPorUsuarioId",
                        column: x => x.CreadoPorUsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "evidencias_registro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RegistroEscolarId = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "varchar(260)", maxLength: 260, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StorageKey = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoMime = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubidoPorUsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidencias_registro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_evidencias_registro_registros_escolares_RegistroEscolarId",
                        column: x => x.RegistroEscolarId,
                        principalTable: "registros_escolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_evidencias_registro_usuarios_SubidoPorUsuarioId",
                        column: x => x.SubidoPorUsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "registro_auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RegistroEscolarId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CampoModificado = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorAnterior = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorNuevo = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comentario = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_auditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registro_auditoria_registros_escolares_RegistroEscolarId",
                        column: x => x.RegistroEscolarId,
                        principalTable: "registros_escolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registro_auditoria_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "registro_urls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RegistroEscolarId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_urls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registro_urls_registros_escolares_RegistroEscolarId",
                        column: x => x.RegistroEscolarId,
                        principalTable: "registros_escolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "disciplinas",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaActualizacion", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Seguimiento de ejercicios y refuerzos.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Matematicas" },
                    { 2, true, "Actividades y materiales de danza.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Danza" },
                    { 3, true, "Practicas y materiales de lectoescritura.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Lectoescritura" },
                    { 4, true, "Mensajes y actividades de comunicacion.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Comunicacion" },
                    { 5, true, "Lecturas y avances.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Plan lector" },
                    { 6, true, "Actividades artisticas.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Arte" }
                });

            migrationBuilder.InsertData(
                table: "estados_seguimiento",
                columns: new[] { "Id", "Activo", "ColorCss", "Descripcion", "FechaActualizacion", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, "#f0b33f", "Registro pendiente de atencion.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Pendiente", 1 },
                    { 2, true, "#2f80ed", "Registro con avance parcial.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "En proceso", 2 },
                    { 3, true, "#2f9e44", "Registro completado.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Concluido", 3 },
                    { 4, true, "#7a828f", "Registro descartado o sin accion.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Descartado", 4 },
                    { 5, true, "#d64545", "Registro vencido.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Vencido", 5 }
                });

            migrationBuilder.InsertData(
                table: "ninos",
                columns: new[] { "Id", "Activo", "Alias", "Apellidos", "FechaActualizacion", "FechaCreacion", "FechaNacimiento", "Nombres" },
                values: new object[] { 1, true, "Dani", "Pendiente", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Daniela" });

            migrationBuilder.InsertData(
                table: "plataformas",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaActualizacion", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Actividades interactivas.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Wordwall" },
                    { 2, true, "Lecturas y biblioteca digital.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Odilo" },
                    { 3, true, "Materiales visuales.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Canva" },
                    { 4, true, "Documento PDF.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "PDF" },
                    { 5, true, "Videos educativos.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "YouTube" },
                    { 6, true, "Archivos compartidos.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Google Drive" },
                    { 7, true, "Otra plataforma o fuente.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Otro" }
                });

            migrationBuilder.InsertData(
                table: "prioridades",
                columns: new[] { "Id", "Activo", "ColorCss", "Descripcion", "FechaActualizacion", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, "#7f8c8d", "Atencion baja.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Baja", 1 },
                    { 2, true, "#3d7df0", "Atencion media.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Media", 2 },
                    { 3, true, "#ef8b2c", "Atencion alta.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Alta", 3 },
                    { 4, true, "#d64545", "Atencion critica.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Critica", 4 }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Gestiona usuarios, catalogos y tiene acceso total.", "Administrador" },
                    { 2, "Puede crear y actualizar registros y seguimientos.", "Editor" },
                    { 3, "Solo puede revisar la informacion disponible.", "Consulta" }
                });

            migrationBuilder.InsertData(
                table: "tipos_registro",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaActualizacion", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Material para reforzar aprendizaje.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Material de refuerzo" },
                    { 2, true, "Material o accion requerida por el colegio.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Requerimiento" },
                    { 3, true, "Lecturas y plan lector.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Lectura" },
                    { 4, true, "Tareas con seguimiento.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Tarea" },
                    { 5, true, "Aviso relevante para control.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Aviso importante" },
                    { 6, true, "Acuerdos de reuniones presenciales o virtuales.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Acuerdo con profesora" },
                    { 7, true, "Actividad escolar.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Actividad" },
                    { 8, true, "Evaluaciones y pruebas.", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Evaluacion" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_correos_origen_FechaCorreo",
                table: "correos_origen",
                column: "FechaCorreo");

            migrationBuilder.CreateIndex(
                name: "IX_correos_origen_MessageId",
                table: "correos_origen",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_correos_origen_Remitente",
                table: "correos_origen",
                column: "Remitente");

            migrationBuilder.CreateIndex(
                name: "IX_disciplinas_Activo",
                table: "disciplinas",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_disciplinas_Nombre",
                table: "disciplinas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estados_seguimiento_Activo_Orden",
                table: "estados_seguimiento",
                columns: new[] { "Activo", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_estados_seguimiento_Nombre",
                table: "estados_seguimiento",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_registro_RegistroEscolarId",
                table: "evidencias_registro",
                column: "RegistroEscolarId");

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_registro_SubidoPorUsuarioId",
                table: "evidencias_registro",
                column: "SubidoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ninos_Activo_Nombres_Apellidos",
                table: "ninos",
                columns: new[] { "Activo", "Nombres", "Apellidos" });

            migrationBuilder.CreateIndex(
                name: "IX_plataformas_Activo",
                table: "plataformas",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_plataformas_Nombre",
                table: "plataformas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prioridades_Activo_Orden",
                table: "prioridades",
                columns: new[] { "Activo", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_prioridades_Nombre",
                table: "prioridades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_registro_auditoria_RegistroEscolarId_FechaCreacion",
                table: "registro_auditoria",
                columns: new[] { "RegistroEscolarId", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_registro_auditoria_UsuarioId",
                table: "registro_auditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_registro_urls_RegistroEscolarId_Orden",
                table: "registro_urls",
                columns: new[] { "RegistroEscolarId", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_Activo_NinoId_EstadoSeguimientoId",
                table: "registros_escolares",
                columns: new[] { "Activo", "NinoId", "EstadoSeguimientoId" });

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_ActualizadoPorUsuarioId",
                table: "registros_escolares",
                column: "ActualizadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_CorreoOrigenId",
                table: "registros_escolares",
                column: "CorreoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_CreadoPorUsuarioId",
                table: "registros_escolares",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_DisciplinaId",
                table: "registros_escolares",
                column: "DisciplinaId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_EstadoSeguimientoId",
                table: "registros_escolares",
                column: "EstadoSeguimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_FechaRecibido_FechaVencimiento_FechaReal~",
                table: "registros_escolares",
                columns: new[] { "FechaRecibido", "FechaVencimiento", "FechaRealizado" });

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_NinoId_DisciplinaId_TipoRegistroId",
                table: "registros_escolares",
                columns: new[] { "NinoId", "DisciplinaId", "TipoRegistroId" });

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_PlataformaId",
                table: "registros_escolares",
                column: "PlataformaId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_PrioridadId_FechaActualizacion",
                table: "registros_escolares",
                columns: new[] { "PrioridadId", "FechaActualizacion" });

            migrationBuilder.CreateIndex(
                name: "IX_registros_escolares_TipoRegistroId",
                table: "registros_escolares",
                column: "TipoRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Nombre",
                table: "roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tipos_registro_Activo",
                table: "tipos_registro",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_registro_Nombre",
                table: "tipos_registro",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Activo_RolId",
                table: "usuarios",
                columns: new[] { "Activo", "RolId" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Correo",
                table: "usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_RolId",
                table: "usuarios",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evidencias_registro");

            migrationBuilder.DropTable(
                name: "registro_auditoria");

            migrationBuilder.DropTable(
                name: "registro_urls");

            migrationBuilder.DropTable(
                name: "registros_escolares");

            migrationBuilder.DropTable(
                name: "correos_origen");

            migrationBuilder.DropTable(
                name: "disciplinas");

            migrationBuilder.DropTable(
                name: "estados_seguimiento");

            migrationBuilder.DropTable(
                name: "ninos");

            migrationBuilder.DropTable(
                name: "plataformas");

            migrationBuilder.DropTable(
                name: "prioridades");

            migrationBuilder.DropTable(
                name: "tipos_registro");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
