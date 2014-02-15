using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GitClicky.Core.Extensions;
using LibGit2Sharp;

namespace GitClicky.Core
{
    public class GitRepositoryService : IGitRepositoryService
    {
        public string GetFetchRemoteForPath(string path)
        {
            var repositoryPath = GetRepositoryPath(path);

            var remote = GetRemote(repositoryPath, "origin", "upstream");

            if (remote.Contains("github.com:"))
            {
                return GetRawPathForGithub(path, repositoryPath, remote);
            }

            if (remote.Contains("bitbucket.org:"))
            {
                return GetRawPathForBitBucket(path, repositoryPath, remote);
            }

            throw new UnknownRemoteProviderException(remote);
        }

        private static string GetRawPathForGithub(string path, string repositoryPath, string remote)
        {
            return "https://github.com/{remotePath}/raw/master/{filePath}".FormatWith(new
            {
                remotePath = GetRemotePath(remote, "git@github.com"),
                filePath = GetFilePath(path,repositoryPath)
            });
        }

        private static string GetRawPathForBitBucket(string path, string repositoryPath, string remote)
        {
            // git@bitbucket.org:centium-ondemand/nextgen.git 
            //https://bitbucket.org/centium-ondemand/nextgen/raw/master/NextGen.Client/App.config

            return "https://bitbucket.org/{remotePath}/raw/master/{filePath}".FormatWith(new
            {
                remotePath = GetRemotePath(remote, "git@bitbucket.org"),
                filePath = GetFilePath(path, repositoryPath)
            });

        }

        private static string GetRemotePath(string remote, string providerPrefix)
        {
            return remote
                .Replace("{0}:".FormatWith(providerPrefix), "")
                .Replace(".git", "");
        }

        private static string GetFilePath(string path, string repositoryPath)
        {
            return path
                .Replace(repositoryPath + "\\", "")
                .Replace("\\", "/");
        }

        private static string GetRemote(string repositoryPath, params string[] preferredRemoteNames)
        {
            using (var repository = new Repository(repositoryPath))
            {
                var remote = GetPreferredRemotes(repository, preferredRemoteNames).FirstOrDefault();
                if (remote == null)
                {
                    throw new RepositoryHasNoRemotesException();
                }

                return remote.Url;
            }
        }

        private static IEnumerable<Remote> GetPreferredRemotes(IRepository repository, string[] preferredRemoteNames)
        {
            foreach (var name in preferredRemoteNames)
            {
                var remote = repository.Network.Remotes
                .FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (remote != null)
                {
                    yield return remote;
                }
            }

            foreach (var remote in repository.Network.Remotes)
            {
                yield return remote;
            }
        }

        private static string GetRepositoryPath(string path)
        {
            while (!Directory.Exists(Path.Combine(path, ".git")))
            {
                path = Path.GetDirectoryName(path);
                if (path == Path.GetPathRoot(path))
                {
                    throw new NotInGitRepositoryException();
                }
            }
            return path;
        }
    }
}
