# Sistema de Facturación - UTA Tech

Proyecto desarrollado con ASP.NET Core + SQL Server + JavaScript (Vanilla).

Permite:
- Gestión de clientes
- Gestión de productos
- Creación de facturas
- Consulta de facturas
- Generación de PDF
  

## 📦 Instalación del Backend (Terminal de VS Code)
- dotnet new webapi    =>   Si no funciona "dotnet new webapi --force"
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer
- dotnet add package Microsoft.EntityFrameworkCore.Tools
- dotnet add package Swashbuckle.AspNetCore

## 🗄️ Configuración de la Base de Datos
```sql

-- =========================
-- CREAR BASE DE DATOS
-- =========================
CREATE DATABASE SistemaFacturacionDB;
GO

USE SistemaFacturacionDB;
GO

-- =========================
-- TABLA: Clientes
-- =========================
CREATE TABLE Clientes
(
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Direccion NVARCHAR(200) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    Correo NVARCHAR(100) NOT NULL,

    CONSTRAINT UQ_Clientes_Correo UNIQUE (Correo)
);

-- =========================
-- TABLA: Productos
-- =========================
CREATE TABLE Productos
(
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL,

    CONSTRAINT UQ_Productos_Nombre UNIQUE (Nombre),
    CONSTRAINT CHK_Productos_Precio CHECK (Precio > 0),
    CONSTRAINT CHK_Productos_Stock CHECK (Stock >= 0)
);

-- =========================
-- TABLA: Facturas
-- =========================
CREATE TABLE Facturas
(
    IdFactura INT IDENTITY(1,1) PRIMARY KEY,
    NumeroFactura INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    IdCliente INT NOT NULL,

    Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    Iva DECIMAL(18,2) NOT NULL DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,

    CONSTRAINT UQ_Facturas_NumeroFactura UNIQUE (NumeroFactura),
    CONSTRAINT CHK_Facturas_Total CHECK (Total >= 0),

    CONSTRAINT FK_Facturas_Clientes FOREIGN KEY (IdCliente)
        REFERENCES Clientes(IdCliente)
);

-- =========================
-- TABLA: DetalleFactura
-- =========================
CREATE TABLE DetalleFactura
(
    IdDetalleFactura INT IDENTITY(1,1) PRIMARY KEY,
    IdFactura INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    TotalLinea DECIMAL(18,2) NOT NULL,

    CONSTRAINT CHK_DetalleFactura_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CHK_DetalleFactura_PrecioUnitario CHECK (PrecioUnitario > 0),
    CONSTRAINT CHK_DetalleFactura_TotalLinea CHECK (TotalLinea >= 0),

    CONSTRAINT FK_DetalleFactura_Facturas FOREIGN KEY (IdFactura)
        REFERENCES Facturas(IdFactura),

    CONSTRAINT FK_DetalleFactura_Productos FOREIGN KEY (IdProducto)
        REFERENCES Productos(IdProducto)
);

-- =========================
-- DATOS INICIALES: CLIENTES
-- =========================
INSERT INTO Clientes (Nombre, Apellido, Direccion, Telefono, Correo)
VALUES 
('Juan', 'Perez', 'Ambato - Ficoa', '0991111111', 'juan.perez@gmail.com'),
('Maria', 'Lopez', 'Ambato - Huachi', '0992222222', 'maria.lopez@gmail.com'),
('Carlos', 'Mendoza', 'Ambato - Atocha', '0993333333', 'carlos.mendoza@gmail.com'),
('Ana', 'Villacis', 'Ambato - Izamba', '0994444444', 'ana.villacis@gmail.com'),
('Luis', 'Quispe', 'Ambato - Centro', '0995555555', 'luis.quispe@gmail.com');

-- =========================
-- DATOS INICIALES: PRODUCTOS
-- =========================
INSERT INTO Productos (Nombre, Precio, Stock)
VALUES
('Laptop Lenovo', 650.00, 10),
('Mouse Logitech', 15.50, 30),
('Teclado Redragon', 28.75, 20),
('Monitor Samsung 24', 180.00, 8),
('Memoria USB 32GB', 9.99, 50),
('Disco SSD 500GB', 55.00, 15),
('Audifonos Sony', 35.90, 12),
('Impresora Epson', 210.00, 5);

-- =========================
-- DATOS INICIALES: FACTURAS
-- =========================
INSERT INTO Facturas (NumeroFactura, Fecha, IdCliente, Subtotal, Iva, Total)
VALUES
(1001, GETDATE(), 1, 681.00, 102.15, 783.15),
(1002, GETDATE(), 2, 45.49, 6.82, 52.31);

-- =========================
-- DETALLE FACTURA 1001
-- =========================
INSERT INTO DetalleFactura (IdFactura, IdProducto, Cantidad, PrecioUnitario, TotalLinea)
VALUES
(1, 1, 1, 650.00, 650.00),
(1, 2, 2, 15.50, 31.00);

-- =========================
-- DETALLE FACTURA 1002
-- =========================
INSERT INTO DetalleFactura (IdFactura, IdProducto, Cantidad, PrecioUnitario, TotalLinea)
VALUES
(2, 5, 1, 9.99, 9.99),
(2, 3, 1, 28.75, 28.75),
(2, 2, 1, 15.50, 15.50);

-- =========================
-- CONSULTAS DE PRUEBA
-- =========================
SELECT * FROM Clientes;
SELECT * FROM Productos;
SELECT * FROM Facturas;
SELECT * FROM DetalleFactura;

-- Vista completa de facturas
SELECT 
    F.NumeroFactura,
    F.Fecha,
    C.Nombre + ' ' + C.Apellido AS Cliente,
    P.Nombre AS Producto,
    D.Cantidad,
    D.PrecioUnitario,
    D.TotalLinea,
    F.Subtotal,
    F.Iva,
    F.Total
FROM Facturas F
INNER JOIN Clientes C ON F.IdCliente = C.IdCliente
INNER JOIN DetalleFactura D ON F.IdFactura = D.IdFactura
INNER JOIN Productos P ON D.IdProducto = P.IdProducto
ORDER BY F.IdFactura, D.IdDetalleFactura;
```
## 🔌 Configurar conexión

"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=SistemaFacturacionDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

## 📁 Configuración de la ruta de guardado de facturas PDF:
La ruta donde se guardan automáticamente las facturas en PDF se encuentra definida en el archivo BackEnd\WebAPI\Controllers\FacturaController.cs, específicamente dentro del método GenerarPdf. En este método existe una línea donde se establece la carpeta de destino.

Por ejemplo:

var carpeta = @"C:\Users\stali\Desktop\Facturas";

## ▶️ Ejecutar el proyecto
dotnet run
