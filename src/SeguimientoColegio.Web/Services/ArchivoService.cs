using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using SeguimientoColegio.Web.Configuration;

namespace SeguimientoColegio.Web.Services;

public interface IArchivoService
{
    Task<ArchivoGuardado> GuardarAdjuntoAsync(IFormFile archivo, int registroId, CancellationToken cancellationToken);
    Task<Stream?> AbrirLecturaAsync(string storageKey, CancellationToken cancellationToken);
    Task EliminarAsync(string storageKey);
}

public sealed record ArchivoGuardado(string StorageKey, string NombreOriginal, string TipoMime, long TamanoBytes);

public sealed class ArchivoService(
    IWebHostEnvironment environment,
    IOptions<FileStorageSettings> settings) : IArchivoService
{
    private static readonly Regex UnsafeCharsRegex = new("[^a-zA-Z0-9._-]", RegexOptions.Compiled);

    public async Task<ArchivoGuardado> GuardarAdjuntoAsync(IFormFile archivo, int registroId, CancellationToken cancellationToken)
    {
        var nombreSeguro = SanitizeFileName(Path.GetFileName(archivo.FileName));
        var extension = Path.GetExtension(nombreSeguro);
        var baseName = Path.GetFileNameWithoutExtension(nombreSeguro);
        var uniqueName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{baseName}{extension}";
        var storageKey = Path.Combine("registros", registroId.ToString(), uniqueName).Replace("\\", "/");
        var fullPath = GetAbsolutePath(storageKey);

        var directory = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(directory);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await archivo.CopyToAsync(stream, cancellationToken);

        return new ArchivoGuardado(storageKey, archivo.FileName, archivo.ContentType, archivo.Length);
    }

    public Task<Stream?> AbrirLecturaAsync(string storageKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var fullPath = GetAbsolutePath(storageKey);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task EliminarAsync(string storageKey)
    {
        var fullPath = GetAbsolutePath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string GetAbsolutePath(string storageKey)
    {
        var root = Path.IsPathRooted(settings.Value.RutaBase)
            ? settings.Value.RutaBase
            : Path.Combine(environment.ContentRootPath, settings.Value.RutaBase);

        return Path.Combine(root, storageKey.Replace("/", Path.DirectorySeparatorChar.ToString()));
    }

    private static string SanitizeFileName(string fileName)
    {
        var normalized = UnsafeCharsRegex.Replace(fileName, "_");
        return string.IsNullOrWhiteSpace(normalized) ? "archivo" : normalized;
    }
}
