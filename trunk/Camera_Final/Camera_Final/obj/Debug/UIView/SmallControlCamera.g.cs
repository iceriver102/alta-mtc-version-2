﻿#pragma checksum "..\..\..\UIView\SmallControlCamera.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "241F8ACF16A63ECAA95DAD7A66B962E9"
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


namespace Camera_Final.UIView {
    
    
    /// <summary>
    /// SmallControlCamera
    /// </summary>
    public partial class SmallControlCamera : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\UIView\SmallControlCamera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas UIRoot;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\UIView\SmallControlCamera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border layoutContent;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\UIView\SmallControlCamera.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Camera_Final;component/uiview/smallcontrolcamera.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UIView\SmallControlCamera.xaml"
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
            
            #line 8 "..\..\..\UIView\SmallControlCamera.xaml"
            ((Camera_Final.UIView.SmallControlCamera)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\UIView\SmallControlCamera.xaml"
            ((Camera_Final.UIView.SmallControlCamera)(target)).Unloaded += new System.Windows.RoutedEventHandler(this.UserControl_Unloaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UIRoot = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.layoutContent = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            
            #line 16 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 18 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SaveCamera);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 19 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveLeftEnd);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveLeftBegin);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 20 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveDownEnd);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveDownBegin);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 21 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveUpEnd);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveUpBegin);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 22 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MoveRightEnd);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\UIView\SmallControlCamera.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveRightBegin);
            
            #line default
            #line hidden
            return;
            case 10:
            this.pictureBox1 = ((System.Windows.Forms.PictureBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

