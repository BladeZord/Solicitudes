-- =============================================
-- Script de inicialización de catálogos
-- Autor: Sistema de Solicitudes
-- Fecha: 2025-06-12
-- Descripción: Script para insertar los datos iniciales de catálogos
-- =============================================

USE [SolicitudesDB]
GO

-- Insertar tipos de personas
INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('TIPO_PERSONA', 'Tipos de personas en el sistema', NULL, NULL);
DECLARE @TipoPersonaId INT = SCOPE_IDENTITY();

INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('SOLICITANTE', 'Solicitante del requerimiento', @TipoPersonaId, 'TIPO_PERSONA');

INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('ANALISTA', 'Analista de revision', @TipoPersonaId, 'TIPO_PERSONA');

-- Insertar estados de solicitud
INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('ESTADO_SOLICITUD', 'Estado de la solicitud', NULL, NULL);
DECLARE @EstadoSolicitudId INT = SCOPE_IDENTITY();

INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('APROBADO', 'Solicitud aprobada', @EstadoSolicitudId, 'ESTADO_SOLICITUD');

INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('RECHAZADO', 'Solicitud rechazada', @EstadoSolicitudId, 'ESTADO_SOLICITUD');

INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('ELIMINADO', 'Solicitud eliminada', @EstadoSolicitudId, 'ESTADO_SOLICITUD');

INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('EN_REVISION', 'Solicitud en revisión', @EstadoSolicitudId, 'ESTADO_SOLICITUD');

INSERT INTO [dbo].[Catalogos] ([Codigo], [Descripcion], [Padre_Id], [Tipo])
VALUES ('PRE_APROBADO', 'Solicitud pre aprobada', @EstadoSolicitudId, 'ESTADO_SOLICITUD'); 