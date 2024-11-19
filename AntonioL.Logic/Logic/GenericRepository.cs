using AntonioL.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using AntonioL.Models.PruebaCoink;
using Npgsql;
using Newtonsoft.Json;
using System.Data;
using AntonioL.Share.Dtos;
using NpgsqlTypes;

namespace AntonioL.Logic.Logic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AntonioLContext _context;

        public GenericRepository(AntonioLContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //throw new NotImplementedException();
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            return await query.ToListAsync();
        }

        public async Task<(int count, IReadOnlyList<T> data)> GetAndCountAsync(Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            int countRegs = await query.CountAsync();

            IReadOnlyList<T> regs = await query.ToListAsync();

            return (countRegs, regs);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            //throw new NotImplementedException();
            return await _context.Set<T>().FindAsync(id);
        }

       

        public async Task<int> Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified; //aqui indico que actualicce los valores y no los duplique
            return await _context.SaveChangesAsync();
        }

        public void AddEntity(T Entity)
        {
            _context.Set<T>().Add(Entity);
        }

        public void UpdateEntity(T Entity)
        {
            _context.Set<T>().Attach(Entity);
            _context.Entry(Entity).State = EntityState.Modified;
        }

        public void DeleteEntity(T Entity)
        {
            _context.Set<T>().Remove(Entity);
        }

        //public async Task<int> RemoveRange(IEnumerable<T> entities)
        //{
        //    _context.Set<T>().RemoveRange(entities);
        //    return await _context.SaveChangesAsync();
        //}

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task<List<T>> ExecuteFunctionAsync<T>(string functionName, params NpgsqlParameter[] parameters)
        {
            var result = new List<T>();
            var connectionString = _context.Database.GetDbConnection().ConnectionString;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"SELECT * FROM {functionName}({string.Join(", ", parameters.Select(p => "@" + p.ParameterName))})", connection))
                {
                    command.CommandType = CommandType.Text; // Para funciones se usa Text en lugar de StoredProcedure
                    command.Parameters.AddRange(parameters);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Usar AutoMapper o manualmente mapear al DTO
                            var entity = MapReaderToEntity<T>(reader);
                            result.Add(entity);
                        }
                    }
                }
            }

            return result;
        }

        // Método para mapear un DataReader al tipo T
        private T MapReaderToEntity<T>(IDataReader reader)
        {
            var properties = typeof(T).GetProperties();
            var entity = Activator.CreateInstance<T>();

            foreach (var property in properties)
            {
                if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                {
                    property.SetValue(entity, reader.GetValue(reader.GetOrdinal(property.Name)));
                }
            }

            return entity;
        }

        public async Task<List<T>> ExecuteStoredProcedureMAsync<T>(string storedProcedureName, params NpgsqlParameter[] parameters)
        {
            var _connectionString = _context.Database.GetDbConnection().ConnectionString;
            var result = new List<T>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Iniciar la transacción
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    bool cursorOpened = false;
                    try
                    {
                        // Ejecutar el procedimiento almacenado (esto abre el cursor)
                        using (var command = new NpgsqlCommand(storedProcedureName, connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters.ToArray());

                            // Aquí, el procedimiento almacenado devuelve un refcursor
                            //var refCursorParameter = new NpgsqlParameter("ref_cursor", NpgsqlDbType.Refcursor) { Direction = ParameterDirection.Output };
                            //command.Parameters.Add(refCursorParameter);

                            // Ejecutar el procedimiento almacenado
                            await command.ExecuteNonQueryAsync();

                            // Recuperar el cursor
                            //var refCursor = (string)command.Parameters["ref_cursor"].Value;

                            // Si el procedimiento es obtener_municipios_proc, usamos el FETCH específico
                            if (storedProcedureName == "prueba_coink.obtener_municipios_proc")
                            {
                                cursorOpened = true;

                                // Obtener el nombre del refcursor devuelto
                                //string refCursorName = refCursorParameter.Value.ToString();


                                // Usar el refcursor para hacer el FETCH de los datos
                                //////using (var fetchCommand = new NpgsqlCommand($"FETCH ALL IN \"{refCursor}\"", connection, transaction))
                                //////{
                                //////    fetchCommand.CommandTimeout = 60; // Configura un tiempo de espera personalizado

                                //////    using (var reader = await fetchCommand.ExecuteReaderAsync())
                                //////    {
                                //////        while (await reader.ReadAsync())
                                //////        {
                                //////            // Mapeo de resultados a un objeto MunicipioLocalizacionDto
                                //////            var municipio = new MunicipioLocalizacionDto
                                //////            {
                                //////                MunicipioId = reader.GetInt32(0),
                                //////                MunicipioCodigo = reader.GetString(1),
                                //////                MunicipioNombre = reader.GetString(2),
                                //////                DepartamentoId = reader.GetInt32(3),
                                //////                DepartamentoCodigo = reader.GetString(4),
                                //////                DepartamentoNombre = reader.GetString(5),
                                //////                PaisId = reader.GetInt32(6),
                                //////                PaisCodigo = reader.GetString(7),
                                //////                PaisNombre = reader.GetString(8)
                                //////            };

                                //////            result.Add((T)(object)municipio); // Convertir a T si es necesario
                                //////        }
                                //////    }
                                //////}


                                using (var fetchCommand = new NpgsqlCommand("FETCH ALL IN resultado", connection, transaction))
                                {
                                    using (var reader = await fetchCommand.ExecuteReaderAsync())
                                    {
                                        while (await reader.ReadAsync())
                                        {
                                            // Mapeo de resultados a un objeto MunicipioLocalizacionDto
                                            var municipio = new MunicipioLocalizacionDto
                                            {
                                                MunicipioId = reader.GetInt32(0),
                                                MunicipioCodigo = reader.GetString(1),
                                                MunicipioNombre = reader.GetString(2),
                                                DepartamentoId = reader.GetInt32(3),
                                                DepartamentoCodigo = reader.GetString(4),
                                                DepartamentoNombre = reader.GetString(5),
                                                PaisId = reader.GetInt32(6),
                                                PaisCodigo = reader.GetString(7),
                                                PaisNombre = reader.GetString(8)
                                            };

                                            result.Add((T)(object)municipio); // Convertir a T si es necesario
                                        }
                                    }
                                }





                                //using (var fetchCommand = new NpgsqlCommand("FETCH ALL IN resultado", connection, transaction))
                                //{
                                //    using (var reader = await fetchCommand.ExecuteReaderAsync())
                                //    {
                                //        while (await reader.ReadAsync())
                                //        {
                                //            // Mapeo de resultados a un objeto MunicipioLocalizacionDto
                                //            var municipio = new MunicipioLocalizacionDto
                                //            {
                                //                MunicipioId = reader.GetInt32(0),
                                //                MunicipioCodigo = reader.GetString(1),
                                //                MunicipioNombre = reader.GetString(2),
                                //                DepartamentoId = reader.GetInt32(3),
                                //                DepartamentoCodigo = reader.GetString(4),
                                //                DepartamentoNombre = reader.GetString(5),
                                //                PaisId = reader.GetInt32(6),
                                //                PaisCodigo = reader.GetString(7),
                                //                PaisNombre = reader.GetString(8)
                                //            };

                                //            result.Add((T)(object)municipio); // Convertir a T si es necesario
                                //        }
                                //    }
                                //}

                                // Cerrar el cursor
                                //using (var closeCommand = new NpgsqlCommand("CLOSE resultado", connection, transaction))
                                //{
                                //    await closeCommand.ExecuteNonQueryAsync();
                                //}
                            }
                            else
                            {
                                // Si es otro procedimiento almacenado, lo manejamos de manera diferente (si es necesario)
                                //using (var reader = await command.ExecuteReaderAsync())
                                //{
                                //    var serializer = new JsonSerializer();
                                //    while (await reader.ReadAsync())
                                //    {
                                //        var item = JsonConvert.DeserializeObject<T>(reader.GetString(0));
                                //        result.Add(item);
                                //    }
                                //}
                            }
                        }
                        // Commit de la transacción si todo sale bien
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {

                        // Si ocurre un error, revertimos la transacción
                        await transaction.RollbackAsync();

                        // Intentar cerrar el cursor si fue abierto
                        if (cursorOpened)
                        {
                            await CloseCursorIfNeededAsync(connection, transaction, "ref_cursor");
                        }

                        Console.WriteLine($"Error durante la transacción: {ex.Message}");
                    }
                    finally
                    {
                        // Cerrar el cursor si no se cerró antes
                        if (cursorOpened)
                        {
                            await CloseCursorIfNeededAsync(connection, transaction, "ref_cursor");
                        }
                    }
                }
            }

            return result;
        }

        private async Task CloseCursorIfNeededAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string cursorName)
        {
            try
            {
                using (var closeCommand = new NpgsqlCommand($"CLOSE {cursorName}", connection, transaction))
                {
                    await closeCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Manejar errores de cierre del cursor si es necesario (registro, etc.)
                Console.WriteLine($"Error al cerrar el cursor '{cursorName}': {ex.Message}");
            }
        }

        public async Task<string> ExecuteStoredProcedureAsync(string procedureName, params NpgsqlParameter[] parameters)
        {
            try
            {
                // Construye la llamada al procedimiento
                var parameterList = string.Join(", ", parameters.Select(p => $"@{p.ParameterName}"));
                var commandText = $"CALL {procedureName}({parameterList}, 'mensaje')";

                await using var command = _context.Database.GetDbConnection().CreateCommand();

                command.CommandText = commandText;
                command.CommandType = CommandType.Text;

                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                //Parámetro de salida
                var outputParameter = new NpgsqlParameter("mensaje", NpgsqlDbType.Varchar)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParameter);

                // Log para depurar el SQL generado
                //Console.WriteLine(command.CommandText);

                await _context.Database.OpenConnectionAsync();

                await command.ExecuteNonQueryAsync();

                //return outputParameter.Value?.ToString();
                return outputParameter.Value?.ToString() ?? "Sin mensaje.";

            }
            catch (PostgresException pgEx)
            {
                // Captura los errores personalizados lanzados desde el procedimiento
                throw new Exception($"Error en PostgreSQL: {pgEx.MessageText}", pgEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar el procedimiento {procedureName}: {ex.Message}", ex);
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }



        //public async Task<IDbContextTransaction> BeginTransactionAsync()
        //{
        //    return await _context.Database.BeginTransactionAsync();
        //}
    }
}
