-- SeguimientoColegio - esquema inicial
-- Ejecutar sobre una base vacia PRACTICAR_CE antes de database/002_seed.sql.
-- Si la base no existe, crearla previamente con:
-- CREATE DATABASE `PRACTICAR_CE` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE `PRACTICAR_CE`;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `correos_origen` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Asunto` varchar(250) CHARACTER SET utf8mb4 NOT NULL,
    `Remitente` varchar(180) CHARACTER SET utf8mb4 NOT NULL,
    `FechaCorreo` datetime(6) NOT NULL,
    `Resumen` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `ReferenciaUrl` varchar(500) CHARACTER SET utf8mb4 NULL,
    `MessageId` varchar(255) CHARACTER SET utf8mb4 NULL,
    `Observaciones` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_correos_origen` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `disciplinas` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    `Nombre` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(500) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_disciplinas` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `estados_seguimiento` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    `Nombre` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(500) CHARACTER SET utf8mb4 NULL,
    `Orden` int NOT NULL,
    `ColorCss` varchar(30) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_estados_seguimiento` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `ninos` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Nombres` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Apellidos` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `Alias` varchar(120) CHARACTER SET utf8mb4 NULL,
    `FechaNacimiento` date NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_ninos` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `plataformas` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    `Nombre` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(500) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_plataformas` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `prioridades` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    `Nombre` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(500) CHARACTER SET utf8mb4 NULL,
    `Orden` int NOT NULL,
    `ColorCss` varchar(30) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_prioridades` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `roles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Nombre` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(250) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_roles` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `tipos_registro` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    `Nombre` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(500) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_tipos_registro` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `usuarios` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Nombres` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Apellidos` varchar(150) CHARACTER SET utf8mb4 NULL,
    `Correo` varchar(180) CHARACTER SET utf8mb4 NOT NULL,
    `PasswordHash` varchar(512) CHARACTER SET utf8mb4 NOT NULL,
    `RolId` int NOT NULL,
    `UltimoLogin` datetime(6) NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_usuarios` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_usuarios_roles_RolId` FOREIGN KEY (`RolId`) REFERENCES `roles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `registros_escolares` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `NinoId` int NOT NULL,
    `DisciplinaId` int NULL,
    `TipoRegistroId` int NOT NULL,
    `PlataformaId` int NULL,
    `CorreoOrigenId` int NULL,
    `EstadoSeguimientoId` int NOT NULL,
    `PrioridadId` int NULL,
    `Titulo` varchar(220) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(4000) CHARACTER SET utf8mb4 NULL,
    `ResponsablePrincipal` varchar(180) CHARACTER SET utf8mb4 NULL,
    `FechaRecibido` date NOT NULL,
    `FechaVencimiento` date NULL,
    `FechaRealizado` date NULL,
    `DiasAlerta` int NULL,
    `RequiereSeguimiento` tinyint(1) NOT NULL,
    `ComentarioGeneral` varchar(4000) CHARACTER SET utf8mb4 NULL,
    `CreadoPorUsuarioId` int NOT NULL,
    `ActualizadoPorUsuarioId` int NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `FechaActualizacion` datetime(6) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_registros_escolares` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_registros_escolares_correos_origen_CorreoOrigenId` FOREIGN KEY (`CorreoOrigenId`) REFERENCES `correos_origen` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_disciplinas_DisciplinaId` FOREIGN KEY (`DisciplinaId`) REFERENCES `disciplinas` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_estados_seguimiento_EstadoSeguimientoId` FOREIGN KEY (`EstadoSeguimientoId`) REFERENCES `estados_seguimiento` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_ninos_NinoId` FOREIGN KEY (`NinoId`) REFERENCES `ninos` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_plataformas_PlataformaId` FOREIGN KEY (`PlataformaId`) REFERENCES `plataformas` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_prioridades_PrioridadId` FOREIGN KEY (`PrioridadId`) REFERENCES `prioridades` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_tipos_registro_TipoRegistroId` FOREIGN KEY (`TipoRegistroId`) REFERENCES `tipos_registro` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_usuarios_ActualizadoPorUsuarioId` FOREIGN KEY (`ActualizadoPorUsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_registros_escolares_usuarios_CreadoPorUsuarioId` FOREIGN KEY (`CreadoPorUsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `evidencias_registro` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RegistroEscolarId` int NOT NULL,
    `NombreArchivo` varchar(260) CHARACTER SET utf8mb4 NOT NULL,
    `StorageKey` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `TipoMime` varchar(180) CHARACTER SET utf8mb4 NOT NULL,
    `TamanoBytes` bigint NOT NULL,
    `Descripcion` varchar(400) CHARACTER SET utf8mb4 NULL,
    `SubidoPorUsuarioId` int NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_evidencias_registro` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_evidencias_registro_registros_escolares_RegistroEscolarId` FOREIGN KEY (`RegistroEscolarId`) REFERENCES `registros_escolares` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_evidencias_registro_usuarios_SubidoPorUsuarioId` FOREIGN KEY (`SubidoPorUsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `registro_auditoria` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RegistroEscolarId` int NOT NULL,
    `Accion` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `CampoModificado` varchar(120) CHARACTER SET utf8mb4 NULL,
    `ValorAnterior` varchar(4000) CHARACTER SET utf8mb4 NULL,
    `ValorNuevo` varchar(4000) CHARACTER SET utf8mb4 NULL,
    `Comentario` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `UsuarioId` int NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_registro_auditoria` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_registro_auditoria_registros_escolares_RegistroEscolarId` FOREIGN KEY (`RegistroEscolarId`) REFERENCES `registros_escolares` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_registro_auditoria_usuarios_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `registro_urls` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RegistroEscolarId` int NOT NULL,
    `Url` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` varchar(250) CHARACTER SET utf8mb4 NULL,
    `Orden` int NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_registro_urls` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_registro_urls_registros_escolares_RegistroEscolarId` FOREIGN KEY (`RegistroEscolarId`) REFERENCES `registros_escolares` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `IX_correos_origen_FechaCorreo` ON `correos_origen` (`FechaCorreo`);
CREATE UNIQUE INDEX `IX_correos_origen_MessageId` ON `correos_origen` (`MessageId`);
CREATE INDEX `IX_correos_origen_Remitente` ON `correos_origen` (`Remitente`);
CREATE INDEX `IX_disciplinas_Activo` ON `disciplinas` (`Activo`);
CREATE UNIQUE INDEX `IX_disciplinas_Nombre` ON `disciplinas` (`Nombre`);
CREATE INDEX `IX_estados_seguimiento_Activo_Orden` ON `estados_seguimiento` (`Activo`, `Orden`);
CREATE UNIQUE INDEX `IX_estados_seguimiento_Nombre` ON `estados_seguimiento` (`Nombre`);
CREATE INDEX `IX_evidencias_registro_RegistroEscolarId` ON `evidencias_registro` (`RegistroEscolarId`);
CREATE INDEX `IX_evidencias_registro_SubidoPorUsuarioId` ON `evidencias_registro` (`SubidoPorUsuarioId`);
CREATE INDEX `IX_ninos_Activo_Nombres_Apellidos` ON `ninos` (`Activo`, `Nombres`, `Apellidos`);
CREATE INDEX `IX_plataformas_Activo` ON `plataformas` (`Activo`);
CREATE UNIQUE INDEX `IX_plataformas_Nombre` ON `plataformas` (`Nombre`);
CREATE INDEX `IX_prioridades_Activo_Orden` ON `prioridades` (`Activo`, `Orden`);
CREATE UNIQUE INDEX `IX_prioridades_Nombre` ON `prioridades` (`Nombre`);
CREATE INDEX `IX_registro_auditoria_RegistroEscolarId_FechaCreacion` ON `registro_auditoria` (`RegistroEscolarId`, `FechaCreacion`);
CREATE INDEX `IX_registro_auditoria_UsuarioId` ON `registro_auditoria` (`UsuarioId`);
CREATE INDEX `IX_registro_urls_RegistroEscolarId_Orden` ON `registro_urls` (`RegistroEscolarId`, `Orden`);
CREATE INDEX `IX_registros_escolares_Activo_NinoId_EstadoSeguimientoId` ON `registros_escolares` (`Activo`, `NinoId`, `EstadoSeguimientoId`);
CREATE INDEX `IX_registros_escolares_ActualizadoPorUsuarioId` ON `registros_escolares` (`ActualizadoPorUsuarioId`);
CREATE INDEX `IX_registros_escolares_CorreoOrigenId` ON `registros_escolares` (`CorreoOrigenId`);
CREATE INDEX `IX_registros_escolares_CreadoPorUsuarioId` ON `registros_escolares` (`CreadoPorUsuarioId`);
CREATE INDEX `IX_registros_escolares_DisciplinaId` ON `registros_escolares` (`DisciplinaId`);
CREATE INDEX `IX_registros_escolares_EstadoSeguimientoId` ON `registros_escolares` (`EstadoSeguimientoId`);
CREATE INDEX `IX_registros_escolares_FechaRecibido_FechaVencimiento_FechaReal~` ON `registros_escolares` (`FechaRecibido`, `FechaVencimiento`, `FechaRealizado`);
CREATE INDEX `IX_registros_escolares_NinoId_DisciplinaId_TipoRegistroId` ON `registros_escolares` (`NinoId`, `DisciplinaId`, `TipoRegistroId`);
CREATE INDEX `IX_registros_escolares_PlataformaId` ON `registros_escolares` (`PlataformaId`);
CREATE INDEX `IX_registros_escolares_PrioridadId_FechaActualizacion` ON `registros_escolares` (`PrioridadId`, `FechaActualizacion`);
CREATE INDEX `IX_registros_escolares_TipoRegistroId` ON `registros_escolares` (`TipoRegistroId`);
CREATE UNIQUE INDEX `IX_roles_Nombre` ON `roles` (`Nombre`);
CREATE INDEX `IX_tipos_registro_Activo` ON `tipos_registro` (`Activo`);
CREATE UNIQUE INDEX `IX_tipos_registro_Nombre` ON `tipos_registro` (`Nombre`);
CREATE INDEX `IX_usuarios_Activo_RolId` ON `usuarios` (`Activo`, `RolId`);
CREATE UNIQUE INDEX `IX_usuarios_Correo` ON `usuarios` (`Correo`);
CREATE INDEX `IX_usuarios_RolId` ON `usuarios` (`RolId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260423003426_InitialCreate', '9.0.0')
ON DUPLICATE KEY UPDATE `ProductVersion` = VALUES(`ProductVersion`);
