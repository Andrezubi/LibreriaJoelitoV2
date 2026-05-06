-- ============================================
-- VERIFICACIÓN Y CORRECCIÓN DE LA BD
-- ============================================

-- 1. Ver la estructura exacta de la tabla Usuario
DESCRIBE Usuario;

-- 2. Ver todos los usuarios existentes
SELECT Id, NombreUsuario, Nombre, Rol, Estado FROM Usuario LIMIT 10;

-- 3. Ver si hay contraseñas en texto plano (sin cifrar)
SELECT Id, NombreUsuario, Password FROM Usuario LIMIT 5;

-- ============================================
-- INSERTAR USUARIO DE PRUEBA
-- ============================================
-- Si no hay usuarios o necesitas crear uno nuevo:

-- Opción 1: Contraseña en texto plano (SOLO PARA PRUEBAS)
INSERT INTO Usuario 
(Nombre, ApellidoPaterno, ApellidoMaterno, Ci, Email, Telefono, FechaNacimiento, FechaIngreso, NombreUsuario, Password, Rol, Estado)
VALUES 
('Admin', 'Joelito', NULL, '87654321', 'admin@libreria.com', '+59165123456', '1990-01-01', NOW(), 'admin', '123456', 'Admin', 1)
ON DUPLICATE KEY UPDATE 
Password='123456', Estado=1;

-- Opción 2: Contraseña hasheada con BCrypt (Recomendado)
-- Hash de "123456" con BCrypt (workFactor 12):
-- $2a$12$eIR/0h8DsKzfEdIc2GgqheHKcNc7hYELy8o0nKLqAJ4XG7TA8X8Li

INSERT INTO Usuario 
(Nombre, ApellidoPaterno, ApellidoMaterno, Ci, Email, Telefono, FechaNacimiento, FechaIngreso, NombreUsuario, Password, Rol, Estado)
VALUES 
('Admin', 'Joelito', NULL, '87654321', 'admin@libreria.com', '+59165123456', '1990-01-01', NOW(), 'admin', '$2a$12$eIR/0h8DsKzfEdIc2GgqheHKcNc7hYELy8o0nKLqAJ4XG7TA8X8Li', 'Admin', 1)
ON DUPLICATE KEY UPDATE 
Password='$2a$12$eIR/0h8DsKzfEdIc2GgqheHKcNc7hYELy8o0nKLqAJ4XG7TA8X8Li', Estado=1;

-- 4. Verificar que se insertó
SELECT * FROM Usuario WHERE NombreUsuario = 'admin';
