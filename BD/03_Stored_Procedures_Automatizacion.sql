-- =====================================================
-- STORED PROCEDURES PARA AUTOMATIZACIÓN DEL SISTEMA RRHH
-- Fecha: $(date)
-- Descripción: Procedimientos para automatizar cálculos y procesos
-- =====================================================

USE DBRRHH;
GO

-- =====================================================
-- 1. PROCEDIMIENTO PARA ACUMULAR VACACIONES MENSUALMENTE
-- =====================================================

CREATE OR ALTER PROCEDURE SP_AcumularVacacionesMensual
    @Mes INT = NULL,
    @Año INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si no se especifica mes/año, usar el actual
    IF @Mes IS NULL SET @Mes = MONTH(GETDATE());
    IF @Año IS NULL SET @Año = YEAR(GETDATE());
    
    DECLARE @DiasVacacionesPorMes DECIMAL(5,2);
    SELECT @DiasVacacionesPorMes = CAST(ValorParametro AS DECIMAL(5,2)) 
    FROM ParametrosSistema 
    WHERE NombreParametro = 'DIAS_VACACIONES_POR_MES';
    
    -- Insertar acumulación para usuarios activos que ya cumplieron un año
    INSERT INTO AcumuloVacaciones (UsuarioID, Mes, Año, DiasAcumulados, Estado)
    SELECT 
        u.UsuarioID,
        @Mes,
        @Año,
        @DiasVacacionesPorMes,
        'Procesado'
    FROM Usuarios u
    WHERE u.UsuarioStatus = 1
        AND u.FechaIngreso IS NOT NULL
        AND DATEDIFF(MONTH, u.FechaIngreso, DATEFROMPARTS(@Año, @Mes, 1)) >= 12
        AND NOT EXISTS (
            SELECT 1 FROM AcumuloVacaciones av 
            WHERE av.UsuarioID = u.UsuarioID 
                AND av.Mes = @Mes 
                AND av.Año = @Año
        );
    
    -- Actualizar saldo de vacaciones
    MERGE SaldoVacaciones AS target
    USING (
        SELECT 
            av.UsuarioID,
            @Año as Año,
            SUM(av.DiasAcumulados) as TotalAcumulado
        FROM AcumuloVacaciones av
        WHERE av.Año = @Año AND av.Estado = 'Procesado'
        GROUP BY av.UsuarioID
    ) AS source ON target.UsuarioID = source.UsuarioID AND target.Año = source.Año
    WHEN MATCHED THEN
        UPDATE SET 
            DiasTotales = source.TotalAcumulado,
            DiasDisponibles = source.TotalAcumulado - target.DiasUsados,
            FechaUpdate = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (UsuarioID, Año, DiasTotales, DiasDisponibles, DiasUsados, FechaCorte)
        VALUES (source.UsuarioID, source.Año, source.TotalAcumulado, source.TotalAcumulado, 0, GETDATE());
    
    PRINT 'Acumulación de vacaciones procesada para ' + CAST(@Mes AS VARCHAR(2)) + '/' + CAST(@Año AS VARCHAR(4));
END;
GO

-- =====================================================
-- 2. PROCEDIMIENTO PARA CALCULAR PLANILLA AUTOMÁTICAMENTE
-- =====================================================

CREATE OR ALTER PROCEDURE SP_CalcularPlanillaAutomatica
    @UsuarioID INT,
    @FechaPlanilla DATETIME,
    @HorasRegulares DECIMAL(5,2) = 160.00, -- 8 horas x 20 días laborales
    @PlanillaID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @SalarioBase DECIMAL(18,2);
        DECLARE @SalarioBruto DECIMAL(18,2);
        DECLARE @TotalDeducciones DECIMAL(18,2) = 0;
        DECLARE @SalarioNeto DECIMAL(18,2);
        DECLARE @PorcentajeCCSS DECIMAL(5,2);
        DECLARE @PorcentajeBancoPopular DECIMAL(5,2);
        DECLARE @DeduccionCCSS DECIMAL(18,2);
        DECLARE @DeduccionBancoPopular DECIMAL(18,2);
        
        -- Obtener salario base del usuario
        SELECT @SalarioBase = SalarioBase FROM Usuarios WHERE UsuarioID = @UsuarioID;
        
        -- Obtener porcentajes de deducciones
        SELECT @PorcentajeCCSS = CAST(ValorParametro AS DECIMAL(5,2)) 
        FROM ParametrosSistema WHERE NombreParametro = 'PORCENTAJE_CCSS';
        
        SELECT @PorcentajeBancoPopular = CAST(ValorParametro AS DECIMAL(5,2)) 
        FROM ParametrosSistema WHERE NombreParametro = 'PORCENTAJE_BANCO_POPULAR';
        
        -- Calcular salario bruto (incluyendo horas extras del período)
        DECLARE @MontoHorasExtras DECIMAL(18,2) = 0;
        SELECT @MontoHorasExtras = ISNULL(SUM(MontoPagoSalario), 0)
        FROM HorasExtra 
        WHERE UsuarioID = @UsuarioID 
            AND MONTH(Fecha) = MONTH(@FechaPlanilla)
            AND YEAR(Fecha) = YEAR(@FechaPlanilla);
        
        SET @SalarioBruto = @SalarioBase + @MontoHorasExtras;
        
        -- Calcular deducciones
        SET @DeduccionCCSS = @SalarioBruto * (@PorcentajeCCSS / 100);
        SET @DeduccionBancoPopular = @SalarioBruto * (@PorcentajeBancoPopular / 100);
        SET @TotalDeducciones = @DeduccionCCSS + @DeduccionBancoPopular;
        
        -- Calcular salario neto
        SET @SalarioNeto = @SalarioBruto - @TotalDeducciones;
        
        -- Insertar planilla (usando la estructura real de la tabla)
        INSERT INTO Planilla (
            UsuarioID, FechaPlanilla, HorasTrabajadas, HorasExtras, 
            TipoDeduccionID, Deducciones, SalarioNeto
        )
        VALUES (
            @UsuarioID, @FechaPlanilla, @HorasRegulares, @MontoHorasExtras,
            1, @TotalDeducciones, @SalarioNeto
        );
        
        SET @PlanillaID = SCOPE_IDENTITY();
        
        -- Insertar detalles de deducciones
        DECLARE @TipoDeduccionCCSS INT, @TipoDeduccionBP INT;
        SELECT @TipoDeduccionCCSS = TipoDeduccionID FROM TipoDeducciones WHERE DeduccionNombre LIKE '%CCSS%';
        SELECT @TipoDeduccionBP = TipoDeduccionID FROM TipoDeducciones WHERE DeduccionNombre LIKE '%Popular%';
        
        INSERT INTO DetalleDeduccionesPlanilla (PlanillaID, TipoDeduccionID, MontoCalculado, PorcentajeAplicado, BaseCalculo, EsAutomatica)
        VALUES 
            (@PlanillaID, @TipoDeduccionCCSS, @DeduccionCCSS, @PorcentajeCCSS, @SalarioBruto, 1),
            (@PlanillaID, @TipoDeduccionBP, @DeduccionBancoPopular, @PorcentajeBancoPopular, @SalarioBruto, 1);
        
        COMMIT TRANSACTION;
        PRINT 'Planilla calculada correctamente. ID: ' + CAST(@PlanillaID AS VARCHAR(10));
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- =====================================================
-- 3. PROCEDIMIENTO PARA CALCULAR AGUINALDOS
-- =====================================================

CREATE OR ALTER PROCEDURE SP_CalcularAguinaldo
    @UsuarioID INT,
    @Año INT,
    @CalculoAguinaldoID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @FechaInicio DATETIME = DATEFROMPARTS(@Año, 12, 1);
        DECLARE @FechaFin DATETIME = DATEFROMPARTS(@Año + 1, 11, 30);
        DECLARE @TotalSalarios DECIMAL(18,2) = 0;
        DECLARE @MontoAguinaldo DECIMAL(18,2) = 0;
        
        -- Calcular total de salarios recibidos en el período (usando SalarioNeto + Deducciones)
        SELECT @TotalSalarios = ISNULL(SUM(ISNULL(SalarioNeto, 0) + ISNULL(Deducciones, 0)), 0)
        FROM Planilla 
        WHERE UsuarioID = @UsuarioID 
            AND FechaPlanilla BETWEEN @FechaInicio AND @FechaFin;
        
        -- Calcular aguinaldo (1/12 del total de salarios)
        SET @MontoAguinaldo = @TotalSalarios / 12;
        
        -- Insertar cálculo de aguinaldo
        INSERT INTO CalculoAguinaldos (
            UsuarioID, Año, FechaInicioCalculo, FechaFinCalculo,
            TotalSalariosRecibidos, MontoAguinaldo, Estado, UsuarioCalcula
        )
        VALUES (
            @UsuarioID, @Año, @FechaInicio, @FechaFin,
            @TotalSalarios, @MontoAguinaldo, 'Calculado', NULL
        );
        
        SET @CalculoAguinaldoID = SCOPE_IDENTITY();
        
        -- Insertar detalles del cálculo (usando SalarioNeto + Deducciones como SalarioBruto)
        INSERT INTO DetalleAguinaldos (CalculoAguinaldoID, PlanillaID, MesPlanilla, SalarioBruto, FechaPlanilla)
        SELECT 
            @CalculoAguinaldoID,
            PlanillaID,
            MONTH(FechaPlanilla),
            ISNULL(SalarioNeto, 0) + ISNULL(Deducciones, 0),
            FechaPlanilla
        FROM Planilla 
        WHERE UsuarioID = @UsuarioID 
            AND FechaPlanilla BETWEEN @FechaInicio AND @FechaFin;
        
        COMMIT TRANSACTION;
        PRINT 'Aguinaldo calculado correctamente. Monto: ₡' + FORMAT(@MontoAguinaldo, 'N2');
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- =====================================================
-- 4. PROCEDIMIENTO PARA PROCESAR INCAPACIDADES
-- =====================================================

CREATE OR ALTER PROCEDURE SP_ProcesarIncapacidad
    @UsuarioID INT,
    @TipoIncapacidadID INT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @DocumentoSoporte VARCHAR(500) = NULL,
    @IncapacidadID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @DiasTotal INT;
        DECLARE @SalarioBase DECIMAL(18,2);
        DECLARE @MontoTotalPago DECIMAL(18,2);
        DECLARE @MontoCCSS DECIMAL(18,2);
        DECLARE @MontoEntidad DECIMAL(18,2);
        DECLARE @DiasMinimosBloqueo INT;
        
        -- Calcular días totales
        SET @DiasTotal = DATEDIFF(DAY, @FechaInicio, @FechaFin) + 1;
        
        -- Obtener salario base
        SELECT @SalarioBase = SalarioBase FROM Usuarios WHERE UsuarioID = @UsuarioID;
        
        -- Obtener días mínimos para bloqueo
        SELECT @DiasMinimosBloqueo = CAST(ValorParametro AS INT) 
        FROM ParametrosSistema WHERE NombreParametro = 'DIAS_MINIMOS_BLOQUEO_INCAPACIDAD';
        
        -- Calcular montos de pago
        SET @MontoTotalPago = (@SalarioBase / 30) * @DiasTotal; -- Pago diario
        SET @MontoCCSS = @MontoTotalPago * 0.60; -- 60% CCSS
        SET @MontoEntidad = @MontoTotalPago * 0.40; -- 40% Empresa
        
        -- Insertar incapacidad
        INSERT INTO Incapacidades (
            UsuarioID, TipoIncapacidadID, FechaInicio, FechaFin, DiasTotal,
            MontoTotalPago, MontoCCSS, MontoEntidad, DocumentoSoporte, Estado
        )
        VALUES (
            @UsuarioID, @TipoIncapacidadID, @FechaInicio, @FechaFin, @DiasTotal,
            @MontoTotalPago, @MontoCCSS, @MontoEntidad, @DocumentoSoporte, 'Activa'
        );
        
        SET @IncapacidadID = SCOPE_IDENTITY();
        
        -- Si la incapacidad es mayor a los días mínimos, bloquear usuario
        IF @DiasTotal >= @DiasMinimosBloqueo
        BEGIN
            UPDATE Usuarios 
            SET UsuarioStatus = 0 -- Bloquear usuario
            WHERE UsuarioID = @UsuarioID;
            
            -- Crear notificación para RRHH
            INSERT INTO Notificaciones (
                UsuarioDestinoID, TipoNotificacion, Titulo, Mensaje, 
                ModuloOrigen, ReferenciaID, Prioridad
            )
            SELECT 
                u.UsuarioID,
                'Incapacidad',
                'Usuario bloqueado por incapacidad',
                'El usuario ha sido bloqueado automáticamente por incapacidad mayor a ' + CAST(@DiasMinimosBloqueo AS VARCHAR(2)) + ' días.',
                'Incapacidades',
                @IncapacidadID,
                'Alta'
            FROM Usuarios u 
            INNER JOIN Roles r ON u.RolId = r.RolId
            WHERE r.NombreRol LIKE '%RRHH%' OR r.NombreRol LIKE '%Administrador%';
        END
        
        COMMIT TRANSACTION;
        PRINT 'Incapacidad procesada correctamente. ID: ' + CAST(@IncapacidadID AS VARCHAR(10));
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- =====================================================
-- 5. PROCEDIMIENTO PARA APROBAR/RECHAZAR SOLICITUDES
-- =====================================================

CREATE OR ALTER PROCEDURE SP_ProcesarAprobacion
    @FlujoAprobacionID INT,
    @UsuarioAprobadorID INT,
    @Accion VARCHAR(10), -- 'Aprobar' o 'Rechazar'
    @Comentarios NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @TipoDocumento VARCHAR(50);
        DECLARE @DocumentoID INT;
        DECLARE @UsuarioSolicitante INT;
        DECLARE @EsAprobacionFinal BIT;
        
        -- Obtener información del flujo
        SELECT 
            @TipoDocumento = TipoDocumento,
            @DocumentoID = DocumentoID,
            @UsuarioSolicitante = UsuarioSolicitante,
            @EsAprobacionFinal = EsAprobacionFinal
        FROM FlujosAprobacion 
        WHERE FlujoAprobacionID = @FlujoAprobacionID;
        
        -- Actualizar el flujo de aprobación
        UPDATE FlujosAprobacion 
        SET 
            Estado = CASE WHEN @Accion = 'Aprobar' THEN 'Aprobado' ELSE 'Rechazado' END,
            FechaAprobacion = GETDATE(),
            Comentarios = @Comentarios
        WHERE FlujoAprobacionID = @FlujoAprobacionID;
        
        -- Si es aprobación y es final, actualizar el documento origen
        IF @Accion = 'Aprobar' AND @EsAprobacionFinal = 1
        BEGIN
            IF @TipoDocumento = 'Permiso'
                UPDATE Permisos SET PermisoStatus = 1 WHERE PermisoId = @DocumentoID; -- Aprobado
            
            IF @TipoDocumento = 'Vacacion'
                UPDATE Vacaciones SET Estado = 'Aprobado' WHERE VacacionID = @DocumentoID;
        END
        
        -- Si es rechazo, actualizar el documento origen
        IF @Accion = 'Rechazar'
        BEGIN
            IF @TipoDocumento = 'Permiso'
                UPDATE Permisos SET PermisoStatus = 3 WHERE PermisoId = @DocumentoID; -- Rechazado
            
            IF @TipoDocumento = 'Vacacion'
                UPDATE Vacaciones SET Estado = 'Rechazado' WHERE VacacionID = @DocumentoID;
        END
        
        -- Crear notificación para el solicitante
        DECLARE @TituloNotif VARCHAR(200) = @TipoDocumento + ' ' + CASE WHEN @Accion = 'Aprobar' THEN 'Aprobado' ELSE 'Rechazado' END;
        DECLARE @MensajeNotif NVARCHAR(MAX) = 'Su solicitud de ' + @TipoDocumento + ' ha sido ' + CASE WHEN @Accion = 'Aprobar' THEN 'aprobada' ELSE 'rechazada' END + '.';
        
        IF @Comentarios IS NOT NULL
            SET @MensajeNotif = @MensajeNotif + ' Comentarios: ' + @Comentarios;
        
        INSERT INTO Notificaciones (
            UsuarioDestinoID, TipoNotificacion, Titulo, Mensaje,
            ModuloOrigen, ReferenciaID, UsuarioRemitente, Prioridad
        )
        VALUES (
            @UsuarioSolicitante, @TipoDocumento, @TituloNotif, @MensajeNotif,
            'Aprobaciones', @FlujoAprobacionID, @UsuarioAprobadorID, 'Alta'
        );
        
        COMMIT TRANSACTION;
        PRINT 'Aprobación procesada correctamente: ' + @Accion;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- =====================================================
-- 6. PROCEDIMIENTO PARA CALCULAR RESUMEN DE ASISTENCIA
-- =====================================================

CREATE OR ALTER PROCEDURE SP_CalcularResumenAsistencia
    @UsuarioID INT,
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @HorasRegulares DECIMAL(5,2) = 0;
    DECLARE @HorasExtras DECIMAL(5,2) = 0;
    DECLARE @MinutosTardanza INT = 0;
    DECLARE @Ausente BIT = 0;
    DECLARE @TipoAusencia VARCHAR(50) = NULL;
    
    -- Verificar si hay marcas para el día
    DECLARE @HoraEntrada DATETIME, @HoraSalida DATETIME;
    SELECT 
        @HoraEntrada = MIN(HoraEntrada),
        @HoraSalida = MAX(HoraSalida)
    FROM Marcas 
    WHERE UsuarioID = @UsuarioID AND CAST(Fecha AS DATE) = @Fecha;
    
    -- Si no hay marcas, verificar ausencias justificadas
    IF @HoraEntrada IS NULL
    BEGIN
        SET @Ausente = 1;
        
        -- Verificar permisos (usando solo fecha de creación como referencia)
        IF EXISTS (SELECT 1 FROM Permisos WHERE UsuarioID = @UsuarioID AND CAST(PermisoCreacion AS DATE) = @Fecha AND PermisoStatus = 1)
            SET @TipoAusencia = 'Permiso';
        
        -- Verificar vacaciones
        ELSE IF EXISTS (SELECT 1 FROM Vacaciones WHERE UsuarioID = @UsuarioID AND CAST(FechaInicio AS DATE) <= @Fecha AND CAST(FechaFin AS DATE) >= @Fecha AND Estado = 'Aprobado')
            SET @TipoAusencia = 'Vacacion';
        
        -- Verificar incapacidades
        ELSE IF EXISTS (SELECT 1 FROM Incapacidades WHERE UsuarioID = @UsuarioID AND CAST(FechaInicio AS DATE) <= @Fecha AND CAST(FechaFin AS DATE) >= @Fecha AND Estado = 'Activa')
            SET @TipoAusencia = 'Incapacidad';
        
        ELSE
            SET @TipoAusencia = 'Injustificada';
    END
    ELSE
    BEGIN
        -- Calcular horas trabajadas
        DECLARE @MinutosTrabajados INT = DATEDIFF(MINUTE, @HoraEntrada, @HoraSalida);
        DECLARE @HorasTrabajadas DECIMAL(5,2) = @MinutosTrabajados / 60.0;
        
        -- Obtener configuración de horario
        DECLARE @HoraEntradaEsperada TIME, @MinutosRegulares INT = 480; -- 8 horas por defecto
        SELECT TOP 1 
            @HoraEntradaEsperada = HoraEntrada,
            @MinutosRegulares = DATEDIFF(MINUTE, HoraEntrada, HoraSalida)
        FROM ConfiguracionHorarios 
        WHERE UsuarioID = @UsuarioID AND Estado = 1 
            AND FechaInicio <= @Fecha AND (FechaFin IS NULL OR FechaFin >= @Fecha);
        
        -- Calcular tardanza
        IF @HoraEntradaEsperada IS NOT NULL
        BEGIN
            DECLARE @HoraEntradaEsperadaCompleta DATETIME = CAST(@Fecha AS DATETIME) + CAST(@HoraEntradaEsperada AS DATETIME);
            IF @HoraEntrada > @HoraEntradaEsperadaCompleta
                SET @MinutosTardanza = DATEDIFF(MINUTE, @HoraEntradaEsperadaCompleta, @HoraEntrada);
        END
        
        -- Calcular horas regulares y extras
        DECLARE @HorasRegularesEsperadas DECIMAL(5,2) = @MinutosRegulares / 60.0;
        
        IF @HorasTrabajadas <= @HorasRegularesEsperadas
        BEGIN
            SET @HorasRegulares = @HorasTrabajadas;
            SET @HorasExtras = 0;
        END
        ELSE
        BEGIN
            SET @HorasRegulares = @HorasRegularesEsperadas;
            SET @HorasExtras = @HorasTrabajadas - @HorasRegularesEsperadas;
        END
    END
    
    -- Insertar o actualizar resumen
    MERGE ResumenAsistencia AS target
    USING (SELECT @UsuarioID as UsuarioID, @Fecha as Fecha) AS source
        ON target.UsuarioID = source.UsuarioID AND target.Fecha = source.Fecha
    WHEN MATCHED THEN
        UPDATE SET 
            HorasRegulares = @HorasRegulares,
            HorasExtras = @HorasExtras,
            MinutosTardanza = @MinutosTardanza,
            Ausente = @Ausente,
            TipoAusencia = @TipoAusencia,
            FechaCreacion = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (UsuarioID, Fecha, HorasRegulares, HorasExtras, MinutosTardanza, Ausente, TipoAusencia)
        VALUES (@UsuarioID, @Fecha, @HorasRegulares, @HorasExtras, @MinutosTardanza, @Ausente, @TipoAusencia);
    
    PRINT 'Resumen de asistencia calculado para usuario ' + CAST(@UsuarioID AS VARCHAR(10)) + ' fecha ' + CAST(@Fecha AS VARCHAR(10));
END;
GO

-- =====================================================
-- 7. PROCEDIMIENTO PARA ENVIAR NOTIFICACIONES MASIVAS
-- =====================================================

CREATE OR ALTER PROCEDURE SP_EnviarNotificacionMasiva
    @TipoNotificacion VARCHAR(50),
    @Titulo VARCHAR(200),
    @Mensaje NVARCHAR(MAX),
    @RolDestino VARCHAR(100) = NULL, -- Si es NULL, envía a todos
    @Prioridad VARCHAR(10) = 'Media'
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Notificaciones (
        UsuarioDestinoID, TipoNotificacion, Titulo, Mensaje, Prioridad
    )
    SELECT 
        u.UsuarioID,
        @TipoNotificacion,
        @Titulo,
        @Mensaje,
        @Prioridad
    FROM Usuarios u
    INNER JOIN Roles r ON u.RolId = r.RolId
    WHERE u.UsuarioStatus = 1
        AND (@RolDestino IS NULL OR r.NombreRol = @RolDestino);
    
    DECLARE @TotalEnviadas INT = @@ROWCOUNT;
    PRINT 'Notificaciones enviadas: ' + CAST(@TotalEnviadas AS VARCHAR(10));
END;
GO

-- =====================================================
-- MENSAJES DE CONFIRMACIÓN
-- =====================================================

PRINT '=== STORED PROCEDURES CREADOS CORRECTAMENTE ===';
PRINT '1. SP_AcumularVacacionesMensual - Acumula vacaciones mensualmente';
PRINT '2. SP_CalcularPlanillaAutomatica - Calcula planilla con deducciones automáticas';
PRINT '3. SP_CalcularAguinaldo - Calcula aguinaldo basado en salarios del año';
PRINT '4. SP_ProcesarIncapacidad - Procesa incapacidades y bloquea usuarios si es necesario';
PRINT '5. SP_ProcesarAprobacion - Procesa aprobaciones y rechazos de solicitudes';
PRINT '6. SP_CalcularResumenAsistencia - Calcula resumen diario de asistencia';
PRINT '7. SP_EnviarNotificacionMasiva - Envía notificaciones masivas por rol';
PRINT '';
PRINT '=== EJEMPLOS DE USO ===';
PRINT '-- Acumular vacaciones del mes actual:';
PRINT 'EXEC SP_AcumularVacacionesMensual;';
PRINT '';
PRINT '-- Calcular planilla automática:';
PRINT 'DECLARE @PlanillaID INT;';
PRINT 'EXEC SP_CalcularPlanillaAutomatica @UsuarioID = 1, @FechaPlanilla = ''2024-01-31'', @PlanillaID = @PlanillaID OUTPUT;';
PRINT '';
PRINT '-- Calcular aguinaldo:';
PRINT 'DECLARE @AguinaldoID INT;';
PRINT 'EXEC SP_CalcularAguinaldo @UsuarioID = 1, @Año = 2024, @CalculoAguinaldoID = @AguinaldoID OUTPUT;';
PRINT '';
PRINT 'Sistema de automatización listo para usar!';
