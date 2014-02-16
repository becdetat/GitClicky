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
        private readonly string[] _preferredRemoteNames =
        {
            "origin", 
            "upstream"
        };

        public string GetFetchRemoteForPath(string path)
        {
            var repositoryPath = GetRepositoryPath(path);

            var remote = GetRemote(repositoryPath, _preferredRemoteNames);
            var provider = GetProviderForRemote(remote);

            switch (provider)
            {
                case GitRemoteProvider.BitBucket:
                    return GetRawPathForBitBucket(path, repositoryPath, remote);
                case GitRemoteProvider.Github:
                    return GetRawPathForGithub(path, repositoryPath, remote);
            }

            throw new ProviderNotImplementedException(provider);
        }

        public bool IsPathInRepository(string path)
        {
            try
            {
                GetRepositoryPath(path);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool HasProviderForRemote(string remote)
        {
            try
            {
                GetProviderForRemote(remote);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public GitRemoteProvider GetProviderForRemote(string remote)
        {
            var providerMap = new Dictionary<GitRemoteProvider, string>
            {
                { GitRemoteProvider.Github, "github.com:" },
                { GitRemoteProvider.BitBucket, "bitbucket.org:" }
            };

            var matchingProviders = providerMap
                .Where(x => remote.Contains(x.Value))
                .Select(x => x.Key)
                .ToList();

            if (matchingProviders.Any())
            {
                return matchingProviders.FirstOrDefault();
            }

            throw new UnknownRemoteProviderException(remote);
        }

        public string GetRepositoryPath(string path)
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

        public string GetRemote(string repositoryPath, params string[] preferredRemoteNames)
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
    }
}
