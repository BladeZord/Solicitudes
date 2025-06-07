-- Crear la base de datos
CREATE DATABASE SolicitudesDB;
GO

-- Usar la base de datos
USE SolicitudesDB;
GO

-- Tabla: Catalogos
CREATE TABLE Catalogos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(20) NOT NULL,
    Descripcion VARCHAR(60) NOT NULL,
    Padre_Id INT NULL, -- FK auto-referenciada
    CONSTRAINT FK_Catalogo_Padre FOREIGN KEY (Padre_Id) REFERENCES Catalogos(Id)
);

-- Tabla: Usuarios
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(200) NOT NULL,
    Correo VARCHAR(200) NOT NULL UNIQUE,
    Contrasenia NVARCHAR(MAX) NOT NULL,
    Rol_Id INT NOT NULL, -- FK hacia Catalogos
    CONSTRAINT FK_Usuarios_Rol FOREIGN KEY (Rol_Id) REFERENCES Catalogos(Id)
);

-- Tabla: Solicitudes
CREATE TABLE Solicitudes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Monto DECIMAL(12, 2) NOT NULL,
    Plazo_meses INT NOT NULL,
    Ingresos_mensual DECIMAL(12, 2) NOT NULL,
    Antiguedad_laboral INT NOT NULL,
    Estado_Id INT NOT NULL, -- FK hacia Catalogos
    Fecha_registro DATETIME2 NOT NULL,
    Usuario_Id INT NOT NULL,
);

-- Tabla: Log_auditoria
CREATE TABLE Log_auditoria (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha_registro DATETIME2 NOT NULL ,
    Estado_anterior_Id INT NULL,
    Estado_actual_Id INT NULL,
    Accion VARCHAR(25) NOT NULL,
    Usuario_Id INT NOT NULL,
    Solicitud_Id INT NOT NULL,
    CONSTRAINT FK_Log_Estado_Anterior FOREIGN KEY (Estado_anterior_Id) REFERENCES Catalogos(Id),
    CONSTRAINT FK_Log_Estado_Actual FOREIGN KEY (Estado_actual_Id) REFERENCES Catalogos(Id),
    CONSTRAINT FK_Log_Usuario FOREIGN KEY (Usuario_Id) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Log_Solicitud FOREIGN KEY (Solicitud_Id) REFERENCES Solicitudes(Id)
);



/*---------------------------------------------*/
USE SolicitudesDB;
GO

-- Crear nueva solicitud
CREATE PROCEDURE sp_CrearSolicitud
    @Monto DECIMAL(12,2),
    @PlazoMeses INT,
    @IngresosMensual DECIMAL(12,2),
    @AntiguedadLaboral INT,
    @Estado_Id INT,
    @Usuario_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Solicitudes (
        Monto,
        Plazo_meses,
        Ingresos_mensual,
        Antiguedad_laboral,
        Estado_Id,
        Fecha_registro,
        Usuario_Id
    )
    VALUES (
        @Monto,
        @PlazoMeses,
        @IngresosMensual,
        @AntiguedadLaboral,
        @Estado_Id,
        SYSDATETIME(),
        @Usuario_Id
    );
END
GO

-- Actualizar solicitud
CREATE PROCEDURE sp_ActualizarSolicitud
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
END
GO

-- Eliminar solicitud (cambiar estado a eliminado)
CREATE PROCEDURE sp_EliminarSolicitud
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

-- Cambiar estado y registrar en log de auditoría
CREATE PROCEDURE sp_CambiarEstadoSolicitudYAuditar
    @SolicitudId INT,
    @NuevoEstadoId INT,
    @UsuarioAccionId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoAnteriorId INT;

    -- Validar que el nuevo estado existe
    IF NOT EXISTS (
        SELECT 1 FROM Catalogos WHERE Id = @NuevoEstadoId
    )
    BEGIN
        RAISERROR('El nuevo estado no existe en el catálogo.', 16, 1);
        RETURN;
    END

    -- Obtener estado anterior
    SELECT @EstadoAnteriorId = Estado_Id FROM Solicitudes WHERE Id = @SolicitudId;

    IF @EstadoAnteriorId IS NULL
    BEGIN
        RAISERROR('La solicitud especificada no existe.', 16, 1);
        RETURN;
    END

    -- Actualizar solicitud
    UPDATE Solicitudes
    SET Estado_Id = @NuevoEstadoId
    WHERE Id = @SolicitudId;

    -- Insertar en log
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

