using Teste.Data;
using Teste.Models;
using Teste.Services;
using Teste.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Teste.Controllers
{
    public class SucursalController : Controller
    {
        private readonly SedeService _sedeService;
        private readonly LocalizacaoService _localizacaoService;
        private readonly ContactoService _contactoService;
        private readonly FueDbContext _context;
        private readonly EmpresaService _empresaService;
        
        public SucursalController(
            SedeService sedeService,
            LocalizacaoService localizacaoService,
            ContactoService contactoService,
            FueDbContext fueDbContext,
            EmpresaService empresaService)
        {
            _sedeService = sedeService;
            _localizacaoService = localizacaoService;
            _contactoService = contactoService;
            _context = fueDbContext;
            _empresaService = empresaService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int empresaId)
        {
            var empresa = await _empresaService.GetByIdAsync(empresaId);
            if (empresa == null) return NotFound();

            var model = new CadastroSucursalViewModel
            {
                EmpresaId = empresaId,
                TipoEntidades_Sucursal = await GetDropdownOptions<TipoEntidade>(),
                SituacaoActividades_Sucursal = await GetDropdownOptions<SituacaoActividade>(),
                GrupoEmpresarials_Sucursal = await GetDropdownOptions<GrupoEmpresarial>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadastroSucursalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Save Location
                var localizacao = new Localizacao
                {
                    Provincia = model.Provincia_Sucursal,
                    Distrito = model.Distrito_Sucursal,
                    Bairro = model.Bairro_Sucursal,
                    AvenidaRua = model.AvenidaRua_Sucursal,
                    Numero = (int)model.Numero_Sucursal,
                    Latitude = (float)model.Latitude_Sucursal,
                    Longitude = (float)model.Longitude_Sucursal,
                    Referencia = model.Referencia_Sucursal
                };
                _context.Localizacoes.Add(localizacao);
                await _context.SaveChangesAsync(); // Save to get LocalizacaoId

                // 2. Save Sucursal
                //var empresa = await _empresaService.GetByIdAsync(model.EmpresaId);
                //if (empresa == null) return NotFound();

                var sucursal = new Sucursal
                {
                    NUIT = model.NUIT_Sucursal,
                    Nome = model.Nome_Sucursal,
                    Sigla = model.Sigla_Sucursal,
                    NumeroAlvara = model.NumeroAlvara_Sucursal,
                    AnoConstituicao = (int)model.AnoConstituicao_Sucursal,
                    DataInicioAno = (int)model.DataInicioAno_Sucursal,
                    DataInicioMes = (int)model.DataInicioMes_Sucursal,
                    LocalizacaoId = localizacao.LocalizacaoId,
                    TipoEntidade = model.TipoEntidade_Sucursal,
                    SituacaoActividade = model.SituacaoActividade_Sucursal,
                    GrupoEmpresarial = model.GrupoEmpresarial_Sucursal,
                    Empresa = _context.Empresas.Find(model.EmpresaId),
                    EmpresaId = model.EmpresaId
                };
                _context.Sucursais.Add(sucursal);
                _context.SaveChanges();

                // 3. Save Contact
                var contacto = new Contacto
                {
                    Fax1 = model.Fax1_Sucursal,
                    Fax2 = model.Fax2_Sucursal,
                    Telemovel1 = model.Telemovel1_Sucursal,
                    Telemovel2 = model.Telemovel2_Sucursal,
                    Telemovel3 = model.Telemovel3_Sucursal,
                    Email = model.Email_Sucursal,
                    Website = model.Website_Sucursal,
                    SedeId = sucursal.Id // Will be set after SaveChanges
                };
                _context.Contactos.Add(contacto);
                _context.SaveChanges();

                // 4. Save Responsible
                var responsavel = new Responsavel
                {
                    SedeId = sucursal.Id, // Will be set after SaveChanges
                    Nome = model.NomeResponsavel_Sucursal,
                    Funcao = model.FuncaoResponsavel_Sucursal,
                    Telemovel = model.TelemovelResponsavel_Sucursal,
                    Email = model.EmailResponsavel_Sucursal
                };
                _context.Responsaveis.Add(responsavel);

                await _context.SaveChangesAsync(); // Save to generate Ids for Sucursal, Contacto, and Responsavel
                 await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Sucursal cadastrada com sucesso!";

                return RedirectToAction("Details", "Sede", new { id = model.EmpresaId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving sucursal: {ex.Message}"); // Replace with proper logging
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Erro ao gravar os dados. Tente novamente ou contate o suporte.");
                await PopulateDropdowns(model);
                return View(model);
            }
        }

        private async Task PopulateDropdowns(CadastroSucursalViewModel model)
        {
            model.TipoEntidades_Sucursal = await GetDropdownOptions<TipoEntidade>();
            model.SituacaoActividades_Sucursal = await GetDropdownOptions<SituacaoActividade>();
            model.GrupoEmpresarials_Sucursal = await GetDropdownOptions<GrupoEmpresarial>();
        }

        private async Task<IEnumerable<SelectListItem>> GetDropdownOptions<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                });
        }
    }
}