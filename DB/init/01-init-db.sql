-- =============================================
-- Script de inicialización de la base de datos
-- Autor: Sistema de Solicitudes
-- Fecha: 2025-06-12
-- Descripción: Script para crear la base de datos y sus objetos
-- =============================================

USE [master]
GO

-- Crear la base de datos
CREATE DATABASE [SolicitudesDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SolicitudesDB', FILENAME = N'/var/opt/mssql/data/SolicitudesDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SolicitudesDB_log', FILENAME = N'/var/opt/mssql/data/SolicitudesDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

-- Configurar la base de datos
ALTER DATABASE [SolicitudesDB] SET COMPATIBILITY_LEVEL = 160
GO
ALTER DATABASE [SolicitudesDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SolicitudesDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SolicitudesDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SolicitudesDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SolicitudesDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [SolicitudesDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SolicitudesDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SolicitudesDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SolicitudesDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SolicitudesDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SolicitudesDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SolicitudesDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SolicitudesDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SolicitudesDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SolicitudesDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [SolicitudesDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SolicitudesDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SolicitudesDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SolicitudesDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SolicitudesDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SolicitudesDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SolicitudesDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SolicitudesDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SolicitudesDB] SET  MULTI_USER 
GO
ALTER DATABASE [SolicitudesDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SolicitudesDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SolicitudesDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SolicitudesDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SolicitudesDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SolicitudesDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [SolicitudesDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [SolicitudesDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

USE [SolicitudesDB]
GO

-- Crear usuario SA y asignar permisos
CREATE USER [sa] FOR LOGIN [sa] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [sa]
GO

-- =============================================
-- Creación de tablas
-- =============================================

-- Tabla de Catálogos
CREATE TABLE [dbo].[Catalogos](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Codigo] [varchar](20) NOT NULL,
    [Descripcion] [varchar](60) NOT NULL,
    [Padre_Id] [int] NULL,
    [Tipo] [varchar](20) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Tabla de Usuarios
CREATE TABLE [dbo].[Usuarios](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Nombre] [varchar](200) NOT NULL,
    [Correo] [varchar](200) NOT NULL,
    [Contrasenia] [nvarchar](max) NOT NULL,
    [Apellidos] [varchar](100) NULL,
    [Domicilio] [varchar](200) NULL,
    [Telefono] [int] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Correo] ASC)
)
GO

-- Tabla de Solicitudes
CREATE TABLE [dbo].[Solicitudes](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Monto] [decimal](12, 2) NOT NULL,
    [Plazo_meses] [int] NOT NULL,
    [Ingresos_mensual] [decimal](12, 2) NOT NULL,
    [Antiguedad_laboral] [int] NOT NULL,
    [Estado_Id] [int] NOT NULL,
    [Fecha_registro] [datetime2](7) NOT NULL,
    [Usuario_Id] [int] NOT NULL,
    [Codigo] [varchar](20) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Tabla de Log de Auditoría
CREATE TABLE [dbo].[Log_auditoria](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Fecha_registro] [datetime2](7) NOT NULL,
    [Estado_anterior_Id] [int] NULL,
    [Estado_actual_Id] [int] NULL,
    [Accion] [varchar](25) NOT NULL,
    [Usuario_Id] [int] NOT NULL,
    [Solicitud_Id] [int] NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Tabla de Roles de Usuario
CREATE TABLE [dbo].[Usuario_Roles](
    [Usuario_Id] [int] NOT NULL,
    [Rol_Id] [int] NOT NULL,
    PRIMARY KEY CLUSTERED ([Usuario_Id] ASC, [Rol_Id] ASC)
)
GO

-- =============================================
-- Creación de Restricciones (Foreign Keys)
-- =============================================

-- Restricciones para Catalogos
ALTER TABLE [dbo].[Catalogos] 
    ADD CONSTRAINT [FK_Catalogo_Padre] 
    FOREIGN KEY([Padre_Id]) REFERENCES [dbo].[Catalogos] ([Id])
GO

-- Restricciones para Log_auditoria
ALTER TABLE [dbo].[Log_auditoria] 
    ADD CONSTRAINT [FK_Log_Estado_Actual] 
    FOREIGN KEY([Estado_actual_Id]) REFERENCES [dbo].[Catalogos] ([Id])
GO

ALTER TABLE [dbo].[Log_auditoria] 
    ADD CONSTRAINT [FK_Log_Estado_Anterior] 
    FOREIGN KEY([Estado_anterior_Id]) REFERENCES [dbo].[Catalogos] ([Id])
GO

ALTER TABLE [dbo].[Log_auditoria] 
    ADD CONSTRAINT [FK_Log_Solicitud] 
    FOREIGN KEY([Solicitud_Id]) REFERENCES [dbo].[Solicitudes] ([Id])
GO

ALTER TABLE [dbo].[Log_auditoria] 
    ADD CONSTRAINT [FK_Log_Usuario] 
    FOREIGN KEY([Usuario_Id]) REFERENCES [dbo].[Usuarios] ([Id])
GO

-- Restricciones para Solicitudes
ALTER TABLE [dbo].[Solicitudes] 
    ADD CONSTRAINT [FK_Solicitudes_Estado] 
    FOREIGN KEY([Estado_Id]) REFERENCES [dbo].[Catalogos] ([Id])
GO

ALTER TABLE [dbo].[Solicitudes] 
    ADD CONSTRAINT [FK_Solicitudes_Usuario] 
    FOREIGN KEY([Usuario_Id]) REFERENCES [dbo].[Usuarios] ([Id])
GO

-- Restricciones para Usuario_Roles
ALTER TABLE [dbo].[Usuario_Roles] 
    ADD CONSTRAINT [FK_UsuarioRol_Rol] 
    FOREIGN KEY([Rol_Id]) REFERENCES [dbo].[Catalogos] ([Id])
GO

ALTER TABLE [dbo].[Usuario_Roles] 
    ADD CONSTRAINT [FK_UsuarioRol_Usuario] 
    FOREIGN KEY([Usuario_Id]) REFERENCES [dbo].[Usuarios] ([Id])
GO

-- =============================================
-- Creación de Procedimientos Almacenados
-- =============================================

-- Procedimiento para actualizar solicitud
CREATE PROCEDURE [dbo].[sp_ActualizarSolicitud]
    @SolicitudId INT,
    @Monto DECIMAL(12,2),
    @PlazoMeses INT,
    @IngresosMensual DECIMAL(12,2),
    @AntiguedadLaboral INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Solicitudes
    SET Monto = @Monto,
        Plazo_meses = @PlazoMeses,
        Ingresos_mensual = @IngresosMensual,
        Antiguedad_laboral = @AntiguedadLaboral
    WHERE Id = @SolicitudId;
    SELECT @SolicitudId AS SolicitudId;
END
GO

-- Procedimiento para cambiar estado y auditar
CREATE PROCEDURE [dbo].[sp_CambiarEstadoSolicitudYAuditar]
    @SolicitudId INT,
    @NuevoEstadoId INT,
    @UsuarioAccionId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @EstadoAnteriorId INT;

    IF NOT EXISTS (SELECT 1 FROM Catalogos WHERE Id = @NuevoEstadoId)
    BEGIN
        RAISERROR('El nuevo estado no existe en el catálogo.', 16, 1);
        RETURN;
    END

    SELECT @EstadoAnteriorId = Estado_Id FROM Solicitudes WHERE Id = @SolicitudId;

    IF @EstadoAnteriorId IS NULL
    BEGIN
        RAISERROR('La solicitud especificada no existe.', 16, 1);
        RETURN;
    END

    UPDATE Solicitudes
    SET Estado_Id = @NuevoEstadoId
    WHERE Id = @SolicitudId;

    INSERT INTO Log_auditoria (
        Fecha_registro,
        Estado_anterior_Id,
        Estado_actual_Id,
        Accion,
        Usuario_Id,
        Solicitud_Id
    )
    VALUES (
        SYSDATETIME(),
        @EstadoAnteriorId,
        @NuevoEstadoId,
        'CAMBIO_ESTADO',
        @UsuarioAccionId,
        @SolicitudId
    );
END
GO

-- Procedimiento para crear solicitud
CREATE PROCEDURE [dbo].[sp_CrearSolicitud]
    @Monto DECIMAL(12,2),
    @PlazoMeses INT,
    @IngresosMensual DECIMAL(12,2),
    @AntiguedadLaboral INT,
    @Usuario_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NuevaSolicitudId INT;
    DECLARE @Contador INT;
    DECLARE @FechaActual CHAR(10) = FORMAT(GETDATE(), 'yyyy-MM-dd');
    DECLARE @Codigo VARCHAR(20);
    DECLARE @Estado_Id_Calculado INT;
    DECLARE @Id_EnRevision INT;
    DECLARE @Id_PreAprobado INT;

    SELECT @Id_EnRevision = Id FROM Catalogos WHERE Codigo = 'EN_REVISION' AND Tipo = 'ESTADO_SOLICITUD';
    SELECT @Id_PreAprobado = Id FROM Catalogos WHERE Codigo = 'PRE_APROBADO' AND Tipo = 'ESTADO_SOLICITUD';

    SET @Estado_Id_Calculado = @Id_EnRevision;

    IF @IngresosMensual >= 1500
    BEGIN
        SET @Estado_Id_Calculado = @Id_PreAprobado;
    END;

    SELECT @Contador = COUNT(*)
    FROM Solicitudes
    WHERE Usuario_Id = @Usuario_Id
      AND YEAR(Fecha_registro) = YEAR(GETDATE());

    SET @Contador = ISNULL(@Contador, 0) + 1;
    SET @Codigo = CONCAT(@FechaActual, '-', FORMAT(@Usuario_Id, '00'), '-', FORMAT(@Contador, '0000'));

    INSERT INTO Solicitudes (
        Monto, Plazo_meses, Ingresos_mensual, Antiguedad_laboral,
        Estado_Id, Fecha_registro, Usuario_Id, Codigo
    )
    VALUES (
        @Monto, @PlazoMeses, @IngresosMensual, @AntiguedadLaboral,
        @Estado_Id_Calculado, SYSDATETIME(), @Usuario_Id, @Codigo
    );

    SET @NuevaSolicitudId = SCOPE_IDENTITY();

    INSERT INTO Log_auditoria (
        Fecha_registro, Estado_anterior_Id, Estado_actual_Id,
        Accion, Usuario_Id, Solicitud_Id
    )
    VALUES (
        SYSDATETIME(), NULL, @Estado_Id_Calculado,
        'CREACION', @Usuario_Id, @NuevaSolicitudId
    );

    SELECT @NuevaSolicitudId AS SolicitudId, @Codigo AS CodigoGenerado;
END
GO

-- Procedimiento para eliminar solicitud
CREATE PROCEDURE [dbo].[sp_EliminarSolicitud]
    @SolicitudId INT,
    @EstadoEliminadoId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Solicitudes
    SET Estado_Id = @EstadoEliminadoId
    WHERE Id = @SolicitudId;
END
GO 


CREATE PROCEDURE dbo.sp_ConsultarHistorialAuditoria
    @SolicitudId INT = 0,
    @UsuarioId INT = 0,
    @EstadoAnteriorId INT = 0,
    @EstadoActualId INT = 0,
    @FechaInicio DATETIME2 = NULL,
    @FechaFin DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        log.Id AS LogId,
        log.Fecha_registro,
        log.Accion,
        ISNULL(estadoAnterior.Descripcion, 'N/A') AS EstadoAnterior,
        ISNULL(estadoActual.Descripcion, 'N/A') AS EstadoActual,
        u.Id AS UsuarioId,
        CONCAT(u.Nombre, ' ', ISNULL(u.Apellidos, '')) AS NombreUsuario,
        s.Id AS SolicitudId,
        s.Codigo AS CodigoSolicitud,
        s.Monto,
        s.Plazo_meses AS PlazoMeses,
        s.Fecha_registro AS FechaSolicitud
    FROM 
        dbo.Log_auditoria log
        INNER JOIN dbo.Solicitudes s ON s.Id = log.Solicitud_Id
        INNER JOIN dbo.Usuarios u ON u.Id = log.Usuario_Id
        LEFT JOIN dbo.Catalogos estadoAnterior ON estadoAnterior.Id = log.Estado_anterior_Id
        LEFT JOIN dbo.Catalogos estadoActual ON estadoActual.Id = log.Estado_actual_Id
    WHERE
        (@SolicitudId = 0 OR log.Solicitud_Id = @SolicitudId)
        AND (@UsuarioId = 0 OR log.Usuario_Id = @UsuarioId)
        AND (@EstadoAnteriorId = 0 OR log.Estado_anterior_Id = @EstadoAnteriorId)
        AND (@EstadoActualId = 0 OR log.Estado_actual_Id = @EstadoActualId)
        AND (@FechaInicio IS NULL OR log.Fecha_registro >= @FechaInicio)
        AND (@FechaFin IS NULL OR log.Fecha_registro <= @FechaFin)
    ORDER BY 
        log.Fecha_registro DESC;
END;