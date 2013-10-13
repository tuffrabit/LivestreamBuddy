﻿#pragma checksum "..\..\..\Controls\Stream.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "476FE4BE7851604585982CDBB7AF65B9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Awesomium.Windows.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace LivestreamBuddyNew.Controls {
    
    
    /// <summary>
    /// Stream
    /// </summary>
    public partial class Stream : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\..\Controls\Stream.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtMessage;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Controls\Stream.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblViewerCount;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Controls\Stream.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstViewers;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Controls\Stream.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Awesomium.Windows.Controls.WebControl webChat;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Controls\Stream.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Awesomium.Windows.Controls.WebControl webViewStream;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Controls\Stream.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.AutoCompleteBox txtStreamTitle;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Controls\Stream.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.AutoCompleteBox txtStreamGame;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LivestreamBuddyNew;component/controls/stream.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\Stream.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtMessage = ((System.Windows.Controls.TextBox)(target));
            
            #line 12 "..\..\..\Controls\Stream.xaml"
            this.txtMessage.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtMessage_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblViewerCount = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.lstViewers = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.webChat = ((Awesomium.Windows.Controls.WebControl)(target));
            return;
            case 5:
            this.webViewStream = ((Awesomium.Windows.Controls.WebControl)(target));
            return;
            case 6:
            this.txtStreamTitle = ((System.Windows.Controls.AutoCompleteBox)(target));
            return;
            case 7:
            this.txtStreamGame = ((System.Windows.Controls.AutoCompleteBox)(target));
            return;
            case 8:
            
            #line 44 "..\..\..\Controls\Stream.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.UpdateStreamClick);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 45 "..\..\..\Controls\Stream.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.GiveawayClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
