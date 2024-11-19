-- Table: prueba_coink.pais

-- DROP TABLE IF EXISTS prueba_coink.pais;

CREATE TABLE IF NOT EXISTS prueba_coink.pais
(
    pais_id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    codigo text COLLATE pg_catalog."default" NOT NULL,
    nombre text COLLATE pg_catalog."default",
    CONSTRAINT pais_pkey PRIMARY KEY (pais_id),
    CONSTRAINT chk_codigo_longitud CHECK (length(codigo) <= 2)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS prueba_coink.pais
    OWNER to postgres;