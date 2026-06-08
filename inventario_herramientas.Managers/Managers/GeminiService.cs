using inventario_herramientas.Managers.Entidades;
using Microsoft.Extensions.Configuration;
using Mysqlx.Notice;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace inventario_herramientas.Managers.Managers
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"]
                ?? throw new ArgumentNullException(
                    "La API Key de Gemini no está configurada en appsettings.json");
        }

        public async Task<string> GenerarDescripcion(string nombre)
        {
            try
            {
                var prompt = $"Sos un experto en herramientas industriales. El usuario quiere registrar una herramienta llamada '{nombre}'. Redactá una descripción técnica, profesional y breve (máximo 2 frases). Escribí la descripción directamente.";

                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
                };

                var json = JsonSerializer.Serialize(requestBody);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                    content
                );

                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseJson);

                    return doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString()?
                        .Trim() ?? string.Empty;
                }
                else
                {
                    Console.WriteLine(
                        $"ERROR DE API GEMINI: Status: {response.StatusCode} - Body: {responseJson}"
                    );

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPCIÓN EN GEMINI SERVICE: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
