﻿#pragma checksum "..\..\..\..\Views\TasksView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B8C90473DCAD33B9701C7E6CA60F2D4A8A3B2E9F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ExtendedPomodoro.Controls;
using ExtendedPomodoro.Converters;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace ExtendedPomodoro.Views {
    
    
    /// <summary>
    /// TasksView
    /// </summary>
    public partial class TasksView : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 207 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ExtendedPomodoro.Controls.Modal ModalCreateTask;
        
        #line default
        #line hidden
        
        
        #line 275 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ExtendedPomodoro.Controls.Modal ModalViewTaskDetail;
        
        #line default
        #line hidden
        
        
        #line 294 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock IdTaskDetail;
        
        #line default
        #line hidden
        
        
        #line 304 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TitleTaskDetail;
        
        #line default
        #line hidden
        
        
        #line 318 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DescriptionTaskDetail;
        
        #line default
        #line hidden
        
        
        #line 333 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox EstPomodoroTaskDetail;
        
        #line default
        #line hidden
        
        
        #line 343 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ActPomodoroTaskDetail;
        
        #line default
        #line hidden
        
        
        #line 360 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox StatusTaskDetail;
        
        #line default
        #line hidden
        
        
        #line 377 "..\..\..\..\Views\TasksView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TimeSpentTaskDetail;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ExtendedPomodoro;component/views/tasksview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\TasksView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 43 "..\..\..\..\Views\TasksView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonCreateTask_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ModalCreateTask = ((ExtendedPomodoro.Controls.Modal)(target));
            return;
            case 3:
            
            #line 265 "..\..\..\..\Views\TasksView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonCancelCreateTaskModal_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ModalViewTaskDetail = ((ExtendedPomodoro.Controls.Modal)(target));
            return;
            case 5:
            this.IdTaskDetail = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.TitleTaskDetail = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.DescriptionTaskDetail = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.EstPomodoroTaskDetail = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.ActPomodoroTaskDetail = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.StatusTaskDetail = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 11:
            this.TimeSpentTaskDetail = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            
            #line 383 "..\..\..\..\Views\TasksView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonCloseTaskDetail_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

