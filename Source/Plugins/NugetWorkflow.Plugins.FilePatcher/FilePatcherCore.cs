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
        /// <summary>
        /// Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>
        private bool WaitForFile(string fullPath)
        {
            int numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    // Attempt to open the file exclusively.
                    using (FileStream fs = new FileStream(fullPath,
                        FileMode.Open, FileAccess.ReadWrite,
                        FileShare.None, 100))
                    {
                        fs.ReadByte();

                        // If we got this far the file is ready
                        break;
                    }
                }
                catch (Exception)
                {
                    if (numTries > 100)
                    {
                        return false;
                    }

                    System.Threading.Thread.Sleep(50);
                }
            }
            return true;
        }

        public bool UpdateDependenciesVersion(UpdateDependenciesVersionRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.NewVersion))
            {
                throw new ArgumentException("Version not defined");
            }
            if (string.IsNullOrEmpty(request.NuGetID))
            {
                throw new ArgumentException("NuGet ID not defined");
            }
            if (string.IsNullOrEmpty(request.GitReposPath))
            {
                throw new ArgumentException("Git paths not defined");
            }
            var packagesFiles = Directory.GetFiles(request.GitReposPath, "packages.config", SearchOption.AllDirectories);
            bool updated = false;
            foreach (var packagesFile in packagesFiles)
            {
                var doc = new XmlDocument();
                doc.Load(packagesFile);
                var packages = doc.GetElementsByTagName("package");
                for (int i = 0; i < packages.Count; i++)
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
                WaitForFile(packagesFile);
            }
            return updated;
        }
    }
}
