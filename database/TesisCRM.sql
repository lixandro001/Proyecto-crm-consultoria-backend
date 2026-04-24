IF DB_ID('TesisCRM') IS NOT NULL
BEGIN
    ALTER DATABASE TesisCRM SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TesisCRM;
END
GO

CREATE DATABASE TesisCRM;
GO
USE TesisCRM;
GO

CREATE TABLE Roles(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL
);
GO
INSERT INTO Roles(Code, Name) VALUES ('ADMIN','Administrador'),('USER','Usuario');
GO

CREATE TABLE Users(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    FullName VARCHAR(150) NOT NULL,
    PasswordHash VARCHAR(200) NOT NULL,
    RoleCode VARCHAR(20) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY(RoleCode) REFERENCES Roles(Code)
);
GO
INSERT INTO Users(Username, FullName, PasswordHash, RoleCode, IsActive, FechaRegistro)
VALUES ('admin','Administrador General','123456','ADMIN',1,GETDATE());
GO

CREATE TABLE Clientes(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    DocumentoTipo VARCHAR(20) NOT NULL,
    DocumentoNumero VARCHAR(20) NOT NULL,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(120) NULL,
    Direccion VARCHAR(250) NULL,
    EstadoCliente VARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Servicios(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo VARCHAR(30) NOT NULL UNIQUE,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(250) NULL,
    PrecioBase DECIMAL(18,2) NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);
GO
INSERT INTO Servicios(Codigo,Nombre,Descripcion,PrecioBase,Activo,FechaRegistro)
VALUES
('ASESORIA','Asesoría','Asesoría de tesis y proyecto de investigación',1400,1,GETDATE()),
('COMPRA','Compra','Compra de materiales o productos relacionados',800,1,GETDATE()),
('VENTA','Venta de Equipos','Venta de laptops, impresoras, accesorios o equipos afines',1800,1,GETDATE()),
('SOPORTE','Soporte Técnico','Soporte técnico presencial o remoto',150,1,GETDATE());
GO

CREATE TABLE ClienteServicios(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    ServicioId INT NOT NULL,
    Observacion VARCHAR(250) NULL,
    EstadoProceso VARCHAR(30) NOT NULL DEFAULT 'EN_PROCESO',
    FechaAsignacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ClienteServicios_Clientes FOREIGN KEY(ClienteId) REFERENCES Clientes(Id),
    CONSTRAINT FK_ClienteServicios_Servicios FOREIGN KEY(ServicioId) REFERENCES Servicios(Id)
);
GO

CREATE TABLE PlantillasContrato(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombrePlantilla VARCHAR(150) NOT NULL,
    RutaArchivoWord VARCHAR(500) NOT NULL,
    Descripcion VARCHAR(250) NULL,
    Activa BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Contratos(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    ServicioId INT NOT NULL,
    PlantillaContratoId INT NULL,
    PrecioTotal DECIMAL(18,2) NOT NULL,
    MontoPagado DECIMAL(18,2) NOT NULL DEFAULT 0,
    FechaContrato DATE NOT NULL,
    FechaEntrega DATE NULL,
    EstadoContrato VARCHAR(30) NOT NULL DEFAULT 'PENDIENTE',
    FirmaRepresentante VARCHAR(250) NULL,
    FirmaCliente VARCHAR(250) NULL,
    RutaWordGenerado VARCHAR(500) NULL,
    RutaPdf VARCHAR(500) NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Contratos_Clientes FOREIGN KEY(ClienteId) REFERENCES Clientes(Id),
    CONSTRAINT FK_Contratos_Servicios FOREIGN KEY(ServicioId) REFERENCES Servicios(Id),
    CONSTRAINT FK_Contratos_Plantillas FOREIGN KEY(PlantillaContratoId) REFERENCES PlantillasContrato(Id)
);
GO

CREATE TABLE AgendaEventos(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    Titulo VARCHAR(150) NOT NULL,
    Descripcion VARCHAR(250) NULL,
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    TipoEvento VARCHAR(30) NOT NULL,
    RecordatorioActivo BIT NOT NULL DEFAULT 0,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_AgendaEventos_Clientes FOREIGN KEY(ClienteId) REFERENCES Clientes(Id)
);
GO

CREATE TABLE Pagos(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    ContratoId INT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    FechaPago DATE NOT NULL,
    TipoMovimiento VARCHAR(20) NOT NULL,
    EstadoPago VARCHAR(20) NOT NULL,
    MetodoPago VARCHAR(50) NULL,
    Observacion VARCHAR(250) NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Pagos_Clientes FOREIGN KEY(ClienteId) REFERENCES Clientes(Id),
    CONSTRAINT FK_Pagos_Contratos FOREIGN KEY(ContratoId) REFERENCES Contratos(Id)
);
GO

CREATE OR ALTER PROCEDURE usp_Auth_Login @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 Id, Username, FullName, PasswordHash, RoleCode, IsActive FROM Users WHERE Username = @Username;
END
GO

CREATE OR ALTER PROCEDURE usp_Users_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, FullName, RoleCode, IsActive, FechaRegistro FROM Users ORDER BY Id DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_Users_Insert
    @Username VARCHAR(50), @FullName VARCHAR(150), @PasswordHash VARCHAR(200), @RoleCode VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Users(Username, FullName, PasswordHash, RoleCode, IsActive, FechaRegistro)
    VALUES(@Username,@FullName,@PasswordHash,@RoleCode,1,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_Users_Update
    @Id INT, @FullName VARCHAR(150), @RoleCode VARCHAR(20), @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users SET FullName=@FullName, RoleCode=@RoleCode, IsActive=@IsActive WHERE Id=@Id;
END
GO

CREATE OR ALTER PROCEDURE usp_Clientes_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,Nombres,Apellidos,DocumentoTipo,DocumentoNumero,Telefono,Email,Direccion,EstadoCliente,FechaRegistro FROM Clientes ORDER BY Id DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_Clientes_GetById @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,Nombres,Apellidos,DocumentoTipo,DocumentoNumero,Telefono,Email,Direccion,EstadoCliente,FechaRegistro FROM Clientes WHERE Id=@Id;
END
GO

CREATE OR ALTER PROCEDURE usp_Clientes_Insert
    @Nombres VARCHAR(100), @Apellidos VARCHAR(100), @DocumentoTipo VARCHAR(20), @DocumentoNumero VARCHAR(20),
    @Telefono VARCHAR(20)=NULL, @Email VARCHAR(120)=NULL, @Direccion VARCHAR(250)=NULL, @EstadoCliente VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Clientes(Nombres,Apellidos,DocumentoTipo,DocumentoNumero,Telefono,Email,Direccion,EstadoCliente,FechaRegistro)
    VALUES(@Nombres,@Apellidos,@DocumentoTipo,@DocumentoNumero,@Telefono,@Email,@Direccion,@EstadoCliente,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_Clientes_Update
    @Id INT, @Nombres VARCHAR(100), @Apellidos VARCHAR(100), @DocumentoTipo VARCHAR(20), @DocumentoNumero VARCHAR(20),
    @Telefono VARCHAR(20)=NULL, @Email VARCHAR(120)=NULL, @Direccion VARCHAR(250)=NULL, @EstadoCliente VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Clientes
    SET Nombres=@Nombres, Apellidos=@Apellidos, DocumentoTipo=@DocumentoTipo, DocumentoNumero=@DocumentoNumero,
        Telefono=@Telefono, Email=@Email, Direccion=@Direccion, EstadoCliente=@EstadoCliente
    WHERE Id=@Id;
END
GO

CREATE OR ALTER PROCEDURE usp_Servicios_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,Codigo,Nombre,Descripcion,PrecioBase,Activo,FechaRegistro FROM Servicios ORDER BY Id DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_Servicios_Insert
    @Codigo VARCHAR(30), @Nombre VARCHAR(100), @Descripcion VARCHAR(250)=NULL, @PrecioBase DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Servicios(Codigo,Nombre,Descripcion,PrecioBase,Activo,FechaRegistro)
    VALUES(@Codigo,@Nombre,@Descripcion,@PrecioBase,1,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_ClienteServicios_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT cs.Id, cs.ClienteId, cs.ServicioId,
           CONCAT(c.Nombres,' ',c.Apellidos) AS ClienteNombre,
           s.Nombre AS ServicioNombre,
           cs.Observacion, cs.EstadoProceso, cs.FechaAsignacion
    FROM ClienteServicios cs
    INNER JOIN Clientes c ON c.Id = cs.ClienteId
    INNER JOIN Servicios s ON s.Id = cs.ServicioId
    ORDER BY cs.Id DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_ClienteServicios_Insert
    @ClienteId INT, @ServicioId INT, @Observacion VARCHAR(250)=NULL, @EstadoProceso VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ClienteServicios(ClienteId,ServicioId,Observacion,EstadoProceso,FechaAsignacion)
    VALUES(@ClienteId,@ServicioId,@Observacion,@EstadoProceso,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_PlantillasContrato_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,NombrePlantilla,RutaArchivoWord,Descripcion,Activa,FechaRegistro FROM PlantillasContrato ORDER BY Id DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_PlantillasContrato_Insert
    @NombrePlantilla VARCHAR(150), @RutaArchivoWord VARCHAR(500), @Descripcion VARCHAR(250)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PlantillasContrato(NombrePlantilla,RutaArchivoWord,Descripcion,Activa,FechaRegistro)
    VALUES(@NombrePlantilla,@RutaArchivoWord,@Descripcion,1,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_Contratos_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.Id, ct.ClienteId, ct.ServicioId, ct.PlantillaContratoId,
           CONCAT(c.Nombres,' ',c.Apellidos) AS ClienteNombre,
           s.Nombre AS ServicioNombre,
           ct.PrecioTotal, ct.MontoPagado, (ct.PrecioTotal - ct.MontoPagado) AS SaldoPendiente,
           ct.FechaContrato, ct.FechaEntrega, ct.EstadoContrato, ct.RutaWordGenerado, ct.RutaPdf, ct.FechaRegistro
    FROM Contratos ct
    INNER JOIN Clientes c ON c.Id = ct.ClienteId
    INNER JOIN Servicios s ON s.Id = ct.ServicioId
    ORDER BY ct.Id DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_Contratos_GetById @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.Id, ct.ClienteId, ct.ServicioId, ct.PlantillaContratoId,
           CONCAT(c.Nombres,' ',c.Apellidos) AS ClienteNombre,
           s.Nombre AS ServicioNombre,
           ct.PrecioTotal, ct.MontoPagado, (ct.PrecioTotal - ct.MontoPagado) AS SaldoPendiente,
           ct.FechaContrato, ct.FechaEntrega, ct.EstadoContrato, ct.RutaWordGenerado, ct.RutaPdf, ct.FechaRegistro
    FROM Contratos ct
    INNER JOIN Clientes c ON c.Id = ct.ClienteId
    INNER JOIN Servicios s ON s.Id = ct.ServicioId
    WHERE ct.Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE usp_Contratos_Insert
    @ClienteId INT, @ServicioId INT, @PlantillaContratoId INT = NULL, @PrecioTotal DECIMAL(18,2),
    @MontoPagado DECIMAL(18,2), @FechaContrato DATE, @FechaEntrega DATE = NULL, @EstadoContrato VARCHAR(30),
    @FirmaRepresentante VARCHAR(250)=NULL, @FirmaCliente VARCHAR(250)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Contratos(ClienteId,ServicioId,PlantillaContratoId,PrecioTotal,MontoPagado,FechaContrato,FechaEntrega,EstadoContrato,FirmaRepresentante,FirmaCliente,FechaRegistro)
    VALUES(@ClienteId,@ServicioId,@PlantillaContratoId,@PrecioTotal,@MontoPagado,@FechaContrato,@FechaEntrega,@EstadoContrato,@FirmaRepresentante,@FirmaCliente,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_Contratos_UpdateRutaWord @ContratoId INT, @RutaWordGenerado VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Contratos SET RutaWordGenerado=@RutaWordGenerado WHERE Id=@ContratoId;
END
GO

CREATE OR ALTER PROCEDURE usp_Contratos_UpdateRutaPdf @ContratoId INT, @RutaPdf VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Contratos SET RutaPdf=@RutaPdf WHERE Id=@ContratoId;
END
GO

CREATE OR ALTER PROCEDURE usp_Contrato_ObtenerDataPlantilla @ContratoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.Id AS ContratoId,
           CONCAT(c.Nombres,' ',c.Apellidos) AS ClienteNombreCompleto,
           c.DocumentoNumero AS ClienteDni,
           s.Nombre AS TipoServicio,
           ct.PrecioTotal,
           CAST(ROUND(ct.PrecioTotal * 0.30, 2) AS DECIMAL(18,2)) AS Pago1,
           CAST(ROUND(ct.PrecioTotal * 0.50, 2) AS DECIMAL(18,2)) AS Pago2,
           CAST(ROUND(ct.PrecioTotal * 0.20, 2) AS DECIMAL(18,2)) AS Pago3,
           ct.FechaEntrega,
           ct.FechaContrato AS FechaFirma,
           ISNULL(ct.FirmaRepresentante,'') AS FirmaRepresentante,
           ISNULL(ct.FirmaCliente,'') AS FirmaCliente,
           ISNULL(pc.RutaArchivoWord,'') AS RutaPlantillaWord
    FROM Contratos ct
    INNER JOIN Clientes c ON c.Id = ct.ClienteId
    INNER JOIN Servicios s ON s.Id = ct.ServicioId
    LEFT JOIN PlantillasContrato pc ON pc.Id = ct.PlantillaContratoId
    WHERE ct.Id = @ContratoId;
END
GO

CREATE OR ALTER PROCEDURE usp_Agenda_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.Id, a.ClienteId, CONCAT(c.Nombres,' ',c.Apellidos) AS ClienteNombre,
           a.Titulo, a.Descripcion, a.FechaInicio, a.FechaFin, a.TipoEvento, a.RecordatorioActivo, a.FechaRegistro
    FROM AgendaEventos a
    INNER JOIN Clientes c ON c.Id = a.ClienteId
    ORDER BY a.FechaInicio ASC;
END
GO

CREATE OR ALTER PROCEDURE usp_Agenda_Insert
    @ClienteId INT, @Titulo VARCHAR(150), @Descripcion VARCHAR(250)=NULL, @FechaInicio DATETIME, @FechaFin DATETIME, @TipoEvento VARCHAR(30), @RecordatorioActivo BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AgendaEventos(ClienteId,Titulo,Descripcion,FechaInicio,FechaFin,TipoEvento,RecordatorioActivo,FechaRegistro)
    VALUES(@ClienteId,@Titulo,@Descripcion,@FechaInicio,@FechaFin,@TipoEvento,@RecordatorioActivo,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_Pagos_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.Id, p.ClienteId, p.ContratoId, CONCAT(c.Nombres,' ',c.Apellidos) AS ClienteNombre,
           p.Monto, p.FechaPago, p.TipoMovimiento, p.EstadoPago, p.MetodoPago, p.Observacion, p.FechaRegistro
    FROM Pagos p
    INNER JOIN Clientes c ON c.Id = p.ClienteId
    ORDER BY p.Id DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_Pagos_Insert
    @ClienteId INT, @ContratoId INT = NULL, @Monto DECIMAL(18,2), @FechaPago DATE,
    @TipoMovimiento VARCHAR(20), @EstadoPago VARCHAR(20), @MetodoPago VARCHAR(50)=NULL, @Observacion VARCHAR(250)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Pagos(ClienteId,ContratoId,Monto,FechaPago,TipoMovimiento,EstadoPago,MetodoPago,Observacion,FechaRegistro)
    VALUES(@ClienteId,@ContratoId,@Monto,@FechaPago,@TipoMovimiento,@EstadoPago,@MetodoPago,@Observacion,GETDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE usp_Dashboard_Resumen
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(1) FROM Clientes) AS TotalClientes,
        (SELECT COUNT(1) FROM Servicios WHERE Activo = 1) AS TotalServiciosActivos,
        (SELECT COUNT(1) FROM Contratos) AS TotalContratos,
        (SELECT ISNULL(SUM(Monto),0) FROM Pagos WHERE TipoMovimiento='INGRESO' AND EstadoPago='PAGADO') AS TotalCobrado,
        (SELECT ISNULL(SUM(PrecioTotal - MontoPagado),0) FROM Contratos WHERE PrecioTotal > MontoPagado) AS TotalDeuda,
        (SELECT COUNT(1) FROM AgendaEventos WHERE FechaInicio >= GETDATE()) AS TotalReunionesProgramadas;
END
GO
