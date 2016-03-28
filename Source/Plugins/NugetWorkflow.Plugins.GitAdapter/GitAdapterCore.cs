using LibGit2Sharp;
using NugetWorkflow.Common.GitAdapter.DTOs.Requests;
using NugetWorkflow.Common.GitAdapter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NugetWorkflow.Common.Base.Extensions;
using System.IO;
using System.Threading;
using System.Security;

namespace NugetWorkflow.Plugins.GitAdapter
{
    public class GitAdapterCore : IGitAdapter
    {
        private void Clone(string URL, string Path, string Username, SecureString Password)
        {
            Repository.Clone(URL, Path, new CloneOptions()
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = Username,
                    Password = Password.ToUnsecuredString()
                }
            });
        }
        public void CloneProjects(CloneProjectsRequestDTO request)
        {
            foreach (var repo in request.ListOfRepos)
            {
                Thread.Sleep(500);//TODO : Remove
                if (string.IsNullOrEmpty(repo.Path))
                {
                    request.ProgressAction(false, string.Format("Path is undefined."));
                }
                else if (string.IsNullOrEmpty(repo.URL))
                {
                    request.ProgressAction(false, string.Format("Path is undefined."));
                }
                else
                {
                    Uri uriResult;
                    bool result = Uri.TryCreate(repo.URL, UriKind.Absolute, out uriResult);
                    if (!result)
                    {
                        request.ProgressAction(false, string.Format("URL : [{0}] is invalid", repo.URL));
                    }
                    else
                    {
                        try
                        {
                            Clone(repo.URL, repo.Path, repo.Username, repo.Password);
                            request.ProgressAction(true, string.Format("Successfully cloned repo from URL : [{0}] to [{1}]", repo.URL, repo.Path));
                        }
                        catch (Exception e)
                        {
                            request.ProgressAction(false, string.Format("Exception while cloning from Url : [{0}], Exception : [{1}]", repo.URL, e.Message));
                        }
                    }
                }
            }
            request.FinishedAction("Finished cloning git repos");
        }

        private void Update(string URL, string Path, string Username, SecureString Password)
        {

        }

        public void UpdateProjectsDependencies(UpdateProjectsDependenciesRequestDTO request)
        {
            foreach (var repo in request.ListOfRepos)
            {
                Thread.Sleep(500);
                if (string.IsNullOrEmpty(request.BasePath))
                {
                    request.ProgressAction(false, string.Format("Base path is undefined."));
                }
                else
                {
                    try
                    {
                        Path.GetFullPath(request.BasePath);

                        Uri uriResult;
                        bool result = Uri.TryCreate(repo.URL, UriKind.Absolute, out uriResult);
                        if (!result)
                        {
                            request.ProgressAction(false, string.Format("URL : [{0}] is invalid", repo.URL));
                        }
                        else
                        {
                            string repoFolder = string.Empty;
                            if (repo.URL.GetFolderFromUrl(ref repoFolder))
                            {
                                try
                                {
                                    Repository.Clone(repo.URL, Path.Combine(request.BasePath, repoFolder), new CloneOptions()
                                    {
                                        CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                                        {
                                            Username = repo.Username,
                                            Password = repo.Password.ToUnsecuredString()
                                        }
                                    });
                                    request.ProgressAction(true, string.Format("Successfully cloned repo from URL : [{0}] to [{1}]", repo.URL, Path.Combine(request.BasePath, repoFolder)));
                                }
                                catch (Exception e)
                                {
                                    request.ProgressAction(false, string.Format("Exception while cloning from Url : [{0}], Exception : [{1}]", repo.URL, e.Message));
                                }

                            }
                            else
                            {
                                request.ProgressAction(false, string.Format("Cannot parse folder name from Url : [{0}]", repo.URL));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        request.ProgressAction(false, string.Format("Base path : [{0}] is invalid", request.BasePath));
                    }
                }
            }
        }
    }
}
