using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Domain.Entities;
using Infrastructure.Data;

namespace Presentation.WebApp.Controllers;

public class PrestamosController : Controller
{
    private readonly PrestamosDbContext _prestamosDbContext;
    private readonly UsuariosDbContext _usuariosDbContext;
    private readonly LibrosDbContext _librosDbContext;

    public PrestamosController(IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection")!;
        _prestamosDbContext = new PrestamosDbContext(conn);
        _usuariosDbContext = new UsuariosDbContext(conn);
        _librosDbContext = new LibrosDbContext(conn);
    }

    public IActionResult Index()
    {
        var data = _prestamosDbContext.List();
        return View(data);
    }

    [HttpGet]
    public IActionResult Details(Guid id)
    {
        var data = _prestamosDbContext.Details(id);
        if (data == null)
            return NotFound();

        // AJAX para modal de detalles
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("DetailsPartial", data);

        return View(data);
    }

    // ========== AGREGAR PRESTAMO (VISTA NORMAL) ==========
    public IActionResult Create()
    {
        ViewBag.UsuarioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_usuariosDbContext.List(), "Id", "Nombre");
        ViewBag.LibroId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_librosDbContext.List(), "Id", "ISBN");
        return View();
    }

    [HttpPost]
    public IActionResult Create(IM253E03Prestamo prestamo)
    {
        prestamo.Id = Guid.NewGuid();
        _prestamosDbContext.Create(prestamo);
        return RedirectToAction("Index");
    }

    // ========== AGREGAR PRESTAMO (MODAL AJAX) ==========
    [HttpGet]
    public IActionResult CreateModal()
    {
        ViewBag.UsuarioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_usuariosDbContext.List(), "Id", "Nombre");
        ViewBag.LibroId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_librosDbContext.List(), "Id", "ISBN");
        return PartialView("CreatePartial");
    }

    [HttpPost]
    public JsonResult CreateModal(IM253E03Prestamo prestamo)
    {
        try
        {
            prestamo.Id = Guid.NewGuid();
            _prestamosDbContext.Create(prestamo);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ========== EDITAR (NORMAL) ==========
    public IActionResult Edit(Guid id)
    {
        var data = _prestamosDbContext.Details(id);
        if (data == null) return NotFound();
        ViewBag.UsuarioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_usuariosDbContext.List(), "Id", "Nombre", data.UsuarioId);
        ViewBag.LibroId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_librosDbContext.List(), "Id", "ISBN", data.LibroId);
        return View(data);
    }

    [HttpPost]
    public IActionResult Edit(IM253E03Prestamo prestamo)
    {
        _prestamosDbContext.Edit(prestamo);
        return RedirectToAction("Index");
    }

    // ========== EDITAR (MODAL AJAX) ==========
    [HttpGet]
    public IActionResult EditModal(Guid id)
    {
        var data = _prestamosDbContext.Details(id);
        if (data == null) return NotFound();
        ViewBag.UsuarioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_usuariosDbContext.List(), "Id", "Nombre", data.UsuarioId);
        ViewBag.LibroId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_librosDbContext.List(), "Id", "ISBN", data.LibroId);
        return PartialView("EditPartial", data);
    }

    [HttpPost]
    public JsonResult EditModal(IM253E03Prestamo prestamo)
    {
        try
        {
            _prestamosDbContext.Edit(prestamo);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ========== ELIMINAR ==========
    public IActionResult Delete(Guid id)
    {
        _prestamosDbContext.Delete(id);
        return RedirectToAction("Index");
    }
}
