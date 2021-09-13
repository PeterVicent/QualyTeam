using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace QualyTeam.Controllers
{
    public class DocumentosController : Controller
    {
        private readonly Contexto _context;
        private readonly INotyfService _notyf;
        public string[] ExtensoesAceitas = new[] { ".pdf", ".doc", ".xls", ".docx", ".xlsx" };
        public bool ChecaDocumento(IFormFile Arquivo)
        {
            if (ExtensoesAceitas.Contains(Path.GetExtension(Arquivo.FileName).ToLower())) return true;
            else
            {
                _notyf.Error("Documento não enviado. Extensão inválida.");
                return false;
            }
        }
        private bool DocumentoExiste(int CodigoNovo)
        {
            if (_context.Documento.Any(e => e.Codigo == CodigoNovo))
            {
                _notyf.Error("O código do documento já existe, por favor, escolha outro.");

                return true;
            } else return false;
        }

        public async Task<Documento> ProcuraDocumento(int? Codigo)
            => await _context.Documento.AsNoTracking().FirstOrDefaultAsync(e => e.Codigo == Codigo);

        public async Task<Processos> ProcuraProcesso (int? Codigo)
            => await _context.Processo.FirstOrDefaultAsync(e => e.Codigo == Codigo);

        public DocumentosController(Contexto context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Documento.OrderBy(e => e.Titulo).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documento
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        public IActionResult Create()
        {
            ViewBag.ListarProcessos = _context.Processo.ToArray();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Titulo,Processo,Categoria")] Documento documento, IFormFile Arquivo)
        {
            ViewBag.ListarProcessos = _context.Processo.ToArray();

            if (ModelState.IsValid)
            {
                if (Arquivo != null)
                {
                    if (!DocumentoExiste(documento.Codigo))
                    {
                        if (ChecaDocumento(Arquivo))
                        {
                            documento.ExtensaoArquivo = Path.GetExtension(Path.GetFileName(Arquivo.FileName)).ToLower();
                            documento.NomeArquivo = documento.Titulo + documento.ExtensaoArquivo;
                            var ProcessoTMP = await ProcuraProcesso(documento.Processo);
                            documento.NomeProcesso = ProcessoTMP.Processo;
                            documento.Processo = ProcessoTMP.Codigo;

                            var target = new MemoryStream();
                            Arquivo.CopyTo(target);
                            documento.Arquivo = target.ToArray();

                            _context.Add(documento);
                            await _context.SaveChangesAsync();
                            _notyf.Success("Documento enviado com sucesso.");

                            return RedirectToAction(nameof(Index));
                        }
                    }
                } else _notyf.Error("Você deve selecionar um arquivo.");
            } else _notyf.Error("Algo deu errado, por favor, tente novamente.");

            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.ListarProcessos = _context.Processo.ToArray();

            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documento.FindAsync(id);
            if (documento == null)
            {
                return NotFound();
            }
            return View(documento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,Titulo,Processo,Categoria")] Documento documento, IFormFile Arquivo)
        {
            ViewBag.ListarProcessos = _context.Processo.ToArray();

            if (ModelState.IsValid)
            {
                if (Arquivo != null)
                {
                    if (ChecaDocumento(Arquivo))
                    {
                        documento.ExtensaoArquivo = Path.GetExtension(Path.GetFileName(Arquivo.FileName)).ToLower();
                        documento.NomeArquivo = documento.Titulo + documento.ExtensaoArquivo;

                        var target = new MemoryStream();
                        Arquivo.CopyTo(target);
                        documento.Arquivo = target.ToArray();
                    } else return View();
                }
                else
                {
                    var DocumentoTMP = await ProcuraDocumento(documento.Codigo);
                    documento.Arquivo = DocumentoTMP.Arquivo;
                    documento.NomeArquivo = documento.Titulo + DocumentoTMP.ExtensaoArquivo;
                    documento.ExtensaoArquivo = DocumentoTMP.ExtensaoArquivo;
                }

                var ProcessoTMP = await ProcuraProcesso(documento.Processo);
                documento.NomeProcesso = ProcessoTMP.Processo;
                documento.Processo = ProcessoTMP.Codigo;

                _context.Update(documento);
                await _context.SaveChangesAsync();
                _notyf.Success("Documento alterado com sucesso.");
                return RedirectToAction(nameof(Index));
            } else _notyf.Error("Algo deu errado, por favor, tente novamente.");

            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documento
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documento = await _context.Documento.FindAsync(id);
            _context.Documento.Remove(documento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int? id)
        {
            try
            {
                var DocumentoTMP = await ProcuraDocumento(id);
                byte[] bytes = DocumentoTMP.Arquivo;

                return File(bytes, "application/octet-stream", DocumentoTMP.NomeArquivo);
            }
            catch
            {
                _notyf.Error("Algo deu errado ao fazer o download do Documento.");

                return View();
            }
        }
    }
}
