using Teste.Data;
using Teste.Models;
using Teste.Services;
using Teste.ViewModels;
using Teste.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

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
            var actividades = await _actividadeService.GetAllAsync();
            var bens = await _bemService.GetAllAsync();

            var model = new CadastroEmpresaViewModel
            {
                TipoEntidades = await GetDropdownOptions<TipoEntidade>(),
                SituacaoActividades = await GetDropdownOptions<SituacaoActividade>(),
                GrupoEmpresarials = await GetDropdownOptions<GrupoEmpresarial>(),
                SucursalNosPaises = await GetDropdownOptions<SucursalNoPais>(),
                TipoContabilidades = await GetDropdownOptions<TipoContabilidade>(),
                GeneroGestores = await GetDropdownOptions<GeneroGestor>(),
                FormaJuridicas = GetFormaJuridicaDropdown(),
                Meses = GetMesesDropdown(),
                Provincias = GetProvinciasDropdown(),

                TodasActividades = actividades.Select(a => new SelectListItem
                {
                    Value = a.ActividadeId.ToString(),
                    Text = $"{a.CodigoCAE} - {a.Descricao}"
                }).ToList(),
                TodosBens = bens.Select(b => new SelectListItem
                {
                    Value = b.BemId.ToString(),
                    Text = $"{b.CodigoCNBS} - {b.Descricao}"
                }).ToList()
            };

            ViewBag.Actividades = new SelectList(await _context.Actividades.ToListAsync(), "ActividadeId", "Descricao");
            ViewBag.Bens = new SelectList(await _context.Bens.ToListAsync(), "BemId", "Descricao");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadastroEmpresaViewModel model)
        {
            model.ActividadesSecundariasIds ??= new List<int>();
            model.BensSecundariosIds ??= new List<int>();
            // Valida os percentuais de capital
            if (Math.Abs(model.CapitalSocialPublico + model.CapitalPrivadoNacional + model.CapitalPrivadoEstrangeiro - 100.0) > 0.01)
            {
                ModelState.AddModelError("TotalCapitalPercentages", "A soma dos percentuais de capital deve ser igual a 100%.");
            }

            // Valida os IDs de Actividade e Bem
            if (model.ActividadePrincipalId <= 0)
            {
                ModelState.AddModelError("ActividadePrincipalId", "Uma atividade principal deve ser selecionada.");
            }
            if (model.BemPrincipalId <= 0)
            {
                ModelState.AddModelError("BemPrincipalId", "Um bem ou serviço principal deve ser selecionado.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                model.TodasActividades = (await _actividadeService.GetAllAsync())
                   .Select(a => new SelectListItem
                   {
                       Value = a.ActividadeId.ToString(),
                       Text = $"{a.CodigoCAE} - {a.Descricao}"
                   }).ToList();
                model.TodosBens= (await _bemService.GetAllAsync())
                  .Select(b => new SelectListItem
                  {
                      Value = b.BemId.ToString(),
                      Text = $"{b.CodigoCNBS} - {b.Descricao}"
                  }).ToList();
                ViewBag.Actividades = new SelectList(await _context.Actividades.ToListAsync(), "ActividadeId", "Descricao", model.ActividadePrincipalId);
                ViewBag.Bens = new SelectList(await _context.Bens.ToListAsync(), "BemId", "Descricao", model.BemPrincipalId);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Salva a Localização
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

                // 2. Salva a Empresa
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

                //// 3. Salva o Contacto
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
                await _context.SaveChangesAsync();
                // 4. Salva o Responsável
                var responsavel = new Responsavel
                {
                    SedeId = empresa.Id,
                    Nome = model.NomeResponsavel,
                    Funcao = model.FuncaoResponsavel,
                    Telemovel = model.TelemovelResponsavel,
                    Email = model.EmailResponsavel
                };
                _context.Responsaveis.Add(responsavel);
                await _context.SaveChangesAsync();
                //// 5. Salva o Gestor
                var gestorEmpresa = new Gestor
                {
                    Genero = model.GeneroGestor,
                    Nacionalidade = model.NacionalidadeGestor,
                    Idade = model.IdadeGestor,
                    SedeId = empresa.Id
                };
                _context.Gestores.Add(gestorEmpresa);
                await _context.SaveChangesAsync();

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

                //// 7. Salva varias Actividades da Empresa
                if (model.ActividadesSecundariasIds != null && model.ActividadesSecundariasIds.Any())
                {
                    foreach (var atividadeId in model.ActividadesSecundariasIds)
                    {
                        // Verificar se a actividade secundária existe
                        var actividadeSecundariaExists = await _context.Actividades.AnyAsync(a => a.ActividadeId == atividadeId);
                        if (!actividadeSecundariaExists)
                        {
                            ModelState.AddModelError("ActividadesSecundariasIds", $"A atividade secundária com ID {atividadeId} não existe.");
                            await transaction.RollbackAsync();
                            await PopulateDropdownsAfterError(model);
                            return View(model);
                        }

                        // Garante que não está tentando salvar a mesma atividade como principal e secundária
                        if (atividadeId != model.ActividadePrincipalId)
                        {
                            var actividadeEmpresaSecundaria = new ActividadeEmpresa
                            {
                                EmpresaId = empresa.Id,
                                ActividadeId = atividadeId,
                                Tipo = "Secundaria"
                            };
                            _context.Add(actividadeEmpresaSecundaria);
                        }
                    }
                }

                var bemPrincipalExists = await _context.Bens.AnyAsync(b => b.BemId == model.BemPrincipalId);
                if (!bemPrincipalExists)
                {
                    ModelState.AddModelError("BemPrincipalId", "O bem principal selecionado não existe.");
                    await transaction.RollbackAsync();
                    await PopulateDropdownsAfterError(model);
                    return View(model);
                }


                //// 8. Salva a EmpresaBem
                var bemEmpresa = new EmpresaBem
                {
                    EmpresaId = empresa.Id,
                    BemId = model.BemPrincipalId,
                    Tipo = "Principal"

                };
                _context.Add(bemEmpresa);
                await _context.SaveChangesAsync();

                if (model.BensSecundariosIds != null && model.BensSecundariosIds.Any())
                {
                    foreach (var bemId in model.BensSecundariosIds)
                    {
                        // Verificar se o bem secundário existe
                        var bemSecundarioExists = await _context.Bens.AnyAsync(b => b.BemId == bemId);
                        if (!bemSecundarioExists)
                        {
                            ModelState.AddModelError("BensSecundariosIds", $"O bem secundário com ID {bemId} não existe.");
                            await transaction.RollbackAsync();
                            await PopulateDropdownsAfterError(model);
                            return View(model);
                        }

                        // Garante que não está tentando salvar o mesmo bem como principal e secundário
                        if (bemId != model.BemPrincipalId)
                        {
                            var bemEmpresaSecundaria = new EmpresaBem
                            {
                                EmpresaId = empresa.Id,
                                BemId = bemId,
                                Tipo = "Secundario"
                            };
                            _context.Add(bemEmpresaSecundaria);
                        }
                    }
                }

                //// Salva todas as alterações

                //await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Empresa criada com sucesso!";
                TempData["EmpresaId"] = empresa.Id;
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", $"Erro ao gravar os dados: {ex.Message}");
                await PopulateDropdownsAfterError(model);
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

        private async Task PopulateDropdownsAfterError(CadastroEmpresaViewModel model)
        {
            await PopulateDropdowns(model);

            var actividades = await _actividadeService.GetAllAsync();
            var bens = await _bemService.GetAllAsync();

            model.TodasActividades = actividades.Select(a => new SelectListItem
            {
                Value = a.ActividadeId.ToString(),
                Text = $"{a.CodigoCAE} - {a.Descricao}",
                Selected = model.ActividadesSecundariasIds != null && model.ActividadesSecundariasIds.Contains(a.ActividadeId)
            }).ToList();

            model.TodosBens = bens.Select(b => new SelectListItem
            {
                Value = b.BemId.ToString(),
                Text = $"{b.CodigoCNBS} - {b.Descricao}",
                Selected = model.BensSecundariosIds != null && model.BensSecundariosIds.Contains(b.BemId)
            }).ToList();

            ViewBag.Actividades = new SelectList(await _context.Actividades.ToListAsync(), "ActividadeId", "Descricao", model.ActividadePrincipalId);
            ViewBag.Bens = new SelectList(await _context.Bens.ToListAsync(), "BemId", "Descricao", model.BemPrincipalId);
        }

        private async Task PopulateDropdowns(CadastroEmpresaViewModel model)
        {
            model.TipoEntidades = await GetDropdownOptions<TipoEntidade>();
            model.SituacaoActividades = await GetDropdownOptions<SituacaoActividade>();
            model.GrupoEmpresarials = await GetDropdownOptions<GrupoEmpresarial>();
            model.SucursalNosPaises = await GetDropdownOptions<SucursalNoPais>();
            model.TipoContabilidades = await GetDropdownOptions<TipoContabilidade>();
            model.GeneroGestores = await GetDropdownOptions<GeneroGestor>();
            model.FormaJuridicas = GetFormaJuridicaDropdown();
            model.Meses = GetMesesDropdown();
            model.Provincias = GetProvinciasDropdown();
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

        private List<SelectListItem> GetFormaJuridicaDropdown()
        {
            return Enum.GetValues(typeof(FormaJuridica))
                .Cast<FormaJuridica>()
                .Select(p => new SelectListItem
                {
                    Value = p.ToString(),
                    Text = FormatFormaJuridica(p.ToString())
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

        private string FormatFormaJuridica(string formaJuridica)
        {
            // Formata os nomes para ficarem mais legíveis
            return formaJuridica switch
            {
                "EmpresaPúblicaEstatal" => "Empresa Pública Estatal",
                "SociedadeAnónima" => "Sociedade Anónima",
                "SociedadePorQuotas" => "Sociedade Por Quotas",
                "SociedadeUnipessoal" => "Sociedade Unipessoal",
                "EmpresaIndividual" => "Empresa Individual",
                "ConfissãoReligiosa" => "Confissão Religiosa",
                _ => formaJuridica
            };
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
            var model = new CadastroEmpresaViewModel
            {
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Verifica se é uma Sede
                var isSede = await _context.Sedes.AnyAsync(s => s.Id == id);
                var entityName = isSede ? "Sede" : "Empresa";

                // Conta as dependências para feedback
                var sucursaisCount = await _empresaService.GetSucursaisCountAsync(id);
                var actividadesCount = await _empresaService.GetActividadesCountAsync(id);
                var bensCount = await _empresaService.GetBensCountAsync(id);

                int contactosCount = 0, responsaveisCount = 0, gestoresCount = 0;

                if (isSede)
                {
                    contactosCount = await _empresaService.GetContactosCountAsync(id);
                    responsaveisCount = await _empresaService.GetResponsaveisCountAsync(id);
                    gestoresCount = await _empresaService.GetGestoresCountAsync(id);
                }

                var result = await _empresaService.DeleteAsync(id);

                if (result)
                {
                    var entity = isSede ?
                        await _empresaService.GetSedeByIdAsync(id) :
                        await _empresaService.GetByIdAsync(id);

                    TempData["SuccessMessage"] = $"{entityName} '{entity?.Nome}' excluída com sucesso!";

                    // Informação sobre dependências removidas
                    var dependencies = new List<string>();
                    if (actividadesCount > 0) dependencies.Add($"{actividadesCount} actividade(s)");
                    if (bensCount > 0) dependencies.Add($"{bensCount} bem(ns)");
                    if (sucursaisCount > 0) dependencies.Add($"{sucursaisCount} sucursal(is)");
                    if (contactosCount > 0) dependencies.Add($"{contactosCount} contacto(s)");
                    if (responsaveisCount > 0) dependencies.Add($"{responsaveisCount} responsável(eis)");
                    if (gestoresCount > 0) dependencies.Add($"{gestoresCount} gestor(e)s");

                    if (dependencies.Any())
                    {
                        TempData["InfoMessage"] = "Foram também removidos: " + string.Join(", ", dependencies);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"{entityName} não encontrada.";
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Não foi possível excluir devido a restrições de integridade do banco de dados.";
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro inesperado: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            // Verifica se é uma Sede
            var isSede = await _context.Sedes.AnyAsync(s => s.Id == id);
            var entityName = isSede ? "Sede" : "Empresa";

            object entity = isSede ?
                await _empresaService.GetSedeByIdAsync(id) :
                await _empresaService.GetByIdAsync(id);

            if (entity == null)
            {
                TempData["ErrorMessage"] = $"{entityName} não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EntityType = entityName;
            ViewBag.Entity = entity;

            // Contar dependências
            ViewBag.SucursaisCount = await _empresaService.GetSucursaisCountAsync(id);
            ViewBag.ActividadesCount = await _empresaService.GetActividadesCountAsync(id);
            ViewBag.BensCount = await _empresaService.GetBensCountAsync(id);

            if (isSede)
            {
                ViewBag.ContactosCount = await _empresaService.GetContactosCountAsync(id);
                ViewBag.ResponsaveisCount = await _empresaService.GetResponsaveisCountAsync(id);
                ViewBag.GestoresCount = await _empresaService.GetGestoresCountAsync(id);
            }

            return View(entity);
        }
    }

    public enum TipoEntidade { Sede, Sucursal }
    public enum GeneroGestor { Masculino, Feminino }
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