﻿#pragma checksum "..\..\..\Window\CyclerAlarmWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DAD6E359C49CC939A513CE177C6C29AC7D893EF8"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
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


namespace Renault_BT6.윈도우 {
    
    
    /// <summary>
    /// CyclerAlarmWindow
    /// </summary>
    public partial class CyclerAlarmWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 37 "..\..\..\Window\CyclerAlarmWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label maintitle;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Window\CyclerAlarmWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label maintitle1;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\Window\CyclerAlarmWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox wLb;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\Window\CyclerAlarmWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox dLb;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\Window\CyclerAlarmWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox gLb;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\Window\CyclerAlarmWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox fLb;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\Window\CyclerAlarmWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button closebt;
        
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
            System.Uri resourceLocater = new System.Uri("/Renault_BT6;component/window/cycleralarmwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Window\CyclerAlarmWindow.xaml"
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
            this.maintitle = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.maintitle1 = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.wLb = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.dLb = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            this.gLb = ((System.Windows.Controls.ListBox)(target));
            return;
            case 6:
            this.fLb = ((System.Windows.Controls.ListBox)(target));
            return;
            case 7:
            this.closebt = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\..\Window\CyclerAlarmWindow.xaml"
            this.closebt.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

