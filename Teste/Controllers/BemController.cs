using Teste.Data;
using Teste.Models;
using Teste.Services;
using Teste.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Teste.Controllers
{
    public class BemController : Controller
    {

        private readonly BemService _bemService;
        private readonly FueDbContext _context;

        public BemController( 
            FueDbContext fueDbContext,
            BemService bemService)
        {
            _context = fueDbContext;           
            _bemService = bemService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadastroBemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Criar e salvar Bem

            var bem = new Bem
            {
                Descricao = model.Descricao,
                CodigoCNBS = model.CodigoCNBS
            };

            _context.Bens.Add(bem);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
