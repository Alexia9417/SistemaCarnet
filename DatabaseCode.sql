CREATE DATABASE SistemaCarnet;
--GO

USE SistemaCarnet;
GO

-- Tabla: tipos_identificacion
CREATE TABLE tipos_identificacion (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(20) NOT NULL
);

-- Tabla: tipos_usuario
CREATE TABLE tipos_usuario (
    id INT PRIMARY KEY,
    nombre VARCHAR(20) NOT NULL
);

--Tabla: estado
Create table estado(
	id INT PRIMARY KEY,
    nombre VARCHAR(10) NOT NULL
);

-- Tabla: usuarios
CREATE TABLE usuarios (
    email VARCHAR(70) PRIMARY KEY,
    tipo_identificacion_id INT NOT NULL,
    identificacion VARCHAR(20) NOT NULL,
    nombre_completo VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL,
    tipo_usuario_id INT NOT NULL,
	estado int Not Null,
    FOREIGN KEY (tipo_identificacion_id) REFERENCES tipos_identificacion(id),
    FOREIGN KEY (tipo_usuario_id) REFERENCES tipos_usuario(id),
	FOREIGN KEY (estado) REFERENCES estado(id)
);

--Tabla: usuario_telefono
Create table usuario_telefonos(
	numero int primary key not null,
	usuario_email VARCHAR(70) not null,
	FOREIGN KEY (usuario_email) REFERENCES usuarios(email)
);

-- Tabla: UsuarioFotografia
CREATE TABLE usuario_foto (
    usuario_email VARCHAR(70) PRIMARY KEY,
    imagen NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (usuario_email) REFERENCES usuarios(email) ON DELETE CASCADE
);

-- Tabla: tokens
CREATE TABLE tokens (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_email VARCHAR(70) NOT NULL,
    token VARCHAR(1000) NOT NULL,
    tipo VARCHAR(50) NOT NULL,
    creado_en DATETIME NOT NULL,
    expiracion DATETIME,
    archivo BIT NOT NULL,
    FOREIGN KEY (usuario_email) REFERENCES usuarios(email) ON DELETE CASCADE
);

-- Tabla: carreras
CREATE TABLE carreras (
    id INT PRIMARY KEY,
    nombre VARCHAR(35) NOT NULL,
    director VARCHAR(50) NOT NULL,
    email VARCHAR(70) NOT NULL,
    telefono VARCHAR(8)
);

-- Tabla: usuario_carreras
CREATE TABLE usuario_carreras (
    usuario_email VARCHAR(70) NOT NULL,
    carrera_id INT NOT NULL,
    PRIMARY KEY (usuario_email, carrera_id),
    FOREIGN KEY (usuario_email) REFERENCES usuarios(email) ON DELETE CASCADE,
    FOREIGN KEY (carrera_id) REFERENCES carreras(id)
);

-- Tabla: areas
CREATE TABLE areas (
    id INT PRIMARY KEY,
    nombre VARCHAR(30) NOT NULL
);

-- Tabla: usuario_areas
CREATE TABLE usuario_areas (
    usuario_email VARCHAR(70) NOT NULL,
    area_id INT NOT NULL,
    PRIMARY KEY (usuario_email, area_id),
    FOREIGN KEY (usuario_email) REFERENCES usuarios(email) ON DELETE CASCADE,
    FOREIGN KEY (area_id) REFERENCES areas(id)
);

-- Tabla: usuario_qr
CREATE TABLE usuario_qr (
    usuario_email VARCHAR(70) PRIMARY KEY,
    codigo_qr_base64 NVARCHAR(MAX) NOT NULL,
    generado_en DATETIME NOT NULL DEFAULT GETDATE(),
    fecha_vencimiento DATETIME NOT NULL,
    FOREIGN KEY (usuario_email) REFERENCES usuarios(email) ON DELETE CASCADE
);


select * from tipos_identificacion

insert into tipos_identificacion (nombre) values
('Cedula'),
('Correo'),
('Pasaporte');


insert into estado (id, nombre) values
(1, 'Activo'),
(2, 'Inactivo')

insert into tipos_usuario (id, nombre) values
(1, 'Estudiante'),
(2, 'Administrador'),
(3, 'Maestro')


insert into usuarios (email, tipo_identificacion_id, nombre_completo, apellidos, contrasena, tipo_usuario_id, estado, identificacion) values
('119150959@cuc.cr', 2, 'Stefany', 'Calvo Umaña', 'gato123', 1, 1, 119150959),
('305154748@cuc.cr', 2, 'Alexander', 'Calvo Navarro', 'hcnHugo', 3, 1, 14151217);

select * from usuarios

select * from usuario_foto
