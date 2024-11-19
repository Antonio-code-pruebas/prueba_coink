-- FUNCTION: prueba_coink.filtrar_usuarios(text, text, integer, integer, integer)

-- DROP FUNCTION IF EXISTS prueba_coink.filtrar_usuarios(text, text, integer, integer, integer);

CREATE OR REPLACE FUNCTION prueba_coink.filtrar_usuarios(
	_nombre text DEFAULT NULL::text,
	_telefono text DEFAULT NULL::text,
	_pais_id integer DEFAULT NULL::integer,
	_departamento_id integer DEFAULT NULL::integer,
	_municipio_id integer DEFAULT NULL::integer)
    RETURNS TABLE(usuario_id integer, nombre text, telefono text, pais_id integer, departamento_id integer, municipio_id integer, direccion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
        u.usuario_id,
        u.nombre,
        u.telefono,
        u.pais_id,
        u.departamento_id,
        u.municipio_id,
        u.direccion
    FROM prueba_coink.usuario u
    WHERE 
        (_nombre IS NULL OR u.nombre ILIKE '%' || _nombre || '%') AND
        (_telefono IS NULL OR u.telefono ILIKE '%' || _telefono || '%') AND
        (_pais_id IS NULL OR u.pais_id = _pais_id) AND
        (_departamento_id IS NULL OR u.departamento_id = _departamento_id) AND
        (_municipio_id IS NULL OR u.municipio_id = _municipio_id);
END;
$BODY$;

ALTER FUNCTION prueba_coink.filtrar_usuarios(text, text, integer, integer, integer)
    OWNER TO postgres;
