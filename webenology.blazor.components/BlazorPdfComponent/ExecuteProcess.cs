﻿using PuppeteerSharp;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace webenology.blazor.components.BlazorPdfComponent
{
    public interface IExecuteProcess
    {
        Task GeneratePdf(string html, string tempFile, PdfOptions pdfOptions);
    }
    public class ExecuteProcess : IExecuteProcess, IDisposable
    {
        private readonly ILogger<ExecuteProcess> _logger;

        public ExecuteProcess(ILogger<ExecuteProcess> logger)
        {
            _logger = logger;
        }

        public async Task GeneratePdf(string html, string tempFile, PdfOptions pdfOptions)
        {
            var browserFetcher = new BrowserFetcher(Product.Chrome);
            if (!browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).Downloaded)
            {
                var results = await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

                _logger.LogDebug("pdf folder {0} and url {1}", results.FolderPath, results.Url);
            }

            Browser browser = null;
            try
            {
                browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                });
                await using var page = await browser.NewPageAsync();
                await page.SetContentAsync(html);

                await page.PdfAsync(tempFile, pdfOptions);
            }
            finally
            {
                if (browser != null)
                    await browser.CloseAsync();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
