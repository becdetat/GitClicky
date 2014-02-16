using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClicky.Core;
using GitClicky.Core.Extensions;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace GitClicky.Extension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class GitClickyExtension : SharpContextMenu
    {
        private readonly IGitRepositoryService _gitRepositoryService;

        public GitClickyExtension()
        {
            _gitRepositoryService = new GitRepositoryService();
        }

        protected override bool CanShowMenu()
        {
            return
                SelectedItemPaths.Any()
                && SelectedItemPaths.All(x => _gitRepositoryService.IsPathInRepository(x));
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            if (!SelectedItemPaths.Any())
            {
                return menu;
            }

            try
            {
                string providerName;
                try
                {
                    var repositoryPath = _gitRepositoryService.GetRepositoryPath(SelectedItemPaths.First());
                    var remote = _gitRepositoryService.GetRemote(repositoryPath);
                    var provider = _gitRepositoryService.GetProviderForRemote(remote);
                    providerName = provider.ToString();
                }
                catch (Exception ex)
                {
                    return menu;
                }

                var getRawGitUrlMenuItem = new ToolStripMenuItem
                {
                    Text = "Copy {0} repository raw URL".FormatWith(providerName)
                };

                getRawGitUrlMenuItem.Click += (sender, args) => ExecuteGitClicky();

                menu.Items.Add(getRawGitUrlMenuItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return menu;
        }

        private void ExecuteGitClicky()
        {
            string error = null;

            try
            {
                var urls = SelectedItemPaths
                    .Select(x => _gitRepositoryService.GetFetchRemoteForPath(x))
                    .Join(Environment.NewLine);
                Clipboard.SetText(urls);
                MessageBox.Show(urls);
            }
            catch (NotInGitRepositoryException)
            {
                error = "The specified path does not exist within a Git repository";
            }
            catch (RepositoryHasNoRemotesException)
            {
                error = "The repository doesn't have any remotes configured";
            }
            catch (UnknownRemoteProviderException e)
            {
                error = "The repository's remote provider is unknown: {0}".FormatWith(e.Message);
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "GitClicky", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
