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
using NugetWorkflow.Common.FilePatcher.Interfaces;
using NugetWorkflow.Plugins.FilePatcher;
using NugetWorkflow.Common.FilePatcher.DTOs.Requests;
using NugetWorkflow.Common.Base.DTOs.GitRepos;
using NugetWorkflow.Plugins.GitAdapter.DTOs;
using NugetWorkflow.Common.GitAdapter.Exceptions;
using NugetWorkflow.Common.GitAdapter.DTOs.Responses;

namespace NugetWorkflow.Plugins.GitAdapter
{
    public class GitAdapterCore : IGitAdapter
    {
        private Signature appSignature
        {
            get
            {
                return new Signature("Nuget workflow manager", "peter.kottas@petfre.gi", DateTime.Now);
            }
        }
        IFilePatcher filePatcher;

        public GitAdapterCore()
        {
            filePatcher = new FilePatcherCore();
        }

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
                if (string.IsNullOrEmpty(repo.Path))
                {
                    request.ProgressAction(true, string.Format("Path is undefined."));
                }
                else if (string.IsNullOrEmpty(repo.URL))
                {
                    request.ProgressAction(true, string.Format("Path is undefined."));
                }
                else
                {
                    Uri uriResult;
                    bool result = Uri.TryCreate(repo.URL, UriKind.Absolute, out uriResult);
                    if (!result)
                    {
                        request.ProgressAction(true, string.Format("URL : [{0}] is invalid", repo.URL));
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
                            request.ProgressAction(true, string.Format("Exception while cloning from Url : [{0}], Exception : [{1}]", repo.URL, e.Message));
                        }
                    }
                }
            }
            request.FinishedAction("Finished cloning git repos");
        }

        private bool HasUncommittedChanges(Repository repo, string Path)
        {

            RepositoryStatus status = repo.RetrieveStatus();
            return status.IsDirty;

        }

        private StashDTO StashAll(Repository repo, string Path)
        {
            var ret = new StashDTO()
            {
                OriginalStashCount = repo.Stashes.Count()
            };
            repo.Stashes.Add(appSignature);
            return ret;
        }

        private void PopAll(Repository repo, StashDTO stashResponse)
        {
            int index = stashResponse.OriginalStashCount;
            foreach (var stash in repo.Stashes)
            {
                repo.Stashes.Pop(index);
                index++;
            }
        }

        private SwitchBranchResponseDTO SwitchBranch(Repository repo, GitRepoDTO gitRepo, bool needToStash)
        {
            var needStash = HasUncommittedChanges(repo, gitRepo.Path);
            var resp = new SwitchBranchResponseDTO()
            {
                Stashed = needStash && needToStash,
            };
            if (needStash && needToStash)
            {
                resp.StashResponse = StashAll(repo, gitRepo.Path);
            }

            resp.InitialBranchName = repo.Head.FriendlyName;
            if (resp.InitialBranchName != gitRepo.UpdateBranch)
            {
                var branch = repo.Branches.Where(a => a.FriendlyName == gitRepo.UpdateBranch).FirstOrDefault();
                if (branch != null)
                {
                    try
                    {
                        repo.Checkout(branch);
                    }
                    catch (Exception e)
                    {
                        throw new GitLibException(
                            string.Format("Exception thrown while switching to branch : [{0}], Exception : [{1}]",
                            gitRepo.UpdateBranch, e.Message));
                    }
                }
                else
                {
                    if (needStash && needToStash)
                    {
                        PopAll(repo, resp.StashResponse);
                    }
                    throw new GitLibException(string.Format("Unable to switch to branch : [{0}]", gitRepo.UpdateBranch));
                }
                resp.Switched = true;
            }
            else
            {
                resp.Switched = false;
            }

            return resp;
        }

        private void Pull(Repository repo, GitRepoDTO gitRepo)
        {

            try
            {
                repo.Network.Pull(
                    appSignature,
                    new PullOptions()
                    {
                        FetchOptions = new FetchOptions()
                        {
                            CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                            {
                                Username = gitRepo.Username,
                                Password = gitRepo.Password.ToUnsecuredString()
                            }
                        }
                    });
            }
            catch (Exception e)
            {
                throw new GitLibException(string.Format("Exception while trying to pull latest from remote : [{0}]", e.Message));
            }
        }

        private void Push(Repository repo, GitRepoDTO gitRepo)
        {

            try
            {
                repo.Network.Push(repo.Branches.Where(a => a.FriendlyName == gitRepo.UpdateBranch).FirstOrDefault(), new PushOptions()
                    {
                        CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = gitRepo.Username,
                            Password = gitRepo.Password.ToUnsecuredString()
                        }
                    });
            }
            catch (Exception e)
            {
                throw new GitLibException(string.Format("Exception while pushing latest : [{0}]", e.Message));
            }

        }

        private void Commit(Repository repo, GitRepoDTO gitRepo, string message)
        {

            try
            {
                var status = repo.RetrieveStatus();
                if (status.IsDirty)
                {
                    var files = status.Where(a => a.State != FileStatus.Untracked).ToList();
                    repo.Stage(files.Select(a => a.FilePath));
                    repo.Commit(message, appSignature, appSignature, new CommitOptions());
                }
            }
            catch (Exception e)
            {
                throw new GitLibException(string.Format("Exception while committing : [{0}]", e.Message));
            }
        }

        public void UpdateProjectsDependencies(UpdateProjectsDependenciesRequestDTO request)
        {
            foreach (var gitRepo in request.ListOfRepos)
            {
                try
                {
                    using (var repo = new Repository(gitRepo.Path))
                    {
                        var switchResp = SwitchBranch(repo, gitRepo, true);
                        Pull(repo, gitRepo);
                        var req = new UpdateDependenciesVersionRequestDTO()
                        {
                            GitReposPath = gitRepo.Path,
                            NewVersion = request.Version,
                            NuGetID = request.NugetID
                        };
                        bool updated = false;
                        try
                        {
                            filePatcher.UpdateDependenciesVersion(req);
                            updated = true;
                        }
                        catch (Exception e)
                        {
                            request.ProgressAction(false, string.Format("Couldn't update version in package.config due to an Exception : [{0}]", e.Message));
                        }
                        if (updated)
                        {
                            Commit(repo, gitRepo, string.Format("Committed NuGetID [{0}] version [{1}] update to [{2}]", req.NuGetID, req.NewVersion, gitRepo.URL));
                            Push(repo, gitRepo);
                        }
                        if (switchResp.Switched)
                        {
                            SwitchBranch(repo, new GitRepoDTO()
                            {
                                Path = gitRepo.Path,
                                UpdateBranch = switchResp.InitialBranchName
                            }, false);
                        }
                        if (switchResp.Stashed)
                        {
                            PopAll(repo, switchResp.StashResponse);
                        }
                        request.ProgressAction(true, string.Format("Finished updating repo : [{0}]", gitRepo.URL));
                    }
                }
                catch (Exception e)
                {
                    request.ProgressAction(true, e.Message);
                }

            }
            request.FinishedAction(string.Format("Finished updating [{0}] to version [{1}]", request.NugetID, request.Version));
        }

        public RepoBranchesResponseDTO GetRepoBranches(RepoBranchesRequestDTO request)
        {
            try
            {
                using (var repo = new Repository(request.Path))
                {
                    return new RepoBranchesResponseDTO()
                    {
                        RepoBranches = repo.Branches.Where(a => !a.IsRemote).Select(a => a.FriendlyName).ToList()
                    };
                }
            }
            catch (Exception)
            {
                return new RepoBranchesResponseDTO()
                {
                    RepoBranches = new List<string>()
                };
            }
        }
    }
}
