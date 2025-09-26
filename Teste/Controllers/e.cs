using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Teste.Data;
using Teste.Models;
using Teste.Services;
using Teste.ViewModels;
using Teste.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace Teste.Controllers
{
    public class e : Controller
    {
        private readonly SedeService _sedeService;
        private readonly LocalizacaoService _localizacaoService;
        private readonly ContactoService _contactoService;
        private readonly FueDbContext _context;
        private readonly ActividadeService _actividadeService;
        private readonly BemService _bemService;
        private readonly EmpresaService _empresaService;

        public e(
            LocalizacaoService localizacaoService,
            ContactoService contactoService,
            FueDbContext fueDbContext,
            ActividadeService actividadeService,
            SedeService sedeService,
            BemService bemService,
            EmpresaService empresaService
            )
        {
            _context = fueDbContext;
            _contactoService = contactoService;
            _localizacaoService = localizacaoService;
            _sedeService = sedeService;
            _actividadeService = actividadeService;
            _bemService = bemService;
            _empresaService = empresaService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new eViewModel
            {
                TipoEntidades = await GetDropdownOptions<TipoEntidades>(),
                FormaJuridicas = await GetDropdownOptions<FormaJuridicas>(),
                SituacaoActividades = await GetDropdownOptions<SituacaoActividades>(),
                GrupoEmpresarials = await GetDropdownOptions<GrupoEmpresarials>(),
                SucursalNosPaises = await GetDropdownOptions<SucursalNoPaiss>(),
                TipoContabilidades = await GetDropdownOptions<TipoContabilidades>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(eViewModel model)
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
                    Provincia = model.Provincia,
                    Distrito = model.Distrito,
                    Bairro = model.Bairro,
                    AvenidaRua = model.AvenidaRua,
                    Numero = model.Numero,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    Referencia = model.Referencia
                };
                _context.Localizacoes.Add(localizacao);
                await _context.SaveChangesAsync();

                // 2. Save Empresa
                var empresa = new Empresa
                {
                    NUIT = model.NUIT,
                    Nome = model.Nome,
                    Sigla = model.Sigla,
                    NumeroAlvara = model.NumeroAlvara,
                    AnoConstituicao = model.AnoConstituicao,
                    DataInicioAno = model.DataInicioAno,
                    DataInicioMes = model.DataInicioMes,
                    LocalizacaoId = localizacao.LocalizacaoId,
                    NumTrabalhadoresHomens = model.NumTrabalhadoresHomens,
                    NumTrabalhadoresMulheres = model.NumTrabalhadoresMulheres,
                    TipoEntidade = model.TipoEntidade,
                    FormaJuridica = model.FormaJuridica,
                    TipoContabilidade = model.TipoContabilidade,
                    SituacaoActividade = model.SituacaoActividade,
                    SucursalNoPais = model.SucursalNoPais,
                    QuantidadeSucursalNoPais = model.QuantidadeSucursalNoPais,
                    GrupoEmpresarial = model.GrupoEmpresarial,
                    NomeGrupoEmpresarial = model.NomeGrupoEmpresarial,
                    PaisGrupoEmpresarial = model.PaisGrupoEmpresarial,
                    CapitalSocial = model.CapitalSocial,
                    VolumeNegocios = model.VolumeNegocios,
                    Despesas = model.Despesas,
                    CapitalSocialPublico = model.CapitalSocialPublico,
                    CapitalPrivadoNacional = model.CapitalPrivadoNacional,
                    CapitalPrivadoEstrangeiro = model.CapitalPrivadoEstrangeiro
                };
                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();

                // 3. Save Contact
                var contacto = new Contacto
                {
                    Fax1 = model.Fax1,
                    Fax2 = model.Fax2,
                    Telemovel1 = model.Telemovel1,
                    Telemovel2 = model.Telemovel2,
                    Telemovel3 = model.Telemovel3,
                    Email = model.Email,
                    Website = model.Website,
                    SedeId = empresa.Id
                };
                _context.Contactos.Add(contacto);

                // 4. Save Responsible
                var responsavel = new Responsavel
                {
                    SedeId = empresa.Id,
                    Nome = model.NomeResponsavel,
                    Funcao = model.FuncaoResponsavel,
                    Telemovel = model.TelemovelResponsavel,
                    Email = model.EmailResponsavel
                };
                _context.Responsaveis.Add(responsavel);

                // 5. Save Actividades (Principal and Secondaries)
                if (model.ActividadePrincipalId != 0)
                {
                    var actividadeEmpresa = new ActividadeEmpresa
                    {
                        EmpresaId = empresa.Id,
                        ActividadeId = model.ActividadePrincipalId,
                        Tipo = "Principal"
                    };
                    _context.ActividadeEmpresas.Add(actividadeEmpresa);
                }

                foreach (var id in model.ActividadesSecundariasId ?? new List<int>())
                {
                    if (id != model.ActividadePrincipalId)
                    {
                        var actividadeEmpresa = new ActividadeEmpresa
                        {
                            EmpresaId = empresa.Id,
                            ActividadeId = id,
                            Tipo = "Secundária"
                        };
                        _context.ActividadeEmpresas.Add(actividadeEmpresa);
                    }
                }

                // 6. Save Bens e Serviços (Principal and Secondaries)
                if (model.BemPrincipalId != 0)
                {
                    var empresaBem = new EmpresaBem
                    {
                        EmpresaId = empresa.Id,
                        BemId = model.BemPrincipalId,
                        Tipo = "Principal"
                    };
                    _context.EmpresaBens.Add(empresaBem);
                }

                foreach (var id in model.BensSecundariosId ?? new List<int>())
                {
                    if (id != model.BemPrincipalId)
                    {
                        var empresaBem = new EmpresaBem
                        {
                            EmpresaId = empresa.Id,
                            BemId = id,
                            Tipo = "Secundária"
                        };
                        _context.EmpresaBens.Add(empresaBem);
                    }
                }

                // Save all changes
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Empresa criada com sucesso!";
                TempData["EmpresaId"] = empresa.Id;
                return RedirectToAction("Create");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}"); // Replace with proper logging
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Erro ao gravar os dados. Tente novamente ou contate o suporte.");
                await PopulateDropdowns(model);
                return View(model);
            }
        }

        public IActionResult Confirmacao()
        {
            if (TempData["EmpresaCriada"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.EmpresaId = TempData["NovaEmpresaId"];
            return View();
        }

        private async Task PopulateDropdowns(eViewModel model)
        {
            model.TipoEntidades = await GetDropdownOptions<TipoEntidades>();
            model.FormaJuridicas = await GetDropdownOptions<FormaJuridicas>();
            model.SituacaoActividades = await GetDropdownOptions<SituacaoActividades>();
            model.GrupoEmpresarials = await GetDropdownOptions<GrupoEmpresarials>();
            model.SucursalNosPaises = await GetDropdownOptions<SucursalNoPaiss>();
            model.TipoContabilidades = await GetDropdownOptions<TipoContabilidades>();
        }

        private async Task<IEnumerable<SelectListItem>> GetDropdownOptions<T>() where T : Enum
        {
            // Ideally, fetch from a database or configuration
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                });
        }

        [HttpGet]
        public async Task<IActionResult> BuscarActividades(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return Json(new List<object>());
            }

            var actividades = await _actividadeService.SearchAsync(termo);
            return Json(actividades.Select(a => new { id = a.ActividadeId, text = $"{a.CodigoCAE} - {a.Descricao}" }));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarBens(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return Json(new List<object>());
            }

            var bensOuServicos = await _bemService.SearchAsync(termo);
            return Json(bensOuServicos.Select(a => new { id = a.BemId, text = $"{a.CodigoCNBS} - {a.Descricao}" }));
        }

        public async Task<IActionResult> Index()
        {
            var empresas = await _context.Empresas.ToListAsync();
            return View(empresas);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var empresa = await _empresaService.GetByIdAsync(id);
            if (empresa == null) return NotFound();
            return View(empresa);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var empresa = await _empresaService.GetByIdAsync(id);
            if (empresa == null) return NotFound();
            var model = new eViewModel
            {
                // Map Empresa properties to ViewModel
                NUIT = empresa.NUIT,
                Nome = empresa.Nome,
                Sigla = empresa.Sigla,
                NumeroAlvara = empresa.NumeroAlvara,
                AnoConstituicao = empresa.AnoConstituicao,
                DataInicioAno = empresa.DataInicioAno,
                DataInicioMes = empresa.DataInicioMes,
                NumTrabalhadoresHomens = empresa.NumTrabalhadoresHomens,
                NumTrabalhadoresMulheres = empresa.NumTrabalhadoresMulheres,
                TipoEntidade = empresa.TipoEntidade
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
    }

    // Define enums for dropdowns (move to a separate file if preferred)
    public enum TipoEntidades { Sede, Sucursal }
    public enum FormaJuridicas
    {
        EmpresaPúblicaEstatal,
        SociedadeAnónima,
        SociedadePorQuotas,
        SociedadeUnipessoal,
        EmpresaIndividual,
        Cooperativa,
        Associação,
        Fundação,
        ConfissãoReligiosa,
        ONG,
        Outra
    }
 
    public enum SituacaoActividades
    {
        AguardaInícioDeActividade,
        EmActividade,
        Interrompida,
        Cessada
    }
    public enum GrupoEmpresarials { Sim, Não }
    public enum SucursalNoPaiss { Sim, Não }
    public enum TipoContabilidades { Organizada, NãoOrganizada }
}