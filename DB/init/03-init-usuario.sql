-- Script de inicialización de la base de datos
-- Autor: Kevin Quito
-- Aplicación: Sistema de Solicitudes
-- Fecha: 2025-06-12
-- Descripción: Crear un usuario administrador y asignarle un rol
-- Requisitos: Ejecutar los scripts anteriores

USE [SolicitudesDB];
GO

-- Obtener el ID del rol correspondiente (por ejemplo 'ANALISTA')
DECLARE @RolId INT;

SELECT @RolId = c.Id
FROM dbo.Catalogos c
WHERE c.Codigo = 'ANALISTA';

-- Crear usuario administrador (contraseña en texto plano)
INSERT INTO dbo.Usuarios (Nombre, Correo, Contrasenia, Apellidos, Domicilio, Telefono)
VALUES (
    'Admin',                 -- Nombre
    'admin@miapp.com',       -- Correo (debe ser único)
    'Svper@dm1n',            -- Contraseña (texto plano, solo para pruebas)
    'Predeterminado',        -- Apellidos
    'Calle 123',             -- Domicilio
    999999999                -- Teléfono
);

-- Obtener el ID del usuario recién insertado
DECLARE @UsuarioId INT = SCOPE_IDENTITY();

-- Insertar relación en Usuario_Roles
INSERT INTO dbo.Usuario_Roles (Usuario_Id, Rol_Id)
VALUES (@UsuarioId, @RolId);

-- Verificación (opcional)
SELECT * FROM dbo.Usuarios WHERE Id = @UsuarioId;
SELECT * FROM dbo.Usuario_Roles WHERE Usuario_Id = @UsuarioId;
