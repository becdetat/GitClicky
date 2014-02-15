using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return repositoryPath;
            //var repository = new Repository(repositoryPath);
            //var remote = new Remote(handle, repository);
        }

        static string GetRepositoryPath(string path)
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
