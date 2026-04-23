# SeguimientoColegio

Aplicativo web para seguimiento escolar, control de pendientes, acuerdos con profesoras, evidencias, correos origen, historial y alertas. El sistema parte con Daniela como nino inicial y queda preparado para Bruno u otros ninos.

## Alcance Implementado

- Autenticacion por correo y contrasena con hash seguro.
- Roles Administrador, Editor y Consulta.
- Selector de nino activo para trabajar por contexto o ver todos.
- CRUD de ninos, usuarios y catalogos administrativos.
- Gestion manual de correos origen, preparada para futura integracion Gmail.
- Registros escolares con nino, disciplina, tipo, plataforma opcional, correo opcional, estado, prioridad, fechas, URLs multiples y evidencias.
- Modulo especializado de acuerdos con profesoras usando el mismo modelo de registros, con UI filtrada y disciplina opcional.
- Historial real de cambios relevantes: creacion, cambios de campos, estado, URLs y evidencias.
- Dashboard compacto con metricas, pendientes, ultimos movimientos y alertas criticas.
- Vista Control y alertas con filtros por nino, disciplina, severidad y texto.
- Carga y descarga de adjuntos en almacenamiento local fuera de `wwwroot`.
- Scripts SQL de esquema y semillas para MySQL/MariaDB.

## Referencia Usada

Se reviso `D:\Programacion\Abastecimiento` y se tomo como referencia sana:

- Solucion simple con un proyecto web MVC bajo `src`.
- Separacion clara por `Controllers`, `Data`, `Models`, `Services`, `Views` y `wwwroot`.
- Vistas Razor compactas, layout propio y CSS custom en vez de una UI generica pesada.
- Configuracion externa mediante `appsettings` y archivo local de ejemplo.
- Patrimonios utiles de CRUD: listados compactos, formularios directos, servicios para logica no trivial y acceso a datos ordenado.

La logica de negocio no se copio. Se adapto la estructura al dominio escolar y se mejoraron algunos puntos: roles mas claros, auditoria de registros, alertas dinamicas, bootstrap seguro de administrador y scripts SQL separados.

## Tecnologia

- ASP.NET Core MVC / Razor sobre `net10.0`.
- Entity Framework Core con proveedor `Pomelo.EntityFrameworkCore.MySql`.
- MySQL / MariaDB como motor de base de datos.
- Autenticacion por cookies de ASP.NET Core.
- CSS propio para una UI compacta, sobria y orientada a escritorio, con soporte responsive.

La eleccion MVC/Razor evita complejidad innecesaria de SPA para este MVP y permite entregar CRUDs, formularios, filtros y seguridad de forma mantenible.

## Estructura

```text
SeguimientoColegio/
  database/
    001_schema.sql
    002_seed.sql
  src/
    SeguimientoColegio.Web/
      Configuration/
      Constants/
      Controllers/
      Data/
        Entities/
        Migrations/
      Dtos/
      Models/
      Services/
      Views/
      wwwroot/
```

## Modelo De Datos

Tablas principales:

- `roles`, `usuarios`
- `ninos`
- `disciplinas`, `plataformas`, `tipos_registro`, `estados_seguimiento`, `prioridades`
- `correos_origen`
- `registros_escolares`
- `registro_urls`
- `evidencias_registro`
- `registro_auditoria`

Relaciones clave:

- Un registro escolar siempre pertenece a un nino.
- Un registro escolar siempre tiene tipo y estado.
- La disciplina es requerida para registros escolares normales; en acuerdos con profesora puede ser opcional.
- Plataforma y correo origen son opcionales.
- Un registro puede tener multiples URLs, multiples evidencias y multiples eventos de auditoria.
- Las alertas se calculan dinamicamente desde `AlertaService`; no se persisten para evitar estados duplicados.

## Alertas

La vista Control y el dashboard calculan alertas con reglas configurables:

- Vencido: fecha de vencimiento pasada y registro no cerrado.
- Pendiente antiguo: abierto mas dias que `DiasAlerta` del registro o `Alertas:DiasPendienteAntiguo`.
- Prioridad critica: abierto y marcado como Critica.
- Prioridad alta: abierto, Alta y con antiguedad mayor al umbral.
- En proceso sin movimiento: estado En proceso sin actualizacion reciente.
- Sin movimiento: abierto sin actualizacion reciente.
- Acuerdo pendiente: acuerdo con profesora sin cierre.
- Actividad abierta: tarea, lectura, material, actividad o evaluacion sin concluir.

Configuracion por defecto en `appsettings.json`:

```json
"Alertas": {
  "DiasPendienteAntiguo": 7,
  "DiasSinMovimiento": 5,
  "DiasPrioridadAlta": 3,
  "MaxAlertasDashboard": 8
}
```

## Configuracion Local

El archivo con secretos reales no se versiona. Crea `src/SeguimientoColegio.Web/appsettings.Local.json` desde el ejemplo:

```powershell
Copy-Item src\SeguimientoColegio.Web\appsettings.Local.example.json src\SeguimientoColegio.Web\appsettings.Local.json
```

Edita la cadena de conexion local con los valores reales del entorno:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_HOST;Port=3306;Database=PRACTICAR_CE;User=TU_USUARIO;Password=TU_PASSWORD;SslMode=None;"
  },
  "BootstrapAdmin": {
    "Habilitado": true,
    "Correo": "admin@local.test",
    "Password": "CambiaEstaClave123!",
    "Nombres": "Administrador",
    "Apellidos": "Local"
  },
  "BaseDeDatos": {
    "AplicarMigracionesAlInicio": false
  }
}
```

`appsettings.Local.json` esta ignorado por Git. Despues de crear el primer administrador, cambia la clave o desactiva `BootstrapAdmin:Habilitado`.

## Base De Datos

Scripts disponibles:

- `database/001_schema.sql`: crea tablas, llaves foraneas, indices y registra la migracion inicial de EF.
- `database/002_seed.sql`: inserta roles, estados, prioridades, plataformas, disciplinas, tipos y Daniela.

Ejecutar con cliente MySQL:

```powershell
mysql -h TU_HOST -P 3306 -u TU_USUARIO -p PRACTICAR_CE < database\001_schema.sql
mysql -h TU_HOST -P 3306 -u TU_USUARIO -p PRACTICAR_CE < database\002_seed.sql
```

Tambien puedes abrir los scripts en MySQL Workbench, DBeaver o HeidiSQL y ejecutarlos sobre `PRACTICAR_CE`.

Las migraciones EF estan en `src/SeguimientoColegio.Web/Data/Migrations`. Por defecto la app no aplica migraciones al iniciar para evitar cambios automaticos no deseados sobre la base real.

## Ejecucion Local

Requisitos:

- .NET SDK 10.0 o compatible con `net10.0`.
- Acceso a MySQL/MariaDB.
- Base `PRACTICAR_CE` creada y scripts ejecutados.

Comandos:

```powershell
dotnet restore
dotnet build SeguimientoColegio.slnx
dotnet run --project src\SeguimientoColegio.Web\SeguimientoColegio.Web.csproj
```

Luego abre la URL mostrada por `dotnet run`.

## Primer Acceso

1. Configura `appsettings.Local.json`.
2. Ejecuta los scripts SQL.
3. Deja `BootstrapAdmin:Habilitado` en `true` temporalmente.
4. Inicia la aplicacion una vez.
5. Entra con el correo y clave definidos en `BootstrapAdmin`.
6. Crea los usuarios reales desde Administracion.
7. Desactiva el bootstrap o cambia la clave.

## Roles

- Administrador: acceso total, usuarios, catalogos, ninos y registros.
- Editor: puede crear y actualizar registros, correos, seguimiento y evidencias.
- Consulta: solo lectura de dashboard, control, registros, acuerdos y correos.

## Notas De Seguridad

- No hay secretos hardcodeados en clases.
- Las contrasenas se almacenan con hash mediante `PasswordHasher`.
- Las cookies son `HttpOnly` y con expiracion deslizante.
- Las evidencias se guardan bajo `App_Data/Uploads`, fuera de `wwwroot`.
- Las acciones de escritura tienen antiforgery tokens y politicas de autorizacion.

## Recomendaciones Futuras

- Integracion Gmail API para importar correos origen.
- Recordatorios y notificaciones por correo o WhatsApp.
- Exportacion a Excel/PDF de pendientes y acuerdos.
- Paginacion server-side si el volumen crece mucho.
- Pruebas automatizadas para servicios de alertas y auditoria.
- Almacenamiento de evidencias en S3, Azure Blob o Google Cloud Storage.
- Auditoria adicional para cambios en catalogos y usuarios.
