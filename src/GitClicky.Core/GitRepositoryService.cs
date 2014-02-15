using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitClicky.Core
{
    public class GitRepositoryService : IGitRepositoryService
    {
        public string GetFetchRemoteForPath(string path)
        {
            var repositoryPath = GetRepositoryPath(path);

            var remote = GetRemote(repositoryPath, "origin", "upstream");

            return remote;
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
