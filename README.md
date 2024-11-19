# prueba_coink
Resultados para prueba desarrollador backend

Se puede ve en funcionamiento el api en:

http://atony100-002-site11.ktempurl.com/swagger/index.html

Donde se ven funcionales los métodos a través de swagger

# Detalles técnicos

Esta aplicación es creada con visual studio 2022 y .net core 8

## Patrones

Repository
UnitOfWWork
Inyeccion de dependencias

## Base de datos

Postgresql V 16
creada y administrada en GCP
se necesita autorización para conectar desde una ip diferente

# Funcionamiento

## Registrar usuario
![image](https://github.com/user-attachments/assets/31b21af5-9ec8-4b39-899a-ab8401b2b7cd)

El método valida que los códigos de pais, departamento y municipio existan y ademas que estén relacionados, es decir que el departamento esté en el país y el municipio esté en el departamento, si esto no es válido no creará el registro.

## Busqueda de codigos id
![image](https://github.com/user-attachments/assets/3ff53de0-7a30-4542-b30c-35b68bf188da)

En este metodo se puede digitar todo el nombre de un municipio o parte desde un caracter en adelante y el servicio devuelve una lista de los municipios que contengan ese o esos caracteres digitados mostrando los id para pais, dpartamento y ciudad que debe utilizar para crear un registro:
![image](https://github.com/user-attachments/assets/fd0b248e-ae11-4c90-aba1-860530980801)

En este caso deberá usar 44,3,149

## Filtro de usuarios

![image](https://github.com/user-attachments/assets/eb33abb7-af4e-45bb-9fe1-3ed8fed34309)

En este método puede filtrar usuarios por cada uno de los campos expuestos o por uno o varios, es decir, podría consultar todos los usuarios del municipio 149 o todos los usuarios que tenga "a" en su nombre.

Aquí si va a enviar un solo parámetro los demás deben ir vacíos o en cero (0).


