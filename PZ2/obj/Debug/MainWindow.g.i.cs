﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "16334829C84B13C6A28746AF29FEB60B7964A6114B48A6D19EA1BF59833A03C0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PZ2;
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


namespace PZ2 {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Viewport3D viewport1;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox hideInactiveNetwork;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox changeSwitchColor;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox changeLineColor;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox hideAllLine;
        
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
            System.Uri resourceLocater = new System.Uri("/PZ2;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
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
            this.viewport1 = ((System.Windows.Controls.Viewport3D)(target));
            
            #line 11 "..\..\MainWindow.xaml"
            this.viewport1.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.viewport1_MouseWheel);
            
            #line default
            #line hidden
            
            #line 11 "..\..\MainWindow.xaml"
            this.viewport1.MouseMove += new System.Windows.Input.MouseEventHandler(this.viewport1_MouseMove);
            
            #line default
            #line hidden
            
            #line 11 "..\..\MainWindow.xaml"
            this.viewport1.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.viewport1_MouseDown);
            
            #line default
            #line hidden
            
            #line 11 "..\..\MainWindow.xaml"
            this.viewport1.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.viewport1_MouseUp);
            
            #line default
            #line hidden
            return;
            case 2:
            this.hideInactiveNetwork = ((System.Windows.Controls.CheckBox)(target));
            
            #line 19 "..\..\MainWindow.xaml"
            this.hideInactiveNetwork.Click += new System.Windows.RoutedEventHandler(this.hideInactiveNetwork_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.changeSwitchColor = ((System.Windows.Controls.CheckBox)(target));
            
            #line 20 "..\..\MainWindow.xaml"
            this.changeSwitchColor.Click += new System.Windows.RoutedEventHandler(this.changeSwitchColor_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.changeLineColor = ((System.Windows.Controls.CheckBox)(target));
            
            #line 21 "..\..\MainWindow.xaml"
            this.changeLineColor.Click += new System.Windows.RoutedEventHandler(this.changeLineColor_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.hideAllLine = ((System.Windows.Controls.CheckBox)(target));
            
            #line 22 "..\..\MainWindow.xaml"
            this.hideAllLine.Click += new System.Windows.RoutedEventHandler(this.hideAllLine_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

