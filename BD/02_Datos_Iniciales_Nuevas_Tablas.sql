-- =====================================================
-- DATOS INICIALES PARA NUEVAS TABLAS DEL SISTEMA RRHH
-- Fecha: $(date)
-- Descripción: Inserts básicos para comenzar a usar las nuevas funcionalidades
-- =====================================================

USE DBRRHH;
GO

-- =====================================================
-- 1. TIPOS DE INCAPACIDADES
-- =====================================================

INSERT INTO TipoIncapacidades (NombreTipo, Descripcion, RequiereDocumento, Estado)
VALUES 
    ('Enfermedad General', 'Incapacidad por enfermedad común', 1, 1),
    ('Accidente Laboral', 'Incapacidad por riesgo del trabajo', 1, 1),
    ('Maternidad', 'Licencia por maternidad', 1, 1),
    ('Paternidad', 'Licencia por paternidad', 1, 1),
    ('Cirugía', 'Incapacidad por procedimiento quirúrgico', 1, 1),
    ('Rehabilitación', 'Incapacidad por proceso de rehabilitación', 1, 1);

-- =====================================================
-- 2. PUESTOS BÁSICOS (Ejemplos)
-- =====================================================

-- Primero necesitamos obtener algunos DepartamentoIDs existentes
-- Estos son ejemplos, ajusta según tus departamentos reales
DECLARE @DeptRRHH int, @DeptContabilidad int, @DeptSistemas int;

SELECT @DeptRRHH = DepartamentoID FROM Departamentos WHERE Departamento LIKE '%Recursos%Humanos%' OR Departamento LIKE '%RRHH%';
SELECT @DeptContabilidad = DepartamentoID FROM Departamentos WHERE Departamento LIKE '%Contabilidad%' OR Departamento LIKE '%Finanzas%';
SELECT @DeptSistemas = DepartamentoID FROM Departamentos WHERE Departamento LIKE '%Sistemas%' OR Departamento LIKE '%IT%' OR Departamento LIKE '%Tecnología%';

-- Si no existen los departamentos, usar los primeros disponibles
IF @DeptRRHH IS NULL SELECT TOP 1 @DeptRRHH = DepartamentoID FROM Departamentos WHERE DepartamentoStatus = 1;
IF @DeptContabilidad IS NULL SELECT TOP 1 @DeptContabilidad = DepartamentoID FROM Departamentos WHERE DepartamentoStatus = 1 AND DepartamentoID != @DeptRRHH;
IF @DeptSistemas IS NULL SELECT TOP 1 @DeptSistemas = DepartamentoID FROM Departamentos WHERE DepartamentoStatus = 1 AND DepartamentoID NOT IN (@DeptRRHH, @DeptContabilidad);

-- Insertar puestos de ejemplo
INSERT INTO Puestos (NombrePuesto, DepartamentoID, SalarioMinimo, SalarioMaximo, Descripcion, Estado)
VALUES 
    ('Director de RRHH', @DeptRRHH, 800000, 1200000, 'Responsable de la gestión estratégica de recursos humanos', 1),
    ('Analista de RRHH', @DeptRRHH, 450000, 650000, 'Apoyo en procesos de recursos humanos y nómina', 1),
    ('Contador General', @DeptContabilidad, 600000, 900000, 'Responsable de la contabilidad general de la empresa', 1),
    ('Asistente Contable', @DeptContabilidad, 350000, 500000, 'Apoyo en procesos contables y financieros', 1),
    ('Desarrollador Senior', @DeptSistemas, 700000, 1000000, 'Desarrollo y mantenimiento de sistemas', 1),
    ('Analista de Sistemas', @DeptSistemas, 500000, 750000, 'Análisis y diseño de sistemas', 1);

-- =====================================================
-- 3. CONFIGURACIÓN DE DEDUCCIONES OBLIGATORIAS
-- =====================================================

-- Obtener IDs de tipos de deducciones existentes o crear si no existen
DECLARE @DeduccionCCSS int, @DeduccionBancoPopular int;

SELECT @DeduccionCCSS = TipoDeduccionID FROM TipoDeducciones WHERE DeduccionNombre LIKE '%CCSS%' OR DeduccionNombre LIKE '%Caja%';
SELECT @DeduccionBancoPopular = TipoDeduccionID FROM TipoDeducciones WHERE DeduccionNombre LIKE '%Banco Popular%' OR DeduccionNombre LIKE '%Popular%';

-- Si no existen, crearlas
IF @DeduccionCCSS IS NULL
BEGIN
    INSERT INTO TipoDeducciones (DeduccionNombre) VALUES ('CCSS - Seguro Social');
    SET @DeduccionCCSS = SCOPE_IDENTITY();
END

IF @DeduccionBancoPopular IS NULL
BEGIN
    INSERT INTO TipoDeducciones (DeduccionNombre) VALUES ('Banco Popular - Aporte Obligatorio');
    SET @DeduccionBancoPopular = SCOPE_IDENTITY();
END

-- Configurar deducciones automáticas
INSERT INTO ConfiguracionDeducciones (TipoDeduccionID, Porcentaje, EsObligatoria, FechaInicio, Descripcion)
VALUES 
    (@DeduccionCCSS, 8.00, 1, '2024-01-01', 'Deducción obligatoria CCSS - 8%'),
    (@DeduccionBancoPopular, 1.00, 1, '2024-01-01', 'Deducción obligatoria Banco Popular - 1%');

-- =====================================================
-- 4. PARÁMETROS DEL SISTEMA
-- =====================================================

INSERT INTO ParametrosSistema (NombreParametro, ValorParametro, Descripcion, TipoDato, Categoria, EsEditable)
VALUES 
    -- Parámetros de Vacaciones
    ('DIAS_VACACIONES_POR_MES', '1.25', 'Días de vacaciones que se acumulan por mes trabajado', 'decimal', 'Vacaciones', 1),
    ('MESES_MINIMOS_VACACIONES', '12', 'Meses mínimos trabajados para tener derecho a vacaciones', 'int', 'Vacaciones', 1),
    ('DIAS_ANTICIPACION_VACACIONES_MIN', '2', 'Días mínimos de anticipación para solicitar vacaciones', 'int', 'Vacaciones', 1),
    ('DIAS_ANTICIPACION_VACACIONES_MAX', '15', 'Días máximos de anticipación para solicitar vacaciones', 'int', 'Vacaciones', 1),
    
    -- Parámetros de Planilla
    ('PORCENTAJE_CCSS', '8.00', 'Porcentaje de deducción CCSS', 'decimal', 'Planilla', 1),
    ('PORCENTAJE_BANCO_POPULAR', '1.00', 'Porcentaje de deducción Banco Popular', 'decimal', 'Planilla', 1),
    ('SALARIO_MINIMO_LEGAL', '350000', 'Salario mínimo legal vigente', 'decimal', 'Planilla', 1),
    
    -- Parámetros de Horas Extras
    ('FACTOR_HORA_EXTRA_DIURNA', '1.5', 'Factor multiplicador para horas extras diurnas', 'decimal', 'HorasExtras', 1),
    ('FACTOR_HORA_EXTRA_NOCTURNA', '1.75', 'Factor multiplicador para horas extras nocturnas', 'decimal', 'HorasExtras', 1),
    ('HORAS_EXTRAS_MAXIMAS_MES', '60', 'Máximo de horas extras permitidas por mes', 'int', 'HorasExtras', 1),
    
    -- Parámetros de Incapacidades
    ('PORCENTAJE_CCSS_INCAPACIDAD', '60.00', 'Porcentaje que paga CCSS en incapacidades', 'decimal', 'Incapacidades', 1),
    ('PORCENTAJE_EMPRESA_INCAPACIDAD', '40.00', 'Porcentaje que paga la empresa en incapacidades', 'decimal', 'Incapacidades', 1),
    ('DIAS_MINIMOS_BLOQUEO_INCAPACIDAD', '3', 'Días mínimos de incapacidad para bloquear usuario', 'int', 'Incapacidades', 1),
    
    -- Parámetros Generales
    ('EMPRESA_NOMBRE', 'Mi Empresa S.A.', 'Nombre de la empresa', 'varchar', 'General', 1),
    ('EMPRESA_CEDULA', '3-101-123456', 'Cédula jurídica de la empresa', 'varchar', 'General', 1),
    ('MONEDA_SISTEMA', 'CRC', 'Moneda del sistema (Colones)', 'varchar', 'General', 1),
    ('TIEMPO_SESION_MINUTOS', '30', 'Tiempo de expiración de sesión en minutos', 'int', 'General', 1);

-- =====================================================
-- 5. FERIADOS DE COSTA RICA 2024 (EJEMPLO)
-- =====================================================

INSERT INTO Feriados (NombreFeriado, Fecha, Año, EsRecurrente, TipoFeriado)
VALUES 
    ('Año Nuevo', '2024-01-01', 2024, 1, 'Nacional'),
    ('Jueves Santo', '2024-03-28', 2024, 0, 'Religioso'),
    ('Viernes Santo', '2024-03-29', 2024, 0, 'Religioso'),
    ('Día de Juan Santamaría', '2024-04-11', 2024, 1, 'Nacional'),
    ('Día del Trabajador', '2024-05-01', 2024, 1, 'Nacional'),
    ('Anexión de Guanacaste', '2024-07-25', 2024, 1, 'Nacional'),
    ('Día de la Virgen de los Ángeles', '2024-08-02', 2024, 1, 'Religioso'),
    ('Día de la Madre', '2024-08-15', 2024, 1, 'Nacional'),
    ('Día de la Independencia', '2024-09-15', 2024, 1, 'Nacional'),
    ('Día de las Culturas', '2024-10-12', 2024, 1, 'Nacional'),
    ('Navidad', '2024-12-25', 2024, 1, 'Nacional');

-- =====================================================
-- 6. CONFIGURACIÓN DE REPORTES BÁSICOS
-- =====================================================

INSERT INTO ConfiguracionReportes (NombreReporte, Descripcion, QuerySQL, TipoReporte, RolesPermitidos)
VALUES 
    ('Planilla Mensual', 'Reporte de planilla mensual por departamento', 
     'SELECT u.Nombre, u.Apellidos, d.Departamento, p.SalarioBruto, p.TotalDeducciones, p.SalarioNeto FROM Planilla p INNER JOIN Usuarios u ON p.UsuarioID = u.UsuarioID INNER JOIN Departamentos d ON u.DepartamentoID = d.DepartamentoID WHERE MONTH(p.FechaPlanilla) = @Mes AND YEAR(p.FechaPlanilla) = @Año', 
     'PDF', '["Administrador", "RRHH"]'),
     
    ('Vacaciones Pendientes', 'Reporte de vacaciones pendientes por empleado', 
     'SELECT u.Nombre, u.Apellidos, sv.DiasDisponibles, sv.Año FROM SaldoVacaciones sv INNER JOIN Usuarios u ON sv.UsuarioID = u.UsuarioID WHERE sv.DiasDisponibles > 0 AND sv.Estado = 1', 
     'Excel', '["Administrador", "RRHH"]'),
     
    ('Horas Extras Mensuales', 'Reporte de horas extras por mes', 
     'SELECT u.Nombre, u.Apellidos, SUM(he.HorasExtra1) as TotalHoras, SUM(he.MontoPagoSalario) as TotalMonto FROM HorasExtra he INNER JOIN Usuarios u ON he.UsuarioID = u.UsuarioID WHERE MONTH(he.Fecha) = @Mes AND YEAR(he.Fecha) = @Año GROUP BY u.Nombre, u.Apellidos', 
     'PDF', '["Administrador", "RRHH", "Jefe"]');

-- =====================================================
-- 7. DATOS PARA PRUEBAS - CONFIGURACIÓN DE HORARIOS
-- =====================================================

-- Obtener algunos usuarios para configurar horarios de ejemplo
DECLARE @Usuario1 int, @Usuario2 int, @TipoJornadaID int;

SELECT TOP 1 @Usuario1 = UsuarioID FROM Usuarios WHERE UsuarioStatus = 1;
SELECT TOP 1 @Usuario2 = UsuarioID FROM Usuarios WHERE UsuarioStatus = 1 AND UsuarioID != @Usuario1;
SELECT TOP 1 @TipoJornadaID = TipoJornadaID FROM TipoJornada;

-- Configurar horarios de ejemplo si existen usuarios y tipo de jornada
IF @Usuario1 IS NOT NULL AND @TipoJornadaID IS NOT NULL
BEGIN
    INSERT INTO ConfiguracionHorarios (UsuarioID, HoraEntrada, HoraSalida, TipoJornadaID, FechaInicio)
    VALUES 
        (@Usuario1, '08:00:00', '17:00:00', @TipoJornadaID, '2024-01-01'),
        (@Usuario2, '07:30:00', '16:30:00', @TipoJornadaID, '2024-01-01');
END

-- =====================================================
-- MENSAJES DE CONFIRMACIÓN
-- =====================================================

PRINT '=== DATOS INICIALES INSERTADOS CORRECTAMENTE ===';
PRINT 'Tipos de Incapacidades: 6 registros';
PRINT 'Puestos de ejemplo: 6 registros';
PRINT 'Configuración de Deducciones: 2 registros';
PRINT 'Parámetros del Sistema: 17 registros';
PRINT 'Feriados 2024: 11 registros';
PRINT 'Configuración de Reportes: 3 registros';
PRINT '';
PRINT '=== PRÓXIMOS PASOS ===';
PRINT '1. Revisar y ajustar los puestos según tu organización';
PRINT '2. Configurar horarios para todos los usuarios';
PRINT '3. Ejecutar proceso inicial de acumulación de vacaciones';
PRINT '4. Configurar notificaciones según necesidades';
PRINT '5. Probar los flujos de aprobación';
PRINT '';
PRINT 'Sistema listo para usar las nuevas funcionalidades!';
