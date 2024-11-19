-- FUNCTION: prueba_coink.function_obtener_municipios(text)

-- DROP FUNCTION IF EXISTS prueba_coink.function_obtener_municipios(text);

CREATE OR REPLACE FUNCTION prueba_coink.function_obtener_municipios(
	municipio_nombre_param text)
    RETURNS TABLE(municipioid integer, municipiocodigo text, municipionombre text, departamentoid integer, departamentocodigo text, departamentonombre text, paisid integer, paiscodigo text, paisnombre text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
        m.municipio_id AS "MunicipioId",
	    m.codigo AS "MunicipioCodigo",
	    m.nombre AS "MunicipioNombre",
	    d.departamento_id AS "DepartamentoId",
	    d.codigo AS "DepartamentoCodigo",
	    d.nombre AS "DepartamentoNombre",
	    p.pais_id AS "PaisId",
	    p.codigo AS "PaisCodigo",
	    p.nombre AS "PaisNombre"
    FROM prueba_coink.municipio m
    JOIN prueba_coink.departamento d ON m.departamento_id = d.departamento_id
    JOIN prueba_coink.pais p ON d.pais_id = p.pais_id
    WHERE m.nombre ILIKE '%' || municipio_nombre_param || '%';
END;
$BODY$;

ALTER FUNCTION prueba_coink.function_obtener_municipios(text)
    OWNER TO postgres;
