﻿#pragma checksum "..\..\..\UIView\ControlCamera.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A460D70133D2B36335944AC1835E887B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
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
    /// ControlCamera
    /// </summary>
    public partial class ControlCamera : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\UIView\ControlCamera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas UIRoot;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\UIView\ControlCamera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Forms.PictureBox pictureBox1;
        
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
            System.Uri resourceLocater = new System.Uri("/Camera_Final;component/uiview/controlcamera.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UIView\ControlCamera.xaml"
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
            
            #line 9 "..\..\..\UIView\ControlCamera.xaml"
            ((Camera_Final.UIView.ControlCamera)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\UIView\ControlCamera.xaml"
            ((Camera_Final.UIView.ControlCamera)(target)).Unloaded += new System.Windows.RoutedEventHandler(this.UserControl_Unloaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UIRoot = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.pictureBox1 = ((System.Windows.Forms.PictureBox)(target));
            return;
            case 4:
            
            #line 22 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveRightBegin);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveRightEnd);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 23 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveLeftBegin);
            
            #line default
            #line hidden
            
            #line 23 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveLeftEnd);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 24 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveDownBegin);
            
            #line default
            #line hidden
            
            #line 24 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveDownEnd);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 25 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveUpBegin);
            
            #line default
            #line hidden
            
            #line 25 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveUpEnd);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 26 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveAuto);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 27 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 28 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.ZoomInBegin);
            
            #line default
            #line hidden
            
            #line 28 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.ZoomInEnd);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 29 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.ZoomOutBegin);
            
            #line default
            #line hidden
            
            #line 29 "..\..\..\UIView\ControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.ZoomOutEnd);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
