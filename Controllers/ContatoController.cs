using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiStorageAzure.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiStorageAzure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContatoController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        #nullable disable
        public ContatoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("SacConnectionString");
            _tableName = configuration.GetValue<string>("AzureTableName");
        }

        private TableClient GetTableClient() {
            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient(_tableName);

            tableClient.CreateIfNotExists();
            return tableClient;
        }

        [HttpGet("Lista")]
        public IActionResult ListaContatos()
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contatos>().ToList();

            return Ok(contatos);
        }

        [HttpGet("ObterPorNome")]
        public IActionResult ObterPorNome(string nome)
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contatos>(c => c.Nome == nome).ToList();

            return Ok(contatos);
        }

        [HttpPost]
        public IActionResult Criar([FromBody] Contatos contato)
        {
            var tableClient = GetTableClient();

            contato.RowKey = Guid.NewGuid().ToString();
            contato.PartitionKey = contato.RowKey;

            tableClient.UpsertEntity(contato);
            return Ok(contato);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, [FromBody] Contatos contato)
        {
            var tableClient = GetTableClient();
            var contatoTable = tableClient.GetEntity<Contatos>(id,id).Value;

            contatoTable.Nome = contato.Nome;
            contatoTable.Email = contato.Email;
            contatoTable.Telefone = contato.Telefone;

            tableClient.UpsertEntity(contatoTable);
            return Ok(contatoTable);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var tableClient = GetTableClient();
            var contatoTable = tableClient.DeleteEntity(id, id);

            return NoContent();
        }
    }
}
