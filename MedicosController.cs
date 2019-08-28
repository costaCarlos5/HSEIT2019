using CuidadosContinuados.Models;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CuidadosContinuados.Controllers
{
    public class MedicosController : Controller
    {
        private HospitalDb db = new HospitalDb();
        //private object iTextSharp;

        [HttpGet]
        public ActionResult Index(string SearchString = null)
        {

            if (SearchString != null || SearchString == "")
            {
                List<Utente> lista = db.uts.ToList().Where(c => c.Name.Contains(SearchString) || c.HSE.Contains(SearchString)).ToList();
                MedicosViewPage dados1 = new MedicosViewPage
                {
                    utentes = lista,
                    refe = db.Dados.ToList().Where(c =>
                    {
                        for (int i = 0; i < lista.Count; i++)
                        {
                            if (lista[i].Id == c.UtenteId) return true;
                        }
                        return false;
                    }).ToList().Take(20).ToList(),
                    enfe = db.enfs.ToList()
                };

                return View(dados1);
            }
            MedicosViewPage dados2 = new MedicosViewPage { utentes = db.uts.ToList(), refe = db.Dados.ToList().Take(20).ToList(), enfe = db.enfs.ToList() };
            return View(dados2);
        }

        // GET: Referenciacaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Referenciacao referenciacao = db.Dados.Find(id);

            if (referenciacao == null)
            {
                return HttpNotFound();
            }
            Utente utente = db.uts.Find(referenciacao.UtenteId);
            return View(new DetailsModel { refe = referenciacao, ut = utente });
        }

        [HttpGet]
        public ActionResult Create(string SearchString = null)
        {
            var refes = db.Dados.ToList();
            var lista = db.uts.ToList().Where(c =>
            {
                for (int i = 0; i < refes.Count; i++)
                {
                    if (refes[i].UtenteId == c.Id) return false;
                }
                return true;
            }).ToList();

            if (SearchString != null)
            {
                lista = lista.Where(c => c.Name.Contains(SearchString)).Take(20).ToList();
                return View(lista);
            }

            return View(lista.Take(20));
        }

        // POST: Referenciacaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(int Id)
        {

            Referenciacao referenciacao = new Referenciacao { UtenteId = Id };
            referenciacao.Cuidador = new Cuidador { Name = "ChangeMe", EstadoCivil = "ChangeMe", GrauDeParentesco = "ChangeMe", Morada = "ChangeMe", Nascimento = new DateTime(), NumeroTelefone = "ChangeMe" };
            referenciacao.DCronicas = new DCER();
            referenciacao.NCCD = new NCCD();
            referenciacao.NTC = new NTC();
            referenciacao.NE = new NE();
            referenciacao.IRS = new IRS();
            referenciacao.CP = new CP();
            referenciacao.ECE = new ECE();
            referenciacao.CuidadorDetalhes = new CuidadorDetalhes();
            referenciacao.Criacao = new DateTime();
            referenciacao.DataDeAlta = new DateTime();

            db.Dados.Add(referenciacao);
            db.SaveChanges();
            return RedirectToAction("CreateEnf", new { Id = referenciacao.Id });



        }

        // GET: Referenciacaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Referenciacao referenciacao = db.Dados.Find(id);
            if (referenciacao == null)
            {
                return HttpNotFound();
            }
            return View(referenciacao);
        }

        // POST: Referenciacaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Cuidador,EntidadeReferenciadora,DiagnosticoClinico,DataDeAlta,CriteriosDeTriagem,DependenciaAVD,Desnutricao,Deteorioracao,ProblemasSensoriais,DCronicas,NCCD,NTC,CP")] Referenciacao referenciacao, int Id)
        {
            if (ModelState.IsValid)
            {
                var refe = db.Dados.Find(Id);
                if (refe != null)
                {
                    refe.EntidadeReferenciadora = referenciacao.EntidadeReferenciadora;
                    refe.DiagnosticoClinico = referenciacao.DiagnosticoClinico;
                    refe.DataDeAlta = referenciacao.DataDeAlta;
                    refe.CriteriosDeTriagem = referenciacao.CriteriosDeTriagem;
                    refe.DependenciaAVD = referenciacao.DependenciaAVD;
                    refe.Desnutricao = referenciacao.Desnutricao;
                    refe.Deteorioracao = referenciacao.Deteorioracao;
                    refe.ProblemasSensoriais = referenciacao.ProblemasSensoriais;
                    refe.DCronicas = referenciacao.DCronicas;
                    refe.NCCD = referenciacao.NCCD;
                    refe.NTC = referenciacao.NTC;
                    refe.CP = referenciacao.CP;
                    refe.MedicoOk = true;
                    db.Entry(refe).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View(referenciacao);
        }

        // GET: Referenciacaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Referenciacao referenciacao = db.Dados.Find(id);
            if (referenciacao == null)
            {
                return HttpNotFound();
            }
            DeleteModel viewTemplate = new DeleteModel { refe = referenciacao, utente = db.uts.Where(c => c.Id == referenciacao.UtenteId).SingleOrDefault() };
            return View(viewTemplate);
        }

        // POST: Referenciacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Referenciacao referenciacao = db.Dados.Find(id);
            db.Dados.Remove(referenciacao);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateEnf(int? Id, string SearchString)
        {
            ViewBag.Message = Id;
            if (SearchString != null)
            {
                var lista = db.enfs.Where(c => c.Name.Contains(SearchString)).Take(20).ToList();
                return View(lista);
            }

            return View(db.enfs.ToList().Take(20));
        }

        [HttpPost]
        public ActionResult CreateEnf(int Id, int EnfId)
        {
            var refe = db.Dados.Find(Id);
            if (refe != null)
            {
                refe.Enfermeiro = EnfId;
                db.Entry(refe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { Id = refe.Id });
            }
            return View();

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult ImprimePDF(int id)
        {

            Byte[] bytes = GerarPDF(id);


            return new FileContentResult(bytes, "application/pdf");


        }

        private Byte[] GerarPDF(int id)
        {
            Referenciacao referenciacao = db.Dados.Find(id);
            Utente utente = db.uts.Where(x => x.Id == referenciacao.UtenteId).FirstOrDefault();

            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var pdfDoc = new Document(PageSize.A4, 25, 25, 25, 50))
                {
                    using (var pdfWriter = PdfWriter.GetInstance(pdfDoc, ms))
                    {
                        pdfDoc.Open();

                        try
                        {
                            pdfDoc.Open();

                            String FONT_CHECKBOX = "c:/windows/fonts/WINGDING.TTF";
                            string Ischecked = "\u00fe";
                            string NotChecked = "o";

                            BaseFont bf = BaseFont.CreateFont(FONT_CHECKBOX, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                            Font f = new Font(bf, 8);

                            Font textoColuna = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
                            Font textoOutras = FontFactory.GetFont("Arial", 7, Font.ITALIC, BaseColor.BLACK);
                            Font textoCabecalho = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);

                            PdfPCell cell = new PdfPCell();
                            PdfPTable table = new PdfPTable(1);
                            Paragraph p = new Paragraph();
                            p.Add(new Chunk("\n"));
                            string imagepath = Server.MapPath("~/Images");
                            Image jpg = Image.GetInstance(imagepath + "/RRCCI.jpg");
                            //jpg.ScalePercent(80f);
                            jpg.ScaleAbsolute(60, 60);
                            p.Add(new Chunk(jpg, 0, -50));
                            imagepath = Server.MapPath("~/Images");
                            Image png = Image.GetInstance(imagepath + "/barra_azul.png");
                            png.ScaleAbsolute(10, 70);
                            p.Add(new Chunk(png, 0, -50));
                            p.Add(new Chunk("Estrutura de Missão - Rede Regional de Cuidados Continuados \n\n                     Integrados\n\n\n\n", FontFactory.GetFont("Arial", 12, Font.NORMAL, new BaseColor(102, 178, 255).Darker())));
                            cell = new PdfPCell(p);
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;                       
                            cell.Border = 0;
                            table.AddCell(cell);
                            pdfDoc.Add(table);

                            /*
                            PdfPCell cell = new PdfPCell();
                            PdfPTable table = new PdfPTable(1);                   
                            Paragraph titulo = new Paragraph();
                            string imagepath = Server.MapPath("~/Images");
                            Image jpg = Image.GetInstance(imagepath + "/RRCCI.jpg");
                            jpg.ScalePercent(20f);
                            titulo.Add(new Chunk(jpg, 0, 0));
                            Chunk chunk = new Chunk("Estrutura de Missão - Rede Regional de Cuidados Continuados \nIntegrados", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLUE));
                            titulo.Add(chunk);
                            //titulo.Alignment = Element.ALIGN_LEFT;
                            cell = new PdfPCell(titulo);
                            table.AddCell(cell);
                            pdfDoc.Add(table);*/

                            Chunk modelo1 = new Chunk("MODELO 1. REFERENCIAÇÃO\n\n", FontFactory.GetFont("Arial", 11, Font.BOLD, new BaseColor(93, 188, 210).Darker()));
                            Paragraph paraMod1 = new Paragraph(modelo1);
                            paraMod1.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(paraMod1);

                            

                            /**
                             * Dados utente 
                             */

                            /* TABELA COM 3 COLUNAS */
                            //PdfPTable tableUtente = new PdfPTable(3);
                            //PdfPCell cabecalho = new PdfPCell(new Phrase(" 1.IDENTIFICAÇÃO DO UTENTE", textoCabecalho));
                            //cabecalho.Colspan = 3;
                            //cabecalho.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                            //tableUtente.AddCell(cabecalho);

                            //PdfPCell cell = new PdfPCell(new Phrase("  Nome: ", textoColuna));                     
                            //tableUtente.AddCell(cell);
                            //cell = new PdfPCell(new Paragraph(utente.Name, textoColuna));
                            //cell.Colspan = 3;
                            //cell.BorderWidthLeft = 0;
                            //tableUtente.AddCell(cell);


                            PdfPTable tableUtente = new PdfPTable(1);
                            PdfPCell cabecalho = new PdfPCell(new Phrase(" 1.IDENTIFICAÇÃO DO UTENTE", textoCabecalho));
                            tableUtente.AddCell(cabecalho);

                            cell = new PdfPCell(new Phrase("  Nome:  " + utente.Name, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Morada:  "+ utente.Morada, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);     

                            cell = new PdfPCell(new Paragraph("  Freguesia:  " + utente.Freguesia, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Telefone:  " + utente.NumeroTelefone, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);

                            string dataNascimento = utente.Nascimento.ToShortDateString();
                            cell = new PdfPCell(new Paragraph("  Data de nascimento:  " + dataNascimento, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Estado civil:  " + utente.EstadoCivil, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Escolaridade:  " + utente.Escolaridade, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Nº de beneficiário:  " + utente.Beneficiario, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtente.AddCell(cell);

                            pdfDoc.Add(tableUtente);

                            /**
                             * Dados cuidador
                             */
                            PdfPTable tableCuidador = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 2.IDENTIFICAÇÃO DO CUIDADOR PRINCIPAL", textoCabecalho));
                            tableCuidador.AddCell(cabecalho);

                            cell = new PdfPCell(new Phrase("  Nome:  " + referenciacao.Cuidador.Name, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Morada:  " + referenciacao.Cuidador.Morada, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Telefone:  " + referenciacao.Cuidador.NumeroTelefone, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidador.AddCell(cell);

                            string dataNascimentoCuidador = referenciacao.Cuidador.Nascimento.ToShortDateString();
                            cell = new PdfPCell(new Paragraph("  Data de nascimento:  " + dataNascimentoCuidador, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Estado civil:  " + referenciacao.Cuidador.EstadoCivil, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell(new Paragraph("  Grau de parentesco:  " + referenciacao.Cuidador.GrauDeParentesco, textoColuna));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidador.AddCell(cell);

                            pdfDoc.Add(tableCuidador);

                            /**
                             * Entidade Referenciadora até Problemas Sensoriais
                             */

                            PdfPTable tableUtenteDetalhes = new PdfPTable(1);
                            cell = new PdfPCell(new Phrase(" 3.ENTIDADE REFERENCIADORA:  " + referenciacao.EntidadeReferenciadora, textoCabecalho));         
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell(new Phrase(" 4.DIAGNÓSTICO CLÍNICO:  " + referenciacao.DiagnosticoClinico, textoCabecalho));
                            tableUtenteDetalhes.AddCell(cell);

                            string dataAlta = referenciacao.DataDeAlta.ToShortDateString();
                            cell = new PdfPCell(new Phrase(" 4.1.Previsão de alta:  " + dataAlta, textoCabecalho));
                            tableUtenteDetalhes.AddCell(cell);

                            cabecalho = new PdfPCell(new Phrase(" 5.CRITÉRIOS DE TRIAGEM PARA CUIDADOS CONTINUADOS", textoCabecalho));
                            tableUtenteDetalhes.AddCell(cabecalho);

                            Paragraph paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add( new Chunk(" 5.1.Dependência nas AVD ", textoCabecalho));
                            _ = referenciacao.DependenciaAVD ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            //if (referenciacao.DependenciaAVD) paragraphCheckBox.Add(new Paragraph(Ischecked, f));
                            //else paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtenteDetalhes.AddCell(cell);                       

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add( new Chunk(" 5.2.Desnutrição ", textoCabecalho));
                            _ = referenciacao.Desnutricao ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtenteDetalhes.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add( new Chunk(" 5.3.Deterioração cognitiva ", textoCabecalho));
                            _ = referenciacao.Deteorioracao ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtenteDetalhes.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk(" 5.4.Problemas sensoriais ", textoCabecalho));
                            _ = referenciacao.ProblemasSensoriais ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableUtenteDetalhes.AddCell(cell);

                            pdfDoc.Add(tableUtenteDetalhes);

                            /**
                            * Doenças Crónicas com Episódios de Reagudização
                            */
                            PdfPTable tableDoencasCronicas = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 5.5.Doenças crónicas com episódios de reagudização:", textoCabecalho));
                            tableDoencasCronicas.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         DPOC ", textoColuna));
                            _ = referenciacao.DCronicas.DPOC ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableDoencasCronicas.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         ICC ", textoColuna));
                            _ = referenciacao.DCronicas.ICC ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableDoencasCronicas.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Doença Cérebro Vascular ", textoColuna));
                            _ = referenciacao.DCronicas.DCV ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableDoencasCronicas.AddCell(cell);


                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Outras ", textoColuna));
                            if (referenciacao.DCronicas.Outras != null)
                            {
                                paragraphCheckBox.Add(new Paragraph(Ischecked, f));
                                paragraphCheckBox.Add(new Chunk("  " + referenciacao.DCronicas.Outras, textoOutras));
                            }
                            else paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                            tableDoencasCronicas.AddCell(cell);

                            pdfDoc.Add(tableDoencasCronicas);

                            /**
                             * Necessidade de Continuidade de Cuidados no Domicílio
                             */
                            PdfPTable tableNCCD = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 5.6.Necessidade de Continuidade de Cuidados no Domicílio:", textoCabecalho));
                            tableNCCD.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Tratamento de feridas/úlceras por pressão ", textoColuna));
                            _ = referenciacao.NCCD.TratamentoDeFeridas ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNCCD.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Reabilitação ", textoColuna));
                            _ = referenciacao.NCCD.Reabilitacao ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNCCD.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Manutenção de dispositivos ", textoColuna));
                            _ = referenciacao.NCCD.ManutencaoDeDispositivos ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNCCD.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Gestão de regime terapêutico ", textoColuna));
                            _ = referenciacao.NCCD.GestaoDeRegimeTerapeutico ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNCCD.AddCell(cell);
                            
                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Outras ", textoColuna));
                            if (referenciacao.NCCD.Outras != null)
                            {
                                paragraphCheckBox.Add(new Paragraph(Ischecked, f));
                                paragraphCheckBox.Add(new Chunk("  " + referenciacao.NCCD.Outras, textoOutras));
                            }
                            else paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                            tableNCCD.AddCell(cell);

                            pdfDoc.Add(tableNCCD);

                            /**
                             * Necessidade de tratamentos complexos
                             */
                            PdfPTable tableNTC = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 5.7.Necessidade de tratamentos complexos:", textoCabecalho));
                            tableNTC.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Úlceras por pressão múltiplas ", textoColuna));
                            _ = referenciacao.NTC.UlcerasPorPressaoMultiplas ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNTC.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Portadores de SNG/PEG ", textoColuna));
                            _ = referenciacao.NTC.PortadoresDeSNG_PEG ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));  
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNTC.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Ventilação assistida ", textoColuna));
                            _ = referenciacao.NTC.VentilacaoAssistida ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNTC.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Outras ", textoColuna));
                            if (referenciacao.NTC.Outras != null)
                            {
                                paragraphCheckBox.Add(new Paragraph(Ischecked, f));
                                paragraphCheckBox.Add(new Chunk("  " + referenciacao.NTC.Outras, textoOutras));
                            }
                            else paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                            tableNTC.AddCell(cell);

                            pdfDoc.Add(tableNTC);

                            /**
                             * Cuidados Paliativos
                             */
                            PdfPTable tableCP = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 5.8.Cuidados Paliativos", textoCabecalho));
                            tableCP.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Controlo de sintomas ", textoColuna));
                            _ = referenciacao.CP.ControloDeSintomas ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));   
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCP.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Processo de luto ", textoColuna));
                            _ = referenciacao.CP.ProcessoDeLuto ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCP.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Outros ", textoColuna));
                            if (referenciacao.CP.Outras != null)
                            {
                                paragraphCheckBox.Add(new Paragraph(Ischecked, f));
                                paragraphCheckBox.Add(new Chunk("  " + referenciacao.CP.Outras, textoOutras));
                            }
                            else paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                            tableCP.AddCell(cell);

                            pdfDoc.Add(tableCP);

                            /**
                             * Necessidade de ensino
                             */
                            PdfPTable tableNE = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 5.9.Necessidade de ensino", textoCabecalho));
                            tableNE.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Execução de técnicas ", textoColuna));
                            _ = referenciacao.NE.ExecucaoDeTecnicas ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNE.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Regime terapêutico ", textoColuna));
                            _ = referenciacao.NE.RegimeTerapeutico ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNE.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Auto-cuidados ", textoColuna));
                            _ = referenciacao.NE.AutoCuidados ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableNE.AddCell(cell);
                            
                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Outros ", textoColuna));
                            if (referenciacao.NE.Outras != null)
                            {
                                paragraphCheckBox.Add(new Paragraph(Ischecked, f));
                                paragraphCheckBox.Add(new Chunk("  " + referenciacao.NE.Outras, textoOutras));
                            }
                            else paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                            tableNE.AddCell(cell);

                            pdfDoc.Add(tableNE);

                            /**
                             * AVALIAÇÃO DO NÍVEL DE DEPENDÊNCIA
                             */
                            pdfDoc.NewPage();
                            pdfDoc.Add(table);  //cabeçalho

                            PdfPTable tableAND = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 6.AVALIAÇÃO DO NÍVEL DE DEPENDÊNCIA", textoCabecalho));
                            tableAND.AddCell(cabecalho);

                            cell = new PdfPCell(new Phrase("      Score da Escala de Barthel  " + referenciacao.AND, FontFactory.GetFont("Arial", 9, Font.ITALIC, BaseColor.BLACK)));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableAND.AddCell(cell);
                            cell = new PdfPCell(new Phrase("\n"));
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                            tableAND.AddCell(cell);

                            pdfDoc.Add(tableAND);

                            /**
                             * Necessidade de ensino
                             */
                            PdfPTable tableECE = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 7.ESTADO DE CONSCIÊNCIA e de EXPRESSÃO", textoCabecalho));
                            tableECE.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Confusão ", textoColuna));
                            _ = referenciacao.ECE.Confusao ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableECE.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Desorientação ", textoColuna));
                            _ = referenciacao.ECE.Desorientacao ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableECE.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Demência ", textoColuna));
                            _ = referenciacao.ECE.Demencia ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableECE.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Afasia ", textoColuna));
                            _ = referenciacao.ECE.Afasia ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableECE.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Disartria ", textoColuna));
                            _ = referenciacao.ECE.Disartria ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableECE.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Inconsciente ", textoColuna));
                            _ = referenciacao.ECE.Inconsciente ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableECE.AddCell(cell);

                            pdfDoc.Add(tableECE);

                            /**
                             * Cuidador
                             */
                            PdfPTable tableCuidadorDetalhes = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 8.CUIDADOR", textoCabecalho));
                            tableCuidadorDetalhes.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Sobrecarga física/emocional do cuidador ", textoColuna));
                            _ = referenciacao.CuidadorDetalhes.Sobrecarga ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidadorDetalhes.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Doença do cuidador principal ", textoColuna));
                            _ = referenciacao.CuidadorDetalhes.DCP ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidadorDetalhes.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Ausência de suporte familiar ", textoColuna));
                            _ = referenciacao.CuidadorDetalhes.ASF ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidadorDetalhes.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Cuidador com idade avançada ", textoColuna));
                            _ = referenciacao.CuidadorDetalhes.CuidadorIdadeAvancada ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableCuidadorDetalhes.AddCell(cell);

                            pdfDoc.Add(tableCuidadorDetalhes);

                            /**
                             * INDICADORES DE RISCO SOCIAL
                             */
                            PdfPTable tableIRS = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase(" 9.INDICADORES DE RISCO SOCIAL", textoCabecalho));
                            tableIRS.AddCell(cabecalho);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Isolamento social/geográfico ", textoColuna));
                            _ = referenciacao.IRS.Isolamento ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Ausência de suporte familiar ", textoColuna));
                            _ = referenciacao.IRS.ASF ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         História de conflitualidade familiar/rutura familiar ", textoColuna));
                            _ = referenciacao.IRS.Conflitualidade ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Antecedentes pessoais/familiares de violência ", textoColuna));
                            _ = referenciacao.IRS.AntecedentesViolencia ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Suspeita de maus tratos ", textoColuna));
                            _ = referenciacao.IRS.MausTratos ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Negligência na prestação de cuidados ", textoColuna));
                            _ = referenciacao.IRS.Negligencia ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Dependência do idoso na sua gestão económica/financeira e de bens ", textoColuna));
                            _ = referenciacao.IRS.DependenciaEconomica ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Baixos rendimentos ", textoColuna));
                            _ = referenciacao.IRS.BaixosRendimentos ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Habitação degradada ", textoColuna));
                            _ = referenciacao.IRS.HabitacaoDegradada ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Condições de salubridade precárias ", textoColuna));
                            _ = referenciacao.IRS.SalubridadePrecaria ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Coabitação/sobrelotação habitacional ", textoColuna));
                            _ = referenciacao.IRS.CoabitacaoHabitacional ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Barreiras arquitectónicas ", textoColuna));
                            _ = referenciacao.IRS.BarreirasArquitetonicas ? paragraphCheckBox.Add(new Paragraph(Ischecked, f)) : paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            tableIRS.AddCell(cell);

                            paragraphCheckBox = new Paragraph();
                            paragraphCheckBox.Add(new Chunk("         Outros ", textoColuna));
                            if (referenciacao.IRS.Outras != null)
                            {
                                paragraphCheckBox.Add(new Paragraph(Ischecked, f));
                                paragraphCheckBox.Add(new Chunk("  " + referenciacao.IRS.Outras, textoOutras));
                            }
                            else paragraphCheckBox.Add(new Paragraph(NotChecked, f));
                            cell = new PdfPCell(paragraphCheckBox);
                            cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                            tableIRS.AddCell(cell);

                            pdfDoc.Add(tableIRS);


                            /**
                             * Rodápé
                             */
                            PdfPTable tableRod = new PdfPTable(1);
                            cabecalho = new PdfPCell(new Phrase("    DATA:  " + referenciacao.Criacao + "                   ASSINATURA:", textoCabecalho));
                            tableRod.AddCell(cabecalho);

                            pdfDoc.Add(tableRod);


                        }
                        finally
                        {
                            pdfWriter.CloseStream = false;
                            pdfDoc.Close();
                        }
                    }
                }
                bytes = ms.ToArray();
            }
            return bytes;
        }

    


        private Byte[] GeraPDF(int id)
        {
            Referenciacao referenciacao = db.Dados.Find(id);
            Utente utente = db.uts.Where(x => x.Id == referenciacao.UtenteId).FirstOrDefault();


            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var pdfDoc = new Document(PageSize.A4, 25, 25, 25, 50))
                {
                    using (var pdfWriter = PdfWriter.GetInstance(pdfDoc, ms))
                    {
                        pdfDoc.Open();                     

                        try
                        {
                            pdfDoc.Open();

                            Font textoCabecalho = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
                            Font textoColuna = FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK);
                            Font textoColunaBold = FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK);
                            Font textoSeccao = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);

                            String FONT_CHECKBOX = "c:/windows/fonts/WINGDING.TTF";
                            string Ischecked = "\u00fe";
                            string NotChecked = "o";
                            
                            BaseFont bf = BaseFont.CreateFont(FONT_CHECKBOX, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                            Font f = new Font(bf, 8);
                            string imagepath = Server.MapPath("~/Images");
                            Image jpg = Image.GetInstance(imagepath + "/logo-hseit.png");

                            jpg.ScalePercent(20f);
                            pdfDoc.Add(jpg);

                            Chunk chunk = new Chunk("Estrutura de Missão - Rede Regional de Cuidados Continuados Integrados", FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.DARK_GRAY));
                            Paragraph titulo = new Paragraph(chunk);
                            titulo.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(titulo);

                            chunk = new Chunk("Hospital de Santo Espírito da Ilha Terceira", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
                            titulo = new Paragraph(chunk);
                            titulo.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(titulo);


                            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                            pdfDoc.Add(line);

                            /**
                             * Dados utente 
                             */
                            //Titulos
                            PdfPCell cell = new PdfPCell();

                            PdfPTable tableUtente = new PdfPTable(new float[] { 50f, 100f, 50f, 100f });
                            tableUtente.WidthPercentage = 100;
                            tableUtente.HorizontalAlignment = 0; //0=Left, 1=Center, 2=Right

                            //cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 4;
                            cell.MinimumHeight = 5;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell.AddElement(new Paragraph("Utente", textoCabecalho));
                            tableUtente.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Nome:", textoColunaBold));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(utente.Name, textoColuna));
                            tableUtente.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Morada: ", textoColunaBold));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(utente.Morada, textoColuna));
                            tableUtente.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Freguesia:", textoColunaBold));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(utente.Freguesia, textoColuna));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Telefone:", textoColunaBold));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(utente.NumeroTelefone, textoColuna));
                            tableUtente.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Data Nascimento:", textoColunaBold));
                            tableUtente.AddCell(cell);

                            string dataNascimento = "";
                            if (utente.Nascimento != null)
                                dataNascimento = utente.Nascimento.ToShortDateString();
                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(dataNascimento, textoColuna));
                            tableUtente.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Estado Civil:", textoColunaBold));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(utente.EstadoCivil, textoColuna));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Escolaridade:", textoColunaBold));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(utente.Escolaridade, textoColuna));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Nº de beneficiário:", textoColunaBold));
                            tableUtente.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(utente.Beneficiario, textoColuna));
                            tableUtente.AddCell(cell);

                            pdfDoc.Add(tableUtente);

                            /**
                            * Dados cuidador 
                            */

                            pdfDoc.Add(line);
                            PdfPTable tableCuidador = new PdfPTable(new float[] { 50f, 100f, 50f, 100f });
                            tableCuidador.WidthPercentage = 100;
                            tableCuidador.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 4;
                            cell.MinimumHeight = 5;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell.AddElement(new Paragraph("Cuidador", textoCabecalho));
                            tableCuidador.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Nome:", textoColunaBold));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.Cuidador.Name, textoColuna));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Morada:", textoColunaBold));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.Cuidador.Morada, textoColuna));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Telefone:", textoColunaBold));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.Cuidador.NumeroTelefone, textoColuna));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Data Nascimento:", textoColunaBold));
                            tableCuidador.AddCell(cell);

                            string dataNascimentoCuidador = "";
                            if (referenciacao.Cuidador.Nascimento != null)
                                dataNascimentoCuidador = referenciacao.Cuidador.Nascimento.ToShortDateString();
                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(dataNascimentoCuidador, textoColuna));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Estado Civil:", textoColunaBold));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.Cuidador.EstadoCivil, textoColuna));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Grau de Parentesco:", textoColunaBold));
                            tableCuidador.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.Cuidador.GrauDeParentesco, textoColuna));
                            tableCuidador.AddCell(cell);

                            pdfDoc.Add(tableCuidador);

                            /**
                            * Entidade Referenciadora até Problemas Sensoriais
                            */

                            pdfDoc.Add(line);

                            PdfPTable tableUtenteDetalhes = new PdfPTable(new float[] { 50f, 100f, 50f, 100f });
                            tableUtenteDetalhes.WidthPercentage = 100;
                            tableUtenteDetalhes.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Entidade Referenciadora:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.EntidadeReferenciadora, textoColuna));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Diagnóstico Clínico:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.DiagnosticoClinico, textoColuna));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Previsão de Alta:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            string dataAlta = "";
                            if (referenciacao.DataDeAlta != null)
                                dataAlta = referenciacao.DataDeAlta.ToShortDateString();
                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(dataAlta, textoColuna));
                            tableUtenteDetalhes.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Critérios de Triagem Para Cuidados Continuados:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph("", textoColuna));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Avaliação do Nível de Dependência (Escala de Barthel):", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.AND));
                            tableUtenteDetalhes.AddCell(cell);


                            Paragraph pCheckbox;

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Dependências AVD:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if(referenciacao.DependenciaAVD)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else                 
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableUtenteDetalhes.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Desnutrição:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.Desnutricao)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableUtenteDetalhes.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Deterioração Cognitiva:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.Deteorioracao)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableUtenteDetalhes.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Problemas Sensoriais:", textoColunaBold));
                            tableUtenteDetalhes.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.ProblemasSensoriais)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableUtenteDetalhes.AddCell(cell);

                            pdfDoc.Add(tableUtenteDetalhes);

                            /**
                            * Doenças Crónicas com Episódios de Reagudização
                            */

                            pdfDoc.Add(line);
                            PdfPTable tableDoencasCronicas = new PdfPTable(new float[] { 50f, 100f, 50f, 100f });
                            tableDoencasCronicas.WidthPercentage = 100;
                            tableDoencasCronicas.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 4;
                            cell.MinimumHeight = 5;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell.AddElement(new Paragraph("Doenças Crónicas com Episódios de Reagudização:", textoCabecalho));
                            tableDoencasCronicas.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("DPOC:", textoColunaBold));
                            tableDoencasCronicas.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.DCronicas.DPOC)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableDoencasCronicas.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("ICC:", textoColunaBold));
                            tableDoencasCronicas.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.DCronicas.ICC)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableDoencasCronicas.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Doença Cérebro Vascular:", textoColunaBold));
                            tableDoencasCronicas.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.DCronicas.DCV)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableDoencasCronicas.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Outras:", textoColunaBold));
                            tableDoencasCronicas.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.DCronicas.Outras, textoColuna));
                            tableDoencasCronicas.AddCell(cell);

                            pdfDoc.Add(tableDoencasCronicas);

                            /**
                            * Necessidade de Continuidade de Cuidados no Domicílio
                            */
                            pdfDoc.NewPage();
                            pdfDoc.Add(line);
                            PdfPTable tableNCCD = new PdfPTable(new float[] { 50f, 100f, 50f, 100f });
                            tableNCCD.WidthPercentage = 100;
                            tableNCCD.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 4;
                            cell.MinimumHeight = 5;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell.AddElement(new Paragraph("Necessidade de Continuidade de Cuidados no Domicílio:", textoCabecalho));
                            tableNCCD.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Tratamento de Feridas/Úlceras por pressão:", textoColunaBold));
                            tableNCCD.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NCCD.TratamentoDeFeridas)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNCCD.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Reabilitação:", textoColunaBold));
                            tableNCCD.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NCCD.Reabilitacao)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNCCD.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Manutenção de Dispositivos:", textoColunaBold));
                            tableNCCD.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NCCD.ManutencaoDeDispositivos)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNCCD.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Gestão do Regime Terapêutico:", textoColunaBold));
                            tableNCCD.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NCCD.GestaoDeRegimeTerapeutico)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNCCD.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Outras:", textoColunaBold));
                            tableNCCD.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.NCCD.Outras, textoColuna));
                            tableNCCD.AddCell(cell);

                            pdfDoc.Add(tableNCCD);

                            /**
                            * Necessidade de Tratamentos Complexos
                            */

                            pdfDoc.Add(line);
                            PdfPTable tableNTC = new PdfPTable(new float[] { 50f, 100f, 50f, 100f });
                            tableNTC.WidthPercentage = 100;
                            tableNTC.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 4;
                            cell.MinimumHeight = 5;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell.AddElement(new Paragraph("Necessidade de Tratamentos Complexos:", textoCabecalho));
                            tableNTC.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Úlceras por pressão múltiplas:", textoColunaBold));
                            tableNTC.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NTC.UlcerasPorPressaoMultiplas)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNTC.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Úlceras por pressão múltiplas:", textoColunaBold));
                            tableNTC.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NTC.UlcerasPorPressaoMultiplas)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNTC.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Portadores de SNG/PEG:", textoColunaBold));
                            tableNTC.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NTC.PortadoresDeSNG_PEG)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNTC.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Ventilação Assistida:", textoColunaBold));
                            tableNTC.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.NTC.VentilacaoAssistida)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableNTC.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Outras:", textoColunaBold));
                            tableNTC.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.NTC.Outras, textoColuna));
                            tableNTC.AddCell(cell);

                            pdfDoc.Add(tableNTC);

                            /**
                            * Cuidados Paliativos
                            */

                            pdfDoc.Add(line);
                            PdfPTable tableCP = new PdfPTable(new float[] { 50f, 100f, 50f, 100f });
                            tableCP.WidthPercentage = 100;
                            tableCP.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 4;
                            cell.MinimumHeight = 5;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell.AddElement(new Paragraph("Cuidados Paliativos:", textoCabecalho));
                            tableCP.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Controlo de Sintomas:", textoColunaBold));
                            tableCP.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.CP.ControloDeSintomas)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableCP.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Processo de Luto:", textoColunaBold));
                            tableCP.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            if (referenciacao.CP.ProcessoDeLuto)
                                pCheckbox = new Paragraph(Ischecked, f);
                            else
                                pCheckbox = new Paragraph(NotChecked, f);
                            cell.AddElement(pCheckbox);
                            tableCP.AddCell(cell);


                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.AddElement(new Paragraph("Outros:", textoColunaBold));
                            tableCP.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.Colspan = 3;
                            cell.AddElement(new Paragraph(referenciacao.CP.Outras, textoColuna));
                            tableCP.AddCell(cell);



                            pdfDoc.Add(tableCP);










                            pdfDoc.Close();
                        }
                        finally
                        {
                            pdfWriter.CloseStream = false;
                            pdfDoc.Close();
                        }
                    }
                }
                bytes = ms.ToArray();
            }
            return bytes;
        }
    }
}
