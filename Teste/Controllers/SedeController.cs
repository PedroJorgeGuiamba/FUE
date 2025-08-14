using Teste.Data;
using Teste.Models;
using Teste.Services;
using Teste.ViewModels;
using Teste.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Teste.Controllers
{
    public class SedeController : Controller
    {
        private readonly SedeService _sedeService;
        private readonly LocalizacaoService _localizacaoService;
        private readonly ContactoService _contactoService;
        private readonly FueDbContext _context;
        private readonly ActividadeService _actividadeService;
        private readonly BemService _bemService;
        private readonly EmpresaService _empresaService;

        public SedeController(
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
            var model = new CadastroEmpresaViewModel
            {
                TipoEntidades = await GetDropdownOptions<TipoEntidade>(),
                FormaJuridicas = await GetDropdownOptions<FormaJuridica>(),
                SituacaoActividades = await GetDropdownOptions<SituacaoActividade>(),
                GrupoEmpresarials = await GetDropdownOptions<GrupoEmpresarial>(),
                SucursalNosPaises = await GetDropdownOptions<SucursalNoPais>(),
                TipoContabilidades = await GetDropdownOptions<TipoContabilidade>()
               
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadastroEmpresaViewModel model)
        {
            // Validate capital percentages
            if (Math.Abs(model.CapitalSocialPublico + model.CapitalPrivadoNacional + model.CapitalPrivadoEstrangeiro - 100.0) > 0.01)
            {
                ModelState.AddModelError("TotalCapitalPercentages", "A soma dos percentuais de capital deve ser igual a 100%.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

           
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage),
                    partialView = await this.RenderViewToStringAsync("Create", model)
                });
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

                // Save all changes for Empresa
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

        private async Task PopulateDropdowns(CadastroEmpresaViewModel model)
        {
            model.TipoEntidades = await GetDropdownOptions<TipoEntidade>();
            model.FormaJuridicas = await GetDropdownOptions<FormaJuridica>();
            model.SituacaoActividades = await GetDropdownOptions<SituacaoActividade>();
            model.GrupoEmpresarials = await GetDropdownOptions<GrupoEmpresarial>();
            model.SucursalNosPaises = await GetDropdownOptions<SucursalNoPais>();
            model.TipoContabilidades = await GetDropdownOptions<TipoContabilidade>();
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
            var actividades = await _actividadeService.SearchAsync(termo);
            return Json(actividades.Select(a => new { id = a.ActividadeId, text = $"{a.CodigoCAE} - {a.Descricao}" }));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarBens(string termo)
        {
            var bensOuServicos = await _bemService.SearchAsync(termo);
            return Json(bensOuServicos.Select(a => new { id = a.BemId, text = $"{a.CodigoCNBS} - {a.Descricao}" }));
        }

        public  async Task<IActionResult> Index()
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
            var model = new CadastroEmpresaViewModel
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
    public enum TipoEntidade { Sede, Sucursal }
    public enum FormaJuridica
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
    public enum SituacaoActividade
    {
        AguardaInícioDeActividade,
        EmActividade,
        Interrompida,
        Cessada
    }
    public enum GrupoEmpresarial { Sim, Não }
    public enum SucursalNoPais { Sim, Não }
    public enum TipoContabilidade { Organizada, NãoOrganizada }
}


// Return JSON response to trigger modal
//return Json(new
//{
//    success = true,
//    empresaId = empresa.Id,
//    message = "Empresa criada com sucesso!",
//    partialView = await this.RenderViewToStringAsync("_SuccessModal", new { EmpresaId = empresa.Id })
//});

//catch (DbUpdateException ex)
//{
//    Console.WriteLine($"Erro ao atualizar o banco de dados: {ex.Message}");
//    await transaction.RollbackAsync();
//    ModelState.AddModelError("", "Erro ao gravar os dados no banco de dados. Verifique os valores inseridos.");
//    await PopulateDropdowns(model);
//    return Json(new
//    {
//        success = false,
//        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage),
//        partialView = await this.RenderViewToStringAsync("Create", model)
//    });
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Error saving data: {ex.Message}"); // Replace with proper logging
//    await transaction.RollbackAsync();
//    await PopulateDropdowns(model);
//    return Json(new
//    {
//        success = false,
//        errors = new[] { "Erro ao gravar os dados. Tente novamente ou contate o suporte." },
//        partialView = await this.RenderViewToStringAsync("Create", model)
//    });
//}