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
    CONSTRAINT FK_Solicitudes_Usuario FOREIGN KEY (Usuario_Id) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Solicitudes_Estado FOREIGN KEY (Estado_Id) REFERENCES Catalogos(Id)
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
