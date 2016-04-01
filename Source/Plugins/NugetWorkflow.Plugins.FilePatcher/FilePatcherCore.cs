using NugetWorkflow.Common.FilePatcher.DTOs.Requests;
using NugetWorkflow.Common.FilePatcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NugetWorkflow.Plugins.FilePatcher
{
    public class FilePatcherCore : IFilePatcher
    {
        public void UpdateDependenciesVersion(UpdateDependenciesVersionRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.NewVersion))
            {
                throw new ArgumentException("Version not defined");
            }
            if (string.IsNullOrEmpty(request.NuGetID))
            {
                throw new ArgumentException("NuGet ID not defined");
            }
            if (request.GitReposPaths==null)
            {
                throw new ArgumentException("Git paths not defined");
            }
            foreach (var path in request.GitReposPaths)
            {
                var packagesFiles = Directory.GetFiles(path, "packages.config", SearchOption.AllDirectories);
                foreach (var packagesFile in packagesFiles)
                {
                    bool updated = false;
                    var doc = new XmlDocument();
                    doc.Load(packagesFile);
                    var packages = doc.GetElementsByTagName("package");
                    for (int i = 0; i < packages.Count;i++ )
                    {
                        if (packages[i].Attributes["id"].Value == request.NuGetID)
                        {
                            if (packages[i].Attributes["version"].Value != request.NewVersion)
                            {
                                packages[i].Attributes["version"].Value = request.NewVersion;
                                updated = true;
                            }
                        }
                    }
                    if (updated)
                    {
                        doc.Save(packagesFile);
                    }
                }
            }
        }
    }
}
