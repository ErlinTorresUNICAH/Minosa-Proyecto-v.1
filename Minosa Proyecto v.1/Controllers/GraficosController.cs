﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
//librerias para reportes
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNetCore.Authorization;


namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class GraficosController : Controller { 

        private readonly IConfiguration _configuration;

        public GraficosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Index para controlar todas las funciones
        public IActionResult Index()
        {
            var equiposPorTipo = ObtenerEquiposPorTipo();
            var equiposPorArea = ObtenerEquiposPorArea();
            var equiposPorZona = ObtenerEquiposPorZona();
            var equiposPorAreaConTipo = ObtenerEquiposPorAreaConTipo();
            var data = new
            {
                EquiposPorTipo = equiposPorTipo,
                EquiposPorArea = equiposPorArea,
                EquiposPorZona = equiposPorZona,
                EquiposPorAreaConTipo = equiposPorAreaConTipo
            };

            return View(data);
        }

        // Lista con todos los equipos por tipo para su uso en grafico
        private List<dynamic> ObtenerEquiposPorTipo()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            var data = new List<dynamic>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GetEquiposPorTipo_G", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            TipoEquipo = reader["Tipo_Equipo"].ToString(),
                            Cantidad = Convert.ToInt32(reader["Cantidad"])
                        });
                    }
                }
            }

            return data;
        }

        // Lista con todos los equipos por area para su uso en grafico
        private List<dynamic> ObtenerEquiposPorArea()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            var data = new List<dynamic>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GetEquiposPorArea_G", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            Area = reader["Nombre_Area"].ToString(),
                            Cantidad = Convert.ToInt32(reader["Cantidad"])
                        });
                    }
                }
            }

            return data;
        }

        // Lista con todos los equipos por zona para su uso en grafico
        private List<dynamic> ObtenerEquiposPorZona()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            var data = new List<dynamic>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GetEquiposPorZona_G", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            Zona = reader["Nombre_Zona"].ToString(),
                            Cantidad = Convert.ToInt32(reader["Cantidad"])
                        });
                    }
                }
            }

            return data;
        }

        // Lista con todos los equipos por area con tipo para su uso en grafico
        private List<dynamic> ObtenerEquiposPorAreaConTipo()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            var data = new List<dynamic>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GetEquiposPorAreaConTipo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            Area = reader["Area"].ToString(),
                            TipoEquipo = reader["TipoEquipo"].ToString(),
                            Cantidad = Convert.ToInt32(reader["Cantidad"])
                        });
                    }
                }
            }

            return data;
        }




        //Generar reporte de equipos por tipo solamente Tabla en PDF
        public ActionResult GenerarReporte()
        {
            MemoryStream ms = new MemoryStream();
            Document doc = new Document();
            PdfWriter.GetInstance(doc, ms);
            doc.Open();
            doc.Add(new Paragraph("Reporte de Equipos por Tipo"));
            doc.Add(new Paragraph(" ")); // Espacio en blanco para separación
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GetEquiposPorTipo_G", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    // Crear una tabla para agregar al PDF
                    PdfPTable table = new PdfPTable(2); // 2 columnas
                    table.AddCell("Tipo de Equipo");
                    table.AddCell("Cantidad");

                    while (reader.Read())
                    {
                        table.AddCell(reader["Tipo_Equipo"].ToString());
                        table.AddCell(reader["Cantidad"].ToString());
                    }

                    // Agregar la tabla al documento PDF
                    doc.Add(table);
                }
            }

            doc.Close();
            byte[] byteArray = ms.ToArray();
            ms.Close();

            return File(byteArray, "application/pdf", "ReporteEquipos.pdf");
        }

        //Generar reporte de equipos por tipo solamente grafico en PDF
        public ActionResult GenerarReporteConGrafico()
        {
            MemoryStream ms = new MemoryStream();
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            doc.Add(new Paragraph("Reporte de Equipos con Gráfico"));
            doc.Add(new Paragraph(" ")); // Espacio en blanco para separación
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GetEquiposPorTipo_G", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    // Crear una tabla para agregar al PDF
                    PdfPTable table = new PdfPTable(2); // 2 columnas
                    table.AddCell("Tipo de Equipo");
                    table.AddCell("Cantidad");

                    // Preparar los datos para el gráfico
                    List<string> labels = new List<string>();
                    List<int> values = new List<int>();

                    while (reader.Read())
                    {
                        string tipoEquipo = reader["Tipo_Equipo"].ToString();
                        int cantidad = int.Parse(reader["Cantidad"].ToString());

                        table.AddCell(tipoEquipo);
                        table.AddCell(cantidad.ToString());

                        // Agregar los datos al gráfico
                        labels.Add(tipoEquipo);
                        values.Add(cantidad);
                    }

                    // Agregar la tabla al documento PDF
                    doc.Add(table);

                    // Generar el gráfico usando iTextSharp (un gráfico de barras básico)
                    PdfContentByte cb = writer.DirectContent;
                    GenerateBarChart(cb, labels, values);
                }
            }

            doc.Close();
            byte[] byteArray = ms.ToArray();
            ms.Close();

            return File(byteArray, "application/pdf", "ReporteConGrafico.pdf");
        }

        //Generar grafico de equipos por tipo en PDF
        private void GenerateBarChart(PdfContentByte cb, List<string> labels, List<int> values)
        {
            // Variables básicas de gráficos
            float chartWidth = 400f;
            float chartHeight = 200f;
            float xStart = 50f;
            float yStart = 500f;
            float barWidth = chartWidth / values.Count;
            float maxBarHeight = 150f;
            int maxValue = values.Max();

            // Dibujar barras y etiquetas
            for (int i = 0; i < values.Count; i++)
            {
                float barHeight = ((float)values[i] / maxValue) * maxBarHeight;
                cb.Rectangle(xStart + (i * barWidth), yStart, barWidth - 5, barHeight);
                cb.Fill();

                // Etiquetas del eje X (nombre del tipo de equipo)
                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(labels[i]), xStart + (i * barWidth) + (barWidth / 2), yStart - 10, 0);

                // Mostrar la cantidad encima de cada barra
                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(values[i].ToString()), xStart + (i * barWidth) + (barWidth / 2), yStart + barHeight + 5, 0);
            }
        }



    }
}
