CREATE DATABASE Testv1_MinosaProyecto;
GO
USE Testv1_MinosaProyecto;
GO

CREATE TABLE [Equipos] (
  [ID_equipo] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [NumeroSerie] nvarchar(255),
  [Descripcion] nvarchar(255),
  [id_tipo_equipo] integer,
  [id_modelo] integer,
  [id_area] integer,
  [id_ip] integer,
  [Estado] nvarchar(255),
  [Activo] bit DEFAULT (1),
  [Respaldo] text,
  [Observaciones] nvarchar(255)
)
GO

CREATE TABLE [Usuarios] (
  [ID_usuario] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Usuario] nvarchar(255),
  [Contrasena] nvarchar(255),
  [id_rol] integer
)
GO

CREATE TABLE [Roles] (
  [ID_rol] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Rol] nvarchar(255),
  [Descripcion] nvarchar(255)
)
GO

CREATE TABLE [Zonas] (
  [ID_zona] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Zona] nvarchar(255),
  [Descripcion_Zona] nvarchar(255),
  [Activo] bit DEFAULT (1),
  [Creacion_Zona] datetime
)
GO

CREATE TABLE [vlans] (
  [ID_vlan] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Vlan] nvarchar(255),
  [SubNet] nvarchar(255),
  [Gateway] nvarchar(255),
  [DhcpIni] nvarchar(255),
  [DhcpFin] nvarchar(255),
  [Observaciones] nvarchar(255),
  [Activo] bit DEFAULT (1),
  [Creacion_vlan] datetime
)
GO

CREATE TABLE [Tipo_Equipos] (
  [ID_tipo_equipo] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Tipo_Equipo] nvarchar(255),
  [Creacion_Tipo_Equipo] datetime
)
GO

CREATE TABLE [Proveedores] (
  [ID_proveedor] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre] nvarchar(255),
  [Direccion] nvarchar(255),
  [Telefono] nvarchar(255),
  [Correo] nvarchar(255),
  [Activo] bit DEFAULT (1)
)
GO

CREATE TABLE [Modelos] (
  [ID_modelo] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Modelo] nvarchar(255),
  [id_Marca] integer,
  [Activo] bit DEFAULT (1)
)
GO

CREATE TABLE [Materiales] (
  [ID_Material] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Descripcion] nvarchar(255),
  [Cantidad] integer,
  [id_area] integer
)
GO

CREATE TABLE [Marcas] (
  [ID_marca] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Marca] nvarchar(255),
  [Activa] bit DEFAULT (1)
)
GO

CREATE TABLE [DireccionesIp] (
  [ID_ip] integer PRIMARY KEY IDENTITY(1, 1),
  [IPV4] nvarchar(255),
  [Estado] nvarchar(255),
  [id_vlan] integer,
  [Activa] bit DEFAULT (1),
  [ping] bit DEFAULT (1), -- nuevo campo para ping
  [UltimaHoraPing] datetime
)
GO

CREATE TABLE [Areas] (
  [ID_area] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Area] nvarchar(255),
  [id_zona] integer,
  [Activo] bit DEFAULT (1)
)
GO

CREATE TABLE [Detalle_Equipo] (
  [ID_detalle_equipo] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Tipo_Voltaje] nvarchar(255),
  [Voltaje] integer,
  [Amperaje] integer,
  [Num_Puertos_RJ45] integer,
  [Num_Puertos_SFP] integer,
  [Fecha_Compra] datetime,
  [Fecha_Garantia] datetime,
  [Tipo_Garantia] nvarchar(255),
  [Canal] nvarchar(255),
  [Firmware] nvarchar(255),
  [Usuario] nvarchar(255),
  [Contracena] nvarchar(255),
  [MAC_Address] nvarchar(255),
  [Fecha_Instalacion] nvarchar(255),
  [Ultima_Actualizacion] nvarchar(255),
  [Voltaje_Energia] nvarchar(255),
  [id_proveedor] integer,
  [id_equipo] integer
)
GO

CREATE TABLE [Radio] (
  [ID_Radio] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Frecuencia] nvarchar(255),
  [Frecuencia_Rango] nvarchar(255),
  [Modo] nvarchar(255),
  [Ssid] nvarchar(255),
  [Modulacion] nvarchar(255),
  [Potencia] nvarchar(255),
  [Tx_Power] nvarchar(255),
  [Rx_Level] nvarchar(255),
  [Tx_Freq] nvarchar(255),
  [id_equipo] integer
)
GO

CREATE TABLE [Destinatarios] (
  [ID_destinatario] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Destinatario] nvarchar(255),
  [Correo_Destinatario] nvarchar(255),
  [Descripcion_Destinatario] nvarchar(255),
  [id_alerta] integer
)
GO

CREATE TABLE [Tipo_Alerta] (
  [ID_alerta] integer PRIMARY KEY NOT NULL IDENTITY(1, 1),
  [Nombre_Alerta] nvarchar(255),
  [Descripcion_Alerta] nvarchar(255)
)
GO

CREATE TABLE [HistorialPings] (
    [ID_HistorialPing] INTEGER PRIMARY KEY IDENTITY(1, 1),
    [ip] NVARCHAR(50), -- ID de la dirección IP asociada
    [HoraPing] DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora del ping
    [ResultadoPing] BIT -- 1 si el ping fue exitoso, 0 si falló

)
GO

ALTER TABLE [Equipos] ADD FOREIGN KEY ([id_tipo_equipo]) REFERENCES [Tipo_Equipos] ([ID_tipo_equipo])
GO

ALTER TABLE [Equipos] ADD FOREIGN KEY ([id_modelo]) REFERENCES [Modelos] ([ID_modelo])
GO

ALTER TABLE [Modelos] ADD FOREIGN KEY ([id_Marca]) REFERENCES [Marcas] ([ID_marca])
GO

ALTER TABLE [DireccionesIp] ADD FOREIGN KEY ([id_vlan]) REFERENCES [vlans] ([ID_vlan])
GO

ALTER TABLE [Equipos] ADD FOREIGN KEY ([id_area]) REFERENCES [Areas] ([ID_area])
GO

ALTER TABLE [Materiales] ADD FOREIGN KEY ([id_area]) REFERENCES [Areas] ([ID_area])
GO

ALTER TABLE [Detalle_Equipo] ADD FOREIGN KEY ([id_proveedor]) REFERENCES [Proveedores] ([ID_proveedor])
GO

ALTER TABLE [Detalle_Equipo] ADD FOREIGN KEY ([id_equipo]) REFERENCES [Equipos] ([ID_equipo])
GO

ALTER TABLE [Areas] ADD FOREIGN KEY ([id_zona]) REFERENCES [Zonas] ([ID_zona])
GO

ALTER TABLE [Equipos] ADD FOREIGN KEY ([id_ip]) REFERENCES [DireccionesIp] ([ID_ip])
GO

ALTER TABLE [Destinatarios] ADD FOREIGN KEY ([id_alerta]) REFERENCES [Tipo_Alerta] ([ID_alerta])
GO

ALTER TABLE [Usuarios] ADD FOREIGN KEY ([id_rol]) REFERENCES [Roles] ([ID_rol])
GO

ALTER TABLE [Radio] ADD FOREIGN KEY ([id_equipo]) REFERENCES [Equipos] ([ID_equipo])


/*Insertar los datos*/
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([ID_rol], [Nombre_Rol], [Descripcion]) VALUES (1, N'Admin', N'Acceso completo al sistema')
INSERT [dbo].[Roles] ([ID_rol], [Nombre_Rol], [Descripcion]) VALUES (2, N'Usuario', N'Acceso limitado a ciertas funciones')
INSERT [dbo].[Roles] ([ID_rol], [Nombre_Rol], [Descripcion]) VALUES (3, N'Supervisor', N'Acceso administrado')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO

SET IDENTITY_INSERT [dbo].[Tipo_Alerta] ON 

INSERT [dbo].[Tipo_Alerta] ([ID_alerta], [Nombre_Alerta], [Descripcion_Alerta]) VALUES (1, N'Desconexión', N'Alerta de desconexión de dispositivo')
SET IDENTITY_INSERT [dbo].[Tipo_Alerta] OFF
GO

SET IDENTITY_INSERT [dbo].[Tipo_Equipos] ON 
INSERT [dbo].[Tipo_Equipos] ([ID_tipo_equipo], [Tipo_Equipo], [Creacion_Tipo_Equipo]) VALUES (1, N'Radio', CAST(N'2024-10-31T10:35:11.203' AS DateTime))

SET IDENTITY_INSERT [dbo].[Tipo_Equipos] OFF
GO
SET IDENTITY_INSERT [dbo].[Usuarios] ON 

INSERT [dbo].[Usuarios] ([ID_usuario], [Nombre_Usuario], [Contrasena], [id_rol]) VALUES (1, N'admin', N'password', 1)
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO