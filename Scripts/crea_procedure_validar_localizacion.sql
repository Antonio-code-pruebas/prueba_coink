-- PROCEDURE: prueba_coink.validar_localizacion(integer, integer, integer)

-- DROP PROCEDURE IF EXISTS prueba_coink.validar_localizacion(integer, integer, integer);

CREATE OR REPLACE PROCEDURE prueba_coink.validar_localizacion(
	IN _pais_id integer,
	IN _departamento_id integer,
	IN _municipio_id integer,
	OUT mensaje character varying)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    pais_codigo VARCHAR;
    pais_nombre VARCHAR;
    departamento_codigo VARCHAR;
    departamento_nombre VARCHAR;
    municipio_codigo VARCHAR;
    municipio_nombre VARCHAR;
BEGIN
    -- Validación para verificar que los parámetros no sean NULL o vacíos
    IF _pais_id IS NULL OR _departamento_id IS NULL OR _municipio_id IS NULL THEN
        RAISE EXCEPTION 'Faltan parámetros: todos los campos (pais_id, departamento_id, municipio_id) son obligatorios.';
    END IF;

    -- Obtener el código y nombre del país
    SELECT codigo, nombre INTO pais_codigo, pais_nombre
    FROM prueba_coink.pais
    WHERE pais_id = _pais_id;

    -- Si no se encuentra el país, lanzar excepción
    IF pais_codigo IS NULL THEN
        RAISE EXCEPTION 'El país con id % no existe.', pais_id;
    END IF;

    -- Obtener el código y nombre del departamento
    SELECT codigo, nombre INTO departamento_codigo, departamento_nombre
    FROM prueba_coink.departamento
    WHERE departamento_id = _departamento_id;

    -- Si no se encuentra el departamento, lanzar excepción
    IF departamento_codigo IS NULL THEN
        RAISE EXCEPTION 'El departamento con id % no existe.', departamento_id;
    END IF;

    -- Obtener el código y nombre del municipio
    SELECT codigo, nombre INTO municipio_codigo, municipio_nombre
    FROM prueba_coink.municipio
    WHERE municipio_id = _municipio_id;

    -- Si no se encuentra el municipio, lanzar excepción
    IF municipio_codigo IS NULL THEN
        RAISE EXCEPTION 'El municipio con id % no existe.', municipio_id;
    END IF;

    -- Validar si el departamento pertenece al país
    IF NOT EXISTS (
        SELECT 1 FROM prueba_coink.departamento
        WHERE departamento_id = _departamento_id AND pais_id = _pais_id
    ) THEN
        RAISE EXCEPTION 'El departamento % (%), no pertenece al país % (%).', departamento_nombre, departamento_codigo, pais_nombre, pais_codigo;
    END IF;

    -- Validar si el municipio pertenece al departamento
    IF NOT EXISTS (
        SELECT 1 FROM prueba_coink.municipio
        WHERE municipio_id = _municipio_id AND departamento_id = _departamento_id
    ) THEN
        RAISE EXCEPTION 'El municipio % (%), no pertenece al departamento % (%).', municipio_nombre, municipio_codigo, departamento_nombre, departamento_codigo;
    END IF;

    -- Si todo es válido, mostrar mensaje de éxito con los valores de los códigos y nombres
    -- RAISE NOTICE 'La localizacion es valida para insertar dirección: Municipio % (%), Departamento % (%), País % (%).', municipio_nombre, municipio_codigo, departamento_nombre, departamento_codigo, pais_nombre, pais_codigo;
	-- Devuelve el mensaje de validación exitosa
    -- mensaje := FORMAT('La localizacion es valida para insertar dirección: Municipio %% (%), Departamento %% (%), País %% (%).', municipio_nombre, municipio_codigo, departamento_nombre, departamento_codigo, pais_nombre, pais_codigo);
	-- mensaje := FORMAT('La localizacion es valida para insertar dirección');
	mensaje := FORMAT('La localizacion es valida para insertar dirección: Municipio %s (%s), Departamento %s (%s), País %s (%s).',
                      municipio_nombre, municipio_codigo, departamento_nombre, departamento_codigo, pais_nombre, pais_codigo);
END;
$BODY$;
ALTER PROCEDURE prueba_coink.validar_localizacion(integer, integer, integer)
    OWNER TO postgres;
