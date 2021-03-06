﻿using System.Windows.Input;

namespace SmaSTraDesigner.Controls.Support
{
   static class Commands
    {
        public static readonly RoutedUICommand DebugTest = new RoutedUICommand
                        (
                                "DebugTest",
                                "DebugTest",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.T, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand New = new RoutedUICommand
                        (
                                "New",
                                "New",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                       
                                }
                        );

        public static readonly RoutedUICommand Save = new RoutedUICommand
                        (
                                "Save",
                                "Save",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.S, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand Load = new RoutedUICommand
                        (
                                "Load",
                                "Load",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.L, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand Exit = new RoutedUICommand
                        (
                                "Exit",
                                "Exit",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.F4, ModifierKeys.Alt)
                                }
                        );

        public static readonly RoutedUICommand Generate = new RoutedUICommand
                        (
                                "Generate",
                                "Generate",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.G, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand Delete = new RoutedUICommand
                        (
                                "Delete",
                                "Delete",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.Delete)
                                }
                        );

        public static readonly RoutedUICommand SelectConnected = new RoutedUICommand
                        (
                                "Select all connected",
                                "SelectConnected",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        
                                }
                        );

        public static readonly RoutedUICommand AddSelected = new RoutedUICommand
                        (
                                "Add to selection",
                                "AddSelected",
                                typeof(Commands),
                                new InputGestureCollection()
                                {

                                }
                        );

        public static readonly RoutedUICommand ToOutputViewer = new RoutedUICommand
                        (
                                "To OutputViewer",
                                "ToOutputViewer",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.O, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand AddToLibrary = new RoutedUICommand
                        (
                                "Add to library",
                                "AddToLibrary",
                                typeof(Commands),
                                new InputGestureCollection()
                                {

                                }
                        );


        public static readonly RoutedUICommand Merge = new RoutedUICommand
                        (
                                "Merge current selection",
                                "Merge",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.M, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand Unmerge = new RoutedUICommand
                        (
                                "Unmerges current selection",
                                "Unmerge",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.U, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand CreateCustomElement = new RoutedUICommand
                        (
                                "Create custom element",
                                "CreateCustomElement",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.N, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand SwitchWorkspace = new RoutedUICommand
                        (
                                "Switch Workspace",
                                "SwitchWorkspace",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.W, ModifierKeys.Alt)
                                }
                        );

        public static readonly RoutedUICommand OnlineTransformations = new RoutedUICommand
                        (
                                "Online Transformations",
                                "OnlineTransformations",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.D, ModifierKeys.Control)
                                }
                        );


        public static readonly RoutedUICommand Undo = new RoutedUICommand
                        (
                                "Undo",
                                "Undo",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.Z, ModifierKeys.Control)
                                }
                        );


        public static readonly RoutedUICommand Redo = new RoutedUICommand
                        (
                                "Redo",
                                "Redo",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.Y, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand PasteNode = new RoutedUICommand
                        (
                                "Paste Node",
                                "PasteNode",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.V, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand About = new RoutedUICommand
                        (
                                "About",
                                "About",
                                typeof(Commands),
                                new InputGestureCollection()
                                {

                                }
                        );

        public static readonly RoutedUICommand OrganizeNodes = new RoutedUICommand
                        (
                                "Organize Nodes",
                                "OrganizeNodes",
                                typeof(Commands),
                                new InputGestureCollection()
                                {
                                    new KeyGesture(Key.F, ModifierKeys.Control)
                                }
                        );

        public static readonly RoutedUICommand CustomCode = new RoutedUICommand
                        (
                                "Custom Code",
                                "CustomCode",
                                typeof(Commands),
                                new InputGestureCollection()
                                {

                                }
                        );

        public static readonly RoutedUICommand FocusInputValue = new RoutedUICommand
                        (
                                "Focus Input Value",
                                "FocusInputValue",
                                typeof(Commands),
                                new InputGestureCollection()
                                {

                                }
                        );

    }
}
