﻿#pragma checksum "..\..\..\UIView\UIMapAlarm.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7BD530F084531797895860533B13227D"
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
    /// UIMapAlarm
    /// </summary>
    public partial class UIMapAlarm : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\UIView\UIMapAlarm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas UIRoot;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\UIView\UIMapAlarm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Icon;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\UIView\UIMapAlarm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock UIName;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\UIView\UIMapAlarm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem UIM_Link;
        
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
            System.Uri resourceLocater = new System.Uri("/Camera_Final;component/uiview/uimapalarm.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UIView\UIMapAlarm.xaml"
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
            
            #line 7 "..\..\..\UIView\UIMapAlarm.xaml"
            ((Camera_Final.UIView.UIMapAlarm)(target)).PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.UserControl_PreviewMouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\UIView\UIMapAlarm.xaml"
            ((Camera_Final.UIView.UIMapAlarm)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.UserControl_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\UIView\UIMapAlarm.xaml"
            ((Camera_Final.UIView.UIMapAlarm)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.UserControl_MouseMove);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\UIView\UIMapAlarm.xaml"
            ((Camera_Final.UIView.UIMapAlarm)(target)).MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.UserControl_MouseDoubleClick);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\UIView\UIMapAlarm.xaml"
            ((Camera_Final.UIView.UIMapAlarm)(target)).Unloaded += new System.Windows.RoutedEventHandler(this.UserControl_Unloaded);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\UIView\UIMapAlarm.xaml"
            ((Camera_Final.UIView.UIMapAlarm)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UIRoot = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.Icon = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.UIName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            
            #line 14 "..\..\..\UIView\UIMapAlarm.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AlarmOff);
            
            #line default
            #line hidden
            return;
            case 6:
            this.UIM_Link = ((System.Windows.Controls.MenuItem)(target));
            
            #line 15 "..\..\..\UIView\UIMapAlarm.xaml"
            this.UIM_Link.Click += new System.Windows.RoutedEventHandler(this.ConnectCamera);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 16 "..\..\..\UIView\UIMapAlarm.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.DisableAlarm);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 17 "..\..\..\UIView\UIMapAlarm.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.EnableAlarm);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 18 "..\..\..\UIView\UIMapAlarm.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteAlarm);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 19 "..\..\..\UIView\UIMapAlarm.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ScheduleAlarm);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

