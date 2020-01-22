using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using SautinSoft;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Moises.Toolkit.Extensions.String;

namespace OCR_Demo
{
    class Program 
    {
        static void Main(string[] args)
        {
            //converte pdf em imagem 
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            f.OpenPdf(@"D:\Projetos\OCR_Demo\Resultado\PDF.pdf");

            if (f.PageCount > 0)
            {
                f.ImageOptions.Dpi = 300;
                f.ToMultipageTiff(@"D:\Projetos\OCR_Demo\Resultado\imagem\PDF.tiff");
            }

            ConvertTiffToJpeg(@"D:\Projetos\OCR_Demo\Resultado\imagem\PDF.tiff");

            var files = Directory.GetFiles(@"D:\Projetos\OCR_Demo\Resultado\imagem\").Where(x => x.EndsWith("jpg"));

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var file in files)
            {
                try
                {
                    using (var engine = new TesseractEngine(@"tessdata", "por", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile(file))
                        {
                            using (var page = engine.Process(img))
                            {
                                var texto = page.GetText();
                                stringBuilder.Append(texto);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro {0}", ex.Message);
                }
                finally
                {

                }
            }
            string filePath = @"D:\Projetos\OCR_Demo\Resultado\textos\teste.txt";
            File.WriteAllText(filePath, stringBuilder.ToString());
        }

        public static string[] ConvertTiffToJpeg(string fileName)
        {
            using (Image imageFile = Image.FromFile(fileName))
            {
                FrameDimension frameDimensions = new FrameDimension(
                    imageFile.FrameDimensionsList[0]);

                // Gets the number of pages from the tiff image (if multipage) 
                int frameNum = imageFile.GetFrameCount(frameDimensions);
                string[] jpegPaths = new string[frameNum];

                for (int frame = 0; frame < frameNum; frame++)
                {
                    // Selects one frame at a time and save as jpeg. 
                    imageFile.SelectActiveFrame(frameDimensions, frame);
                    using (Bitmap bmp = new Bitmap(imageFile))
                    {
                        jpegPaths[frame] = String.Format("{0}\\{1}{2}.jpg",
                            Path.GetDirectoryName(fileName),
                            Path.GetFileNameWithoutExtension(fileName),
                            frame);
                        bmp.Save(jpegPaths[frame], System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }

                return jpegPaths;
            }
        }
        public static void Filtrar(string[] args)
        {
            Filtro filtro = new Filtro();
            List<string> novasLinhas = new List<string>();
            string[] todasAsLinhas = File.ReadAllLines(@"D:\Projetos\OCR_Demo\Resultado\textos\teste.txt");
            foreach (string linha in todasAsLinhas)
            {
                if (linha.Contains("Nome"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.nome = linha.ScrapeBetween("] NomeZ", " |")?.FirstOrDefault();
                }
                if (linha.Contains("Data de Nascimento"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.dataNasc = linha.ScrapeBetween("Data de Nascimento: ", " ")?.FirstOrDefault();
                }
                if (linha.Contains("Nacionalidade"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.nacionalidade = linha.ScrapeBetween("Nacionalidade: ", " ! Data de Nascimento: 23/09/1982")?.FirstOrDefault();
                }
                if (linha.Contains("Profissã"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.profissao = linha.ScrapeBetween("ProfissãOI", "")?.FirstOrDefault();
                }
                if (linha.Contains("CPF"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.CPF = linha.ScrapeBetween("] NomeZSILVIA NIIMOTO | ", "")?.FirstOrDefault();
                }
                if (linha.Contains("Endereço"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.endereco = linha.ScrapeBetween("Endereço: ", " | CEP: 03. 262—070")?.FirstOrDefault();
                }
                if (linha.Contains("CEP"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.CEP = linha.ScrapeBetween("CEP: ", "")?.FirstOrDefault();
                }
                if (linha.Contains("Estado Civil"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.estadoCivil = linha.ScrapeBetween("Estado Civil: ", "Nacionalidade: ")?.FirstOrDefault();
                }
                if (linha.Contains("Renda Bruta Declarada"))
                {
                    string Something = string.Join(",", novasLinhas);
                   filtro.renda = linha.ScrapeBetween("Renda Bruta Declarada: ", "-")?.FirstOrDefault();
                }
                if (linha.Contains("email"))
                {
                    string Something = string.Join(",", novasLinhas);
                    filtro.email = linha.ScrapeBetween("E-mail: ", " ' |")?.FirstOrDefault();
                }
            }
        }
    }
}