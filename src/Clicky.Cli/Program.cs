using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClicky.Core;
using GitClicky.Core.Extensions;

namespace Clicky.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            IGitRepositoryService gitRepositoryService = new GitRepositoryService();

            var inputPath = args[0];

            Console.WriteLine("Input: {0}".FormatWith(inputPath));

            try
            {
                Console.WriteLine("Fetch path: {0}".FormatWith(gitRepositoryService.GetFetchRemoteForPath(inputPath)));
            }
            catch (NotInGitRepositoryException)
            {
                Console.WriteLine("The specified path does not exist within a Git repository");
            }
            catch (RepositoryHasNoRemotesException)
            {
                Console.WriteLine("The repository doesn't have any remotes configured");
            }
            catch (UnknownRemoteProviderException e)
            {
                Console.WriteLine("The repository's remote provider is unknown: {0}".FormatWith(e.Message));
            }
        }
    }
}
