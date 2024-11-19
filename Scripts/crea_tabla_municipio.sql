-- Table: prueba_coink.municipio

-- DROP TABLE IF EXISTS prueba_coink.municipio;

CREATE TABLE IF NOT EXISTS prueba_coink.municipio
(
    municipio_id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    departamento_id integer NOT NULL,
    codigo text COLLATE pg_catalog."default",
    nombre text COLLATE pg_catalog."default",
    CONSTRAINT municipio_pkey PRIMARY KEY (municipio_id),
    CONSTRAINT fk_municipio_departamento FOREIGN KEY (departamento_id)
        REFERENCES prueba_coink.departamento (departamento_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS prueba_coink.municipio
    OWNER to postgres;