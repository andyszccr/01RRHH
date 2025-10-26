-- =====================================================
-- SCRIPTS PARA NUEVAS TABLAS DEL SISTEMA RRHH
-- Fecha: $(date)
-- Descripción: Tablas adicionales para completar funcionalidades
-- =====================================================

USE DBRRHH;
GO

-- =====================================================
-- 1. MÓDULO DE INCAPACIDADES
-- =====================================================

-- Tabla: TipoIncapacidades
CREATE TABLE TipoIncapacidades (
    TipoIncapacidadID int IDENTITY(1,1) PRIMARY KEY,
    NombreTipo varchar(100) NOT NULL,
    Descripcion text NULL,
    RequiereDocumento bit NOT NULL DEFAULT 1,
    Estado bit NOT NULL DEFAULT 1,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    FechaUpdate datetime NULL,
    UsuarioCreacion int NULL
);

-- Tabla: Incapacidades
CREATE TABLE Incapacidades (
    IncapacidadID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioID int NOT NULL,
    TipoIncapacidadID int NOT NULL,
    FechaInicio datetime NOT NULL,
    FechaFin datetime NOT NULL,
    DiasTotal int NOT NULL,
    PorcentajeCCSS decimal(5,2) NOT NULL DEFAULT 60.00,
    PorcentajeEntidad decimal(5,2) NOT NULL DEFAULT 40.00,
    MontoTotalPago decimal(18,2) NULL,
    MontoCCSS decimal(18,2) NULL,
    MontoEntidad decimal(18,2) NULL,
    Estado varchar(20) NOT NULL DEFAULT 'Activa', -- Activa, Finalizada, Cancelada
    DocumentoSoporte varchar(500) NULL,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    FechaUpdate datetime NULL,
    UsuarioCreacion int NULL,
    Observaciones text NULL,
    
    CONSTRAINT FK_Incapacidades_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT FK_Incapacidades_TipoIncapacidades FOREIGN KEY (TipoIncapacidadID) REFERENCES TipoIncapacidades(TipoIncapacidadID),
    CONSTRAINT CK_Incapacidades_Fechas CHECK (FechaFin >= FechaInicio),
    CONSTRAINT CK_Incapacidades_Porcentajes CHECK (PorcentajeCCSS + PorcentajeEntidad = 100.00)
);

-- =====================================================
-- 2. MÓDULO DE PUESTOS DE TRABAJO
-- =====================================================

-- Tabla: Puestos
CREATE TABLE Puestos (
    PuestoID int IDENTITY(1,1) PRIMARY KEY,
    NombrePuesto varchar(150) NOT NULL,
    DepartamentoID int NOT NULL,
    SalarioMinimo decimal(18,2) NULL,
    SalarioMaximo decimal(18,2) NULL,
    Descripcion text NULL,
    Requisitos text NULL,
    JefeInmediatoID int NULL, -- Referencia a otro puesto
    Estado bit NOT NULL DEFAULT 1,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    FechaUpdate datetime NULL,
    UsuarioCreacion int NULL,
    
    CONSTRAINT FK_Puestos_Departamentos FOREIGN KEY (DepartamentoID) REFERENCES Departamentos(DepartamentoID),
    CONSTRAINT FK_Puestos_JefeInmediato FOREIGN KEY (JefeInmediatoID) REFERENCES Puestos(PuestoID),
    CONSTRAINT CK_Puestos_Salarios CHECK (SalarioMaximo >= SalarioMinimo OR SalarioMaximo IS NULL OR SalarioMinimo IS NULL)
);

-- Agregar campo PuestoID a la tabla Usuarios (modificación)
ALTER TABLE Usuarios ADD PuestoID int NULL;
ALTER TABLE Usuarios ADD FechaIngreso datetime NULL; -- Necesario para cálculo de vacaciones
ALTER TABLE Usuarios ADD CONSTRAINT FK_Usuarios_Puestos FOREIGN KEY (PuestoID) REFERENCES Puestos(PuestoID);

-- =====================================================
-- 3. MÓDULO DE HISTÓRICO DE SALARIOS
-- =====================================================

-- Tabla: HistorialSalarios
CREATE TABLE HistorialSalarios (
    HistorialSalarioID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioID int NOT NULL,
    SalarioAnterior decimal(18,2) NULL,
    SalarioNuevo decimal(18,2) NOT NULL,
    FechaCambio datetime NOT NULL DEFAULT GETDATE(),
    MotivoAumento varchar(200) NULL,
    PorcentajeAumento decimal(5,2) NULL,
    UsuarioAutoriza int NULL,
    Estado varchar(20) NOT NULL DEFAULT 'Activo', -- Activo, Cancelado
    Observaciones text NULL,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_HistorialSalarios_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT FK_HistorialSalarios_UsuarioAutoriza FOREIGN KEY (UsuarioAutoriza) REFERENCES Usuarios(UsuarioID)
);

-- =====================================================
-- 4. MÓDULO DE VACACIONES MEJORADO
-- =====================================================

-- Tabla: SaldoVacaciones
CREATE TABLE SaldoVacaciones (
    SaldoVacacionID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioID int NOT NULL,
    Año int NOT NULL,
    DiasTotales decimal(5,2) NOT NULL DEFAULT 0.00,
    DiasDisponibles decimal(5,2) NOT NULL DEFAULT 0.00,
    DiasUsados decimal(5,2) NOT NULL DEFAULT 0.00,
    DiasVencidos decimal(5,2) NOT NULL DEFAULT 0.00,
    FechaCorte datetime NOT NULL,
    Estado bit NOT NULL DEFAULT 1,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    FechaUpdate datetime NULL,
    
    CONSTRAINT FK_SaldoVacaciones_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT UK_SaldoVacaciones_Usuario_Año UNIQUE (UsuarioID, Año)
);

-- Tabla: AcumuloVacaciones
CREATE TABLE AcumuloVacaciones (
    AcumuloID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioID int NOT NULL,
    Mes int NOT NULL,
    Año int NOT NULL,
    DiasAcumulados decimal(5,2) NOT NULL DEFAULT 1.25,
    FechaProceso datetime NOT NULL DEFAULT GETDATE(),
    Estado varchar(20) NOT NULL DEFAULT 'Procesado',
    Observaciones text NULL,
    
    CONSTRAINT FK_AcumuloVacaciones_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT UK_AcumuloVacaciones_Usuario_Mes_Año UNIQUE (UsuarioID, Mes, Año),
    CONSTRAINT CK_AcumuloVacaciones_Mes CHECK (Mes BETWEEN 1 AND 12)
);

-- =====================================================
-- 5. MÓDULO DE NOTIFICACIONES
-- =====================================================

-- Tabla: Notificaciones
CREATE TABLE Notificaciones (
    NotificacionID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioDestinoID int NOT NULL,
    TipoNotificacion varchar(50) NOT NULL, -- Permiso, Vacacion, Planilla, HorasExtras, etc.
    Titulo varchar(200) NOT NULL,
    Mensaje text NOT NULL,
    FechaEnvio datetime NOT NULL DEFAULT GETDATE(),
    FechaLeida datetime NULL,
    Estado varchar(20) NOT NULL DEFAULT 'Pendiente', -- Pendiente, Leida, Archivada
    ModuloOrigen varchar(50) NULL,
    ReferenciaID int NULL, -- ID del registro origen
    Prioridad varchar(10) NOT NULL DEFAULT 'Media', -- Alta, Media, Baja
    UsuarioRemitente int NULL,
    
    CONSTRAINT FK_Notificaciones_UsuarioDestino FOREIGN KEY (UsuarioDestinoID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT FK_Notificaciones_UsuarioRemitente FOREIGN KEY (UsuarioRemitente) REFERENCES Usuarios(UsuarioID)
);

-- =====================================================
-- 6. MÓDULO DE APROBACIONES
-- =====================================================

-- Tabla: FlujosAprobacion
CREATE TABLE FlujosAprobacion (
    FlujoAprobacionID int IDENTITY(1,1) PRIMARY KEY,
    TipoDocumento varchar(50) NOT NULL, -- Permiso, Vacacion, HorasExtras, Incapacidad
    DocumentoID int NOT NULL,
    NivelAprobacion int NOT NULL, -- 1: Jefe inmediato, 2: RRHH, 3: Gerencia
    UsuarioAprobadorID int NOT NULL,
    FechaSolicitud datetime NOT NULL DEFAULT GETDATE(),
    FechaAprobacion datetime NULL,
    Estado varchar(20) NOT NULL DEFAULT 'Pendiente', -- Pendiente, Aprobado, Rechazado
    Comentarios text NULL,
    EsAprobacionFinal bit NOT NULL DEFAULT 0,
    UsuarioSolicitante int NOT NULL,
    
    CONSTRAINT FK_FlujosAprobacion_UsuarioAprobador FOREIGN KEY (UsuarioAprobadorID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT FK_FlujosAprobacion_UsuarioSolicitante FOREIGN KEY (UsuarioSolicitante) REFERENCES Usuarios(UsuarioID)
);

-- =====================================================
-- 7. MÓDULO DE CONTROL DE ASISTENCIA MEJORADO
-- =====================================================

-- Tabla: ConfiguracionHorarios
CREATE TABLE ConfiguracionHorarios (
    ConfiguracionID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioID int NOT NULL,
    HoraEntrada time NOT NULL,
    HoraSalida time NOT NULL,
    HoraAlmuerzoInicio time NULL,
    HoraAlmuerzoFin time NULL,
    TipoJornadaID int NOT NULL,
    DiasLaborales varchar(20) NOT NULL DEFAULT 'L,M,M,J,V', -- L,M,M,J,V,S,D
    FechaInicio datetime NOT NULL,
    FechaFin datetime NULL,
    Estado bit NOT NULL DEFAULT 1,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_ConfiguracionHorarios_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT FK_ConfiguracionHorarios_TipoJornada FOREIGN KEY (TipoJornadaID) REFERENCES TipoJornada(TipoJornadaID)
);

-- Tabla: ResumenAsistencia
CREATE TABLE ResumenAsistencia (
    ResumenAsistenciaID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioID int NOT NULL,
    Fecha date NOT NULL,
    HorasRegulares decimal(5,2) NOT NULL DEFAULT 0.00,
    HorasExtras decimal(5,2) NOT NULL DEFAULT 0.00,
    HorasTardanza decimal(5,2) NOT NULL DEFAULT 0.00,
    MinutosTardanza int NOT NULL DEFAULT 0,
    Ausente bit NOT NULL DEFAULT 0,
    TipoAusencia varchar(50) NULL, -- Permiso, Vacacion, Incapacidad, Injustificada
    Estado varchar(20) NOT NULL DEFAULT 'Procesado',
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    Observaciones text NULL,
    
    CONSTRAINT FK_ResumenAsistencia_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT UK_ResumenAsistencia_Usuario_Fecha UNIQUE (UsuarioID, Fecha)
);

-- =====================================================
-- 8. MÓDULO DE DEDUCCIONES AUTOMÁTICAS
-- =====================================================

-- Tabla: ConfiguracionDeducciones
CREATE TABLE ConfiguracionDeducciones (
    ConfiguracionDeduccionID int IDENTITY(1,1) PRIMARY KEY,
    TipoDeduccionID int NOT NULL,
    Porcentaje decimal(5,2) NULL,
    MontoFijo decimal(18,2) NULL,
    EsObligatoria bit NOT NULL DEFAULT 0,
    FechaInicio datetime NOT NULL,
    FechaFin datetime NULL,
    Estado bit NOT NULL DEFAULT 1,
    Descripcion text NULL,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_ConfiguracionDeducciones_TipoDeducciones FOREIGN KEY (TipoDeduccionID) REFERENCES TipoDeducciones(TipoDeduccionID),
    CONSTRAINT CK_ConfiguracionDeducciones_Valores CHECK ((Porcentaje IS NOT NULL AND MontoFijo IS NULL) OR (Porcentaje IS NULL AND MontoFijo IS NOT NULL))
);

-- Tabla: DetalleDeduccionesPlanilla
CREATE TABLE DetalleDeduccionesPlanilla (
    DetalleDeduccionID int IDENTITY(1,1) PRIMARY KEY,
    PlanillaID int NOT NULL,
    TipoDeduccionID int NOT NULL,
    MontoCalculado decimal(18,2) NOT NULL,
    PorcentajeAplicado decimal(5,2) NULL,
    BaseCalculo decimal(18,2) NULL,
    EsAutomatica bit NOT NULL DEFAULT 1,
    Observaciones text NULL,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_DetalleDeduccionesPlanilla_Planilla FOREIGN KEY (PlanillaID) REFERENCES Planilla(PlanillaID),
    CONSTRAINT FK_DetalleDeduccionesPlanilla_TipoDeducciones FOREIGN KEY (TipoDeduccionID) REFERENCES TipoDeducciones(TipoDeduccionID)
);

-- =====================================================
-- 9. MÓDULO DE AGUINALDOS
-- =====================================================

-- Tabla: CalculoAguinaldos
CREATE TABLE CalculoAguinaldos (
    CalculoAguinaldoID int IDENTITY(1,1) PRIMARY KEY,
    UsuarioID int NOT NULL,
    Año int NOT NULL,
    FechaInicioCalculo datetime NOT NULL,
    FechaFinCalculo datetime NOT NULL,
    TotalSalariosRecibidos decimal(18,2) NOT NULL DEFAULT 0.00,
    MontoAguinaldo decimal(18,2) NOT NULL DEFAULT 0.00,
    Estado varchar(20) NOT NULL DEFAULT 'Calculado', -- Calculado, Pagado, Cancelado
    FechaCalculo datetime NOT NULL DEFAULT GETDATE(),
    FechaPago datetime NULL,
    UsuarioCalcula int NULL,
    Observaciones text NULL,
    
    CONSTRAINT FK_CalculoAguinaldos_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT FK_CalculoAguinaldos_UsuarioCalcula FOREIGN KEY (UsuarioCalcula) REFERENCES Usuarios(UsuarioID),
    CONSTRAINT UK_CalculoAguinaldos_Usuario_Año UNIQUE (UsuarioID, Año)
);

-- Tabla: DetalleAguinaldos
CREATE TABLE DetalleAguinaldos (
    DetalleAguinaldoID int IDENTITY(1,1) PRIMARY KEY,
    CalculoAguinaldoID int NOT NULL,
    PlanillaID int NOT NULL,
    MesPlanilla int NOT NULL,
    SalarioBruto decimal(18,2) NOT NULL,
    FechaPlanilla datetime NOT NULL,
    
    CONSTRAINT FK_DetalleAguinaldos_CalculoAguinaldos FOREIGN KEY (CalculoAguinaldoID) REFERENCES CalculoAguinaldos(CalculoAguinaldoID),
    CONSTRAINT FK_DetalleAguinaldos_Planilla FOREIGN KEY (PlanillaID) REFERENCES Planilla(PlanillaID),
    CONSTRAINT CK_DetalleAguinaldos_Mes CHECK (MesPlanilla BETWEEN 1 AND 12)
);

-- =====================================================
-- 10. MÓDULO DE AUDITORÍA
-- =====================================================

-- Tabla: AuditoriaTablas
CREATE TABLE AuditoriaTablas (
    AuditoriaID int IDENTITY(1,1) PRIMARY KEY,
    NombreTabla varchar(100) NOT NULL,
    Operacion varchar(10) NOT NULL, -- INSERT, UPDATE, DELETE
    RegistroID int NOT NULL,
    ValoresAnteriores text NULL, -- JSON
    ValoresNuevos text NULL, -- JSON
    UsuarioID int NULL,
    FechaOperacion datetime NOT NULL DEFAULT GETDATE(),
    DireccionIP varchar(45) NULL,
    UserAgent text NULL,
    
    CONSTRAINT FK_AuditoriaTablas_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
);

-- =====================================================
-- 11. MÓDULO DE REPORTES
-- =====================================================

-- Tabla: ConfiguracionReportes
CREATE TABLE ConfiguracionReportes (
    ReporteID int IDENTITY(1,1) PRIMARY KEY,
    NombreReporte varchar(150) NOT NULL,
    Descripcion text NULL,
    QuerySQL text NOT NULL,
    Parametros text NULL, -- JSON
    TipoReporte varchar(20) NOT NULL DEFAULT 'PDF', -- PDF, Excel, CSV
    RolesPermitidos text NULL, -- JSON array de roles
    Estado bit NOT NULL DEFAULT 1,
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    FechaUpdate datetime NULL,
    UsuarioCreacion int NULL,
    
    CONSTRAINT FK_ConfiguracionReportes_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuarios(UsuarioID)
);

-- =====================================================
-- 12. TABLAS DE CONFIGURACIÓN ADICIONALES
-- =====================================================

-- Tabla: ParametrosSistema
CREATE TABLE ParametrosSistema (
    ParametroID int IDENTITY(1,1) PRIMARY KEY,
    NombreParametro varchar(100) NOT NULL UNIQUE,
    ValorParametro varchar(500) NOT NULL,
    Descripcion text NULL,
    TipoDato varchar(20) NOT NULL DEFAULT 'varchar', -- int, decimal, varchar, bit, date
    Categoria varchar(50) NOT NULL DEFAULT 'General', -- Planilla, Vacaciones, General, etc.
    EsEditable bit NOT NULL DEFAULT 1,
    FechaUpdate datetime NOT NULL DEFAULT GETDATE(),
    UsuarioUpdate int NULL,
    
    CONSTRAINT FK_ParametrosSistema_UsuarioUpdate FOREIGN KEY (UsuarioUpdate) REFERENCES Usuarios(UsuarioID)
);

-- Tabla: Feriados
CREATE TABLE Feriados (
    FeriadoID int IDENTITY(1,1) PRIMARY KEY,
    NombreFeriado varchar(150) NOT NULL,
    Fecha date NOT NULL,
    Año int NOT NULL,
    EsRecurrente bit NOT NULL DEFAULT 0, -- Si se repite cada año
    Estado bit NOT NULL DEFAULT 1,
    TipoFeriado varchar(50) NOT NULL DEFAULT 'Nacional', -- Nacional, Regional, Religioso
    FechaCreacion datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT UK_Feriados_Fecha_Año UNIQUE (Fecha, Año)
);

-- =====================================================
-- ÍNDICES PARA OPTIMIZACIÓN
-- =====================================================

-- Índices para Incapacidades
CREATE INDEX IX_Incapacidades_UsuarioID_Estado ON Incapacidades(UsuarioID, Estado);
CREATE INDEX IX_Incapacidades_FechaInicio_FechaFin ON Incapacidades(FechaInicio, FechaFin);

-- Índices para Notificaciones
CREATE INDEX IX_Notificaciones_UsuarioDestino_Estado ON Notificaciones(UsuarioDestinoID, Estado);
CREATE INDEX IX_Notificaciones_FechaEnvio ON Notificaciones(FechaEnvio DESC);

-- Índices para FlujosAprobacion
CREATE INDEX IX_FlujosAprobacion_TipoDocumento_DocumentoID ON FlujosAprobacion(TipoDocumento, DocumentoID);
CREATE INDEX IX_FlujosAprobacion_UsuarioAprobador_Estado ON FlujosAprobacion(UsuarioAprobadorID, Estado);

-- Índices para ResumenAsistencia
CREATE INDEX IX_ResumenAsistencia_UsuarioID_Fecha ON ResumenAsistencia(UsuarioID, Fecha);
CREATE INDEX IX_ResumenAsistencia_Fecha ON ResumenAsistencia(Fecha);

-- Índices para SaldoVacaciones
CREATE INDEX IX_SaldoVacaciones_UsuarioID_Año ON SaldoVacaciones(UsuarioID, Año);

-- Índices para AuditoriaTablas
CREATE INDEX IX_AuditoriaTablas_NombreTabla_FechaOperacion ON AuditoriaTablas(NombreTabla, FechaOperacion DESC);
CREATE INDEX IX_AuditoriaTablas_UsuarioID_FechaOperacion ON AuditoriaTablas(UsuarioID, FechaOperacion DESC);

-- =====================================================
-- FIN DE SCRIPTS
-- =====================================================

PRINT 'Scripts de creación de nuevas tablas ejecutados correctamente.';
PRINT 'Total de tablas creadas: 19';
PRINT 'Se recomienda ejecutar los datos iniciales después de este script.';
