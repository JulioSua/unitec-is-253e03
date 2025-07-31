using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Domain.Entities;
using Infrastructure.Data;
using Application.Services;

namespace Presentation.WebApp.Controllers
{
    public class LibrosController : Controller
    {
        private readonly LibrosDbContext _librosDbContext;

        public LibrosController(IConfiguration configuration)
        {
            _librosDbContext = new LibrosDbContext(configuration.GetConnectionString("DefaultConnection")!);
        }

        public IActionResult Index()
        {
            var data = _librosDbContext.List();
            return View(data);
        }

        // ===== DETALLES: Vista normal o AJAX modal =====
        public IActionResult Details(Guid id)
        {
            var data = _librosDbContext.Details(id);
            if (data == null) return NotFound();

            // Esto detecta si es una llamada AJAX (para el modal)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("DetailsPartial", data);

            // Si NO es AJAX, muestra la vista completa de detalles
            return View(data);
        }

        // ========== CREAR LIBRO ==========

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(IM253E03Libro libro, IFormFile? file)
        {
            if (file != null && file.Length > 0)
                libro.Foto = FileConverterService.ConvertToBase64(file.OpenReadStream());
            libro.Id = Guid.NewGuid();
            _librosDbContext.Create(libro);
            return RedirectToAction("Index");
        }

        // ========== CREAR LIBRO DESDE MODAL (AJAX) ==========

        [HttpGet]
        public IActionResult CreateModal()
        {
            return PartialView("CreatePartial");
        }

        [HttpPost]
        public JsonResult CreateModal(IM253E03Libro libro, IFormFile? file)
        {
            try
            {
                if (file != null && file.Length > 0)
                    libro.Foto = FileConverterService.ConvertToBase64(file.OpenReadStream());
                libro.Id = Guid.NewGuid();
                _librosDbContext.Create(libro);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ========== EDITAR LIBRO (VISTA NORMAL) ==========

        public IActionResult Edit(Guid id)
        {
            var data = _librosDbContext.Details(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(IM253E03Libro libro, IFormFile? file)
        {
            if (file != null && file.Length > 0)
                libro.Foto = FileConverterService.ConvertToBase64(file.OpenReadStream());
            _librosDbContext.Edit(libro);
            return RedirectToAction("Index");
        }

        // ========== EDITAR LIBRO DESDE MODAL (AJAX) ==========

        [HttpGet]
        public IActionResult EditModal(Guid id)
        {
            var data = _librosDbContext.Details(id);
            if (data == null) return NotFound();
            return PartialView("EditPartial", data);
        }

        [HttpPost]
        public JsonResult EditModal(IM253E03Libro libro, IFormFile? file)
        {
            try
            {
                if (file != null && file.Length > 0)
                    libro.Foto = FileConverterService.ConvertToBase64(file.OpenReadStream());
                _librosDbContext.Edit(libro);
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
            _librosDbContext.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
