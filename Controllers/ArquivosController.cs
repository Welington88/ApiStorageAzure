using ApiStorageAzure.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiStorageAzure.Controllers;

[ApiController]
[Route("[Controller]")]
public class ArquivosController : ControllerBase
{
    private readonly string _connectionString;
    private readonly string _containerName;
    #nullable disable
    public ArquivosController(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("BlobConnectionString");
        _containerName = configuration.GetValue<string>("BlobContainerName");
    }

    [HttpGet("Lista")]
    public IActionResult ListaArquivo()
    {

        BlobContainerClient _containerClient = new(_connectionString, _containerName);

        var blobsDto = new List<BlobDto>();
        foreach (var blob in _containerClient.GetBlobs())
        {
            blobsDto.Add(new BlobDto {
                Nome = blob.Name,
                Tipo = blob.Properties.ContentType,
                Uri  = _containerClient.Uri.AbsoluteUri + "/" + blob.Name
            });
        }

        return Ok(blobsDto);
    }

    [HttpGet("Download/{nome}")]
    public IActionResult DownloadArquivo(string nome)
    {

        BlobContainerClient _containerClient = new(_connectionString, _containerName);

        BlobClient blob = _containerClient.GetBlobClient(nome);

        if (!blob.Exists())
        {
            return BadRequest();
        }
        var retorno = blob.DownloadContent();
        return File(retorno.Value.Content.ToArray(), retorno.Value.Details.ContentType, blob.Name);
    }

    [HttpPost("Upload/{arquivo}")]
    public IActionResult UploadArquivo(IFormFile arquivo){

        BlobContainerClient _containerClient  = new (_connectionString,_containerName);

        BlobClient blob = _containerClient.GetBlobClient(arquivo.FileName);

        using var data = arquivo.OpenReadStream();
        blob.Upload(data, new BlobUploadOptions{
            HttpHeaders = new BlobHttpHeaders{
                ContentType = arquivo.ContentType
            }
        });

        return Ok(blob.Uri.ToString());
    }

    [HttpDelete("Apagar/{nome}")]
    public IActionResult DeleteArquivo(string nome)
    {

        BlobContainerClient _containerClient = new(_connectionString, _containerName);

        BlobClient blob = _containerClient.GetBlobClient(nome);

        blob.DeleteIfExists();
        return NoContent();
    }
}
