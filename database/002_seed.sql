-- SeguimientoColegio - datos semilla iniciales
-- Ejecutar despues de database/001_schema.sql.
-- No crea usuarios con password; el primer administrador se genera por BootstrapAdmin en appsettings.Local.json.

USE `PRACTICAR_CE`;
SET NAMES utf8mb4;

SET @seed_now = '2026-04-22 00:00:00';

INSERT INTO `roles` (`Id`, `Descripcion`, `Nombre`) VALUES
(1, 'Gestiona usuarios, catalogos y tiene acceso total.', 'Administrador'),
(2, 'Puede crear y actualizar registros y seguimientos.', 'Editor'),
(3, 'Solo puede revisar la informacion disponible.', 'Consulta')
ON DUPLICATE KEY UPDATE
    `Descripcion` = VALUES(`Descripcion`),
    `Nombre` = VALUES(`Nombre`);

INSERT INTO `estados_seguimiento` (`Id`, `Activo`, `ColorCss`, `Descripcion`, `FechaActualizacion`, `FechaCreacion`, `Nombre`, `Orden`) VALUES
(1, TRUE, '#f0b33f', 'Registro pendiente de atencion.', @seed_now, @seed_now, 'Pendiente', 1),
(2, TRUE, '#2f80ed', 'Registro con avance parcial.', @seed_now, @seed_now, 'En proceso', 2),
(3, TRUE, '#2f9e44', 'Registro completado.', @seed_now, @seed_now, 'Concluido', 3),
(4, TRUE, '#7a828f', 'Registro descartado o sin accion.', @seed_now, @seed_now, 'Descartado', 4),
(5, TRUE, '#d64545', 'Registro vencido.', @seed_now, @seed_now, 'Vencido', 5)
ON DUPLICATE KEY UPDATE
    `Activo` = VALUES(`Activo`),
    `ColorCss` = VALUES(`ColorCss`),
    `Descripcion` = VALUES(`Descripcion`),
    `FechaActualizacion` = VALUES(`FechaActualizacion`),
    `Nombre` = VALUES(`Nombre`),
    `Orden` = VALUES(`Orden`);

INSERT INTO `prioridades` (`Id`, `Activo`, `ColorCss`, `Descripcion`, `FechaActualizacion`, `FechaCreacion`, `Nombre`, `Orden`) VALUES
(1, TRUE, '#7f8c8d', 'Atencion baja.', @seed_now, @seed_now, 'Baja', 1),
(2, TRUE, '#3d7df0', 'Atencion media.', @seed_now, @seed_now, 'Media', 2),
(3, TRUE, '#ef8b2c', 'Atencion alta.', @seed_now, @seed_now, 'Alta', 3),
(4, TRUE, '#d64545', 'Atencion critica.', @seed_now, @seed_now, 'Critica', 4)
ON DUPLICATE KEY UPDATE
    `Activo` = VALUES(`Activo`),
    `ColorCss` = VALUES(`ColorCss`),
    `Descripcion` = VALUES(`Descripcion`),
    `FechaActualizacion` = VALUES(`FechaActualizacion`),
    `Nombre` = VALUES(`Nombre`),
    `Orden` = VALUES(`Orden`);

INSERT INTO `plataformas` (`Id`, `Activo`, `Descripcion`, `FechaActualizacion`, `FechaCreacion`, `Nombre`) VALUES
(1, TRUE, 'Actividades interactivas.', @seed_now, @seed_now, 'Wordwall'),
(2, TRUE, 'Lecturas y biblioteca digital.', @seed_now, @seed_now, 'Odilo'),
(3, TRUE, 'Materiales visuales.', @seed_now, @seed_now, 'Canva'),
(4, TRUE, 'Documento PDF.', @seed_now, @seed_now, 'PDF'),
(5, TRUE, 'Videos educativos.', @seed_now, @seed_now, 'YouTube'),
(6, TRUE, 'Archivos compartidos.', @seed_now, @seed_now, 'Google Drive'),
(7, TRUE, 'Otra plataforma o fuente.', @seed_now, @seed_now, 'Otro')
ON DUPLICATE KEY UPDATE
    `Activo` = VALUES(`Activo`),
    `Descripcion` = VALUES(`Descripcion`),
    `FechaActualizacion` = VALUES(`FechaActualizacion`),
    `Nombre` = VALUES(`Nombre`);

INSERT INTO `disciplinas` (`Id`, `Activo`, `Descripcion`, `FechaActualizacion`, `FechaCreacion`, `Nombre`) VALUES
(1, TRUE, 'Seguimiento de ejercicios y refuerzos.', @seed_now, @seed_now, 'Matematicas'),
(2, TRUE, 'Actividades y materiales de danza.', @seed_now, @seed_now, 'Danza'),
(3, TRUE, 'Practicas y materiales de lectoescritura.', @seed_now, @seed_now, 'Lectoescritura'),
(4, TRUE, 'Mensajes y actividades de comunicacion.', @seed_now, @seed_now, 'Comunicacion'),
(5, TRUE, 'Lecturas y avances.', @seed_now, @seed_now, 'Plan lector'),
(6, TRUE, 'Actividades artisticas.', @seed_now, @seed_now, 'Arte')
ON DUPLICATE KEY UPDATE
    `Activo` = VALUES(`Activo`),
    `Descripcion` = VALUES(`Descripcion`),
    `FechaActualizacion` = VALUES(`FechaActualizacion`),
    `Nombre` = VALUES(`Nombre`);

INSERT INTO `tipos_registro` (`Id`, `Activo`, `Descripcion`, `FechaActualizacion`, `FechaCreacion`, `Nombre`) VALUES
(1, TRUE, 'Material para reforzar aprendizaje.', @seed_now, @seed_now, 'Material de refuerzo'),
(2, TRUE, 'Material o accion requerida por el colegio.', @seed_now, @seed_now, 'Requerimiento'),
(3, TRUE, 'Lecturas y plan lector.', @seed_now, @seed_now, 'Lectura'),
(4, TRUE, 'Tareas con seguimiento.', @seed_now, @seed_now, 'Tarea'),
(5, TRUE, 'Aviso relevante para control.', @seed_now, @seed_now, 'Aviso importante'),
(6, TRUE, 'Acuerdos de reuniones presenciales o virtuales.', @seed_now, @seed_now, 'Acuerdo con profesora'),
(7, TRUE, 'Actividad escolar.', @seed_now, @seed_now, 'Actividad'),
(8, TRUE, 'Evaluaciones y pruebas.', @seed_now, @seed_now, 'Evaluacion')
ON DUPLICATE KEY UPDATE
    `Activo` = VALUES(`Activo`),
    `Descripcion` = VALUES(`Descripcion`),
    `FechaActualizacion` = VALUES(`FechaActualizacion`),
    `Nombre` = VALUES(`Nombre`);

INSERT INTO `ninos` (`Id`, `Activo`, `Alias`, `Apellidos`, `FechaActualizacion`, `FechaCreacion`, `FechaNacimiento`, `Nombres`) VALUES
(1, TRUE, 'Dani', 'Pendiente', @seed_now, @seed_now, NULL, 'Daniela')
ON DUPLICATE KEY UPDATE
    `Activo` = VALUES(`Activo`),
    `Alias` = VALUES(`Alias`),
    `Apellidos` = VALUES(`Apellidos`),
    `FechaActualizacion` = VALUES(`FechaActualizacion`),
    `FechaNacimiento` = VALUES(`FechaNacimiento`),
    `Nombres` = VALUES(`Nombres`);
