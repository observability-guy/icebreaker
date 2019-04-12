﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Icebreaker.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Icebreaker.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to 😓 there was an error while processing request. Retry?.
        /// </summary>
        internal static string ErrorOccured {
            get {
                return ResourceManager.GetString("ErrorOccured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 😕 no idea what you just said. My tiny 🤖 🧠 doesn&apos;t understand how to handle that yet!.
        /// </summary>
        internal static string IDontKnow {
            get {
                return ResourceManager.GetString("IDontKnow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you&apos;re reading this, it&apos;s because I was added to {0}. I get to help you meet more people around your organization by randomly pairing you with someone new every week. You get to make more friends and learn about the people you work with. It&apos;s a win-win-*win* situation..
        /// </summary>
        internal static string InstallMessage {
            get {
                return ResourceManager.GetString("InstallMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hey there! Looks like {0} matched us this week. It&apos;d be great to meet up for a coffee or a lunch or a call if you&apos;ve got time..
        /// </summary>
        internal static string MeetupContent {
            get {
                return ResourceManager.GetString("MeetupContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} / {1} Meet up.
        /// </summary>
        internal static string MeetupTitle {
            get {
                return ResourceManager.GetString("MeetupTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Yay! Welcome back, I have resumed your pairings. You can always pause again if needed in future..
        /// </summary>
        internal static string OptInConfirmation {
            get {
                return ResourceManager.GetString("OptInConfirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to I have paused your pairings. Click Resume pairings when you&apos;re ready to start meeting people again!.
        /// </summary>
        internal static string OptOutConfirmation {
            get {
                return ResourceManager.GetString("OptOutConfirmation", resourceCulture);
            }
        }
    }
}
