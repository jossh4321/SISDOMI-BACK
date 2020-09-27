using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class UsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuarios;
        public UsuarioService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _usuarios = database.GetCollection<Usuario>("usuarios");
        }

        public List<Usuario> GetAll()
        {
            List<Usuario> usuarios = new List<Usuario>();
            usuarios = _usuarios.Find(Usuario => true).ToList();
            return usuarios;
        }
        public Usuario GetById(string id)
        {
            Usuario usuario = new Usuario();
            usuario = _usuarios.Find(usuario => usuario.id == id).FirstOrDefault();
            return usuario;
        }

        public Usuario GetByUserNameAndPass(string username, string pass)
        {
            Usuario usuario = new Usuario();
            usuario = _usuarios.Find(usuario => usuario.usuario == username & usuario.clave == pass).FirstOrDefault();
            return usuario;
        }
        public Usuario GetByUserName(string username)
        {
            Usuario usuario = new Usuario();
            usuario = _usuarios.Find(usuario => usuario.usuario == username).FirstOrDefault();
            return usuario;
        }

        public Usuario CreateUser(Usuario usuario)
        {
            _usuarios.InsertOne(usuario);
            return usuario;
        }

        public Usuario ModifyUser(Usuario usuario)
        {
            var filter = Builders<Usuario>.Filter.Eq("id", usuario.id);
            var update = Builders<Usuario>.Update
                .Set("usuario", usuario.usuario)
                .Set("clave", usuario.clave)
                .Set("datos", usuario.datos)
                .Set("rol", usuario.rol);
            usuario = _usuarios.FindOneAndUpdate<Usuario>(filter, update, new FindOneAndUpdateOptions<Usuario>
            {
                ReturnDocument = ReturnDocument.After
            });
            return usuario;
        }

        public async Task<List<UsuarioDTOR>> ObtenerUsuariosRoles()
        {
            var subpipeline = new BsonArray
                                        {
                                            new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$eq",
                                            new BsonArray
                                                        {
                                                            "$_id",
                                                            new BsonDocument("$toObjectId", "$$idrol")
                                                        })))
                                        };

            var lookup = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "roles" },
                                        { "let",
                                new BsonDocument("idrol", "$rol") },
                                        { "pipeline", subpipeline },
                                        { "as", "rolobj" }
                                    });

            var project = new BsonDocument("$project",
                        new BsonDocument
                            {
                                { "_id", "$_id" },
                                { "usuario", "$usuario" },
                                { "clave", "$clave" },
                                { "datos", "$datos" },
                                { "estado", "$estado" },
                                { "rol",
                        new BsonDocument("$arrayElemAt",
                        new BsonArray
                                    {
                                        "$rolobj",
                                        0
                                    }) }
                            });


            List<UsuarioDTOR> usuario = new List<UsuarioDTOR>();
            usuario = await _usuarios.Aggregate()
                .AppendStage<UsuarioDTO>(lookup)
                .AppendStage<UsuarioDTOR>(project)
                .ToListAsync();
            return usuario;

        }

        public async Task<UsuarioDTOR> ObtenerUsuarioRol(string id)
        {

            var match = new BsonDocument("$match",
                                new BsonDocument("_id", new ObjectId(id)));

            var subpipeline = new BsonArray
                                        {
                                            new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$eq",
                                            new BsonArray
                                                        {
                                                            "$_id",
                                                            new BsonDocument("$toObjectId", "$$idrol")
                                                        })))
                                        };

            var lookup = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "roles" },
                                        { "let",
                                new BsonDocument("idrol", "$rol") },
                                        { "pipeline", subpipeline },
                                        { "as", "rolobj" }
                                    });

            var project = new BsonDocument("$project",
                        new BsonDocument
                            {
                                { "_id", "$_id" },
                                { "usuario", "$usuario" },
                                { "clave", "$clave" },
                                { "datos", "$datos" },
                                { "estado", "$estado" },
                                { "rol",
                        new BsonDocument("$arrayElemAt",
                        new BsonArray
                                    {
                                        "$rolobj",
                                        0
                                    }) }
                            });


            UsuarioDTOR usuario = new UsuarioDTOR();
            usuario = await _usuarios.Aggregate()
                .AppendStage<Usuario>(match)
                .AppendStage<UsuarioDTO>(lookup)
                .AppendStage<UsuarioDTOR>(project)
                .SingleOrDefaultAsync();
            return usuario;

        }

        public async Task DeleteUser(string id)
        {
            FilterDefinition<Usuario> filtro = Builders<Usuario>.Filter.Eq("id", id);
            await _usuarios.DeleteOneAsync(filtro);
        }

    }
}
