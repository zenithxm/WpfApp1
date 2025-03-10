using Microsoft.Xaml.Behaviors;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfApp1.View_Model;

namespace WpfApp1.Helper
{
    public class MoveCursorToEndBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;
                //make sure it only move when assign value not when user typing
                if (textBox != null && textBox.CaretIndex == 0)
                {
                    textBox.Dispatcher.BeginInvoke(() =>
                    {
                        textBox.CaretIndex = textBox.Text.Length;
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public class GotFocusSelectAllBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += GotFocusChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= GotFocusChanged;
        }

        private void GotFocusChanged(object sender, EventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;

                if (textBox != null)
                {
                    textBox.Dispatcher.BeginInvoke(() =>
                    {
                        textBox.SelectAll();
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public class ClearFocusBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var control = sender as DataGrid;

                if (control != null)
                {
                    Keyboard.Focus(control);
                    Keyboard.ClearFocus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public class ClearFocusBehaviorKeyDown : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += OnKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var control = sender as TextBox;

                if (control != null && e.Key == Key.Enter)
                {
                    Keyboard.Focus(control);
                    Keyboard.ClearFocus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public class KeyBindingLostFocusBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key", typeof(Key), typeof(KeyBindingLostFocusBehavior), new PropertyMetadata(default(Key)));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(KeyBindingLostFocusBehavior), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty TargetFocusProperty = DependencyProperty.Register(
            "TargetFocus", typeof(object), typeof(KeyBindingLostFocusBehavior), new PropertyMetadata(default(object)));

        public Key Key
        {
            get => (Key)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object TargetFocus
        {
            get => (object)GetValue(TargetFocusProperty);
            set => SetValue(TargetFocusProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += OnKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key)
                {
                    if (Command?.CanExecute(null) == true)
                    {
                        Command.Execute(null);

                        // Ensure the focus change is done on the UI thread
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            DispatcherTimer timer = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromMilliseconds(100)
                            };
                            timer.Tick += (s, ev) =>
                            {
                                timer.Stop();

                                var target = TargetFocus as UIElement;

                                if (target != null)
                                {
                                    Keyboard.Focus(target);
                                    Keyboard.ClearFocus();
                                }
                            };
                            timer.Start();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

}
