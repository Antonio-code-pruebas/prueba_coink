-- PROCEDURE: prueba_coink.obtener_municipios_proc_3(text)

-- DROP PROCEDURE IF EXISTS prueba_coink.obtener_municipios_proc_3(text);

CREATE OR REPLACE PROCEDURE prueba_coink.obtener_municipios_proc_3(
	IN municipio_nombre_param text,
	OUT ref_cursor refcursor)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    resultado CURSOR FOR 
        SELECT 
            m.municipio_id,
            m.codigo AS municipio_codigo,
            m.nombre AS municipio_nombre,
            d.departamento_id,
            d.codigo AS departamento_codigo,
            d.nombre AS departamento_nombre,
            p.pais_id,
            p.codigo AS pais_codigo,
            p.nombre AS pais_nombre
        FROM prueba_coink.municipio m
        JOIN prueba_coink.departamento d ON m.departamento_id = d.departamento_id
        JOIN prueba_coink.pais p ON d.pais_id = p.pais_id
        WHERE m.nombre ILIKE '%' || municipio_nombre_param || '%';
BEGIN
    -- Asignar el nombre del cursor de salida
    ref_cursor := 'resultado';

    -- Abrir el cursor con el nombre asignado
    OPEN resultado;
END;
$BODY$;
ALTER PROCEDURE prueba_coink.obtener_municipios_proc_3(text)
    OWNER TO postgres;
