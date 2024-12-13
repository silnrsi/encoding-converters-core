﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SilEncConverters40.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SilEncConverters40.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Click to browse for the converter table/map
        ///    .
        /// </summary>
        internal static string BrowseFileSpecHelpString {
            get {
                return ResourceManager.GetString("BrowseFileSpecHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Click to close this dialog
        ///  .
        /// </summary>
        internal static string CloseButtonHelpString {
            get {
                return ResourceManager.GetString("CloseButtonHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Legacy-encoded data is treated internally to many newer programs as a string of wide characters converted by a certain
        ///code page. For example, SIL IPA93 font is treated internally as a wide symbol font encoding (aka. code page 42).
        ///The Annapurna Devanagari font is treated internally as wide characters of the &quot;latin&quot; code page 1252.
        ///
        ///EncConverters needs to use the same code page as the calling program when it wants to turn that string of wide 
        ///characters back into a string of narrow bytes for the conv [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CodePageHelpString {
            get {
                return ResourceManager.GetString("CodePageHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Enter the file path to the converter table/map.
        ///    .
        /// </summary>
        internal static string ConverterFileSpecHelpString {
            get {
                return ResourceManager.GetString("ConverterFileSpecHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Choose the encoding type for the input to the table/map: either Unicode or Legacy bytes.
        ///For example, if this is an Unicode encoding converter, then the input will be Legacy bytes.
        ///    .
        /// </summary>
        internal static string ConvTypeExpectsHelpString {
            get {
                return ResourceManager.GetString("ConvTypeExpectsHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Choose the encoding type for the output of the table/map: either Unicode or Legacy bytes
        ///For example, if this is an Unicode encoding converter, then the output will be Unicode encoding.
        ///    .
        /// </summary>
        internal static string ConvTypeReturnsHelpString {
            get {
                return ResourceManager.GetString("ConvTypeReturnsHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference.
        /// </summary>
        internal static string NetRegexQuickReferenceLink {
            get {
                return ResourceManager.GetString("NetRegexQuickReferenceLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///This box displays the result of the conversion using the selected converter given some data
        ///from the client application. You can select different converters in the list above and see the
        ///results. You can also adjust the Conversion Options (e.g. Direction, etc) and see what effect
        ///those have on the given data. You can also right-click in this box to change the font so that 
        ///the data is displayed using a particular font. 
        ///    .
        /// </summary>
        internal static string PreviewBoxHelpString {
            get {
                return ResourceManager.GetString("PreviewBoxHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Click to add this converter to the system repository permanently.
        ///    .
        /// </summary>
        internal static string SaveInRepositoryHelpString {
            get {
                return ResourceManager.GetString("SaveInRepositoryHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///This box shows the decimal or hex values of the characters in the text box above.
        ///    .
        /// </summary>
        internal static string TestHexDecOutputBoxesHelpString {
            get {
                return ResourceManager.GetString("TestHexDecOutputBoxesHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///This box is for entering data which corresponds to the left-hand side of the conversion 
        ///(usually, the non-Unicode/Legacy data). 
        ///
        ///You can right-click in this box and change the font so that it displays using a particular font. 
        ///
        ///You can also type Alt+X to convert the preceding character to its 4 (hex) digit Unicode value.
        ///
        ///Click the &apos;Test&apos; button to execute the conversion and see the result in the Output box below
        ///  .
        /// </summary>
        internal static string TestInputBoxHelpString {
            get {
                return ResourceManager.GetString("TestInputBoxHelpString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///This box shows the result of converting the Input data using the currently configured converter. 
        ///
        ///You can right-click in this box and change the font so that it displays using a particular font. 
        ///
        ///You can also type Alt+X to convert the preceding character to its 4 (hex) digit Unicode value.
        ///    .
        /// </summary>
        internal static string TestOutputBoxHelpString {
            get {
                return ResourceManager.GetString("TestOutputBoxHelpString", resourceCulture);
            }
        }
    }
}
