﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMCToolsTest.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\..\\Output\\MCTrajectory_{0}.txt")]
        public string MCFilePath {
            get {
                return ((string)(this["MCFilePath"]));
            }
            set {
                this["MCFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\..\\Output\\CPTrajectory_{0}.txt")]
        public string CPFilePath {
            get {
                return ((string)(this["CPFilePath"]));
            }
            set {
                this["CPFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\..\\Output\\TrTrajectory.txt")]
        public string TransmitterFilePath {
            get {
                return ((string)(this["TransmitterFilePath"]));
            }
            set {
                this["TransmitterFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\..\\Output\\BaseStations.txt")]
        public string BaseStationsFilePath {
            get {
                return ((string)(this["BaseStationsFilePath"]));
            }
            set {
                this["BaseStationsFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\..\\Output\\Filter_{0}.txt")]
        public string FilterFilePath {
            get {
                return ((string)(this["FilterFilePath"]));
            }
            set {
                this["FilterFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\..\\Output\\CriterionValues_{0}.txt")]
        public string CriterionsFilePath {
            get {
                return ((string)(this["CriterionsFilePath"]));
            }
            set {
                this["CriterionsFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\..\\Output\\CriterionCalculationLog.txt")]
        public string CriterionsCountLog {
            get {
                return ((string)(this["CriterionsCountLog"]));
            }
            set {
                this["CriterionsCountLog"] = value;
            }
        }
    }
}
