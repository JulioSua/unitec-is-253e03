using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Domain.Entities;
using Infrastructure.Data;

namespace Presentation.WebApp.Controllers;

public class UsuariosController : Controller
{
    private readonly UsuariosDbContext _usuariosDbContext;

    public UsuariosController(IConfiguration configuration)
    {
        _usuariosDbContext = new UsuariosDbContext(configuration.GetConnectionString("DefaultConnection")!);
    }

    public IActionResult Index()
    {
        var data = _usuariosDbContext.List();
        return View(data);
    }

    [HttpGet]
    public IActionResult Details(Guid id)
    {
        var data = _usuariosDbContext.Details(id);
        if (data == null)
            return NotFound();

        // AJAX: devuelve partial para modal
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("DetailsPartial", data);

        // Completo si es navegación normal
        return View(data);
    }

    // ==== CREAR EN MODAL ====
    [HttpGet]
    public IActionResult CreateModal()
    {
        return PartialView("CreatePartial", new IM253E03Usuario());
    }

    [HttpPost]
    public JsonResult CreateModal(IM253E03Usuario usuario)
    {
        try
        {
            usuario.Id = Guid.NewGuid();
            _usuariosDbContext.Create(usuario);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ==== EDITAR EN MODAL ====
    [HttpGet]
    public IActionResult EditModal(Guid id)
    {
        var data = _usuariosDbContext.Details(id);
        if (data == null) return NotFound();
        return PartialView("EditPartial", data);
    }

    [HttpPost]
    public JsonResult EditModal(IM253E03Usuario usuario)
    {
        try
        {
            _usuariosDbContext.Edit(usuario);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ==== PARA FORMULARIO NORMAL (no modal) ====
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(IM253E03Usuario usuario)
    {
        usuario.Id = Guid.NewGuid();
        _usuariosDbContext.Create(usuario);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(Guid id)
    {
        var data = _usuariosDbContext.Details(id);
        if (data == null) return NotFound();
        return View(data);
    }

    [HttpPost]
    public IActionResult Edit(IM253E03Usuario usuario)
    {
        _usuariosDbContext.Edit(usuario);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(Guid id)
    {
        _usuariosDbContext.Delete(id);
        return RedirectToAction("Index");
    }
}
