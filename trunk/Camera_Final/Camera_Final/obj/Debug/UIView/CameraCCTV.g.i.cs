﻿#pragma checksum "..\..\..\UIView\CameraCCTV.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7EEA8EEFC288BCCB296B0D5E06956FCD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
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
using Vlc.DotNet.Wpf;


namespace Camera_Final.UIView {
    
    
    /// <summary>
    /// CameraCCTV
    /// </summary>
    public partial class CameraCCTV : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\UIView\CameraCCTV.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Camera_Final.UIView.CameraCCTV UIRootView;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\UIView\CameraCCTV.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas UIRoot;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\UIView\CameraCCTV.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border Border;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\UIView\CameraCCTV.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Forms.PictureBox pictureBox1;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\UIView\CameraCCTV.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock UIFull;
        
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
            System.Uri resourceLocater = new System.Uri("/Camera_Final;component/uiview/cameracctv.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UIView\CameraCCTV.xaml"
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
            this.UIRootView = ((Camera_Final.UIView.CameraCCTV)(target));
            
            #line 9 "..\..\..\UIView\CameraCCTV.xaml"
            this.UIRootView.Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\UIView\CameraCCTV.xaml"
            this.UIRootView.Unloaded += new System.Windows.RoutedEventHandler(this.UserControl_Unloaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\UIView\CameraCCTV.xaml"
            this.UIRootView.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.UIRootView_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UIRoot = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.Border = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            this.pictureBox1 = ((System.Windows.Forms.PictureBox)(target));
            return;
            case 5:
            this.UIFull = ((System.Windows.Controls.TextBlock)(target));
            
            #line 18 "..\..\..\UIView\CameraCCTV.xaml"
            this.UIFull.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

