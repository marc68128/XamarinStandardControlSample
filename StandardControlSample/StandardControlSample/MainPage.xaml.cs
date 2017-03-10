using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StandardControlSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var assembly = Assembly.Load(new AssemblyName("Xamarin.Forms.Core"));
            var methods = assembly.GetType().GetRuntimeMethods().ToList();
            var getTypesMethod = methods.Where(s => s.Name.Contains("GetTypes")).First();
            var types = (getTypesMethod.Invoke(assembly, new []{ true as object }) as IEnumerable<TypeInfo>).Where(t => t.IsSubclassOf(typeof(View) )&& !t.ContainsGenericParameters).ToList();
            foreach (var typeInfo in types)
            {
                var button = new Xamarin.Forms.Button {Margin = 10, Text = typeInfo.Name};
                button.Clicked += (sender, args) => Navigation.PushAsync(new ComponentPage(typeInfo.AsType()));
                StackLayout.Children.Add(button);
            }
        }
    }
}
