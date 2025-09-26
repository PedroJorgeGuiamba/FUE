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
        private readonly SucursalService _sucursalService;
        private readonly ActividadeService _actividadeService;
        
        public SucursalController(
            SedeService sedeService,
            LocalizacaoService localizacaoService,
            ContactoService contactoService,
            FueDbContext fueDbContext,
            EmpresaService empresaService,
            SucursalService sucursalService, 
            ActividadeService actividadeService)
        {
            _sedeService = sedeService;
            _localizacaoService = localizacaoService;
            _contactoService = contactoService;
            _context = fueDbContext;
            _empresaService = empresaService;
            _sucursalService = sucursalService;
            _actividadeService = actividadeService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int empresaId)
        {
            var actividades = await _actividadeService.GetAllAsync();
            var empresa = await _empresaService.GetByIdAsync(empresaId);
            if (empresa == null) return NotFound();

            var model = new CadastroSucursalViewModel
            {
                EmpresaId = empresaId,
                SituacaoActividades_Sucursal = await GetDropdownOptions<SituacaoActividade>(),
                GrupoEmpresarials_Sucursal = await GetDropdownOptions<GrupoEmpresarial>(),
                GeneroGestores = await GetDropdownOptions<GeneroGestor>(),
                Meses = GetMesesDropdown(),
                Provincias = GetProvinciasDropdown()
            };
            ViewBag.Actividades = new SelectList(await _context.Actividades.ToListAsync(), "ActividadeId", "Descricao");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadastroSucursalViewModel model)
        {
            if (model.ActividadePrincipalId <= 0)
            {
                ModelState.AddModelError("ActividadePrincipalId", "Uma atividade principal deve ser selecionada.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                ViewBag.Actividades = new SelectList(await _context.Actividades.ToListAsync(), "ActividadeId", "Descricao", model.ActividadePrincipalId);
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
                await _context.SaveChangesAsync();

                // 2. Save Sucursal
                var empresa = await _empresaService.GetByIdAsync(model.EmpresaId);
                if (empresa == null) return NotFound();

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
                    NumTrabalhadoresHomens = (int)model.NumTrabalhadoresHomens_Sucursal,
                    NumTrabalhadoresMulheres = (int)model.NumTrabalhadoresMulheres_Sucursal,
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
                    SedeId = sucursal.Id
                };
                _context.Contactos.Add(contacto);
                _context.SaveChanges();


                var gestorEmpresa = new Gestor
                {
                    Genero = model.GeneroGestor_Sucursal,
                    Nacionalidade = model.NacionalidadeGestor_Sucursal,
                    Idade = model.IdadeGestor_Sucursal,
                    SedeId = sucursal.Id
                };
                _context.Gestores.Add(gestorEmpresa);
                await _context.SaveChangesAsync();

                //var actividadePrincipalExists = await _context.Actividades.AnyAsync(a => a.ActividadeId == model.ActividadePrincipalId_Sucursal);
                //if (!actividadePrincipalExists)
                //{
                //    ModelState.AddModelError("ActividadePrincipalId_Sucursal", "A atividade principal selecionada não existe.");
                //    await transaction.RollbackAsync();
                //    await PopulateDropdownsAfterError(model);
                //    return View(model);
                //}


                //// 6. Salva a ActividadeEmpresa
                //var actividadeEmpresa = new ActividadeEmpresa
                //{
                //    EmpresaId = sucursal.Id,
                //    ActividadeId = model.ActividadePrincipalId_Sucursal,
                //    Tipo = "Principal"
                //};
                //_context.Add(actividadeEmpresa);
                //await _context.SaveChangesAsync();

                var actividadePrincipalExists = await _context.Actividades.AnyAsync(a => a.ActividadeId == model.ActividadePrincipalId);
                if (!actividadePrincipalExists)
                {
                    ModelState.AddModelError("ActividadePrincipalId", "A atividade principal selecionada não existe.");
                    await transaction.RollbackAsync();
                    await PopulateDropdownsAfterError(model);
                    return View(model);
                }


                //// 6. Salva a ActividadeEmpresa
                var actividadeEmpresa = new ActividadeEmpresa
                {
                    EmpresaId = empresa.Id,
                    ActividadeId = model.ActividadePrincipalId,
                    Tipo = "Principal"
                };
                _context.Add(actividadeEmpresa);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Sucursal cadastrada com sucesso!";

                return RedirectToAction("Details", "Sede", new { id = model.EmpresaId });
            }
            catch (Exception ex)
            {
                //    Console.WriteLine($"Error saving sucursal: {ex.Message}");
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Erro ao gravar os dados. Tente novamente ou contate o suporte.");
                await PopulateDropdowns(model);
                return View(model);
            }
        }


        public async Task<IActionResult> Index()
        {
            var sucursal = await _context.Sucursais.ToListAsync();
            return View(sucursal);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sucursal = await _sucursalService.GetByIdAsync(id);
            if (sucursal == null) return NotFound();
            return View(sucursal);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sucursal = await _sucursalService.GetByIdAsync(id);
            if (sucursal == null) return NotFound();
            var model = new CadastroSucursalViewModel
            {
                // Map Empresa properties to ViewModel
                NUIT_Sucursal = sucursal.NUIT,
                Nome_Sucursal = sucursal.Nome,
                Sigla_Sucursal = sucursal.Sigla,
                NumeroAlvara_Sucursal = sucursal.NumeroAlvara,
                AnoConstituicao_Sucursal = sucursal.AnoConstituicao,
                DataInicioAno_Sucursal = sucursal.DataInicioAno,
                DataInicioMes_Sucursal = sucursal.DataInicioMes,
                NumTrabalhadoresHomens_Sucursal = sucursal.NumTrabalhadoresHomens,
                NumTrabalhadoresMulheres_Sucursal = sucursal.NumTrabalhadoresMulheres,
                TipoEntidade_Sucursal = sucursal.TipoEntidade
            };
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _empresaService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateDropdowns(CadastroSucursalViewModel model)
        {
            //model.TipoEntidades_Sucursal = await GetDropdownOptions<TipoEntidade>();
            model.SituacaoActividades_Sucursal = await GetDropdownOptions<SituacaoActividade>();
            model.GrupoEmpresarials_Sucursal = await GetDropdownOptions<GrupoEmpresarial>();
            model.GeneroGestores = await GetDropdownOptions<GeneroGestor>();
            model.Provincias = GetProvinciasDropdown();
            model.Meses = GetMesesDropdown();
        }

        private async Task PopulateDropdownsAfterError(CadastroSucursalViewModel model)
        {
            await PopulateDropdowns(model);

            var actividades = await _actividadeService.GetAllAsync();

            ViewBag.Actividades = new SelectList(await _context.Actividades.ToListAsync(), "ActividadeId", "Descricao", model.ActividadePrincipalId);
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

        private List<SelectListItem> GetMesesDropdown()
        {
            return Enum.GetValues(typeof(Mes))
                .Cast<Mes>()
                .Select(m => new SelectListItem
                {
                    Value = ((int)m).ToString(),
                    Text = m.ToString()
                })
                .ToList();
        }

        private List<SelectListItem> GetProvinciasDropdown()
        {
            return Enum.GetValues(typeof(Provincia))
                .Cast<Provincia>()
                .Select(p => new SelectListItem
                {
                    Value = p.ToString(),
                    Text = FormatProvinciaName(p.ToString())
                })
                .ToList();
        }

        private string FormatProvinciaName(string provinciaName)
        {
            // Formata os nomes para ficarem mais legíveis
            return provinciaName switch
            {
                "MaputoCidade" => "Cidade de Maputo",
                "MaputoProvincia" => "Província de Maputo",
                "CaboDelgado" => "Cabo Delgado",
                _ => provinciaName
            };
        }

        public enum Mes
        {
            Janeiro = 1,
            Fevereiro = 2,
            Marco = 3,
            Abril = 4,
            Maio = 5,
            Junho = 6,
            Julho = 7,
            Agosto = 8,
            Setembro = 9,
            Outubro = 10,
            Novembro = 11,
            Dezembro = 12
        }
        public enum Provincia
        {
            MaputoCidade,
            MaputoProvincia,
            Gaza,
            Inhambane,
            Sofala,
            Manica,
            Tete,
            Zambezia,
            Nampula,
            CaboDelgado,
            Niassa
        }

    }
}