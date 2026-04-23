using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web.Controllers;

[Authorize]
public sealed class AdjuntosController(
    SeguimientoDbContext dbContext,
    IArchivoService archivoService,
    IUsuarioActualService usuarioActualService) : Controller
{
    public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
    {
        var evidencia = await dbContext.EvidenciasRegistro
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (evidencia is null)
        {
            return NotFound();
        }

        var stream = await archivoService.AbrirLecturaAsync(evidencia.StorageKey, cancellationToken);
        if (stream is null)
        {
            return NotFound();
        }

        return File(stream, evidencia.TipoMime, evidencia.NombreArchivo);
    }

    [Authorize(Policy = "PuedeEditar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int registroId, CancellationToken cancellationToken)
    {
        var evidencia = await dbContext.EvidenciasRegistro
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (evidencia is null)
        {
            return NotFound();
        }

        await archivoService.EliminarAsync(evidencia.StorageKey);
        dbContext.EvidenciasRegistro.Remove(evidencia);
        dbContext.RegistroAuditoria.Add(new Data.Entities.RegistroAuditoria
        {
            RegistroEscolarId = evidencia.RegistroEscolarId,
            Accion = "Evidencia eliminada",
            CampoModificado = "Adjunto",
            ValorAnterior = evidencia.NombreArchivo,
            UsuarioId = usuarioActualService.GetUsuarioIdOrThrow(),
            FechaCreacion = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Adjunto eliminado correctamente.";
        return RedirectToAction("Details", "Registros", new { id = registroId });
    }
}
