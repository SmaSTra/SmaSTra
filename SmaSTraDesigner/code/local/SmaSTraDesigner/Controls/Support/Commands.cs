using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmaSTraDesigner.Controls.Support
{
   static class Commands
    {
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


    }
}
