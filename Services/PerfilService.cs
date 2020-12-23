using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SISDOMI.DTOs;

namespace SISDOMI.Services
{
    public class PerfilService
    {
        private readonly IMongoCollection<Usuario> _usuarios;


        public PerfilService(ISysdomiDatabaseSettings settings)
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


        public async Task<Usuario> ModifyUser(Usuario usuario)
        {
            var filter = Builders<Usuario>.Filter.Eq("id", usuario.id);
            var update = Builders<Usuario>.Update
                .Set("usuario", usuario.usuario)
                .Set("datos", usuario.datos);
            usuario = await _usuarios.FindOneAndUpdateAsync<Usuario>(filter, update, new FindOneAndUpdateOptions<Usuario>
            {
                ReturnDocument = ReturnDocument.After
            });
            return usuario;
        }



    }
}
