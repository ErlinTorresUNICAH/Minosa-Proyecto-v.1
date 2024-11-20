-- Procedimiento: [dbo].[P_GRUD_ActualizarProveedor]
-- Actualizar proveedor

CREATE PROCEDURE P_GRUD_ActualizarProveedor

    @ID_proveedor INT,

    @Nombre NVARCHAR(255),

    @Direccion NVARCHAR(255),

    @Telefono NVARCHAR(255),

    @Correo NVARCHAR(255)

AS

BEGIN

    UPDATE Proveedores

    SET Nombre = @Nombre, Direccion = @Direccion, Telefono = @Telefono, Correo = @Correo

    WHERE ID_proveedor = @ID_proveedor;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerRadioPorID]
CREATE PROCEDURE P_GRUD_ObtenerRadioPorID

    @ID_Radio INT

AS

BEGIN

    SELECT 

        Radio.ID_Radio, 

        Radio.Frecuencia, 

        Radio.Frecuencia_Rango, 

        Radio.Modo, 

        Radio.Ssid, 

        Radio.Modulacion, 

        Radio.Potencia, 

        Radio.Tx_Power, 

        Radio.Rx_Level, 

        Radio.Tx_Freq, 

        Radio.id_equipo, 

        Equipos.Descripcion

    FROM Radio

    INNER JOIN Equipos ON Radio.id_equipo = Equipos.ID_equipo

    WHERE Radio.ID_Radio = @ID_Radio

END
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarProveedor]
-- Eliminar proveedor

CREATE PROCEDURE [dbo].[P_GRUD_EliminarProveedor]

    @ID_proveedor INT

AS

BEGIN

    BEGIN TRY

        BEGIN TRANSACTION;



        -- Actualizar el estado del proveedor a inactivo

        UPDATE Proveedores 

        SET Activo = 0 

        WHERE ID_proveedor = @ID_proveedor;



        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0

        BEGIN

            ROLLBACK TRANSACTION;

        END

        -- Manejar y relanzar el error capturado

        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarRadio]
CREATE PROCEDURE [dbo].[P_GRUD_ActualizarRadio]

    @ID_Radio INT,

    @Frecuencia NVARCHAR(50),

    @Frecuencia_Rango NVARCHAR(50),

    @Modo NVARCHAR(50),

    @Ssid NVARCHAR(50),

    @Modulacion NVARCHAR(50),

    @Potencia NVARCHAR(50),

    @Tx_Power NVARCHAR(50),

    @Rx_Level NVARCHAR(50),

    @Tx_Freq NVARCHAR(50),

    @ID_equipo INT,

    @NumeroSerie NVARCHAR(50),

    @Descripcion NVARCHAR(200),

    @Estado NVARCHAR(50),

    @Activo BIT,

    @Respaldo NVARCHAR(50),

    @Observaciones NVARCHAR(200),

    @Tipo_Voltaje NVARCHAR(50),

    @Voltaje INT,

    @Amperaje INT,

    @Num_Puertos_RJ45 INT,

    @Num_Puertos_SFP INT,

    @Fecha_Compra DATE,

    @Fecha_Garantia DATE,

    @Tipo_Garantia NVARCHAR(50),

    @Canal NVARCHAR(50),

    @Firmware NVARCHAR(50),

    @Usuario NVARCHAR(50),

    @Contracena NVARCHAR(50),

    @MAC_Address NVARCHAR(50),

    @Fecha_Instalacion NVARCHAR(50),

    @Ultima_Actualizacion NVARCHAR(50),

    @Voltaje_Energia NVARCHAR(50),

    @ID_proveedor INT,

    @ID_modelo INT,

    @ID_area INT,

    @ID_ip INT

AS

BEGIN

    -- Update Radio table

    UPDATE Radio

    SET 

        Frecuencia = @Frecuencia,

        Frecuencia_Rango = @Frecuencia_Rango,

        Modo = @Modo,

        Ssid = @Ssid,

        Modulacion = @Modulacion,

        Potencia = @Potencia,

        Tx_Power = @Tx_Power,

        Rx_Level = @Rx_Level,

        Tx_Freq = @Tx_Freq

    WHERE 

        ID_Radio = @ID_Radio;



    -- Update Equipos table

    UPDATE Equipos

    SET 

        NumeroSerie = @NumeroSerie,

        Descripcion = @Descripcion,

        Estado = @Estado,

        Activo = @Activo,

        Respaldo = @Respaldo,

        Observaciones = @Observaciones,

        id_modelo = @ID_modelo,

        id_area = @ID_area,

        id_ip = @ID_ip

    WHERE 

        ID_equipo = @ID_equipo;



    -- Update Detalle_Equipo table based on ID_equipo

    UPDATE Detalle_Equipo

    SET 

        Tipo_Voltaje = @Tipo_Voltaje,

        Voltaje = @Voltaje,

        Amperaje = @Amperaje,

        Num_Puertos_RJ45 = @Num_Puertos_RJ45,

        Num_Puertos_SFP = @Num_Puertos_SFP,

        Fecha_Compra = @Fecha_Compra,

        Fecha_Garantia = @Fecha_Garantia,

        Tipo_Garantia = @Tipo_Garantia,

        Canal = @Canal,

        Firmware = @Firmware,

        Usuario = @Usuario,

        Contracena = @Contracena,

        MAC_Address = @MAC_Address,

        Fecha_Instalacion = @Fecha_Instalacion,

        Ultima_Actualizacion = @Ultima_Actualizacion,

        Voltaje_Energia = @Voltaje_Energia,

        id_proveedor = @ID_proveedor

    WHERE 

        id_equipo = @ID_equipo;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerProveedorPorID]
CREATE PROCEDURE P_GRUD_ObtenerProveedorPorID

    @ID_proveedor INT

AS

BEGIN

    SELECT * 

    FROM Proveedores 

    WHERE ID_proveedor = @ID_proveedor;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarRadio]
CREATE PROCEDURE [dbo].[P_GRUD_EliminarRadio]

    @ID_Radio INT

AS

BEGIN

    BEGIN TRY

        -- Verificar si existen equipos asociados al radio

        IF EXISTS (SELECT 1 FROM Equipos WHERE id_equipo IN (SELECT id_equipo FROM Radio WHERE ID_Radio = @ID_Radio))

        BEGIN

            -- Lanzar un error si hay dependencias asociadas

            RAISERROR('No se puede eliminar el radio porque hay equipos asociados.', 16, 1);

            RETURN;

        END



        BEGIN TRANSACTION;



        -- Eliminar el registro de Radio

        DELETE FROM Radio WHERE ID_Radio = @ID_Radio;



        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0

        BEGIN

            ROLLBACK TRANSACTION;

        END

        -- Relanzar el error capturado

        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH

END
GO

-- Procedimiento: [dbo].[P_EliminarEquipoYDependencias]
CREATE PROCEDURE P_EliminarEquipoYDependencias

    @ID_equipo INT

AS

BEGIN

    DELETE FROM Detalle_Equipo WHERE ID_equipo = @ID_equipo;

    DELETE FROM Equipos WHERE ID_equipo = @ID_equipo;

END
GO

-- Procedimiento: [dbo].[P_ObtenerActividadTotal]
CREATE PROCEDURE [dbo].[P_ObtenerActividadTotal]

AS

BEGIN

SELECT

    di.ID_ip AS IDDireccionIP,

    di.IPV4 AS DireccionIP,

    e.ID_equipo,

    e.Descripcion AS DescripcionEquipo,

    a.Nombre_Area AS Area,

    te.Tipo_Equipo AS TipoEquipo,

    di.ping AS Ping

FROM 

    DireccionesIp di

LEFT JOIN 

    Equipos e ON di.ID_ip = e.id_ip

LEFT JOIN 

    Areas a ON e.id_area = a.ID_area

LEFT JOIN 

    Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo



END;
GO

-- Procedimiento: [dbo].[P_ObtenerRadiosConDetalles]
CREATE PROCEDURE P_ObtenerRadiosConDetalles

AS

BEGIN

    SELECT 

        R.ID_Radio, 

        R.Frecuencia, 

        R.Frecuencia_Rango, 

        R.Modo, 

        R.Ssid, 

        R.Modulacion, 

        R.Potencia, 

        R.Tx_Power, 

        R.Rx_Level, 

        R.Tx_Freq, 

        E.Descripcion AS Nombre_Equipo, 

        DE.Tipo_Voltaje, 

        DE.Voltaje, 

        DE.Amperaje, 

        DE.Num_Puertos_RJ45, 

        DE.Num_Puertos_SFP, 

        DE.Fecha_Compra, 

        DE.Fecha_Garantia

    FROM Radio R

    INNER JOIN Equipos E ON R.id_equipo = E.ID_equipo

    INNER JOIN Detalle_Equipo DE ON E.ID_equipo = DE.id_equipo

END
GO

-- Procedimiento: [dbo].[P_ObtenerActividadConTiempo]
create PROCEDURE [dbo].[P_ObtenerActividadConTiempo]

AS

BEGIN

SELECT

    e.ID_equipo,

    di.IPV4 AS DireccionIP,

    a.Nombre_Area AS Area,

    e.Descripcion AS DescripcionEquipo,

    te.Tipo_Equipo AS TipoEquipo,

    di.ping AS Ping,

    di.UltimaHoraPing AS UltimaHoraPing

FROM 

    Equipos e

JOIN 

    DireccionesIp di ON e.id_ip = di.ID_ip

JOIN 

    Areas a ON e.id_area = a.ID_area

JOIN 

    Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

WHERE 

    di.Activa = 1

    AND

    e.Activo = 1

END;
GO

-- Procedimiento: [dbo].[P_ActualizarEquipo]
CREATE PROCEDURE P_ActualizarEquipo

    @ID_equipo INT,

    @NumeroSerie NVARCHAR(255),

    @Descripcion NVARCHAR(255),

    @id_tipo_equipo INT,

    @id_modelo INT,

    @id_area INT,

    @id_ip INT,

    @Estado NVARCHAR(255),

    @Activo BIT,

    @Respaldo TEXT,

    @Observaciones NVARCHAR(255),

    @Tipo_Voltaje NVARCHAR(50),

    @Voltaje INT,

    @Amperaje INT,

    @Num_Puertos_RJ45 INT,

    @Num_Puertos_SFP INT,

    @Fecha_Compra DATE,

    @Fecha_Garantia DATE,

    @Tipo_Garantia NVARCHAR(100),

    @Canal NVARCHAR(50),

    @Firmware NVARCHAR(255),

    @Usuario NVARCHAR(255),

    @Contracena NVARCHAR(255),

    @MAC_Address NVARCHAR(50),

    @Fecha_Instalacion NVARCHAR(50),

    @Ultima_Actualizacion NVARCHAR(50),

    @Voltaje_Energia NVARCHAR(50),

    @ID_proveedor INT

AS

BEGIN

    UPDATE Equipos

    SET 

        NumeroSerie = @NumeroSerie,

        Descripcion = @Descripcion,

        id_tipo_equipo = @id_tipo_equipo,

        id_modelo = @id_modelo,

        id_area = @id_area,

        id_ip = @id_ip,

        Estado = @Estado,

        Activo = @Activo,

        Respaldo = @Respaldo,

        Observaciones = @Observaciones

    WHERE ID_equipo = @ID_equipo;



    UPDATE Detalle_Equipo

    SET 

        Tipo_Voltaje = @Tipo_Voltaje,

        Voltaje = @Voltaje,

        Amperaje = @Amperaje,

        Num_Puertos_RJ45 = @Num_Puertos_RJ45,

        Num_Puertos_SFP = @Num_Puertos_SFP,

        Fecha_Compra = @Fecha_Compra,

        Fecha_Garantia = @Fecha_Garantia,

        Tipo_Garantia = @Tipo_Garantia,

        Canal = @Canal,

        Firmware = @Firmware,

        Usuario = @Usuario,

        Contracena = @Contracena,

        MAC_Address = @MAC_Address,

        Fecha_Instalacion = @Fecha_Instalacion,

        Ultima_Actualizacion = @Ultima_Actualizacion,

        Voltaje_Energia = @Voltaje_Energia,

        ID_proveedor = @ID_proveedor

    WHERE ID_equipo = @ID_equipo;

END;
GO

-- Procedimiento: [dbo].[P_ActualizarEquipoDetalle]
CREATE PROCEDURE P_ActualizarEquipoDetalle

    @ID_equipo INT,

    @NumeroSerie NVARCHAR(255),

    @Descripcion NVARCHAR(255),

    @id_tipo_equipo INT,

    @id_modelo INT,

    @id_area INT,

    @id_ip INT,

    @Estado NVARCHAR(255),

    @Activo BIT,

    @Respaldo TEXT,

    @Observaciones NVARCHAR(255),

    @Tipo_Voltaje NVARCHAR(255),

    @Voltaje INT,

    @Amperaje INT,

    @Num_Puertos_RJ45 INT,

    @Num_Puertos_SFP INT,

    @Fecha_Compra DATETIME,

    @Fecha_Garantia DATETIME,

    @Tipo_Garantia NVARCHAR(255),

    @Canal NVARCHAR(255),

    @Firmware NVARCHAR(255),

    @Usuario NVARCHAR(255),

    @Contracena NVARCHAR(255),

    @MAC_Address NVARCHAR(255),

    @Fecha_Instalacion NVARCHAR(255),

    @Ultima_Actualizacion NVARCHAR(255),

    @Voltaje_Energia NVARCHAR(255),

    @id_proveedor INT

AS

BEGIN

    BEGIN TRANSACTION;



    -- Actualizar la tabla Equipos

    UPDATE Equipos

    SET 

        NumeroSerie = @NumeroSerie,

        Descripcion = @Descripcion,

        id_tipo_equipo = @id_tipo_equipo,

        id_modelo = @id_modelo,

        id_area = @id_area,

        id_ip = @id_ip,

        Estado = @Estado,

        Activo = @Activo,

        Respaldo = @Respaldo,

        Observaciones = @Observaciones

    WHERE 

        ID_equipo = @ID_equipo;



    -- Actualizar la tabla Detalle_Equipo

    UPDATE Detalle_Equipo

    SET 

        Tipo_Voltaje = @Tipo_Voltaje,

        Voltaje = @Voltaje,

        Amperaje = @Amperaje,

        Num_Puertos_RJ45 = @Num_Puertos_RJ45,

        Num_Puertos_SFP = @Num_Puertos_SFP,

        Fecha_Compra = @Fecha_Compra,

        Fecha_Garantia = @Fecha_Garantia,

        Tipo_Garantia = @Tipo_Garantia,

        Canal = @Canal,

        Firmware = @Firmware,

        Usuario = @Usuario,

        Contracena = @Contracena,

        MAC_Address = @MAC_Address,

        Fecha_Instalacion = @Fecha_Instalacion,

        Ultima_Actualizacion = @Ultima_Actualizacion,

        Voltaje_Energia = @Voltaje_Energia,

        id_proveedor = @id_proveedor

    WHERE 

        id_equipo = @ID_equipo;



    -- Confirmar la transacción

    COMMIT TRANSACTION;

END;
GO

-- Procedimiento: [dbo].[P_ObtenerEquipoDetalle]
CREATE PROCEDURE P_ObtenerEquipoDetalle

    @ID_equipo INT

AS

BEGIN

    SELECT 

        e.ID_equipo,

        e.NumeroSerie,

        e.Descripcion,

        e.id_tipo_equipo,

        e.id_modelo,

        e.id_area,

        e.id_ip,

        e.Estado,

        e.Activo,

        e.Respaldo,

        e.Observaciones,

        de.ID_detalle_equipo,

        de.Tipo_Voltaje,

        de.Voltaje,

        de.Amperaje,

        de.Num_Puertos_RJ45,

        de.Num_Puertos_SFP,

        de.Fecha_Compra,

        de.Fecha_Garantia,

        de.Tipo_Garantia,

        de.Canal,

        de.Firmware,

        de.Usuario,

        de.Contracena,

        de.MAC_Address,

        de.Fecha_Instalacion,

        de.Ultima_Actualizacion,

        de.Voltaje_Energia,

        de.id_proveedor,

        p.Nombre AS Nombre_Proveedor,

        p.Direccion AS Direccion_Proveedor,

        p.Telefono AS Telefono_Proveedor,

        p.Correo AS Correo_Proveedor,

        m.Nombre_Modelo,

        a.Nombre_Area,

        ip.IPV4 AS Direccion_IP,

        te.Tipo_Equipo

    FROM 

        Equipos e

    INNER JOIN 

        Detalle_Equipo de ON e.ID_equipo = de.id_equipo

    LEFT JOIN 

        Proveedores p ON de.id_proveedor = p.ID_proveedor

    LEFT JOIN 

        Modelos m ON e.id_modelo = m.ID_modelo

    LEFT JOIN 

        Areas a ON e.id_area = a.ID_area

    LEFT JOIN 

        DireccionesIp ip ON e.id_ip = ip.ID_ip

    LEFT JOIN 

        Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

    WHERE 

        e.ID_equipo = @ID_equipo;

END;
GO

-- Procedimiento: [dbo].[P_EliminarEquipoCompleto]
CREATE PROCEDURE P_EliminarEquipoCompleto

    @ID_equipo INT

AS

BEGIN

    -- Iniciar una transacción para asegurar la atomicidad de la operación

    BEGIN TRANSACTION;

    

    BEGIN TRY

        -- Eliminar registros relacionados en la tabla Radio

        DELETE FROM Radio WHERE id_equipo = @ID_equipo;



        -- Eliminar registros relacionados en la tabla Detalle_Equipo

        DELETE FROM Detalle_Equipo WHERE id_equipo = @ID_equipo;



        -- Eliminar el equipo de la tabla Equipos

        DELETE FROM Equipos WHERE ID_equipo = @ID_equipo;



        -- Confirmar los cambios si no hubo errores

        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        -- Revertir los cambios si ocurrió algún error

        ROLLBACK TRANSACTION;

        -- Opcional: Propagar el mensaje de error

        THROW;

    END CATCH

END;
GO

-- Procedimiento: [dbo].[P_InsertarHistorialPing]
CREATE PROCEDURE [dbo].[P_InsertarHistorialPing]

    @ip NVARCHAR(50),  -- Dirección IP asociada

    @HoraPing DATETIME, -- Nuevo parámetro para el tiempo

    @ResultadoPing BIT  -- Resultado del ping

AS

BEGIN

    INSERT INTO [HistorialPings] ([ip], [HoraPing], [ResultadoPing])

    VALUES (@ip, @HoraPing, @ResultadoPing);

END
GO

-- Procedimiento: [dbo].[P_InsertarRadio]
CREATE PROCEDURE P_InsertarRadio

    @Frecuencia NVARCHAR(255),

    @Frecuencia_Rango NVARCHAR(255),

    @Modo NVARCHAR(255),

    @Ssid NVARCHAR(255),

    @Modulacion NVARCHAR(255),

    @Potencia NVARCHAR(255),

    @Tx_Power NVARCHAR(255),

    @Rx_Level NVARCHAR(255),

    @Tx_Freq NVARCHAR(255),

    @id_equipo INT

AS

BEGIN

    INSERT INTO Radio (Frecuencia, Frecuencia_Rango, Modo, Ssid, Modulacion, Potencia, Tx_Power, Rx_Level, Tx_Freq, id_equipo)

    VALUES (@Frecuencia, @Frecuencia_Rango, @Modo, @Ssid, @Modulacion, @Potencia, @Tx_Power, @Rx_Level, @Tx_Freq, @id_equipo);

END;
GO

-- Procedimiento: [dbo].[P_ObtenerHistorialPings]
CREATE PROCEDURE [dbo].[P_ObtenerHistorialPings]

    @ip NVARCHAR(50) = NULL -- Parámetro opcional para filtrar por IP específica

AS

BEGIN

    SELECT [ID_HistorialPing], [ip], [HoraPing], [ResultadoPing]

    FROM [HistorialPings]

    WHERE (@ip IS NULL OR [ip] = @ip)

    ORDER BY [HoraPing] DESC;

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerRadios]
CREATE PROCEDURE P_GRUD_ObtenerRadios

AS

BEGIN

    DECLARE @ID_tipo_equipo_radio INT;



    -- Obtener el ID del tipo de equipo para 'Radio'

    SELECT @ID_tipo_equipo_radio = ID_tipo_equipo

    FROM Tipo_Equipos

    WHERE Tipo_Equipo = 'Radio';



    -- Si el tipo de equipo 'Radio' no existe, detener el procedimiento

    IF @ID_tipo_equipo_radio IS NULL

    BEGIN

        RAISERROR('Tipo de equipo ''Radio'' no encontrado', 16, 1);

        RETURN;

    END



    -- Obtener los datos de los equipos que son radios, junto con sus detalles

    SELECT 

        r.ID_Radio,

        e.ID_equipo,

        e.NumeroSerie,

        e.Descripcion,

        e.Estado,

        e.Respaldo,

        e.Observaciones,

        e.Activo,

        de.ID_detalle_equipo,

        de.Tipo_Voltaje,

        de.Voltaje,

        de.Amperaje,

        de.Num_Puertos_RJ45,

        de.Num_Puertos_SFP,

        de.Fecha_Compra,

        de.Fecha_Garantia,

        de.Tipo_Garantia,

        de.Canal,

        de.Firmware,

        de.Usuario,

        de.Contracena,

        de.MAC_Address,

        de.Fecha_Instalacion,

        de.Ultima_Actualizacion,

        de.Voltaje_Energia,

        p.Nombre AS Nombre_Proveedor,

        p.Direccion AS Direccion_Proveedor,

        p.Telefono AS Telefono_Proveedor,

        p.Correo AS Correo_Proveedor,

        m.Nombre_Modelo,

        a.Nombre_Area,

        ip.IPV4 AS Direccion_IP,

        te.Tipo_Equipo

    FROM Radio r

    INNER JOIN Equipos e ON r.id_equipo = e.ID_equipo

    INNER JOIN Detalle_Equipo de ON e.ID_equipo = de.id_equipo

    INNER JOIN Proveedores p ON de.id_proveedor = p.ID_proveedor

    INNER JOIN Modelos m ON e.id_modelo = m.ID_modelo

    INNER JOIN Areas a ON e.id_area = a.ID_area

    INNER JOIN DireccionesIp ip ON e.id_ip = ip.ID_ip

    INNER JOIN Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

    WHERE e.ID_tipo_equipo = @ID_tipo_equipo_radio -- Usar el ID dinámico del tipo de equipo 'Radio'

    AND e.Activo = 1; -- Solo seleccionar los equipos activos

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerTiposEquipos]
CREATE PROCEDURE P_GRUD_ObtenerTiposEquipos AS

BEGIN

    SELECT ID_tipo_equipo, Tipo_Equipo, Creacion_Tipo_Equipo 

    FROM Tipo_Equipos;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_CrearTipoEquipo]
CREATE PROCEDURE P_GRUD_CrearTipoEquipo 

    @Tipo_Equipo NVARCHAR(255)

AS

BEGIN

    INSERT INTO Tipo_Equipos (Tipo_Equipo, Creacion_Tipo_Equipo) 

    VALUES (@Tipo_Equipo, GETDATE());

END;
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarTipoEquipo]
CREATE PROCEDURE P_GRUD_ActualizarTipoEquipo 

    @ID_TipoEquipo INT,

    @Tipo_Equipo NVARCHAR(255)

AS

BEGIN

    UPDATE Tipo_Equipos 

    SET Tipo_Equipo = @Tipo_Equipo 

    WHERE ID_tipo_equipo = @ID_TipoEquipo;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarTipoEquipo]
CREATE PROCEDURE [dbo].[P_GRUD_EliminarTipoEquipo] 

    @ID_TipoEquipo INT

AS

BEGIN

    BEGIN TRY

        -- Verificar si existen equipos asociados al tipo de equipo

        IF EXISTS (SELECT 1 FROM Equipos WHERE id_tipo_equipo = @ID_TipoEquipo)

        BEGIN

            -- Lanzar un error si hay equipos asociados

            RAISERROR('No se puede eliminar el tipo de equipo porque hay equipos asociados.', 16, 1);

            RETURN;

        END



        BEGIN TRANSACTION;



        -- Eliminar el registro del Tipo de Equipo

        DELETE FROM Tipo_Equipos 

        WHERE ID_tipo_equipo = @ID_TipoEquipo;



        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0

        BEGIN

            ROLLBACK TRANSACTION;

        END

        -- Manejar y relanzar el error capturado

        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerTipoEquipoPorID]
CREATE PROCEDURE P_GRUD_ObtenerTipoEquipoPorID

    @ID_TipoEquipo INT

AS

BEGIN

    SELECT ID_tipo_equipo, Tipo_Equipo, Creacion_Tipo_Equipo

    FROM Tipo_Equipos

    WHERE ID_tipo_equipo = @ID_TipoEquipo;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerMarcas]
CREATE PROCEDURE P_GRUD_ObtenerMarcas

AS

BEGIN

    SELECT ID_marca, Marca, Activa FROM Marcas;

END
GO

-- Procedimiento: [dbo].[P_GRUD_CrearMarca]
CREATE PROCEDURE P_GRUD_CrearMarca

    @Marca NVARCHAR(255)

AS

BEGIN

    INSERT INTO Marcas (Marca, Activa) VALUES (@Marca, 1);

END
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarMarca]
CREATE PROCEDURE [dbo].[P_GRUD_EliminarMarca]

    @ID_Marca INT

AS

BEGIN

    BEGIN TRY

        -- Verificar si hay modelos asociados a la marca

        IF EXISTS (SELECT 1 FROM Modelos WHERE id_Marca = @ID_Marca)

        BEGIN

            -- Lanzar un error si hay modelos asociados

            RAISERROR('No se puede eliminar la marca porque hay modelos asociados.', 16, 1);

            RETURN;

        END



        -- Eliminar la marca si no hay modelos asociados

        DELETE FROM Marcas WHERE ID_marca = @ID_Marca;

    END TRY

    BEGIN CATCH

        -- Manejo de errores

        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerModelos]
-- Obtener todos los modelos con su respectiva marca

CREATE PROCEDURE P_GRUD_ObtenerModelos

AS

BEGIN

    SELECT 

        Modelos.ID_modelo, 

        Modelos.Nombre_Modelo, 

        Marcas.Marca, 

        Modelos.Activo

    FROM Modelos

    INNER JOIN Marcas ON Modelos.id_Marca = Marcas.ID_marca

END
GO

-- Procedimiento: [dbo].[P_GRUD_CrearModelo]
-- Crear un nuevo modelo

CREATE PROCEDURE P_GRUD_CrearModelo

    @Nombre_Modelo NVARCHAR(255),

    @id_Marca INT

AS

BEGIN

    INSERT INTO Modelos (Nombre_Modelo, id_Marca) 

    VALUES (@Nombre_Modelo, @id_Marca)

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerModeloPorID]
-- Obtener un modelo por su ID

CREATE PROCEDURE P_GRUD_ObtenerModeloPorID

    @ID_modelo INT

AS

BEGIN

    SELECT 

        Modelos.ID_modelo, 

        Modelos.Nombre_Modelo, 

        Modelos.id_Marca, 

        Marcas.Marca, 

        Modelos.Activo

    FROM Modelos

    INNER JOIN Marcas ON Modelos.id_Marca = Marcas.ID_marca

    WHERE Modelos.ID_modelo = @ID_modelo

END
GO

-- Procedimiento: [dbo].[P_ObtenerUsuarioPorNombreYContrasena]
------------------------------PROCEDIMIENTOS ALMACENADOS---------------------------------------------

CREATE PROCEDURE P_ObtenerUsuarioPorNombreYContrasena

    @Nombre_Usuario nvarchar(255),

    @Contrasena nvarchar(255)

AS

BEGIN

    SELECT * 

    FROM Usuarios 

    WHERE Nombre_Usuario = @Nombre_Usuario AND Contrasena = @Contrasena;

END
GO

-- Procedimiento: [dbo].[P_GetEquiposPorArea_G]
CREATE PROCEDURE [dbo].[P_GetEquiposPorArea_G]

AS

BEGIN

    SELECT 

        a.Nombre_Area, 

        COUNT(e.ID_equipo) AS Cantidad

    FROM 

        Equipos e

    INNER JOIN 

        Areas a ON e.id_area = a.ID_area

    GROUP BY 

        a.Nombre_Area

END
GO

-- Procedimiento: [dbo].[P_ObtenerDatosEquipos]
CREATE PROCEDURE P_ObtenerDatosEquipos

AS

BEGIN

    SELECT 

        e.ID_equipo,

        e.NumeroSerie,

        e.Descripcion,

        t.Tipo_Equipo AS Tipo_Equipo,

        m.Nombre_Modelo AS Modelo,

        a.Nombre_Area AS Area,

        d.IPV4 AS Direccion_IP,

        e.Estado,

        e.Activo,

        e.Respaldo,

        e.Observaciones

    FROM 

        Equipos e

    LEFT JOIN 

        Tipo_Equipos t ON e.id_tipo_equipo = t.ID_tipo_equipo

    LEFT JOIN 

        Modelos m ON e.id_modelo = m.ID_modelo

    LEFT JOIN 

        Areas a ON e.id_area = a.ID_area

    LEFT JOIN 

        DireccionesIp d ON e.id_ip = d.ID_ip;

END
GO

-- Procedimiento: [dbo].[P_ObtenerUsuarioPorID]
CREATE PROCEDURE P_ObtenerUsuarioPorID

    @ID_usuario INT

AS

BEGIN

    SELECT 

        u.ID_usuario,

        u.Nombre_Usuario,

        u.Contrasena,

        r.Nombre_Rol,

        r.Descripcion

    FROM 

        Usuarios u

    LEFT JOIN 

        Roles r ON u.id_rol = r.ID_rol

    WHERE 

        u.ID_usuario = @ID_usuario;

END
GO

-- Procedimiento: [dbo].[P_GetEquiposPorZona_G]
CREATE PROCEDURE [dbo].[P_GetEquiposPorZona_G]

AS

BEGIN

    SELECT 

        z.Nombre_Zona, 

        COUNT(e.ID_equipo) AS Cantidad

    FROM 

        Equipos e

    INNER JOIN 

        Areas a ON e.id_area = a.ID_area

    INNER JOIN 

        Zonas z ON a.id_zona = z.ID_zona

    GROUP BY 

        z.Nombre_Zona

END
GO

-- Procedimiento: [dbo].[P_ActualizarUsuario]
CREATE PROCEDURE P_ActualizarUsuario

    @ID_usuario INT,

    @Nombre_Usuario NVARCHAR(255),

    @Contrasena NVARCHAR(255)

AS

BEGIN

    UPDATE Usuarios

    SET 

        Nombre_Usuario = @Nombre_Usuario,

        Contrasena = @Contrasena

    WHERE 

        ID_usuario = @ID_usuario;

END
GO

-- Procedimiento: [dbo].[P_GetEquiposPorAreaConTipo]
CREATE PROCEDURE P_GetEquiposPorAreaConTipo

AS

BEGIN

    SET NOCOUNT ON;



    SELECT 

        a.Nombre_Area AS Area,

        te.Tipo_Equipo AS TipoEquipo,

        COUNT(e.ID_equipo) AS Cantidad

    FROM 

        Equipos e

    INNER JOIN 

        Areas a ON e.id_area = a.ID_area

    INNER JOIN 

        Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

    GROUP BY 

        a.Nombre_Area, te.Tipo_Equipo

    ORDER BY 

        a.Nombre_Area, te.Tipo_Equipo;

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarModelo]
CREATE PROCEDURE P_GRUD_ActualizarModelo

    @ID_modelo INT,

    @Nombre_Modelo NVARCHAR(255),

    @id_Marca INT

AS

BEGIN

    -- Verificamos si existe el modelo antes de actualizar

    IF EXISTS (SELECT 1 FROM Modelos WHERE ID_modelo = @ID_modelo)

    BEGIN

        UPDATE Modelos

        SET Nombre_Modelo = @Nombre_Modelo,

            id_Marca = @id_Marca

        WHERE ID_modelo = @ID_modelo

    END

    ELSE

    BEGIN

        -- Si el modelo no existe, lanzamos un error

        RAISERROR ('El modelo con el ID especificado no existe.', 16, 1);

    END

END
GO

-- Procedimiento: [dbo].[ObtenerDetallesEquipoPorID]
CREATE PROCEDURE ObtenerDetallesEquipoPorID

    @ID_equipo INT

AS

BEGIN

    SELECT 

        e.ID_equipo,

        e.NumeroSerie,

        e.Descripcion,

        e.Estado,

        e.Activo,

        e.Respaldo,

        e.Observaciones,

        d.Tipo_Voltaje,

        d.Voltaje,

        d.Amperaje,

        d.Num_Puertos_RJ45,

        d.Num_Puertos_SFP,

        d.Fecha_Compra,

        d.Fecha_Garantia,

        d.Tipo_Garantia,

        d.Canal,

        d.Firmware,

        d.Usuario,

        d.Contracena,

        d.MAC_Address,

        d.Fecha_Instalacion,

        d.Ultima_Actualizacion,

        d.Voltaje_Energia,

        p.Nombre AS Nombre_Proveedor,

        p.Direccion AS Direccion_Proveedor,

        p.Telefono AS Telefono_Proveedor,

        p.Correo AS Correo_Proveedor

    FROM 

        Equipos e

    LEFT JOIN 

        Detalle_Equipo d ON e.ID_equipo = d.id_equipo

    LEFT JOIN 

        Proveedores p ON d.id_proveedor = p.ID_proveedor

    WHERE 

        e.ID_equipo = @ID_equipo;

END
GO

-- Procedimiento: [dbo].[P_EliminarModeloCompleto]
CREATE PROCEDURE P_EliminarModeloCompleto



    @ID_modelo INT

AS

BEGIN

    -- Iniciar una transacción para asegurar la atomicidad de la operación

    BEGIN TRANSACTION;

    

    BEGIN TRY

        -- Verificar si el modelo está asociado a algún equipo

        IF EXISTS (SELECT 1 FROM Equipos WHERE id_modelo = @ID_modelo)

        BEGIN

            -- Si el modelo está asociado a uno o más equipos, lanzamos un error

            RAISERROR ('No se puede eliminar el modelo porque está asociado a uno o más equipos.', 16, 1);

            ROLLBACK TRANSACTION;

            RETURN;

        END

        

        -- Eliminar el modelo de la tabla Modelos

        DELETE FROM Modelos WHERE ID_modelo = @ID_modelo;



        -- Confirmar los cambios si no hubo errores

        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        -- Revertir los cambios si ocurrió algún error

        ROLLBACK TRANSACTION;

        -- Propagar el mensaje de error

        THROW;

    END CATCH

END;
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarModelo]
CREATE PROCEDURE P_GRUD_EliminarModelo

    @ID_modelo INT

AS

BEGIN

    -- Verificar si el modelo existe

    IF NOT EXISTS (SELECT 1 FROM Modelos WHERE ID_modelo = @ID_modelo)

    BEGIN

        -- Si el modelo no existe, lanzamos un error

        RAISERROR ('El modelo no existe.', 16, 1);

        RETURN;

    END



    -- Verificar si el modelo está asociado a algún equipo

    IF EXISTS (SELECT 1 FROM Equipos WHERE id_modelo = @ID_modelo)

    BEGIN

        -- Si existe, lanzar error

        RAISERROR ('No se puede eliminar el modelo porque está asociado a uno o más equipos.', 16, 1);

        RETURN;

    END



    -- Si no está asociado, eliminar el modelo

    DELETE FROM Modelos WHERE ID_modelo = @ID_modelo;

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerMarcaPorID]
CREATE PROCEDURE P_GRUD_ObtenerMarcaPorID

    @ID_marca INT

AS

BEGIN

    SELECT 

        ID_marca, 

        Marca, 

        Activa 

    FROM Marcas 

    WHERE ID_marca = @ID_marca

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarMarca]
Create PROCEDURE [dbo].[P_GRUD_ActualizarMarca]

    @ID_Marca INT,

    @Marca NVARCHAR(255),

    @Activa BIT

AS

BEGIN

    UPDATE Marcas SET Marca = @Marca, Activa = @Activa WHERE ID_marca = @ID_Marca;

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerAreas]
CREATE PROCEDURE P_GRUD_ObtenerAreas

AS

BEGIN

    SELECT 

        Areas.ID_area, 

        Areas.Nombre_Area, 

        Zonas.Nombre_Zona, 

        Areas.Activo

    FROM Areas

    INNER JOIN Zonas ON Areas.id_zona = Zonas.ID_zona

END
GO

-- Procedimiento: [dbo].[P_GRUD_CrearArea]
CREATE PROCEDURE P_GRUD_CrearArea

    @Nombre_Area NVARCHAR(255),

    @id_zona INT

AS

BEGIN

    INSERT INTO Areas (Nombre_Area, id_zona, Activo) 

    VALUES (@Nombre_Area, @id_zona, 1)  -- Asignamos Activo como 1 (true) por defecto

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerAreaPorID]
CREATE PROCEDURE P_GRUD_ObtenerAreaPorID

    @ID_area INT

AS

BEGIN

    SELECT 

        Areas.ID_area, 

        Areas.Nombre_Area, 

        Areas.id_zona, 

        Zonas.Nombre_Zona, 

        Areas.Activo

    FROM Areas

    INNER JOIN Zonas ON Areas.id_zona = Zonas.ID_zona

    WHERE Areas.ID_area = @ID_area

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarArea]
CREATE PROCEDURE P_GRUD_ActualizarArea

    @ID_area INT,

    @Nombre_Area NVARCHAR(255),

    @id_zona INT

AS

BEGIN

    UPDATE Areas

    SET Nombre_Area = @Nombre_Area,

        id_zona = @id_zona

    WHERE ID_area = @ID_area

END
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarArea]
CREATE PROCEDURE P_GRUD_EliminarArea

    @ID_area INT

AS

BEGIN

    -- Verificar si el área está asociada a algún equipo

    IF EXISTS (SELECT 1 FROM Equipos WHERE id_area = @ID_area)

    BEGIN

        -- Si existe, lanzar error

        RAISERROR ('No se puede eliminar el área porque está asociada a uno o más equipos.', 16, 1);

        RETURN;

    END



    -- Si no está asociada, eliminar el área

    DELETE FROM Areas WHERE ID_area = @ID_area

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerZonas]
CREATE PROCEDURE P_GRUD_ObtenerZonas

AS

BEGIN

    SELECT 

        ID_zona, 

        Nombre_Zona, 

        Descripcion_Zona, 

        Activo, 

        Creacion_Zona 

    FROM Zonas

END
GO

-- Procedimiento: [dbo].[P_GRUD_CrearZona]
CREATE PROCEDURE P_GRUD_CrearZona

    @Nombre_Zona NVARCHAR(255),

    @Descripcion_Zona NVARCHAR(255)

AS

BEGIN

    INSERT INTO Zonas (Nombre_Zona, Descripcion_Zona, Activo, Creacion_Zona) 

    VALUES (@Nombre_Zona, @Descripcion_Zona, 1, GETDATE())  -- Activo por defecto y fecha de creación actual

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerZonaPorID]
CREATE PROCEDURE P_GRUD_ObtenerZonaPorID

    @ID_zona INT

AS

BEGIN

    SELECT 

        ID_zona, 

        Nombre_Zona, 

        Descripcion_Zona, 

        Activo, 

        Creacion_Zona

    FROM Zonas 

    WHERE ID_zona = @ID_zona

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarZona]
CREATE PROCEDURE P_GRUD_ActualizarZona

    @ID_zona INT,

    @Nombre_Zona NVARCHAR(255),

    @Descripcion_Zona NVARCHAR(255),

    @Activo BIT

AS

BEGIN

    UPDATE Zonas

    SET Nombre_Zona = @Nombre_Zona,

        Descripcion_Zona = @Descripcion_Zona,

        Activo = @Activo

    WHERE ID_zona = @ID_zona

END
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarZona]
CREATE PROCEDURE P_GRUD_EliminarZona

    @ID_zona INT

AS

BEGIN

    -- Verificar si la zona está asociada a alguna área

    IF EXISTS (SELECT 1 FROM Areas WHERE id_zona = @ID_zona)

    BEGIN

        -- Si existe, lanzar error

        RAISERROR ('No se puede eliminar la zona porque está asociada a una o más áreas.', 16, 1);

        RETURN;

    END



    -- Si no está asociada, eliminar la zona

    DELETE FROM Zonas WHERE ID_zona = @ID_zona;

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerMateriales]
CREATE PROCEDURE P_GRUD_ObtenerMateriales

AS

BEGIN

    SELECT 

        Materiales.ID_Material, 

        Materiales.Descripcion, 

        Materiales.Cantidad, 

        Areas.Nombre_Area

    FROM Materiales

    INNER JOIN Areas ON Materiales.id_area = Areas.ID_area

END
GO

-- Procedimiento: [dbo].[P_ObtenerSoloRadios]
-- Obtener solo Radios

CREATE PROCEDURE P_ObtenerSoloRadios

AS

BEGIN

SELECT e.ID_equipo, e.NumeroSerie, e.Descripcion, e.Estado, r.Frecuencia, r.Modulacion, r.Potencia, r.Tx_Power, r.Rx_Level

FROM Equipos e

INNER JOIN Radio r ON e.ID_equipo = r.id_equipo

INNER JOIN Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

WHERE UPPER(te.Tipo_Equipo) = 'RADIO';

END;
GO

-- Procedimiento: [dbo].[P_GRUD_CrearMaterial]
CREATE PROCEDURE P_GRUD_CrearMaterial

    @Descripcion NVARCHAR(255),

    @Cantidad INT,

    @id_area INT

AS

BEGIN

    INSERT INTO Materiales (Descripcion, Cantidad, id_area) 

    VALUES (@Descripcion, @Cantidad, @id_area)

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerMaterialPorID]
CREATE PROCEDURE P_GRUD_ObtenerMaterialPorID

    @ID_Material INT

AS

BEGIN

    SELECT 

        Materiales.ID_Material, 

        Materiales.Descripcion, 

        Materiales.Cantidad, 

        Materiales.id_area, 

        Areas.Nombre_Area

    FROM Materiales

    INNER JOIN Areas ON Materiales.id_area = Areas.ID_area

    WHERE Materiales.ID_Material = @ID_Material

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarMaterial]
CREATE PROCEDURE P_GRUD_ActualizarMaterial

    @ID_Material INT,

    @Descripcion NVARCHAR(255),

    @Cantidad INT,

    @id_area INT

AS

BEGIN

    UPDATE Materiales

    SET Descripcion = @Descripcion,

        Cantidad = @Cantidad,

        id_area = @id_area

    WHERE ID_Material = @ID_Material

END
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarMaterial]
CREATE PROCEDURE [dbo].[P_GRUD_EliminarMaterial]

    @ID_Material INT

AS

BEGIN

    DELETE FROM Materiales WHERE ID_Material = @ID_Material

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerDireccionesIp]
CREATE PROCEDURE [dbo].[P_GRUD_ObtenerDireccionesIp]

AS

BEGIN

    SELECT 

        DireccionesIp.ID_ip, 

        DireccionesIp.IPV4, 

        DireccionesIp.Estado, 

        DireccionesIp.Activa, 

        vlans.Nombre_Vlan,

        DireccionesIp.ping

    FROM DireccionesIp

    INNER JOIN vlans ON DireccionesIp.id_vlan = vlans.ID_vlan

END
GO

-- Procedimiento: [dbo].[P_GRUD_CrearDireccionIp]
CREATE PROCEDURE [dbo].[P_GRUD_CrearDireccionIp]

    @IPV4 NVARCHAR(255),

    @Estado NVARCHAR(255),

    @id_vlan INT,

    @ping BIT,

    @Activa BIT

AS

BEGIN

    INSERT INTO DireccionesIp (IPV4, Estado, id_vlan, ping, Activa) 

    VALUES (@IPV4, @Estado, @id_vlan, 1, @Activa)  -- Activa se asigna como 1 (true) por defecto

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerDireccionIpPorID]
CREATE PROCEDURE P_GRUD_ObtenerDireccionIpPorID

    @ID_ip INT

AS

BEGIN

    SELECT 

        DireccionesIp.ID_ip, 

        DireccionesIp.IPV4, 

        DireccionesIp.Estado, 

        DireccionesIp.id_vlan, 

        DireccionesIp.Activa, 

        vlans.Nombre_Vlan

    FROM DireccionesIp

    INNER JOIN vlans ON DireccionesIp.id_vlan = vlans.ID_vlan

    WHERE DireccionesIp.ID_ip = @ID_ip

END
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarVlan]
CREATE PROCEDURE [dbo].[P_GRUD_EliminarVlan]

    @ID_vlan INT

AS

BEGIN

    BEGIN TRY

        -- Verificar si existen dispositivos asociados a la VLAN

        IF EXISTS (SELECT 1 FROM Equipos WHERE id_ip IN (SELECT ID_ip FROM DireccionesIp WHERE id_vlan = @ID_vlan))

        BEGIN

            -- Lanzar un error si hay dispositivos asociados

            RAISERROR('No se puede eliminar la VLAN porque hay dispositivos asociados.', 16, 1);

            RETURN;

        END



        BEGIN TRANSACTION;



        -- Eliminar todas las IPs asociadas a la VLAN

        DELETE FROM DireccionesIp WHERE id_vlan = @ID_vlan;



        -- Eliminar la VLAN

        DELETE FROM vlans WHERE ID_vlan = @ID_vlan;



        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0

        BEGIN

            ROLLBACK TRANSACTION;

        END

        -- Relanzar el error capturado

        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarVlan]
CREATE PROCEDURE P_GRUD_ActualizarVlan

    @ID_vlan INT,

    @Nombre_Vlan NVARCHAR(255),

    @SubNet NVARCHAR(255),

    @Gateway NVARCHAR(255),

    @DhcpIni NVARCHAR(255),

    @DhcpFin NVARCHAR(255),

    @Observaciones NVARCHAR(255),

    @Activo BIT

AS

BEGIN

    UPDATE vlans

    SET Nombre_Vlan = @Nombre_Vlan,

        SubNet = @SubNet,

        Gateway = @Gateway,

        DhcpIni = @DhcpIni,

        DhcpFin = @DhcpFin,

        Observaciones = @Observaciones,

        Activo = @Activo

    WHERE ID_vlan = @ID_vlan

END
GO

-- Procedimiento: [dbo].[P_GRUD_ActualizarDireccionIp]
CREATE PROCEDURE P_GRUD_ActualizarDireccionIp

    @ID_ip INT,

    @IPV4 NVARCHAR(255),

    @Estado NVARCHAR(255),

    @id_vlan INT,

    @Activa BIT

AS

BEGIN

    UPDATE DireccionesIp

    SET IPV4 = @IPV4,

        Estado = @Estado,

        id_vlan = @id_vlan,

        Activa = @Activa

    WHERE ID_ip = @ID_ip

END
GO

-- Procedimiento: [dbo].[P_GRUD_EliminarDireccionIp]
CREATE PROCEDURE [dbo].[P_GRUD_EliminarDireccionIp]

    @ID_ip INT

AS

BEGIN

    BEGIN TRY

        -- Verificar si hay un dispositivo asociado con la dirección IP

        IF EXISTS (SELECT 1 FROM Equipos WHERE id_ip = @ID_ip)

        BEGIN

            -- Lanzar un error si hay un dispositivo asociado

            RAISERROR('No se puede eliminar la dirección IP porque hay un dispositivo asociado.', 16, 1);

            RETURN;

        END



        -- Eliminar la dirección IP si no hay dispositivos asociados

        DELETE FROM DireccionesIp WHERE ID_ip = @ID_ip;

    END TRY

    BEGIN CATCH

        -- Manejo de errores

        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerVlans]
CREATE PROCEDURE P_GRUD_ObtenerVlans

AS

BEGIN

    SELECT 

        ID_vlan, 

        Nombre_Vlan, 

        SubNet, 

        Gateway, 

        DhcpIni, 

        DhcpFin, 

        Observaciones, 

        Activo, 

        Creacion_vlan

    FROM vlans

END
GO

-- Procedimiento: [dbo].[P_GRUD_CrearVlan]
CREATE PROCEDURE P_GRUD_CrearVlan

    @Nombre_Vlan NVARCHAR(255),

    @SubNet NVARCHAR(255),

    @Gateway NVARCHAR(255),

    @DhcpIni NVARCHAR(255),

    @DhcpFin NVARCHAR(255),

    @Observaciones NVARCHAR(255)

AS

BEGIN

    INSERT INTO vlans (Nombre_Vlan, SubNet, Gateway, DhcpIni, DhcpFin, Observaciones, Activo, Creacion_vlan) 

    VALUES (@Nombre_Vlan, @SubNet, @Gateway, @DhcpIni, @DhcpFin, @Observaciones, 1, GETDATE())  -- Activo por defecto

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerVlanPorID]
CREATE PROCEDURE P_GRUD_ObtenerVlanPorID

    @ID_vlan INT

AS

BEGIN

    SELECT 

        ID_vlan, 

        Nombre_Vlan, 

        SubNet, 

        Gateway, 

        DhcpIni, 

        DhcpFin, 

        Observaciones, 

        Activo

    FROM vlans

    WHERE ID_vlan = @ID_vlan

END
GO

-- Procedimiento: [dbo].[P_InsertarEquipoYRadio]
CREATE PROCEDURE P_InsertarEquipoYRadio

    @NumeroSerie NVARCHAR(255),

    @Descripcion NVARCHAR(255),

    @id_tipo_equipo INT,

    @id_modelo INT,

    @id_area INT,

    @id_ip INT,

    @Estado NVARCHAR(255),

    @Activo BIT,

    @Respaldo TEXT,

    @Observaciones NVARCHAR(255),

    @Tipo_Voltaje NVARCHAR(255),

    @Voltaje INT,

    @Amperaje INT,

    @Num_Puertos_RJ45 INT,

    @Num_Puertos_SFP INT,

    @Fecha_Compra DATETIME,

    @Fecha_Garantia DATETIME,

    @Tipo_Garantia NVARCHAR(255),

    @Canal NVARCHAR(255),

    @Firmware NVARCHAR(255),

    @Usuario NVARCHAR(255),

    @Contracena NVARCHAR(255),

    @MAC_Address NVARCHAR(255),

    @Fecha_Instalacion NVARCHAR(255),

    @Ultima_Actualizacion NVARCHAR(255),

    @Voltaje_Energia NVARCHAR(255),

    @id_proveedor INT,

    @Frecuencia NVARCHAR(255),

    @Frecuencia_Rango NVARCHAR(255),

    @Modo NVARCHAR(255),

    @Ssid NVARCHAR(255),

    @Modulacion NVARCHAR(255),

    @Potencia NVARCHAR(255),

    @Tx_Power NVARCHAR(255),

    @Rx_Level NVARCHAR(255),

    @Tx_Freq NVARCHAR(255)

AS

BEGIN

    BEGIN TRANSACTION;



    -- Verificar si el tipo de equipo es "Radio"

    DECLARE @Tipo_Equipo NVARCHAR(50);

    SELECT @Tipo_Equipo = Tipo_Equipo FROM Tipo_Equipos WHERE ID_tipo_equipo = @id_tipo_equipo;



    IF UPPER(@Tipo_Equipo) = 'RADIO'

    BEGIN

        -- Insertar el equipo

        INSERT INTO Equipos (NumeroSerie, Descripcion, id_tipo_equipo, id_modelo, id_area, id_ip, Estado, Activo, Respaldo, Observaciones)

        VALUES (@NumeroSerie, @Descripcion, @id_tipo_equipo, @id_modelo, @id_area, @id_ip, @Estado, @Activo, @Respaldo, @Observaciones);



        -- Obtener el ID del equipo insertado

        DECLARE @ID_equipo INT;

        SET @ID_equipo = SCOPE_IDENTITY();



        -- Insertar en la tabla Detalle_Equipo

        INSERT INTO Detalle_Equipo (Tipo_Voltaje, Voltaje, Amperaje, Num_Puertos_RJ45, Num_Puertos_SFP, Fecha_Compra, Fecha_Garantia, Tipo_Garantia, Canal, Firmware, Usuario, Contracena, MAC_Address, Fecha_Instalacion, Ultima_Actualizacion, Voltaje_Energia, id_proveedor, id_equipo)

        VALUES (@Tipo_Voltaje, @Voltaje, @Amperaje, @Num_Puertos_RJ45, @Num_Puertos_SFP, @Fecha_Compra, @Fecha_Garantia, @Tipo_Garantia, @Canal, @Firmware, @Usuario, @Contracena, @MAC_Address, @Fecha_Instalacion, @Ultima_Actualizacion, @Voltaje_Energia, @id_proveedor, @ID_equipo);



        -- Insertar en la tabla Radio

        INSERT INTO Radio (Frecuencia, Frecuencia_Rango, Modo, Ssid, Modulacion, Potencia, Tx_Power, Rx_Level, Tx_Freq, id_equipo)

        VALUES (@Frecuencia, @Frecuencia_Rango, @Modo, @Ssid, @Modulacion, @Potencia, @Tx_Power, @Rx_Level, @Tx_Freq, @ID_equipo);



        PRINT 'Radio agregado exitosamente.';

    END

    ELSE

    BEGIN

        PRINT 'Error: El tipo de equipo especificado no es "Radio". Operación cancelada.';

        ROLLBACK TRANSACTION;

        RETURN;

    END



    COMMIT TRANSACTION;

END;
GO

-- Procedimiento: [dbo].[P_AllActualizarDetallesEquipoYProveedor]
CREATE PROCEDURE [dbo].[P_AllActualizarDetallesEquipoYProveedor]

    @ID_equipo INT,

    @NumeroSerie NVARCHAR(50),

    @Descripcion NVARCHAR(255),

    @Estado NVARCHAR(50),

    @Activo BIT,

    @Respaldo NVARCHAR(MAX),

    @Observaciones NVARCHAR(MAX),



    -- Detalle_Equipo parameters

    @ID_detalle_equipo INT,

    @Tipo_Voltaje NVARCHAR(50),

    @Voltaje INT,

    @Amperaje INT,

    @Num_Puertos_RJ45 INT,

    @Num_Puertos_SFP INT,

    @Fecha_Compra DATE,

    @Fecha_Garantia DATE,

    @Tipo_Garantia NVARCHAR(50),

    @Canal NVARCHAR(50),

    @Firmware NVARCHAR(50),

    @Usuario NVARCHAR(50),

    @Contracena NVARCHAR(50),

    @MAC_Address NVARCHAR(50),

    @Fecha_Instalacion NVARCHAR(50),

    @Ultima_Actualizacion NVARCHAR(50),

    @Voltaje_Energia NVARCHAR(50),



    -- Proveedor parameters

    @ID_proveedor INT,

    @Nombre_Proveedor NVARCHAR(100),

    @Direccion_Proveedor NVARCHAR(255),

    @Telefono_Proveedor NVARCHAR(20),

    @Correo_Proveedor NVARCHAR(100)

AS

BEGIN

    BEGIN TRANSACTION;



    BEGIN TRY

        -- Actualización de la tabla Equipos

        PRINT 'Actualizando Equipos...';

        UPDATE Equipos

        SET 

            NumeroSerie = @NumeroSerie,

            Descripcion = @Descripcion,

            Estado = @Estado,

            Activo = @Activo,

            Respaldo = @Respaldo,

            Observaciones = @Observaciones

        WHERE ID_equipo = @ID_equipo;



        -- Actualización de la tabla Detalle_Equipo

        PRINT 'Actualizando Detalle_Equipo...';

        UPDATE Detalle_Equipo

        SET 

            Tipo_Voltaje = @Tipo_Voltaje,

            Voltaje = @Voltaje,

            Amperaje = @Amperaje,

            Num_Puertos_RJ45 = @Num_Puertos_RJ45,

            Num_Puertos_SFP = @Num_Puertos_SFP,

            Fecha_Compra = @Fecha_Compra,

            Fecha_Garantia = @Fecha_Garantia,

            Tipo_Garantia = @Tipo_Garantia,

            Canal = @Canal,

            Firmware = @Firmware,

            Usuario = @Usuario,

            Contracena = @Contracena,

            MAC_Address = @MAC_Address,

            Fecha_Instalacion = @Fecha_Instalacion,

            Ultima_Actualizacion = @Ultima_Actualizacion,

            Voltaje_Energia = @Voltaje_Energia

        WHERE ID_detalle_equipo = @ID_detalle_equipo;



        -- Verificación y actualización de la tabla Proveedores

        PRINT 'Verificando cambios en Proveedores...';

        IF EXISTS (

            SELECT 1 

            FROM Proveedores 

            WHERE ID_proveedor = @ID_proveedor

              AND (Nombre != @Nombre_Proveedor 

                   OR Direccion != @Direccion_Proveedor 

                   OR Telefono != @Telefono_Proveedor 

                   OR Correo != @Correo_Proveedor)

        )

        BEGIN

            PRINT 'Actualizando Proveedores...';

            UPDATE Proveedores

            SET 

                Nombre = @Nombre_Proveedor,

                Direccion = @Direccion_Proveedor,

                Telefono = @Telefono_Proveedor,

                Correo = @Correo_Proveedor

            WHERE ID_proveedor = @ID_proveedor;

        END

        ELSE

        BEGIN

            PRINT 'No se realizó ninguna actualización en Proveedores porque los valores eran iguales.';

        END



        -- Confirmar transacción

        COMMIT TRANSACTION;

        PRINT 'Transacción completada con éxito.';

    END TRY

    BEGIN CATCH

        -- Revertir transacción en caso de error

        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();

        PRINT 'Error: ' + @ErrorMessage;

        THROW;

    END CATCH

END;
GO

-- Procedimiento: [dbo].[P_AllObtenerDetallesEquipoPorID]
CREATE PROCEDURE [dbo].[P_AllObtenerDetallesEquipoPorID]

    @ID_equipo INT

AS

BEGIN

    -- Manejo de errores con TRY...CATCH para mayor estabilidad

    BEGIN TRY

        SELECT 

            e.ID_equipo,

            e.NumeroSerie,

            e.Descripcion,

            e.Estado,

            e.Activo,

            e.Respaldo,

            e.Observaciones,

            e.id_area,

            e.id_modelo,

            e.id_ip,

            e.id_tipo_equipo,



            d.ID_detalle_equipo,

            d.Tipo_Voltaje,

            d.Voltaje,

            d.Amperaje,

            d.Num_Puertos_RJ45,

            d.Num_Puertos_SFP,

            d.Fecha_Compra,

            d.Fecha_Garantia,

            d.Tipo_Garantia,

            d.Canal,

            d.Firmware,

            d.Usuario,

            d.Contracena,

            d.MAC_Address,

            d.Fecha_Instalacion,

            d.Ultima_Actualizacion,

            d.Voltaje_Energia,



            p.ID_proveedor,

            p.Nombre AS Nombre_Proveedor,

            p.Direccion AS Direccion_Proveedor,

            p.Telefono AS Telefono_Proveedor,

            p.Correo AS Correo_Proveedor,



            a.ID_area,

            a.Nombre_Area,



            m.ID_modelo,

            m.Nombre_Modelo,



            ip.ID_ip,

            ip.IPV4 AS Direccion_IP,



            te.ID_tipo_equipo,

            te.Tipo_Equipo

        FROM 

            Equipos e

        LEFT JOIN 

            Detalle_Equipo d ON e.ID_equipo = d.id_equipo

        LEFT JOIN 

            Proveedores p ON d.id_proveedor = p.ID_proveedor

        LEFT JOIN 

            Areas a ON e.id_area = a.ID_area

        LEFT JOIN 

            Modelos m ON e.id_modelo = m.ID_modelo

        LEFT JOIN 

            DireccionesIp ip ON e.id_ip = ip.ID_ip

        LEFT JOIN 

            Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

        WHERE 

            e.ID_equipo = @ID_equipo;

    END TRY

    BEGIN CATCH

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();

        RAISERROR(@ErrorMessage, 16, 1);

    END CATCH

END;
GO

-- Procedimiento: [dbo].[P_CRUD_ObtenerRadioDetalle]
CREATE PROCEDURE [dbo].[P_CRUD_ObtenerRadioDetalle]

    @ID_Radio INT

AS

BEGIN

    SELECT 

        r.ID_Radio,

        r.Frecuencia,

        r.Frecuencia_Rango,

        r.Modo,

        r.Ssid,

        r.Modulacion,

        r.Potencia,

        r.Tx_Power,

        r.Rx_Level,

        r.Tx_Freq,

        e.ID_equipo,

        e.NumeroSerie,

        e.Descripcion,

        e.Estado,

        e.Activo,

        e.Respaldo,

        e.Observaciones,

        de.ID_detalle_equipo,

        de.Tipo_Voltaje,

        de.Voltaje,

        de.Amperaje,

        de.Num_Puertos_RJ45,

        de.Num_Puertos_SFP,

        de.Fecha_Compra,

        de.Fecha_Garantia,

        de.Tipo_Garantia,

        de.Canal,

        de.Firmware,

        de.Usuario,

        de.Contracena,

        de.MAC_Address,

        de.Fecha_Instalacion,

        de.Ultima_Actualizacion,

        de.Voltaje_Energia,

        p.ID_proveedor,

        p.Nombre AS Nombre_Proveedor,

        p.Direccion AS Direccion_Proveedor,

        p.Telefono AS Telefono_Proveedor,

        p.Correo AS Correo_Proveedor,

        m.ID_modelo,

        m.Nombre_Modelo,

        a.ID_area,

        a.Nombre_Area,

        ip.ID_ip,

        ip.IPV4 AS Direccion_IP,

        te.ID_tipo_equipo,

        te.Tipo_Equipo

    FROM 

        Radio r

    INNER JOIN Equipos e ON r.id_equipo = e.ID_equipo

    LEFT JOIN Detalle_Equipo de ON de.id_equipo = e.ID_equipo

    LEFT JOIN Proveedores p ON de.id_proveedor = p.ID_proveedor

    LEFT JOIN Modelos m ON e.id_modelo = m.ID_modelo

    LEFT JOIN Areas a ON e.id_area = a.ID_area

    LEFT JOIN DireccionesIp ip ON e.id_ip = ip.ID_ip

    LEFT JOIN Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

    WHERE 

        r.ID_Radio = @ID_Radio;

END;
GO

-- Procedimiento: [dbo].[P_ObtenerProveedores]
CREATE PROCEDURE P_ObtenerProveedores AS

BEGIN

    SELECT ID_proveedor, Nombre FROM Proveedores WHERE Activo = 1;

END;
GO

-- Procedimiento: [dbo].[P_CRUD_EliminarRadio]
--select * from Radio

CREATE PROCEDURE P_CRUD_EliminarRadio

    @ID_Radio INT

AS

BEGIN

    DELETE FROM Radio

    WHERE ID_Radio = @ID_Radio;

END;
GO

-- Procedimiento: [dbo].[P_ObtenerModelos]
CREATE PROCEDURE P_ObtenerModelos AS

BEGIN

    SELECT ID_modelo, Nombre_Modelo FROM Modelos WHERE Activo = 1;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_CrearVlanID]
CREATE PROCEDURE P_GRUD_CrearVlanID

    @Nombre_Vlan NVARCHAR(255),

    @SubNet NVARCHAR(255),

    @Gateway NVARCHAR(255),

    @DhcpIni NVARCHAR(255),

    @DhcpFin NVARCHAR(255),

    @Observaciones NVARCHAR(255),

    @ID_vlan INT OUTPUT

AS

BEGIN

    INSERT INTO vlans (Nombre_Vlan, SubNet, Gateway, DhcpIni, DhcpFin, Observaciones, Activo, Creacion_vlan)

    VALUES (@Nombre_Vlan, @SubNet, @Gateway, @DhcpIni, @DhcpFin, @Observaciones, 1, GETDATE());



    SET @ID_vlan = SCOPE_IDENTITY();

END
GO

-- Procedimiento: [dbo].[P_ObtenerTiposEquipos]
CREATE PROCEDURE P_ObtenerTiposEquipos AS

BEGIN

    SELECT ID_tipo_equipo, Tipo_Equipo FROM Tipo_Equipos;

END;
GO

-- Procedimiento: [dbo].[P_ObtenerAreas]
CREATE PROCEDURE P_ObtenerAreas AS

BEGIN

    SELECT ID_area, Nombre_Area FROM Areas WHERE Activo = 1;

END;
GO

-- Procedimiento: [dbo].[P_ObtenerIPs]
CREATE PROCEDURE P_ObtenerIPs AS

BEGIN

    SELECT ID_ip, IPV4 FROM DireccionesIp WHERE Activa = 1;

END;
GO

-- Procedimiento: [dbo].[P_ObtenerActividad]
CREATE PROCEDURE [dbo].[P_ObtenerActividad]

AS

BEGIN

SELECT

    e.ID_equipo,

    di.IPV4 AS DireccionIP,

    a.Nombre_Area AS Area,

    e.Descripcion AS DescripcionEquipo,

    te.Tipo_Equipo AS TipoEquipo,

    di.ping AS Ping

FROM 

    Equipos e

JOIN 

    DireccionesIp di ON e.id_ip = di.ID_ip

JOIN 

    Areas a ON e.id_area = a.ID_area

JOIN 

    Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

WHERE 

    di.Activa = 1

    AND

    e.Activo = 1

END;
GO

-- Procedimiento: [dbo].[P_InsertarEquipoCompleto]
CREATE PROCEDURE P_InsertarEquipoCompleto

    @NumeroSerie NVARCHAR(255),

    @Descripcion NVARCHAR(255),

    @id_tipo_equipo INT,

    @id_modelo INT,

    @id_area INT,

    @id_ip INT,

    @Estado NVARCHAR(255),

    @Activo BIT,

    @Respaldo TEXT,

    @Observaciones NVARCHAR(255),

    @Tipo_Voltaje NVARCHAR(255),

    @Voltaje INT,

    @Amperaje INT,

    @Num_Puertos_RJ45 INT,

    @Num_Puertos_SFP INT,

    @Fecha_Compra DATETIME,

    @Fecha_Garantia DATETIME,

    @Tipo_Garantia NVARCHAR(255),

    @Canal NVARCHAR(255),

    @Firmware NVARCHAR(255),

    @Usuario NVARCHAR(255),

    @Contracena NVARCHAR(255),

    @MAC_Address NVARCHAR(255),

    @Fecha_Instalacion NVARCHAR(255),

    @Ultima_Actualizacion NVARCHAR(255),

    @Voltaje_Energia NVARCHAR(255),

    @id_proveedor INT

AS

BEGIN

    BEGIN TRANSACTION;



    DECLARE @ID_equipo INT;



    -- Insertar en la tabla Equipos

    INSERT INTO Equipos 

    (

        NumeroSerie, Descripcion, id_tipo_equipo, id_modelo, 

        id_area, id_ip, Estado, Activo, Respaldo, Observaciones

    )

    VALUES 

    (

        @NumeroSerie, @Descripcion, @id_tipo_equipo, @id_modelo, 

        @id_area, @id_ip, @Estado, @Activo, @Respaldo, @Observaciones

    );



    -- Obtener el ID del equipo recién insertado

    SET @ID_equipo = SCOPE_IDENTITY();



    -- Insertar en la tabla Detalle_Equipo

    INSERT INTO Detalle_Equipo 

    (

        id_equipo, Tipo_Voltaje, Voltaje, Amperaje, Num_Puertos_RJ45, 

        Num_Puertos_SFP, Fecha_Compra, Fecha_Garantia, Tipo_Garantia, 

        Canal, Firmware, Usuario, Contracena, MAC_Address, 

        Fecha_Instalacion, Ultima_Actualizacion, Voltaje_Energia, id_proveedor

    )

    VALUES 

    (

        @ID_equipo, @Tipo_Voltaje, @Voltaje, @Amperaje, @Num_Puertos_RJ45, 

        @Num_Puertos_SFP, @Fecha_Compra, @Fecha_Garantia, @Tipo_Garantia, 

        @Canal, @Firmware, @Usuario, @Contracena, @MAC_Address, 

        @Fecha_Instalacion, @Ultima_Actualizacion, @Voltaje_Energia, @id_proveedor

    );



    -- Confirmar la transacción

    COMMIT TRANSACTION;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_CrearProveedor]
CREATE PROCEDURE P_GRUD_CrearProveedor

    @Nombre NVARCHAR(255),

    @Direccion NVARCHAR(255),

    @Telefono NVARCHAR(255),

    @Correo NVARCHAR(255)

AS

BEGIN

    INSERT INTO Proveedores (Nombre, Direccion, Telefono, Correo, Activo)

    VALUES (@Nombre, @Direccion, @Telefono, @Correo, 1);

END;
GO

-- Procedimiento: [dbo].[P_GetEquiposPorTipo_G]
CREATE PROCEDURE P_GetEquiposPorTipo_G

AS

BEGIN

    SELECT 

        te.Tipo_Equipo, 

        COUNT(e.ID_equipo) AS Cantidad

    FROM 

        Equipos e

    INNER JOIN 

        Tipo_Equipos te ON e.id_tipo_equipo = te.ID_tipo_equipo

    GROUP BY 

        te.Tipo_Equipo

END
GO

-- Procedimiento: [dbo].[P_GRUD_ObtenerProveedores]
-- Obtener todos los proveedores

CREATE PROCEDURE P_GRUD_ObtenerProveedores AS

BEGIN

    SELECT * FROM Proveedores WHERE Activo = 1;

END;
GO

-- Procedimiento: [dbo].[P_GRUD_CrearRadio]
CREATE PROCEDURE P_GRUD_CrearRadio

    @Frecuencia NVARCHAR(255),

    @Frecuencia_Rango NVARCHAR(255),

    @Modo NVARCHAR(255),

    @Ssid NVARCHAR(255),

    @Modulacion NVARCHAR(255),

    @Potencia NVARCHAR(255),

    @Tx_Power NVARCHAR(255),

    @Rx_Level NVARCHAR(255),

    @Tx_Freq NVARCHAR(255),

    @id_equipo INT

AS

BEGIN

    INSERT INTO Radio (Frecuencia, Frecuencia_Rango, Modo, Ssid, Modulacion, Potencia, Tx_Power, Rx_Level, Tx_Freq, id_equipo) 

    VALUES (@Frecuencia, @Frecuencia_Rango, @Modo, @Ssid, @Modulacion, @Potencia, @Tx_Power, @Rx_Level, @Tx_Freq, @id_equipo)

END
GO

CREATE PROCEDURE ObtenerNombreIdDeZona
AS
BEGIN
    SELECT ID_zona, Nombre_Zona FROM Zonas;

END
GO

CREATE PROCEDURE P_ObtenerVlanNombreDeVlans
AS 
BEGIN
    SELECT ID_vlan, Nombre_Vlan FROM vlans
END
GO
CREATE PROCEDURE P_ObtenerAreaIdDeAreas
AS 
BEGIN
    SELECT ID_area, Nombre_Area FROM Areas
END
GO
CREATE PROCEDURE P_ObtenerMarcaIdDeMarcas
AS 
BEGIN
    SELECT ID_marca, Marca FROM Marcas
END
GO