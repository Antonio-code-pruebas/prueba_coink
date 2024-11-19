-- Table: prueba_coink.departamento

-- DROP TABLE IF EXISTS prueba_coink.departamento;

CREATE TABLE IF NOT EXISTS prueba_coink.departamento
(
    departamento_id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    pais_id integer NOT NULL,
    nombre text COLLATE pg_catalog."default",
    codigo text COLLATE pg_catalog."default",
    CONSTRAINT departamento_pkey PRIMARY KEY (departamento_id),
    CONSTRAINT fk_departamento_pais FOREIGN KEY (pais_id)
        REFERENCES prueba_coink.pais (pais_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS prueba_coink.departamento
    OWNER to postgres;