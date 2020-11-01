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
        private readonly IMongoCollection<Rol> _roles;
        public UsuarioService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _usuarios = database.GetCollection<Usuario>("usuarios");
            _roles = database.GetCollection<Rol>("roles");
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

        public Usuario ModifyState(string id, string nuevoestado)
        {
            Usuario usuario = new Usuario();
            var filter = Builders<Usuario>.Filter.Eq("id", id);
            var update = Builders<Usuario>.Update
                .Set("estado", nuevoestado);
            usuario = _usuarios.FindOneAndUpdate<Usuario>(filter, update, new FindOneAndUpdateOptions<Usuario>
            {
                ReturnDocument = ReturnDocument.After
            });
            return usuario;
        }

        public async Task<UsuarioDTO_UnwindRol> ObtenerUsuarioRol(string id)
        {
            var match = new BsonDocument("$match",
                               new BsonDocument("_id", new ObjectId(id)));

            //lookup para roles
            var subpipeline_rol = new BsonArray
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

            var lookup_rol = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "roles" },
                                        { "let",
                                new BsonDocument("idrol", "$rol") },
                                        { "pipeline",subpipeline_rol },
                                        { "as", "rolobj" }
                                    });
            //lookup para permisos
            var subpipeline_permiso = new BsonArray
                            {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray
                                            {
                                                "$_id",
                                                new BsonDocument("$toObjectId", "$$idpermiso")
                                            })))
                            };
            var lookup_permiso = new BsonDocument("$lookup",
                                     new BsonDocument
                                         {
                                        { "from", "permisos" },
                                        { "let",
                                new BsonDocument("idpermiso", "$rolobj.permisos") },
                                        { "pipeline",subpipeline_permiso
                                    },
                                        { "as", "permisos" }
                                         });

            UsuarioDTO_UnwindRol usuario = new UsuarioDTO_UnwindRol();
            usuario = await _usuarios.Aggregate()
                .AppendStage<Usuario>(match)
                .AppendStage<UsuarioDTO>(lookup_rol)
                .Unwind<UsuarioDTO, UsuarioDTO_UnwindRol>(x => x.rolobj).SingleOrDefaultAsync();
            return usuario;

        }
        public async Task<List<UsuarioDTO_UnwindRol>> ObtenerUsuariosRoles()
        {
            //lookup para roles
            var subpipeline_rol = new BsonArray
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

            var lookup_rol = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "roles" },
                                        { "let",
                                new BsonDocument("idrol", "$rol") },
                                        { "pipeline",subpipeline_rol },
                                        { "as", "rolobj" }
                                    });

            List<UsuarioDTO_UnwindRol> usuario = new List<UsuarioDTO_UnwindRol>();
            usuario = await _usuarios.Aggregate()
                .AppendStage<UsuarioDTO>(lookup_rol)
                .Unwind<UsuarioDTO, UsuarioDTO_UnwindRol>(x => x.rolobj).ToListAsync();
            return usuario;
        }

        public async Task<List<UsuarioDTOR>> ObtenerUsuariosRolesPermisos()
        {
            //lookup para roles
            var subpipeline_rol = new BsonArray
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

            var lookup_rol = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "roles" },
                                        { "let",
                                new BsonDocument("idrol", "$rol") },
                                        { "pipeline",subpipeline_rol },
                                        { "as", "rolobj" }
                                    });
            //lookup para permisos
            var subpipeline_permiso = new BsonArray
                            {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray
                                            {
                                                "$_id",
                                                new BsonDocument("$toObjectId", "$$idpermiso")
                                            })))
                            };
           var lookup_permiso =  new BsonDocument("$lookup",
                                    new BsonDocument
                                        {
                                        { "from", "permisos" },
                                        { "let",
                                new BsonDocument("idpermiso", "$rolobj.permisos") },
                                        { "pipeline",subpipeline_permiso
                                    },
                                        { "as", "permisos" }
                                        });

            //agrupacion de documentos divididos
            var group = new BsonDocument("$group",
                    new BsonDocument
                        {
                            { "_id", "$_id" },
                            { "usuario",
                    new BsonDocument("$first", "$usuario") },
                            { "clave",
                    new BsonDocument("$first", "$clave") },
                            { "datos",
                    new BsonDocument("$first", "$datos") },
                            { "estado",
                    new BsonDocument("$first", "$estado") },
                            { "rol",
                    new BsonDocument("$first", "$rolobj") },
                            { "permisos",
                    new BsonDocument("$push", "$permisos") }
                        });
            //proyeccion de cada documento
            var project = new BsonDocument("$project",
                    new BsonDocument
                        {
                            { "_id", "$_id" },
                            { "usuario", "$usuario" },
                            { "clave", "$clave" },
                            { "datos", "$datos" },
                            { "estado", "$estado" },
                            { "rol",
                    new BsonDocument
                            {
                                { "_id", "$rol.id" },
                                { "descripcion", "$rol.descripcion" },
                                { "area", "$rol.area" },
                                { "permisos","$permisos" }
                            } }
                        });



            List<UsuarioDTOR> usuario = new List<UsuarioDTOR>();
            usuario = await _usuarios.Aggregate()
                .AppendStage<UsuarioDTO>(lookup_rol)
                .Unwind<UsuarioDTO,UsuarioDTO_UnwindRol>(x => x.rolobj)
                .Unwind<UsuarioDTO_UnwindRol, UsuarioDTO_UnwindPermiso>(x => x.rolobj.permisos)
                .AppendStage<UsuarioDTO_LK>(lookup_permiso)
                .Unwind<UsuarioDTO_LK, UsuarioDTO_LK_UW>(x => x.permisos)
                .AppendStage<Usuario_Group>(group)
                .AppendStage<UsuarioDTOR>(project)
                .ToListAsync();
            return usuario;

        }

        public async Task<UsuarioDTOR> ObtenerUsuarioRolPermiso(string id)
        {

            var match = new BsonDocument("$match",
                                new BsonDocument("_id", new ObjectId(id)));

            //lookup para roles
            var subpipeline_rol = new BsonArray
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

            var lookup_rol = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "roles" },
                                        { "let",
                                new BsonDocument("idrol", "$rol") },
                                        { "pipeline",subpipeline_rol },
                                        { "as", "rolobj" }
                                    });
            //lookup para permisos
            var subpipeline_permiso = new BsonArray
                            {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray
                                            {
                                                "$_id",
                                                new BsonDocument("$toObjectId", "$$idpermiso")
                                            })))
                            };
            var lookup_permiso = new BsonDocument("$lookup",
                                     new BsonDocument
                                         {
                                        { "from", "permisos" },
                                        { "let",
                                new BsonDocument("idpermiso", "$rolobj.permisos") },
                                        { "pipeline",subpipeline_permiso
                                    },
                                        { "as", "permisos" }
                                         });

            //agrupacion de documentos divididos
            var group = new BsonDocument("$group",
                    new BsonDocument
                        {
                            { "_id", "$_id" },
                            { "usuario",
                    new BsonDocument("$first", "$usuario") },
                            { "clave",
                    new BsonDocument("$first", "$clave") },
                            { "datos",
                    new BsonDocument("$first", "$datos") },
                            { "estado",
                    new BsonDocument("$first", "$estado") },
                            { "rol",
                    new BsonDocument("$first", "$rolobj") },
                            { "permisos",
                    new BsonDocument("$push", "$permisos") }
                        });
            //proyeccion de cada documento
            var project = new BsonDocument("$project",
                    new BsonDocument
                        {
                            { "_id", "$_id" },
                            { "usuario", "$usuario" },
                            { "clave", "$clave" },
                            { "datos", "$datos" },
                            { "estado", "$estado" },
                            { "rol",
                    new BsonDocument
                            {
                                { "_id", "$rol._id" },
                                { "nombre","$rol.nombre"},
                                { "descripcion", "$rol.descripcion" },
                                { "area", "$rol.area" },
                                { "permisos","$permisos" }
                            } }
                        });



            UsuarioDTOR usuario = new UsuarioDTOR();
            usuario = await _usuarios.Aggregate()
                .AppendStage<Usuario>(match)
                .AppendStage<UsuarioDTO>(lookup_rol)
                .Unwind<UsuarioDTO, UsuarioDTO_UnwindRol>(x => x.rolobj)
                .Unwind<UsuarioDTO_UnwindRol, UsuarioDTO_UnwindPermiso>(x => x.rolobj.permisos)
                .AppendStage<UsuarioDTO_LK>(lookup_permiso)
                .Unwind<UsuarioDTO_LK, UsuarioDTO_LK_UW>(x => x.permisos)
                .AppendStage<Usuario_Group>(group)
                .AppendStage<UsuarioDTOR>(project)
                .SingleOrDefaultAsync();
            return usuario;

        }
        public async Task<List<Rol>> obtenerRolesSistema()
        {
            return await _roles.Find(x => true).ToListAsync();
        }

        public async Task DeleteUser(string id)
        {
            FilterDefinition<Usuario> filtro = Builders<Usuario>.Filter.Eq("id", id);
            await _usuarios.DeleteOneAsync(filtro);
        }

    }
}
