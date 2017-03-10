using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace StandardControlSample
{
    public static class EditorHelper
    {
        public static View GetEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(Thickness))
            {
                return GetThicknessEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType == typeof(bool))
            {
                return GetBooleanEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType == typeof(Color))
            {
                return GetColorEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType == typeof(double))
            {
                return GetDoubleEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType == typeof(int))
            {
                return GetIntEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType == typeof(IEnumerable))
            {
                return GetEnumerableEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType == typeof(ImageSource))
            {
                return GetImageSourceEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType.GetTypeInfo().IsValueType && propertyInfo.PropertyType.GetTypeInfo().IsEnum)
            {
                return GetEnumEditor(visualElement, propertyInfo);
            }
            if (propertyInfo.PropertyType.GetTypeInfo().IsValueType && !propertyInfo.PropertyType.GetTypeInfo().IsEnum)
            {
                return GetStructEditor(visualElement, propertyInfo);
            }

            return GetStringEditor(visualElement, propertyInfo);
        }

        private static View GetImageSourceEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var combobox = new Picker() { VerticalOptions = LayoutOptions.Center, Items = { "Climbing", "Skydive", "Portrait" } };

            combobox.SelectedIndexChanged += (sender, args) =>
            {
                string imageName = "";
                switch (combobox.SelectedIndex)
                {
                    case 0:
                        imageName = "image.jpg";
                        break;
                    case 1:
                        imageName = "sd.jpg";
                        break;
                    case 2:
                        imageName = "portrai.jpg";
                        break;
                }
                propertyInfo.SetValue(visualElement, ImageSource.FromFile(imageName));
            };

            return combobox;
        }

        private static View GetEnumEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var values = Enum.GetValues(propertyInfo.PropertyType);
            var combobox = new Picker() { VerticalOptions = LayoutOptions.Center };
            foreach (var value in values)
            {
                combobox.Items.Add(value.ToString());
            }
            combobox.SelectedIndex = (int)propertyInfo.GetValue(visualElement);

            combobox.SelectedIndexChanged += (sender, args) =>
            {
                foreach (var value in values)
                {
                    if ((int)value == combobox.SelectedIndex)
                        propertyInfo.SetValue(visualElement, value);
                }
            };

            return combobox;
        }

        private static View GetStructEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var fields = propertyInfo.PropertyType.GetRuntimeFields().Where(f => f.IsPublic).ToList();

            var combobox = new Picker() { VerticalOptions = LayoutOptions.Center };
            foreach (var field in fields.Select(f => f.Name))
                combobox.Items.Add(field);

            combobox.SelectedIndex =
                fields.IndexOf(fields.FirstOrDefault(f => f.GetValue(null) == propertyInfo.GetValue(visualElement)));

            combobox.SelectedIndexChanged += (sender, args) =>
            {
                propertyInfo.SetValue(visualElement, fields[combobox.SelectedIndex].GetValue(null));
            };

            return combobox;
        }

        private static View GetColorEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var currentValue = (Color)propertyInfo.GetValue(visualElement);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


            var labelA = new Label { Text = "A : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
            var labelR = new Label { Text = "R : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
            var labelG = new Label { Text = "G : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
            var labelB = new Label { Text = "B : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };

            var sliderA = new Slider { Minimum = 0, Maximum = 255, Value = currentValue.A, VerticalOptions = LayoutOptions.Center };
            var sliderR = new Slider { Minimum = 0, Maximum = 255, Value = currentValue.R, VerticalOptions = LayoutOptions.Center };
            var sliderG = new Slider { Minimum = 0, Maximum = 255, Value = currentValue.G, VerticalOptions = LayoutOptions.Center };
            var sliderB = new Slider { Minimum = 0, Maximum = 255, Value = currentValue.B, VerticalOptions = LayoutOptions.Center };


            var marginChangedEventHandler = new EventHandler<ValueChangedEventArgs>(
                delegate (object sender, ValueChangedEventArgs args)
                {
                    try
                    {
                        var newValue = Color.FromRgba(sliderR.Value / 255, sliderG.Value / 255, sliderB.Value / 255, sliderA.Value / 255);
                        propertyInfo.SetValue(visualElement, newValue);
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                });

            sliderA.ValueChanged += marginChangedEventHandler;
            sliderR.ValueChanged += marginChangedEventHandler;
            sliderG.ValueChanged += marginChangedEventHandler;
            sliderB.ValueChanged += marginChangedEventHandler;

            labelA.SetValue(Grid.ColumnProperty, 0);
            labelR.SetValue(Grid.ColumnProperty, 0);
            labelG.SetValue(Grid.ColumnProperty, 0);
            labelB.SetValue(Grid.ColumnProperty, 0);

            labelA.SetValue(Grid.RowProperty, 0);
            labelR.SetValue(Grid.RowProperty, 1);
            labelG.SetValue(Grid.RowProperty, 2);
            labelB.SetValue(Grid.RowProperty, 3);

            sliderA.SetValue(Grid.ColumnProperty, 1);
            sliderR.SetValue(Grid.ColumnProperty, 1);
            sliderG.SetValue(Grid.ColumnProperty, 1);
            sliderB.SetValue(Grid.ColumnProperty, 1);

            sliderA.SetValue(Grid.RowProperty, 0);
            sliderR.SetValue(Grid.RowProperty, 1);
            sliderG.SetValue(Grid.RowProperty, 2);
            sliderB.SetValue(Grid.RowProperty, 3);

            grid.Children.Add(labelA);
            grid.Children.Add(labelR);
            grid.Children.Add(labelG);
            grid.Children.Add(labelB);
            grid.Children.Add(sliderA);
            grid.Children.Add(sliderR);
            grid.Children.Add(sliderG);
            grid.Children.Add(sliderB);

            return grid;
        }

        private static View GetStringEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var editor = new Editor { Text = propertyInfo.GetValue(visualElement)?.ToString(), VerticalOptions = LayoutOptions.Center };
            editor.TextChanged += (sender, args) => propertyInfo.SetValue(visualElement, args.NewTextValue);
            return editor;
        }

        private static View GetDoubleEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            //var editor = new Editor { Text = propertyInfo.GetValue(visualElement)?.ToString(), VerticalOptions = LayoutOptions.Center };
            //editor.TextChanged += (sender, args) =>
            //{
            //    double d = 0;
            //    double.TryParse(args.NewTextValue, out d);
            //    propertyInfo.SetValue(visualElement, d);
            //};
            //return editor;

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });

            var minEditor = new Editor { Text = "0", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            var maxEditor = new Editor { Text = "255", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            var slider = new Slider { Minimum = 0, Maximum = 255 };

            minEditor.SetValue(Grid.ColumnProperty, 0);
            slider.SetValue(Grid.ColumnProperty, 1);
            maxEditor.SetValue(Grid.ColumnProperty, 2);

            minEditor.TextChanged += (sender, args) =>
            {
                double newValue;
                if (double.TryParse(args.NewTextValue, out newValue))
                    slider.Minimum = newValue;
            };

            maxEditor.TextChanged += (sender, args) =>
            {
                double newValue;
                if (double.TryParse(args.NewTextValue, out newValue))
                    slider.Maximum = newValue;
            };

            slider.ValueChanged += (sender, args) =>
            {
                propertyInfo.SetValue(visualElement, slider.Value);
            };

            grid.Children.Add(minEditor);
            grid.Children.Add(maxEditor);
            grid.Children.Add(slider);

            return grid;
        }

        private static View GetIntEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var editor = new Editor { Text = propertyInfo.GetValue(visualElement)?.ToString(), VerticalOptions = LayoutOptions.Center };
            editor.TextChanged += (sender, args) =>
            {
                int d = 0;
                int.TryParse(args.NewTextValue, out d);
                propertyInfo.SetValue(visualElement, d);
            };
            return editor;
        }

        private static View GetEnumerableEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var editor = new Editor { VerticalOptions = LayoutOptions.Center };
            var button = new Xamarin.Forms.Button { Text = "ADD", VerticalOptions = LayoutOptions.Center };

            editor.SetValue(Grid.ColumnProperty, 0);
            button.SetValue(Grid.ColumnProperty, 1);

            button.Clicked += (sender, args) =>
            {
                var toAdd = editor.Text;

                if (string.IsNullOrWhiteSpace(toAdd))
                    return;

                if (propertyInfo.GetValue(visualElement) == null)
                    propertyInfo.SetValue(visualElement, new ObservableCollection<string> { toAdd });
                else
                {
                    var list = propertyInfo.GetValue(visualElement);
                    var addMethod = list.GetType().GetRuntimeMethods().FirstOrDefault(m => m.Name == "Add");
                    addMethod.Invoke(list, new object[] { toAdd });
                }
                editor.Text = "";
            };

            grid.Children.Add(editor);
            grid.Children.Add(button);

            return grid;
        }

        private static View GetBooleanEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var comboBox = new Picker { Items = { "True", "False" }, VerticalOptions = LayoutOptions.Center };
            comboBox.SelectedIndex = (bool)propertyInfo.GetValue(visualElement) ? 0 : 1;
            comboBox.SelectedIndexChanged += (sender, args) =>
            {
                propertyInfo.SetValue(visualElement, comboBox.SelectedIndex == 0);
            };
            return comboBox;
        }

        private static View GetThicknessEditor(VisualElement visualElement, PropertyInfo propertyInfo)
        {
            var currentValue = (Thickness)propertyInfo.GetValue(visualElement);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


            var labelLeft = new Label { Text = "Left : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
            var labelTop = new Label { Text = "Top : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
            var labelRight = new Label { Text = "Right : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
            var labelBottom = new Label { Text = "Bottom : ", VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };

            var editorLeft = new Editor { Text = currentValue.Left.ToString(), VerticalOptions = LayoutOptions.Center };
            var editorTop = new Editor { Text = currentValue.Top.ToString(), VerticalOptions = LayoutOptions.Center };
            var editorRight = new Editor { Text = currentValue.Right.ToString(), VerticalOptions = LayoutOptions.Center };
            var editorBottom = new Editor { Text = currentValue.Bottom.ToString(), VerticalOptions = LayoutOptions.Center };


            var marginChangedEventHandler = new EventHandler<TextChangedEventArgs>(
                delegate (object sender, TextChangedEventArgs args)
                {
                    try
                    {
                        var left = string.IsNullOrWhiteSpace(editorLeft.Text) ? 0 : double.Parse(editorLeft.Text);
                        var top = string.IsNullOrWhiteSpace(editorTop.Text) ? 0 : double.Parse(editorTop.Text);
                        var right = string.IsNullOrWhiteSpace(editorRight.Text) ? 0 : double.Parse(editorRight.Text);
                        var bottom = string.IsNullOrWhiteSpace(editorBottom.Text) ? 0 : double.Parse(editorBottom.Text);
                        var newValue = new Thickness(left, top, right, bottom);
                        propertyInfo.SetValue(visualElement, newValue);
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                });

            editorLeft.TextChanged += marginChangedEventHandler;
            editorTop.TextChanged += marginChangedEventHandler;
            editorRight.TextChanged += marginChangedEventHandler;
            editorBottom.TextChanged += marginChangedEventHandler;

            labelLeft.SetValue(Grid.ColumnProperty, 0);
            labelTop.SetValue(Grid.ColumnProperty, 0);
            labelRight.SetValue(Grid.ColumnProperty, 0);
            labelBottom.SetValue(Grid.ColumnProperty, 0);

            labelLeft.SetValue(Grid.RowProperty, 0);
            labelTop.SetValue(Grid.RowProperty, 1);
            labelRight.SetValue(Grid.RowProperty, 2);
            labelBottom.SetValue(Grid.RowProperty, 3);

            editorLeft.SetValue(Grid.ColumnProperty, 1);
            editorTop.SetValue(Grid.ColumnProperty, 1);
            editorRight.SetValue(Grid.ColumnProperty, 1);
            editorBottom.SetValue(Grid.ColumnProperty, 1);

            editorLeft.SetValue(Grid.RowProperty, 0);
            editorTop.SetValue(Grid.RowProperty, 1);
            editorRight.SetValue(Grid.RowProperty, 2);
            editorBottom.SetValue(Grid.RowProperty, 3);

            grid.Children.Add(labelLeft);
            grid.Children.Add(labelTop);
            grid.Children.Add(labelRight);
            grid.Children.Add(labelBottom);
            grid.Children.Add(editorLeft);
            grid.Children.Add(editorTop);
            grid.Children.Add(editorRight);
            grid.Children.Add(editorBottom);

            return grid;
        }
    }
}
