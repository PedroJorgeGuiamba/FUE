using Teste.Data;
using Teste.Models;
using Teste.Services;
using Teste.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Teste.Controllers
{
    public class ActividadeController : Controller
    {

        private readonly ActividadeService _actividadeservice;
        private readonly FueDbContext _context;

        public ActividadeController(
            FueDbContext fueDbContext,
            ActividadeService actividadeService)
        {
            _context = fueDbContext;           
            _actividadeservice = actividadeService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadastroActividadeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Criar e salvar Localização
            var actividade = new Actividade
            {
                Descricao = model.Descricao,
                CodigoCAE = model.CodigoCAE
            };

            _context.Actividades.Add(actividade);
            await _context.SaveChangesAsync();




            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
