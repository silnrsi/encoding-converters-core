//
// File History:
//  Created by Jim Kornelsen on July 9 2013.
//
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// A bundle of maps and xml settings should be put into the Common App
    /// Data folder during installation.
    ///
    /// The ConvertersActivator allows the user to add any of these at runtime
    /// to their mapping repository by checking a box.
    ///
    /// These converters can then be removed just like any other converter, by
    /// right-clicking and selecting Delete in the SelectConverter window.
    ///
    /// This replaces the functionality of the "Converter Options Installer"
    /// VisualBasic application and the tree list of converters that was shown
    /// during installation in previous versions.
    /// </summary>
    public class ConvertersActivator : System.Windows.Forms.Form
    {

    }
}

