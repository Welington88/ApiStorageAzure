using System;
using Azure;
using Azure.Data.Tables;

namespace ApiStorageAzure.Models;
#nullable disable
public class Contatos : ITableEntity
{
    public string Nome { get; set; }

    public string Telefone { get; set; }

    public string Email { get; set; }

    public string PartitionKey { get ; set ; }

    public string RowKey { get ; set ; }

    public DateTimeOffset? Timestamp { get ; set ; }

    public ETag ETag { get ; set ; }
}

