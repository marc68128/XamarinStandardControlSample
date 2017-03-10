using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace StandardControlSample
{
    public partial class ComponentPage
    {
        static readonly List<Type> SupportedTypes = new List<Type> { typeof(int), typeof(double), typeof(long), typeof(bool), typeof(Thickness), typeof(string), typeof(Color), typeof(IEnumerable), typeof(LayoutOptions), typeof(LineBreakMode), typeof(TextAlignment), typeof(FontAttributes), typeof(ImageSource), typeof(Aspect) };

        public ComponentPage(Type componentType)
        {
            InitializeComponent();

            this.Title = componentType.Name;

            var component = Activator.CreateInstance(componentType) as View;
            component.Margin = new Thickness(10);

            ComponentGrid.Children.Add(component);

            

            var properties = componentType.GetRuntimeProperties().Where(p => p.CanRead && p.CanWrite && SupportedTypes.Contains(p.PropertyType));
            var unsupportedProperties = componentType.GetRuntimeProperties().Where(p => p.CanRead && p.CanWrite && !SupportedTypes.Contains(p.PropertyType)).ToList();
            foreach (var propertyInfo in properties)
            {
                PropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var label = new Label { Text = propertyInfo.Name, Margin = new Thickness(5, 0), VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center};
                label.SetValue(Grid.ColumnProperty, 0);
                label.SetValue(Grid.RowProperty, PropertiesGrid.RowDefinitions.Count - 1);

                var editor = EditorHelper.GetEditor(component, propertyInfo);
                editor.Margin = new Thickness(5, 0);
                editor.SetValue(Grid.ColumnProperty, 1);
                editor.SetValue(Grid.RowProperty, PropertiesGrid.RowDefinitions.Count - 1);

                PropertiesGrid.Children.Add(label);
                PropertiesGrid.Children.Add(editor);

                PropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var separator = new StackLayout { HeightRequest = 2, BackgroundColor = Color.Black, HorizontalOptions = LayoutOptions.FillAndExpand, Margin = new Thickness(0, 5) };
                separator.SetValue(Grid.RowProperty, PropertiesGrid.RowDefinitions.Count - 1);
                separator.SetValue(Grid.ColumnProperty, 0);
                separator.SetValue(Grid.ColumnSpanProperty, 2);

                PropertiesGrid.Children.Add(separator);
            }

        }


        private void CloseErrorPopup(object sender, EventArgs e)
        {
            ErrorGrid.IsVisible = false;
        }
    }
}
