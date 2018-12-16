using System.IO;
using FluentEmail.Core.Interfaces;

namespace PAM.Services
{
    public class EmailHelper
    {
        public string AppUrl { get; set; }
        public string AppEmail { get; set; }
        public string TemplateFolder { get; set; }

        public string GetSubjectFromTemplate<T>(string emailName, T model, ITemplateRenderer renderer)
        {
            var template = "";
            using (var reader = new StreamReader(File.OpenRead(GetSubjectTemplateFile(emailName))))
            {
                template = reader.ReadToEnd().Trim();
            }
            return renderer.Parse(template, model, false);
        }

        public string GetSubjectTemplateFile(string emailName) => $"{TemplateFolder}/{emailName}.Subject.cshtml";

        public string GetBodyTemplateFile(string templateName) => $"{TemplateFolder}/{templateName}.Body.cshtml";
    }
}
