DROP DATABASE IF EXISTS bdlibreria;
CREATE DATABASE bdlibreria;
USE bdlibreria;

-- ========================
-- USUARIO
-- ========================
CREATE TABLE Usuario(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    ApellidoPaterno VARCHAR(100) NOT NULL,
    ApellidoMaterno VARCHAR(100),
    Ci INT NOT NULL,
    Complemento VARCHAR(10),
    FechaNacimiento DATE,
    Email VARCHAR(150) NOT NULL,
    DireccionDomicilio VARCHAR(200),
    Telefono VARCHAR(20) NOT NULL,
    FechaIngreso DATE NOT NULL,

    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Rol VARCHAR(50) NOT NULL,

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- CLIENTE
-- ========================
CREATE TABLE Cliente(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    ApellidoPaterno VARCHAR(100) NOT NULL,
    ApellidoMaterno VARCHAR(100),
    Ci INT NOT NULL,
    Complemento VARCHAR(10),
    Email VARCHAR(150),
    ClienteFrecuente BOOLEAN DEFAULT FALSE,

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- CATEGORIA
-- ========================
CREATE TABLE Categoria(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- MARCA
-- ========================
CREATE TABLE Marca(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255),
    PaginaWeb VARCHAR(150),
    Industria VARCHAR(100),

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- PRODUCTO
-- ========================
CREATE TABLE Producto(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCategoria INT,
    IdMarca INT,
    Nombre VARCHAR(150) NOT NULL,
    Stock INT DEFAULT 0,

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    FOREIGN KEY (IdCategoria) REFERENCES Categoria(Id),
    FOREIGN KEY (IdMarca) REFERENCES Marca(Id),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- PRESENTACION
-- ========================
CREATE TABLE Presentacion(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- PRESENTACION PRODUCTO
-- ========================
CREATE TABLE PresentacionProducto(
    IdProducto INT NOT NULL,
    IdPresentacion INT NOT NULL,
    FactorConversion INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    PRIMARY KEY (IdProducto, IdPresentacion),

    FOREIGN KEY (IdProducto) REFERENCES Producto(Id),
    FOREIGN KEY (IdPresentacion) REFERENCES Presentacion(Id),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- VENTA
-- ========================
CREATE TABLE Venta(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCliente INT NOT NULL,
    Fecha DATETIME DEFAULT CURRENT_TIMESTAMP,
    Total DECIMAL(10,2) NOT NULL,

    Estado BOOLEAN DEFAULT TRUE,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaUltimaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    IdUsuario INT NOT NULL,

    FOREIGN KEY (IdCliente) REFERENCES Cliente(Id),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

-- ========================
-- DETALLE VENTA
-- ========================
CREATE TABLE DetalleVenta(
    IdVenta INT NOT NULL,
    IdProducto INT NOT NULL,
    IdPresentacion INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,

    PRIMARY KEY (IdVenta, IdProducto, IdPresentacion),

    FOREIGN KEY (IdVenta) REFERENCES Venta(Id),
    FOREIGN KEY (IdProducto, IdPresentacion) 
        REFERENCES PresentacionProducto(IdProducto, IdPresentacion)
);