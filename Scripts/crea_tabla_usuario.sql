-- Table: prueba_coink.usuario

-- DROP TABLE IF EXISTS prueba_coink.usuario;

CREATE TABLE IF NOT EXISTS prueba_coink.usuario
(
    usuario_id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    nombre text COLLATE pg_catalog."default",
    telefono text COLLATE pg_catalog."default",
    pais_id integer,
    departamento_id integer,
    municipio_id integer,
    direccion text COLLATE pg_catalog."default",
    CONSTRAINT usuario_pkey PRIMARY KEY (usuario_id),
    CONSTRAINT chk_direccion_longitud CHECK (length(direccion) <= 255),
    CONSTRAINT chk_telefono_longitud CHECK (length(telefono) <= 15)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS prueba_coink.usuario
    OWNER to postgres;