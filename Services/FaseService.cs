using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using SISDOMI.DTOs;


namespace SISDOMI.Services
{
    public class FaseService
    {
        private readonly IMongoCollection<Fase> _documentofase;

        public FaseService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _documentofase = database.GetCollection<Fase>("fases");

        }
        public Fase GetByIdResidente(string idresidente)
        {
            Fase fase = new Fase();
            fase = _documentofase.Find(fase => fase.idresidente == idresidente).FirstOrDefault();
            return fase;
        }
        public Fase ModifyFase(Fase documentofase)
        {
            var filter = Builders<Fase>.Filter.Eq("id", documentofase.id);
            var update = Builders<Fase>.Update
                .Set("idresidente", documentofase.idresidente)
                .Set("progreso", documentofase.progreso);

            documentofase = _documentofase.FindOneAndUpdate<Fase>(filter, update, new FindOneAndUpdateOptions<Fase>
            {
                ReturnDocument = ReturnDocument.After
            });
            return documentofase;
        }

        public Fase ModifyStateForDocument(string idresidente, string fase, string area, string tipodocumento)
        {
            Fase faseactualizar = GetByIdResidente(idresidente);
            for (int i = 0; i < faseactualizar.progreso.Count ; i++)
            {
                if (i == Convert.ToInt32(fase)-1)
                {
                    if (area == "educativa")
                    {
                        faseactualizar.progreso[i].educativa.estado = "completo";
                        for (int j = 0; j < faseactualizar.progreso[i].educativa.documentos.Count;j++)
                        {
                            if(faseactualizar.progreso[i].educativa.documentos[j].tipo == tipodocumento)
                            {
                                faseactualizar.progreso[i].educativa.documentos[j].estado = "Completo";
                            }
                            if (!(faseactualizar.progreso[i].educativa.documentos[j].estado == "Completo"))
                            {
                                faseactualizar.progreso[i].educativa.estado = "incompleto";
                            }
                        }
                    }
                    else if (area == "social")
                    {
                        faseactualizar.progreso[i].social.estado = "completo";
                        for (int j = 0; j < faseactualizar.progreso[i].social.documentos.Count; j++)
                        {
                            if (faseactualizar.progreso[i].social.documentos[j].tipo == tipodocumento)
                            {
                                faseactualizar.progreso[i].social.documentos[j].estado = "Completo";
                            }
                            if(!(faseactualizar.progreso[i].social.documentos[j].estado == "Completo"))
                            {
                                faseactualizar.progreso[i].social.estado = "incompleto";
                            }
                        }
                    }
                    else if (area == "psicologica")
                    {
                        faseactualizar.progreso[i].psicologica.estado = "completo";
                        for (int j = 0; j < faseactualizar.progreso[i].psicologica.documentos.Count; j++)
                        {
                            if (faseactualizar.progreso[i].psicologica.documentos[j].tipo == tipodocumento)
                            {
                                faseactualizar.progreso[i].psicologica.documentos[j].estado = "Completo";
                            }
                            if (!(faseactualizar.progreso[i].psicologica.documentos[j].estado == "Completo"))
                            {
                                faseactualizar.progreso[i].psicologica.estado = "incompleto";
                            }
                        }
                    }
                }
            }
            return ModifyFase(faseactualizar);
        }
    }
}