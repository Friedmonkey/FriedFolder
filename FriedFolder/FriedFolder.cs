using SharpShell.SharpNamespaceExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FriedFolder
{
    [ComVisible(true)]
    [NamespaceExtensionJunctionPoint(NamespaceExtensionAvailability.Everyone, VirtualFolder.MyComputer, "FriedFolder")]
    public class FriedFolder : SharpNamespaceExtension
    {
        public override NamespaceExtensionRegistrationSettings GetRegistrationSettings()
        {
            return new NamespaceExtensionRegistrationSettings
            {
                ExtensionAttributes = AttributeFlags.IsFolder | AttributeFlags.MayContainSubFolders | AttributeFlags.IsBrowsable | AttributeFlags.IsStorage
            };
        }

        protected override IEnumerable<IShellNamespaceItem> GetChildren(ShellNamespaceEnumerationFlags flags)
        {
            return null;
        }

        protected override ShellNamespaceFolderView GetView()
        {
            return new CustomNamespaceFolderView(new FolderFrame());
        }
    }
}
